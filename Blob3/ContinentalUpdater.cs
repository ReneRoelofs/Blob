using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using BlobHandler;
using FmsBlobToContinental;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Statics = FmsBlobToContinental.Statics;
[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace Blob3
{
    public class ContinentalUpdater
    {
        //TODO: hoe gaan we om met cancelations?
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        BlobContainerClient containerClient = null;
        public Boolean useTimeStampNow;
        DateTime since = new DateTime(2000, 10, 21, 23, 59, 59);
        private ConcurrentQueue<BlobItem> myQueue = new System.Collections.Concurrent.ConcurrentQueue<BlobItem>();
        List<Task> mySenderTasks = new List<Task>();
        public VehicleSenderList vehicleSenderList = new VehicleSenderList();

        public ContinentalUpdater()
        {
            useTimeStampNow = false;
            //   Task.Factory.StartNew(() => StartSenderTasks());
        }


        Task Getter = null;
        DateTime getterStartTime = DateTime.MinValue;
        Stopwatch getterStopWatch = new Stopwatch();
        int getterAmmount = 0;




        /// <summary>
        /// This routine can be used in a service.. Just get the data, sendit, and do it again.
        /// calles GetItemsAndUpdateContinental with a lot of default parameters.
        /// <param name="testProd"></param>
        /// </summary>
        public Task GetItemsAndUpdateContinental(TestProd testProd)
        {
            return GetItemsAndUpdateContinentalAsync(
                testProd: testProd,
                onlyRecent: true,
                paramSince: null,
                until : null,
                onlyBus: "",
                doLoop: true);
        }

        /// <summary>
        /// Get all data, used for service and debugging. Just get the data, sendit, and do it again. Build a list First.
        /// </summary>
        /// <param name="testProd"></param>
        /// <param name="onlyRecent"></param>
        /// <param name="paramSince"></param>
        /// <param name="onlyBus"></param>
        /// <returns></returns>
        public async Task GetItemsAndUpdateContinentalAsync(
                        TestProd testProd,
                        Boolean onlyRecent,         // = true,
                        DateTime? paramSince,            // = null,
                        DateTime? until,             // = null,
                        string onlyBus,             // = "",
                        Boolean doLoop)             //=true)
        {
            containerClient = BlobHandler.Statics.GetContainerClient(testProd);
            List<BlobItem> items = new List<BlobItem>();
            while (true)  // break if !doLoop
            {
                try
                {
                    getterStartTime = DateTime.Now;
                    getterStopWatch.Restart();

                    if (!onlyRecent)
                    {
                        this.since = (DateTime)paramSince;
                    }
                    else
                    {
                        this.since = RR.MyRegistry.GetDate("latestModified", new DateTime(2021, 12, 11));

                        if (this.since.Year < 2021)
                        {
                            this.since = new DateTime(2021, 12, 11);
                        }
                    }

                    DateTime lastModified = this.since;

                    //Getter = Task.Factory.StartNew(() =>
                    //   {
                    //+
                    //--- get a list of blobitems
                    //-
                    Statics.SaveSensorList();
                    Statics.SaveVehicleList();
                    BlobHandler.BlobItemList blobItemlist = new BlobHandler.BlobItemList(DownloadAndEnqueueToSend);
                    items = await blobItemlist.GetBlobItemsASync(
                        testProd,
                        this.since,
                        until,
                        downloadToFile: false,
                        onlyVehicle: onlyBus);

                    if (items.Count > 0)
                    {
                        BlobFileName blobFilename = new BlobFileName();
                        blobFilename.Name = items.Last().Name;
                        lastModified = blobFilename.timeStamp;

                        if (lastModified > this.since)
                        {
                            RR.MyRegistry.PutDate("latestModified", lastModified);
                        }
                    }
                    getterStopWatch.Stop();
                    getterAmmount = items.Count;

                    if (doLoop)
                    {
                        Thread.Sleep(TimeSpan.FromMinutes(FmsBlobToContinental.Statics.MinutesForMainLoop));
                    }

                }
                catch (Exception ex)
                {
                    log.Error("", ex);
                }
                if (!doLoop)
                {
                    break; // only once if ! doloop;
                }
            }
        }


        private int nItemsInQueues = 0;
        private int nPayloadsInQueues = 0;

        /// <summary>
        /// How many items a in the sendqueue
        /// </summary>
        /// <returns></returns>
        public string Text()
        {
            lock (vehicleSenderList)
            {
                nItemsInQueues = 0;
                nPayloadsInQueues = 0;

                vehicleSenderList.Sort((x, y) => x.vehicleNumber.CompareTo(y.vehicleNumber));

                foreach (VehicleWithSendQueue vc in vehicleSenderList)
                {
                    nItemsInQueues += vc.blobItemQueueCount;
                    nPayloadsInQueues += vc.PayloadQueueCount;
                    // if (vc.PayloadQueueCount() != 0)
                    {
                        //     vc.WriteStatusToFeedback();
                    }
                }
            }
            string Getterinfo;
            if (!getterStopWatch.IsRunning)
            {
                Getterinfo = string.Format(" got {0} items in {1} ", getterAmmount, RR.Lib.Elapsed(getterStopWatch));
            }
            else
            {
                Getterinfo = string.Format(" getting items started at {0:HH:mm:ss} elapsed {1} ", getterStartTime, RR.Lib.Elapsed(getterStopWatch));
            }
            return string.Format("{0:HH:mm:ss} Send TirePressure to Continental : {1} blobitems, {2} PayloadItems in {3} vehiclequeues. {4}",
                DateTime.Now,
                nItemsInQueues,
                nPayloadsInQueues,
                vehicleSenderList.Count, Getterinfo);
        }

        public Boolean Empty()
        {
            return (nItemsInQueues == 0 && nPayloadsInQueues == 0);
        }

      
        /// <summary>
        /// Put a new blobitem in the vehicle's queue in order to send the data.
        /// </summary>
        /// <param name="blobItem"></param>
        public void DownloadAndEnqueueToSend(BlobItem blobItem)
        {
            BlobFileName blobFilename = new BlobFileName();
            blobFilename.Name = blobItem.Name;


            CCVehicle vehicle =  Statics.vehicleList.GetOrAdd(blobFilename.vehicleNumber, blobFilename.AgentSerialNumber);
            vehicle.timestampFileSeen = blobFilename.timeStamp;

            //DateTime lastModified = (((DateTimeOffset)blobItem.Properties.LastModified).ToUniversalTime()).DateTime;
            if (blobFilename.timeStamp > this.since)
            {
                RR.MyRegistry.PutDate("latestModified", blobFilename.timeStamp);
                this.since = blobFilename.timeStamp;
            }

            //+
            //--- create a vehiclesender per vehicle
            //-
            lock (vehicleSenderList)
            {
                VehicleWithSendQueue vehicleSender = vehicleSenderList.GetOrAdd(blobFilename.vehicleNumber, containerClient);
                //-
                //--- enqueue the blobitem to the vehicleSender
                //-
                vehicleSender.EnqueueBlobItemForDownload(blobItem);
            }

        }


    }
}