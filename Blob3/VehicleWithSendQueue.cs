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
        public int nFailed, nDownload, nSucces, nTotal;
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
                    log.InfoFormat("Vehicle {0,3} :  Update Failed at ={1:HH:mm} retry from {2:HH:mm}",
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

        public VehicleWithSendQueue(CancellationToken ct)
        {
            StartSenderTasks(ct);
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

        public void StartSenderTasks(CancellationToken ct)
        {
            Task.Factory.StartNew(() => DownloadBlobItem(ct));
            //   Task.Factory.StartNew(() => SendPayloadToContinental());
        }

        /// <summary>
        /// download the blobitem en enqueue each payload to send to continental
        /// </summary>
        private void DownloadBlobItem(CancellationToken ct)
        {
            BlobItem blobItem;
            CCVehicle vehicle;
            while (!ct.IsCancellationRequested)
            {
                // Thread.Sleep(TimeSpan.FromMilliseconds(10));// heel even rustig aan.

                if (blobItemQueue.TryDequeue(out blobItem))
                {
                    //log.DebugFormat("Vehicle {0} I'm Alive {1} items left in queue", vehicleNumber, blobItemQueueCount);
                    nTotal++;
                    if (UpdateFailedRecently())
                    {
                        //log.DebugFormat("Vehicle {0,3} :  Update Failed retry at {2:HH:mm}",
                        //    vehicleNumber, updateFailed, updateFailed.AddMinutes(60));
                        nFailed++;
                        continue;
                    }

                    //+
                    //--- inv: update not failed or failed longer then 60 min ago
                    //-
                    BlobClient blobClient = containerClient.GetBlobClient(blobItem.Name);
                    BlobDownloadResult x = blobClient.DownloadContent();
                    nDownload++;
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
                            nSucces++;
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Warn("Blobfilename "+ blobItem.Name, ex);
                        nFailed++;
                    }
                    //+
                    //--- always relax a little bit
                    //-
                    Thread.Sleep(TimeSpan.FromMilliseconds(100));
                }
                else
                {
                   // log.DebugFormat("Vehicle {0} I'm asleep {1} items in queue", vehicleNumber,blobItemQueueCount);
                    Thread.Sleep(TimeSpan.FromSeconds(10));
                }
            }
            if (ct.IsCancellationRequested)
            {
                log.DebugFormat("Vehicle {0} I'm Canceled {1} items left in queue", vehicleNumber, blobItemQueueCount);
            }
            else
            {
                log.DebugFormat("Vehicle {0} I'm terminated without cancel should NOT happen", vehicleNumber, blobItemQueueCount);
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



            //  List<SensorData> sensorsInThisPayload;
            //  Boolean XGetListResult = payLoad.GetSensorsList(useTimestampNow, out sensorsInThisPayload);

            payLoad.EnrichSensorWithTTM();// even alle TTM data over de andere data heenschrijven.

            //+
            //--- send any new or alarm (significatnChange) data directly to Continental, but wait for the TIMEOUT records.
            //---
            //-
            foreach (SensorData newSensorData in payLoad.sensorsDataList)  // sensorsInThisPayload)
            {
                SensorData latestSendSensorData = this.sensorDataList.Find(S => S.location == newSensorData.location);
#if DEBUG
                if (newSensorData.ses == 2)
                {
                    int debug = 0;
                    debug++;
                }
#endif

                newSensorData.CopyDataFromPrev(latestSendSensorData);
                newSensorData.EnrichWithTTM();// even alle TTM nogmaals data over de andere data heenschrijven.

#if DEBUG
                if (latestSendSensorData != null && latestSendSensorData.ses == 2)
                {
                    int debug = 0;
                    debug++;
                }
#endif
#if DEBUG
                if (newSensorData.ses == 2)
                {
                    int debug = 0;
                    debug++;
                }
#endif
                if (useTimestampNow)
                {
                    newSensorData.why = "TIMENOW";
                }
                if (latestSendSensorData == null)
                {
                    newSensorData.why = "NEW";
                    if (newSensorData.SidOk())
                    {
                        newSensorData.doSendData = true;
                    }
                }
                if (latestSendSensorData != null)
                {
                    if (newSensorData.SignificantChange(latestSendSensorData, out newSensorData.why))
                    {
                        log.DebugFormat("NEW {1} {0}", newSensorData.Text(), newSensorData.why);
                        log.DebugFormat("OLD {1} {0}", latestSendSensorData.Text(), newSensorData.why);
                        if (newSensorData.SidOk())
                        {
                            newSensorData.doSendData = true;
                        }
                    }
                }
                //+
                //--- als er iets belangrijks gebeurd is, dan sturen we de data direct.
                //-
                if (newSensorData.doSendData)
                {
                    SendSensorDataToContinental(newSensorData, useTimestampNow);
#if DEBUG
                    if (newSensorData.ses == 2)
                    {
                        int debug = 0;
                        debug++;
                    }
#endif
                    Statics.ReplaceSensorInList(latestSendSensorData, newSensorData);
                    if (UpdateFailedRecently())
                    {
                        break;
                    }

                }
                //+
                //--- replace sensorDataList with this one.
                //-
                if (latestSendSensorData != null)
                {
                    this.sensorDataList.Remove(latestSendSensorData);
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
                if (sensorData.SidOk())
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
                log.InfoFormat("Veh={0,4} Loc={1,2} Time={3} Url={4} SensorHex={5} Status={6} Why={2,4} ",
                    this.vehicleNumber,
                    sensorData.location,
                    sensorData.why,
                    sensorData.timestamp,
                    client.BaseUrl,
                    sensorData.sidHex,
                    response.StatusCode);
                if (Statics.DetailedContiLogging)
                {
                    if (Statics.DetailedContiLoggingFilter == null || Statics.DetailedContiLoggingFilter.Contains(vehicleNumber))
                    {
                        log.DebugFormat("                Json={0}",
                            sJson.Replace("\r\n", ""));
                        log.DebugFormat("                Text={0}",
                            sensorData.Text());
                    }
                }
            }
            else // Not Response.IsSuccesfull // more feedback and no sensorupdate
            {
                this.updateFailed = DateTime.Now;
                log.WarnFormat("Veh={0,4} Loc={1,2} Time={3} Url={4} SensorHex={5} Status={6} Why={2,4} Response={7}",
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

        public VehicleWithSendQueue GetOrAdd(int vehicleNumber, BlobContainerClient containerClient,CancellationToken ct)
        {
            lock (this)
            {
                VehicleWithSendQueue result = this.Find(V => V.vehicleNumber == vehicleNumber.ToString());
                if (result == null)
                {
                    result = new VehicleWithSendQueue(ct)
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
