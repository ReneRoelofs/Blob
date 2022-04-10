using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using BlobHandler;
using FmsBlobToContinental;
using RestSharp;
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

     //   [XmlIgnore]
     //   private ConcurrentQueue<Payload> payloadQueue = new ConcurrentQueue<Payload>();

        private ConcurrentQueue<BlobItem> blobItemQueue = new ConcurrentQueue<BlobItem>(); // the main queue of incoming blobs, handled one by one. in DownloadBlobItem
        public Boolean useTimestampNow;
        public BlobContainerClient containerClient;
        [XmlIgnore]
        public List<SensorData> sensorDataList = new List<SensorData>();



        private DateTime _updateFailed = DateTime.MinValue;

        private DateTime updateFailed
        {
            get 
            {
                return _updateFailed;
            }
            set
            {
                _updateFailed = value;
                if (value != DateTime.MinValue)
                {
                    log.DebugFormat("Vehicle {0,3} :  Update Failed at ={1:HH:mm} retry from {2:HH:mm}",
                                    vehicleNumber, updateFailed, updateFailed.AddMinutes(60));
                }
            }
        }

        private Boolean UpdateFailedRecently()
        {
            if (DateTime.Now.Subtract(updateFailed).TotalMinutes > 60)
            {
                updateFailed = DateTime.MinValue;
                return false;
            }
            return true;
        }

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
        //public int PayloadQueueCount
        //{
        //    get
        //    {
        //        return payloadQueue.Count;
        //    }
        //}

        //public void EnqueuePayload(Payload payload)
        //{
        //    payloadQueue.Enqueue(payload);
        //}

        public void EnqueueBlobItemForDownload(BlobItem blobItem)
        {
            blobItemQueue.Enqueue(blobItem);
            // Feedback(string.Format("{0,3} : Enqueue   {1} {2}", vehicleNumber, blobItemQueue.Count, blobItem.Name));
        }

        public void StartSenderTasks()
        {
            Task.Factory.StartNew(() => DownloadBlobItem());
         //   Task.Factory.StartNew(() => SendPayloadToContinental());
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
                    if (UpdateFailedRecently())
                    {
                        //log.DebugFormat("Vehicle {0,3} :  Update Failed retry at {2:HH:mm}",
                        //    vehicleNumber, updateFailed, updateFailed.AddMinutes(60));
                        continue;
                    }

                    //+
                    //--- inv: update not failed or failed longer then 60 min ago
                    //-
                    BlobClient blobClient = containerClient.GetBlobClient(blobItem.Name);
                    BlobDownloadResult x = blobClient.DownloadContent();
                    string str = x.Content.ToString();
                    str = str.Replace("{\"ts\"", "\r\n{\"ts\"");

                    if (Statics.VehiclesShowingDownloadTrace.Contains(iVehicleNumber))
                    {
                        log.DebugFormat("Vehicle {0,3} :  BlobQueueLen={1,2} PayloadQuelen={2,3}",
                            vehicleNumber,
                            blobItemQueue.Count);
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
                            //  log.InfoFormat("Veh={0,4} Send significant changed from blob {1}", vehicleNumber, blobItem.Name);
                            foreach (Payload payload in payloadList)
                            {
                                payload.vehicle = vehicle;
                                UpdateSensorDataListViaPayload(payload);

                                if (UpdateFailedRecently())
                                {
                                    break;
                                }
                            }
                            //   log.InfoFormat("Veh={0,4} Send timeout date from blob {1}", vehicleNumber, blobItem.Name);
                            SendAllSensorForTimeout();
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
        /// Nieuwe routine!
        /// Update all sensordata and send directly if something important changed, but NOT when a timeout occured that will be handled later.
        /// </summary>
        private Boolean UpdateSensorDataListViaPayload(Payload payLoad)
        {
            Boolean SendResult = payLoad.SendMasterDataToContinental(useTimestampNow);
            if (!SendResult)
            {
                //+
                //--- sending went wrong, flush the rest.
                //-
                updateFailed = DateTime.Now;
                //log.DebugFormat("{0}: SendToMasterdataToContinental failed.. flush the blobQueue for this vehicle", this.vehicleNumber);
                //FlushBlobQueue();
                //FlushPayLoadQueue();
                return false;
            }
            //+
            //--- inv: Masterdata for this vehicle is ok.
            //---      now get the sensordata
            //-
            List<SensorData> sensorsInThisPayload;
            Boolean XGetListResult = payLoad.GetSensorsList(useTimestampNow, out sensorsInThisPayload);
            //+
            //--- send any new or alarm (significatnChange) data directly to Continental, but wait for the TIMEOUT records.
            //---
            //-
            foreach (SensorData newSensorData in sensorsInThisPayload)
            {
                SensorData sensorDataInVehicle = this.sensorDataList.Find(S => S.location == newSensorData.location);
                newSensorData.CopyDataFromPrev(sensorDataInVehicle);

                if (useTimestampNow)
                {
                    newSensorData.why = "TIMENOW";
                }
                if (sensorDataInVehicle == null)
                {
                    newSensorData.why = "NEW";
                    newSensorData.doSendData = true;
                }
                if (sensorDataInVehicle != null)
                {
                    if (newSensorData.SignificantChange(sensorDataInVehicle, out newSensorData.why))
                    {
                        // log.DebugFormat("NEW {1} {0}", sensorData.Text(),why);
                        // log.DebugFormat("OLD {1} {0}", prevSend.Text(),why);
                        newSensorData.doSendData = true;
                    }
                }
                //+
                //--- als er iets belangrijks gebeurd is, dan sturen we de data direct.
                //-
                if (newSensorData.doSendData)
                {
                    SendSensorDataToContinental(newSensorData, useTimestampNow);
                    Statics.ReplaceSensorInList(sensorDataInVehicle, newSensorData);
                    if (UpdateFailedRecently())
                    {
                        break;
                    }
                }
                //+
                //--- replace sensorDataList with this one.
                //-
                if (sensorDataInVehicle != null)
                {
                    this.sensorDataList.Remove(sensorDataInVehicle);
                }
                this.sensorDataList.Add(newSensorData);
            }
            return true;
        }

        /// <summary>
        /// Nieuwe routine!
        /// Update all sensordata via timeout. Called Once after each blob has been processed completely.
        /// </summary>
        private void SendAllSensorForTimeout()
        {
            foreach (SensorData sensorData in this.sensorDataList)
            {
                if (sensorData.timestampUploaded.AddMinutes(Statics.MinutesForIgnoreSensorDataAfterDeserializing) < sensorData.timestamp)
                {
                    sensorData.why = "TIME";
                    SendSensorDataToContinental(sensorData, useTimestampNow);

                    if (UpdateFailedRecently())
                    {
                        break;
                    }
                }
            }
        }

        private Boolean SendSensorDataToContinental(SensorData sensorData, Boolean useTimeStampNow)
        {
            var request = new CCRestRequest(Method.PUT, this.testProd);
            RestClient client = new CCRestClient(this.testProd, "sensorad/sensors/" + sensorData.sid);
            // update timestamp for debugging  sensorData.setTimestamp(DateTime.Now);
            if (useTimeStampNow)
            {
                sensorData.setTimestamp(DateTime.Now);
            }

            //+
            //--- Do send the updated sensordata to continental
            //-
            string sJson = sensorData.Json();
            request.SetBody(sJson);
            IRestResponse response = client.Execute(request);

            //+
            //--- ook als het mislukt is updaten we toch de timestamp van deze sensor anders blijven we aan de gang
            //-
            sensorData.timestampUploaded = sensorData.timestamp;

            if (response.IsSuccessful)
            {
                this.UpdateSimpleInfo(sensorData.location, sensorData.sidHex, sensorData.timestamp, sensorData.temperature, sensorData.pressure);
                log.InfoFormat("Veh={0,4} Loc={1,2} Why={2,4} Time={3} Url={4} Status={5} ",
                    this.vehicleNumber,
                    sensorData.location,
                    sensorData.why,
                    sensorData.timestamp,
                    client.BaseUrl,
                    response.StatusCode);
                if (Statics.DetailedContiLogging)
                {
                    log.DebugFormat("                Json={0}",
                        sJson.Replace("\r\n", ""));
                    log.DebugFormat("                Text={0}",
                        sensorData.Text());
                }
            }
            else // Not Response.IsSuccesfull // more feedback and no sensorupdate
            {
                this.updateFailed = DateTime.Now;
                log.WarnFormat("Veh={0,4} Loc={1,2} Why={2,4} Time={3} Url={4} SensorHex={5} Status={6} Response={7}",
                    this.vehicleNumber,
                    sensorData.location,
                    sensorData.why,
                    sensorData.timestamp,
                    client.BaseUrl,
                    sensorData.sidHex,
                    response.StatusCode,
                    response.Content);
                if (Statics.DetailedContiLogging)
                {
                    log.DebugFormat("                Json={0}",
                        sJson.Replace("\r\n", ""));
                    log.DebugFormat("                Text={0}",
                        sensorData.Text());
                    //FeedbackResponse(client, response);
                }

            }
            return response.IsSuccessful;

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
