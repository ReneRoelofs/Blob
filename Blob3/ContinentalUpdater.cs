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
                        DateTime? paramSince,       // = null,
                        DateTime? until,            // = null,
                        string onlyBus,             // = "",
                        Boolean doLoop,             //=true)
                        CancellationToken ct)             
        {
            containerClient = BlobHandler.Statics.GetContainerClient(testProd);
            List<BlobItem> items = new List<BlobItem>();
            while (true)  // break if !doLoop
            {
                try
                {
                    getterStartTime = DateTime.Now;
                    getterStopWatch.Restart();


                    //+
                    //--- non default behavour: parameters given for from and until date.
                    //-
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
                        onlyVehicle: onlyBus,
                        ct);

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
                        while (!Empty())
                        {
                            await Task.Delay(TimeSpan.FromSeconds(1));
                        }
                        if (ct.IsCancellationRequested)
                        {
                            break;
                        }

                        for (int i = 0; i < 10; i++)
                        {
                            Thread.Sleep(TimeSpan.FromSeconds(Statics.MinutesForMainLoop / 10)); // 
                            if (ct.IsCancellationRequested)
                            {
                                break;
                            }
                        }
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
                if (ct.IsCancellationRequested)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Update to continental in a loop for the pas half hour. Used in the service.
        /// </summary>
        /// <param name="testProd"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task DoNowInALoop(TestProd testProd, string onlyVehicle, CancellationToken ct)
        {
            containerClient = BlobHandler.Statics.GetContainerClient(testProd);
            List<BlobItem> items = new List<BlobItem>();

            while (!ct.IsCancellationRequested)
            {
                getterStartTime = DateTime.Now;
                getterStopWatch.Restart();

                since = DateTime.Now.ToUniversalTime().AddMinutes(-30);
                DateTime until = since.AddDays(1);

                Statics.SaveSensorList();
                Statics.SaveVehicleList();

                BlobHandler.BlobItemList blobItemlist = new BlobHandler.BlobItemList(DownloadAndEnqueueToSend);
                await blobItemlist.GetBlobItemsASync(
                    testProd,
                    since,
                    until,
                    downloadToFile: false,
                    onlyVehicle: onlyVehicle,
                    ct);

                getterStopWatch.Stop();
                getterAmmount = items.Count;

              
                if (!ct.IsCancellationRequested)
                {
                    int partialDelay = (int)(Statics.MinutesForMainLoop*60.0 / 10.0);
                    log.DebugFormat("Waiting for {0} minutes to go again", Statics.MinutesForMainLoop);
                    for (int i = 0; i < 10; i++)
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(partialDelay));//
                        if (ct.IsCancellationRequested)
                        {
                            break;
                        }
                    }
                    //await Task.Delay(TimeSpan.FromMinutes(FmsBlobToContinental.Statics.MinutesForMainLoop));
                }
                if (ct.IsCancellationRequested)
                {
                    log.Info("Cancelation requested");
                    break;
                }
                while (!Empty())
                {
                    log.DebugFormat("Waiting until empty. ");
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    if (ct.IsCancellationRequested)
                    {
                        break;
                    }
                }
            }
        }


        private int nItemsInQueues = 0;
       // private int nPayloadsInQueues = 0;

        /// <summary>
        /// How many items a in the sendqueue
        /// </summary>
        /// <returns></returns>
        public string Text()
        {
            lock (vehicleSenderList)
            {
                nItemsInQueues = 0;
               // nPayloadsInQueues = 0;

                vehicleSenderList.Sort((x, y) => x.vehicleNumber.CompareTo(y.vehicleNumber));

                foreach (VehicleWithSendQueue vc in vehicleSenderList)
                {
                    nItemsInQueues += vc.blobItemQueueCount;
                   // nPayloadsInQueues += vc.PayloadQueueCount;
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
            return string.Format("{0:HH:mm:ss} Send TirePressure to Continental : {1} blobitems, in {2} vehiclequeues. {3}",
                DateTime.Now,
                nItemsInQueues,
                //nPayloadsInQueues,
                vehicleSenderList.Count, Getterinfo);
        }

        public Boolean Empty()
        {
            return (nItemsInQueues == 0);
        }


        /// <summary>
        /// Put a new blobitem in the vehicle's queue in order to send the data.
        /// </summary>
        /// <param name="blobItem"></param>
        public void DownloadAndEnqueueToSend(BlobItem blobItem)
        {
            BlobFileName blobFilename = new BlobFileName();
            blobFilename.Name = blobItem.Name;


            CCVehicle vehicle = Statics.vehicleList.GetOrAdd(blobFilename.vehicleNumber, blobFilename.AgentSerialNumber);
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