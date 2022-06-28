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
using System.Windows.Forms;
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
        public Boolean running = false;
        CancellationToken ct;

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
        /// Get Testprod. 
        /// Set Testprod en thus TestProd for all vehicles in the vehicleSenderList.
        /// </summary>
        public TestProd TestProd
        {
            get
            {
                return vehicleSenderList.TestProd;
            }
            set
            {
                vehicleSenderList.TestProd = value;
            }
        }

        /*****
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
                        log.Debug("DoLoop A Start");
                        while (!Empty())
                        {
                            log.Debug("DoLoop B !Empty"); 
                            await Task.Delay(TimeSpan.FromSeconds(1));
                        }
                        if (ct.IsCancellationRequested)
                        {
                            break;
                        }

                        log.Debug("DoLoop C Empty");
                        for (int i = 0; i < 10; i++)
                        {
                            log.DebugFormat("DoLoop D {i}",i+1);
                            await Task.Delay(TimeSpan.FromSeconds(Statics.MinutesForMainLoop / 10)); // 
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
        */



        /// <summary>
        /// Update to continental in a loop for the pas half hour. Used in the form for testing
        /// </summary>
        /// <param name="testProd"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task DoNowInALoopAsych(TestProd testProd, string onlyVehicle, CancellationToken ct, DateTime? paramSince)
        {
            try
            {
                RestartAllVehicles(ct);
                running = true;
                //List<BlobItem> items = new List<BlobItem>();
                containerClient = BlobHandler.Statics.GetContainerClient(testProd);
                this.ct = ct;

                while (!ct.IsCancellationRequested)
                {
                    DateTime until = StartOneRun(paramSince);
                    BlobHandler.BlobItemList blobItemlist = new BlobHandler.BlobItemList(DownloadAndEnqueueToSend);
                    int nItems = 0;
                    nItems = await blobItemlist.GetBlobItemsASync(
                        testProd,
                        since,
                        until,
                        downloadToFile: false,
                        onlyVehicle: onlyVehicle,
                        ct);

                    DoneOneRun(nItems);
                    WaitForEmpty(ct);
                }
            }
            catch (Exception ex)
            {
                log.Error("", ex);
            }
            running = false;

        }

        /// <summary>
        /// Update to continental in a loop for the pas half hour. Used in the service.
        /// </summary>
        /// <param name="testProd"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public void DoNowInALoop(TestProd testProd, string onlyVehicle, CancellationToken ct, DateTime? paramSince = null)
        {
            try
            {
                containerClient = BlobHandler.Statics.GetContainerClient(testProd);
                vehicleSenderList.TestProd = testProd;
                RestartAllVehicles(ct);
                running = true;
                //List<BlobItem> items = new List<BlobItem>();
                this.ct = ct;

                while (!ct.IsCancellationRequested)
                {
                    DateTime until = StartOneRun(paramSince);
                    BlobHandler.BlobItemList blobItemlist = new BlobHandler.BlobItemList(DownloadAndEnqueueToSend);
                    int nItems;

                    nItems = blobItemlist.GetBlobItems(
                        testProd,
                        since,
                        until,
                        downloadToFile: false,
                        onlyVehicle: onlyVehicle,
                        ct);
                    //  nItems = items.Count;
                    // items.Clear();
                    DoneOneRun(nItems);
                    WaitForEmpty(ct);
                }
            }
            catch (Exception ex)
            {
                log.Error("", ex);
            }
            running = false;
            log.DebugFormat("Done in a Loop");
        }


        private void RestartAllVehicles(CancellationToken ct)
        {
            foreach (VehicleWithSendQueue vehicle in vehicleSenderList)
            {
                vehicle.StartSenderTasks(ct);
            }
        }

        private DateTime StartOneRun(DateTime? paramSince)
        {
            log.DebugFormat("StartOneRun");
            getterStartTime = DateTime.Now;
            getterStopWatch.Restart();

            if (paramSince != null)
            {
                since = (DateTime)paramSince;
            }
            else
            {
                since = DateTime.Now.ToUniversalTime().AddMinutes(-30);
            }
            DateTime until = since.AddDays(1);

            Statics.SaveSensorList();
            Statics.SaveVehicleList();
            return until;
        }

        private void DoneOneRun(int nItems)
        {
            getterStopWatch.Stop();
            getterAmmount = nItems;

        }

        private void WaitForEmpty(CancellationToken ct)
        {
            try
            {
                if (!ct.IsCancellationRequested)
                {
                    int partialDelay = (int)(Statics.MinutesForMainLoop * 60.0 / 10.0);
                    log.InfoFormat("Waiting for {0} minutes to go again in 10 steps of {1} seconds. nItmes in queue = {2} mem={3}",
                        Statics.MinutesForMainLoop,
                        partialDelay,
                        ItemsInQueue(),
                        RR.Lib.MemoryUsageStringShort());
                    for (int i = 0; i < 10; i++)
                    {
                        MyWait(partialDelay);//
                        if (ct.IsCancellationRequested)
                        {
                            break;
                        }
                        log.DebugFormat("Step. nItmes in queue = {0} mem={1}", ItemsInQueue(), RR.Lib.MemoryUsageStringShort());
                    }
                    if (!ct.IsCancellationRequested)
                    {
                        log.DebugFormat("Waited for {0} minutes to go again", Statics.MinutesForMainLoop);
                    }
                }
                if (ct.IsCancellationRequested)
                {
                    log.Info("Cancelation requested");
                }

                if (Empty())
                {
                    log.DebugFormat("No need to wait. Queues are empty.");
                }

                while (!Empty())
                {
                    log.DebugFormat("Waiting until empty. ");
                    log.DebugFormat("Waiting until queue is empty. nItmes in queue = {0} mem = {1}", ItemsInQueue(), RR.Lib.MemoryUsageStringShort());

                    int n = ItemsInQueue();
                    MyWait(20);

                    //+
                    //--- debug: als er na 20 seconden wachten nog steeds even veel items in de queue zitten dan is er iets raars aan de hand\
                    //---        dus gaan we die even debuggen.
                    //-
                    if (ItemsInQueue() == n)
                    {
                        log.Warn("********************************** queue raakt niet leeg we maken alles leeg *****************************");
                        DebugQueueInfo();
                        FlushAllQueues();
                    }
                    if (ct.IsCancellationRequested)
                    {
                        break;
                    }
                }
                //log.DebugFormat("Done Waiting.");
            }
            catch (Exception ex)
            {
                log.Error("", ex);
            }
        }

        private void MyWait(int sec)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            //Thread.Sleep(TimeSpan.FromSeconds(sec));
            //Task.Delay(TimeSpan.FromSeconds(sec));
            while (sw.Elapsed.TotalMilliseconds < sec * 1000)
            {
                Application.DoEvents();
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
            return ItemsInQueue() == 0;
        }

        private DateTime counterEvaluated = DateTime.MinValue;
        int counter = 0;

        /// <summary>
        /// Count the total number of blobs in the queues of the vehicles
        /// </summary>
        /// <returns></returns>
        public int ItemsInQueue()
        {
            if (DateTime.Now.Subtract(counterEvaluated) > TimeSpan.FromSeconds(5))
            {
                counterEvaluated = DateTime.Now;
                counter = 0;
                foreach (VehicleWithSendQueue vc in vehicleSenderList)
                {
                    counter += vc.blobItemQueueCount;
                }
                //DebugQueueInfo();
            }
            return counter;
        }


        /// <summary>
        /// indicate the status of the vehiclequeues. (first 20)
        /// </summary>
        /// <returns></returns>
        private void DebugQueueInfo()
        {
            int i = 0;
            int t = 0;
            foreach (VehicleWithSendQueue vc in vehicleSenderList)
            {
                t += vc.blobItemQueueCount;
                log.Debug(vc.DebugQueueInfo(t));
                i++;
                if (i > 100)
                {
                    break;
                }
            }
        }

        private void FlushAllQueues()
        {
            foreach (VehicleWithSendQueue vc in vehicleSenderList)
            {
                vc.FlushQueue();
            }
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
            vehicle.testProd = this.TestProd;
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
                VehicleWithSendQueue vehicleSender = vehicleSenderList.GetOrAdd(vehicle, containerClient, ct);
                //-
                //--- enqueue the blobitem to the vehicleSender
                //-
                vehicleSender.EnqueueBlobItemForDownload(blobItem);
            }
            blobFilename = null;
        }


    }
}