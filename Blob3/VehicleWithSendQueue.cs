using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using BlobHandler;
using FmsBlobToContinental;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Statics = FmsBlobToContinental.Statics;

namespace Blob3
{
    /// <summary>
    /// A vehicle with a queue to download blob items and a queue to send the payload items to continental
    /// </summary>
    public class VehicleWithSendQueue : CCVehicle
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [XmlIgnore]
        private ConcurrentQueue<Payload> payloadQueue = new ConcurrentQueue<Payload>();

        private ConcurrentQueue<BlobItem> blobItemQueue = new ConcurrentQueue<BlobItem>();
        public Boolean useTimestampNow;
        public BlobContainerClient containerClient;
        [XmlIgnore]
        public List<SensorData> sensorDataList = new List<SensorData>();

        public VehicleWithSendQueue()
        {
            StartSenderTasks();
        }

        public int blobItemQueueCount
        {
            get
            {
                return blobItemQueue.Count;
            }
        }
        public int PayloadQueueCount
        {
            get
            {
                return payloadQueue.Count;
            }
        }

        public void EnqueuePayload(Payload payload)
        {
            payloadQueue.Enqueue(payload);
        }

        public void EnqueueBlobItemForDownload(BlobItem blobItem)
        {
            blobItemQueue.Enqueue(blobItem);
            // Feedback(string.Format("{0,3} : Enqueue   {1} {2}", vehicleNumber, blobItemQueue.Count, blobItem.Name));
        }

        public void StartSenderTasks()
        {
            Task.Factory.StartNew(() => DownloadBlobItem());
            Task.Factory.StartNew(() => SendPayloadToContinental());
        }

        public void WriteStatusToFeedback()
        {
            log.Debug(string.Format("Vehicle {0,3} :  BlobQueueLen={1,2} PayloadQuelen={2,3}",
                           vehicleNumber,
                           blobItemQueue.Count,
                           PayloadQueueCount));
        }

        /// <summary>
        /// download the blobitem en enqueue each payload to send to continental
        /// </summary>
        private void DownloadBlobItem()
        {
            BlobItem blobItem;
            CCVehicle vehicle;
            while (true)
            {
                // Thread.Sleep(TimeSpan.FromMilliseconds(10));// heel even rustig aan.

                if (blobItemQueue.TryDequeue(out blobItem))
                {
                    //+
                    //--- NIET ignoren want er kan een alarm in zitten.
                    //-

                    /** 
                    if (allSensorsUpToDate())
                    {
                        log.DebugFormat("{0}: All up-to-date Ignore blob", this.vehicleNumber);
                        continue;// alles is up-to-date..negeren die hap
                    }
                    **/

                    //  Feedback(string.Format("{0,3} : Dequeue   {1} {2}", vehicleNumber, blobItemQueue.Count, blobItem.Name));
                    //+
                    //--- download to string
                    //-

                    BlobClient blobClient = containerClient.GetBlobClient(blobItem.Name);
                    BlobDownloadResult x = blobClient.DownloadContent();
                    string str = x.Content.ToString();
                    str = str.Replace("{\"ts\"", "\r\n{\"ts\"");

                    if (Statics.VehiclesShowingDownloadTrace.Contains(iVehicleNumber))
                    {
                        log.DebugFormat("Vehicle {0,3} :  BlobQueueLen={1,2} PayloadQuelen={2,3}",
                            vehicleNumber,
                            blobItemQueue.Count,
                            PayloadQueueCount);
                    }
                    try
                    {
                        CloudFmsRootObject rootobject = RR_Serialize.JSON.FromString<CloudFmsRootObject>(str);
                        //TODO : VehicleNo moet uit AQAD komen en niet uit het rootobject.
                        vehicle = Statics.vehicleList.GetOrAdd(rootobject.vehicle.vehicleNo, rootobject.agentSerial);
                        {
                            Boolean ok = true;
                            List<Payload> payloadList = rootobject.payload.ToList().FindAll(P => P.HasSensorData && P.HasPressureInfo());
                            //+
                            //--- put every payload in this blob into the payloadStack.
                            //--- to be sure the last one is realy in position 0 lock the stack first 
                            //--- so it cannot be read in the SendPayloadToContinental where the first one is handled 
                            //-
                            foreach (Payload payload in payloadList)
                            {
                                payload.vehicle = vehicle;
                                EnqueuePayload(payload);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        log.WarnFormat("Oops for {0} {1}", blobItem.Name, ex.Message);
                    }
                }
                else
                {
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                }
            }
        }

        /// <summary>
        /// Have a look if all sensordata's are very young
        /// </summary>
        /// <returns></returns>
        private Boolean allSensorsUpToDate()
        {
            if (sensorDataList.Count < 6 && vehicleNumber != "1111")
            {
                return false;
            }
            if (sensorDataList.Count < 4 && vehicleNumber == "1111")
            {
                return false;
            }
            Boolean result = true;
            foreach (SensorData sensorData in this.sensorDataList)
            {
                if (!sensorData.UpToDate())
                {
                    result = false;
                    break;
                }
            }
            return result;
        }


        /// <summary>
        /// Handle the payloadQueue and send all data to continental
        /// </summary>
        private void SendPayloadToContinental()
        {
            Payload payLoad = null;
            try
            {
                while (true)
                {
                    if (payloadQueue.TryDequeue(out payLoad)) //VehicleWithSendQueu main playload loop
                    {
                        //+
                        //--- als we 6 sensors kennen en ze zijn allemaal up-to-date, dan direct flushen
                        //-

                        /** NEE niet flushen, er kan een alarm in zitten.
                        if (allSensorsUpToDate())
                        {
                            log.InfoFormat("{0}: All up-to-date flush the payload", this.vehicleNumber);
                            FlushPayLoadQueue();
                            continue;
                        }**/

                        List<SensorData> sensorsInThisPayload;
                        if (payloadQueue.Count == 0)
                        {
                            //+
                            //--- this is the last one in the queue, we will always send that one.
                            //-
                            Boolean last = true;
                        }
                        Boolean SendResult = payLoad.SendToContinentalIfNeeded(useTimestampNow, out sensorsInThisPayload);
                        if (!SendResult)
                        {
                            //+
                            //--- sending went wrong, flush the rest.
                            log.DebugFormat("{0}: SendToContinental failed.. flush the payload and the blobQueue", this.vehicleNumber);
                            FlushBlobQueue();
                            FlushPayLoadQueue();
                            continue;
                        }
                        else // SendResult == true, sending went OK
                        {
                            //+
                            //--- upload OK, lets see for which sensor this was, update to administrate there timestamps.
                            //-
                            int i = sensorsInThisPayload.Count;
                            foreach (SensorData newSensorData in sensorsInThisPayload)
                            {
                                SensorData sensorDataInVehicle = this.sensorDataList.Find(S => S.location == newSensorData.location);
                                if (sensorDataInVehicle != null)
                                {
                                    this.sensorDataList.Remove(sensorDataInVehicle);
                                }
                                this.sensorDataList.Add(newSensorData);
                            }
                        }
                    }
                    else
                    {
                        Thread.Sleep(TimeSpan.FromMilliseconds(10));
                    }
                }
            }
            catch (Exception ex)
            {
                log.WarnFormat("oops for vehicle {0} : {1}", vehicleNumber, RR.Lib.ExceptionInfoStackTrace(ex));
                FlushBlobQueue();
                FlushPayLoadQueue();
            }
        }

        /// <summary>
        /// Flush all the payloadqueue, something was wrong in there..
        /// </summary>
        public void FlushPayLoadQueue()
        {
            //+
            //--- flush the queue for for now
            //-
            Payload payLoad;
            while (payloadQueue.TryDequeue(out payLoad)) // FLUSH
            {
                ;// just flush it.
            }
        }

        /// <summary>
        /// Flush all the blobqueued, somthing was wrong so skip the hole thing.
        /// </summary>
        public void FlushBlobQueue()
        {
            BlobItem blobItem;
            while (blobItemQueue.TryDequeue(out blobItem))
            {
            }
        }
    }

    public class VehicleSenderList : List<VehicleWithSendQueue>
    {
        Boolean useTimestampNow = false;
        public void SetUpdateNow(Boolean useTimestampNow)
        {
            this.useTimestampNow = useTimestampNow;
            foreach (VehicleWithSendQueue v in this)
            {
                v.useTimestampNow = this.useTimestampNow;
            }
        }

        public VehicleWithSendQueue GetOrAdd(int vehicleNumber, BlobContainerClient containerClient)
        {
            lock (this)
            {
                VehicleWithSendQueue result = this.Find(V => V.vehicleNumber == vehicleNumber.ToString());
                if (result == null)
                {
                    result = new VehicleWithSendQueue
                    {
                        vehicleNumber = vehicleNumber.ToString(),
                        containerClient = containerClient,
                        useTimestampNow = this.useTimestampNow
                    };
                    this.Add(result);
                }
                return result;
            }
        }

    }
}
