#define TTMDATAX
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

        private ConcurrentQueue<BlobItem> blobItemQueue = new ConcurrentQueue<BlobItem>(); // the main queue of incoming blobs, handled one by one. in DownloadBlobItem
        public int nFailed, nDownload, nSucces, nTotal;
        public Boolean useTimestampNow;
        public BlobContainerClient containerClient;
        public List<SensorData> sensorDataListSend = new List<SensorData>(); // List of all that sensors actualy have been send (or failed to send) to continental;
        public List<SensorData> sensorDataListAll = new List<SensorData>();  // List of all sensors. Refreshed with payload data for one blob, and then analysed for timeouts

        private DateTime _updateFailed = DateTime.MinValue;

        private const int RetryAfterFail = 60;//this number of minutes after a fail it's tried again


        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="ct"></param>
        public VehicleWithSendQueue(CancellationToken ct)
        {
            StartSenderTasks(ct);
        }

        public void StartSenderTasks(CancellationToken ct)
        {
            Task.Factory.StartNew(() => DownloadBlobItem(ct));
            //   Task.Factory.StartNew(() => SendPayloadToContinental());
        }


        /// <summary>
        /// Indicate the update to continental failed.
        /// see UpdateFailedRecently() 
        /// </summary>
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
                                    vehicleNumber, updateFailed, updateFailed.AddMinutes(RetryAfterFail));
                }
            }
        }

        /// <summary>
        /// Have we failed recently, if so, don't try again
        /// </summary>
        private Boolean UpdateFailedRecently()
        {
            if (DateTime.Now.Subtract(updateFailed).TotalMinutes > RetryAfterFail)
            {
                updateFailed = DateTime.MinValue;
                return false;
            }
            return true;
        }

        /// <summary>
        /// How many items are in the blob-queue to be handled.
        /// </summary>
        public int blobItemQueueCount
        {
            get
            {
                return blobItemQueue.Count;
            }
        }

        /// <summary>
        /// Enqueue one blobitem to be downlaoded later.
        /// </summary>
        /// <param name="blobItem"></param>        
        public void EnqueueBlobItemForDownload(BlobItem blobItem)
        {
            blobItemQueue.Enqueue(blobItem);
            // Feedback(string.Format("{0,3} : Enqueue   {1} {2}", vehicleNumber, blobItemQueue.Count, blobItem.Name));
        }


        /// <summary>
        /// Dequeue the blobitems download it and handle each payload to send to continental
        /// </summary>
        private void DownloadBlobItem(CancellationToken ct)
        {
            BlobItem blobItem;
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

                    HandleBlobItem(blobItem);
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
        /// Check all the payloads in the blobitem and see if we need to update Continental
        /// </summary>
        /// <param name="blobItem"></param>
        private void HandleBlobItem(BlobItem blobItem)
        {

            if (Statics.VehiclesShowingDownloadTrace.Contains(iVehicleNumber))
            {
                log.DebugFormat("Vehicle {0,3} :  BlobQueueLen={1,2} PayloadQuelen={2,3}",
                    vehicleNumber,
                    blobItemQueue.Count);
            }
            try
            {
                nDownload++;
                BlobClient blobClient = containerClient.GetBlobClient(blobItem.Name);
                BlobDownloadResult x = blobClient.DownloadContent();
                string str = x.Content.ToString();
                str = str.Replace("{\"ts\"", "\r\n{\"ts\"");

                CloudFmsRootObject rootobject = RR_Serialize.JSON.FromString<CloudFmsRootObject>(str);
                //TODO : VehicleNo moet uit AQAD komen en niet uit het rootobject.
                CCVehicle vehicle = Statics.vehicleList.GetOrAdd(rootobject.vehicle.vehicleNo, rootobject.agentSerial);
                {
                    Boolean ok = true;
                    List<Payload> payloadList = rootobject.payload.ToList().FindAll(P => P.HasSensorData && P.HasPressureInfo());
                    //+
                    //--- Get every payload (having sensordata) update now if change or alarm detected.
                    //--- update via timeout afterwards.
                    //-
                    foreach (Payload payload in payloadList)
                    {
                        payload.vehicle = vehicle;
                        UpdateSensorDataListViaPayload(payload);
                        if (UpdateFailedRecently())  // code right below
                        {
                            break;
                        }
                    }
                    //   log.InfoFormat("Veh={0,4} Send timeout date from blob {1}", vehicleNumber, blobItem.Name);
                    if (!UpdateFailedRecently())
                    {
                        SendAllSensorForTimeout();  // code below right below
                    }
                    nSucces++;
                }
            }
            catch (Exception ex)
            {
                log.Warn("Blobfilename " + blobItem.Name, ex);
                nFailed++;
            }
        }

        /// <summary>
        /// Update all sensordata and send directly if something important changed, but NOT when a timeout occured that will be handled later.
        /// </summary>
        private Boolean UpdateSensorDataListViaPayload(Payload payLoad)
        {
            Boolean SendResult = SendMasterDataToContinental(useTimestampNow); //TODO RENE: raar moment en rare plek voor die code.
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
            //--- now handle the sensordata
            //--- send any new or alarm (significatnChange) data directly to Continental, but wait for the TIMEOUT records.
            //---
            //-
            foreach (SensorData sensorData in payLoad.sensorsDataList)  // sensorsInThisPayload)
            {
                SensorData latestSendSensorData = this.sensorDataListSend.Find(S => S.location == sensorData.location);
                //+
                //--- copy some fields from the previous sensordata because not all PGN's contain the same data.
                //-
                sensorData.CopyDataFromPrev(latestSendSensorData);
                //NEW NIET MEER : newSensorData.EnrichWithTTM();// even alle TTM nogmaals data over de andere data heenschrijven.
                if (useTimestampNow && sensorData.ses != Statics.UNKNOWN)
                {
                    sensorData.why = "TIMENOW";
                }
                if (latestSendSensorData == null && sensorData.ses != Statics.UNKNOWN && sensorData.SidOk())
                {
                    sensorData.why = "NEW";
                    sensorData.doSendData = true;
                }
                if (latestSendSensorData != null)
                {
                    if (sensorData.SignificantChange(latestSendSensorData, out sensorData.why) && sensorData.SidOk())
                    {
                        log.DebugFormat("NEW {1} {0}", sensorData.Text(), sensorData.why);
                        log.DebugFormat("OLD {1} {0}", latestSendSensorData.Text(), sensorData.why);
                        sensorData.doSendData = true;
                    }
                }
                //+
                //--- als er iets belangrijks gebeurd is, dan sturen we de data direct.
                //-
                if (sensorData.doSendData)
                {
                    SendSensorDataToContinental(sensorData, useTimestampNow);
                    Statics.ReplaceSensorInList(latestSendSensorData, sensorData);
                    if (UpdateFailedRecently())
                    {
                        // als er iets fout ging, dan stoppen we met deze hele sensordatalist.
                        break;
                    }
                }

                //+
                //--- replace sensorDataList with this one
                //-
                SensorData existingSensorinAll = this.sensorDataListAll.Find(S => S.location == sensorData.location);
                if (existingSensorinAll != null)
                {
                    this.sensorDataListSend.Remove(existingSensorinAll);
                }

                this.sensorDataListAll.Add(sensorData);

            }
            return true;
        }

        /// <summary>
        /// Nieuwe routine!
        /// Update all sensordata via timeout. Called Once after each blob has been processed completely.
        /// </summary>
        private void SendAllSensorForTimeout()
        {
            foreach (SensorData sensorData in this.sensorDataListAll.FindAll(S => S.SidOk() && S.ses != Statics.UNKNOWN))
            {
                SensorData latestSendSensorData = this.sensorDataListSend.Find(S => S.location == sensorData.location);
                if (latestSendSensorData != null)
                {
                    if (latestSendSensorData.timestamp.AddMinutes(Statics.MinutesForIgnoreSensorDataAfterDeserializing) < sensorData.timestamp)
                    {
                        sensorData.why = String.Format("TIME {0:HH:mm} was {1:HH:mm} ", sensorData.timestamp, latestSendSensorData.timestamp);
                        SendSensorDataToContinental(sensorData, useTimestampNow);
                        if (UpdateFailedRecently())
                        {
                            break;
                        }
                    }
                }

            }
        }

        /// <summary>
        /// Actually send the sensor data to continental.
        /// </summary>
        /// <param name="sensorData"></param>
        /// <param name="useTimeStampNow"></param>
        /// <returns></returns>
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

            //+
            //--- replace sensorDataList with this one
            //-
            SensorData latestSendSensorData = this.sensorDataListSend.Find(S => S.location == sensorData.location);
            if (latestSendSensorData != null)
            {
                this.sensorDataListSend.Remove(latestSendSensorData);
            }
            if (sensorData.ses == Statics.UNKNOWN)
            {
                int magniet = 09;
            }
            this.sensorDataListSend.Add(sensorData);

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
        /// Send the masterdata to continental is something chnanged, and update the vehice.masterdataok boolean.
        /// </summary>
        /// <param name="useTimeStampNow"></param>
        /// <returns></returns>
        public Boolean SendMasterDataToContinental(Boolean useTimeStampNow)
        {
            Boolean succesfull = true;
            string sJson;
            /// <summary>
            /// Send the metadata and gps to continental
            /// </summary>
            /// <param name="useTimeStampNow"></param>
            /// <returns></returns>

            //+
            //--- get sensor master data
            //-

            Boolean metadataChanged = false;
            string why = "";

#if !TTMDATA
            foreach (SensorData sensorData in this.sensorDataListAll)  // was eerder  //.FindAll(S => S.ttmData != null))
            {
                SensorMasterData smd = GetOrAddSensorMasterData(sensorData.sid.ToString(), sensorData.graphicalPosition.ToString("X"),
                    out Boolean tmpMetaDataChanged, out string tmpWhy);

                metadataChanged = metadataChanged || tmpMetaDataChanged;
                why += " " + tmpWhy;

            }
#endif

#if TTMDATA
            foreach (TTMData ttmData in this.ttmDataList)
        {
            SensorMasterData smd = vehicle.GetOrAddSensorMasterData(ttmData.TTMID.ToString(), ttmData.GraphicalPosition.ToString("X"),
                out Boolean tmpMetaDataChanged, out string tmpWhy);
            metadataChanged = metadataChanged || tmpMetaDataChanged;
            why += " " + tmpWhy;
        }
#endif

            if (metadataChanged)
            {
                //+
                //---  send sensor master data, only if changes in sensores were made
                //-
                IRestResponse response = SendMD(out sJson);
                //1111

                if (Statics.DetailedContiLogging)
                {
                    log.DebugFormat("Veh={0,4} MasterDataUpdate  Url={1} Status={2} {3}",
                        vehicleNumber,
                        clientMd().BaseUrl,
                        response.StatusCode, why);

#if TTMDATA  //defined in 1st line of this document

                foreach (TTMData ttmData in this.ttmDataList)
                {
                    log.DebugFormat("Veh={0,4} pos={1,2} Sid={2} SidHex={2:X}",
                        vehicleNr, ttmData.tireLocation, ttmData.TTMID);
                }
#else
                    foreach (SensorData sensorData in this.sensorDataListAll)
                    {
                        log.DebugFormat("Veh={0,4} pos={1,2} Sid={2} SidHex={2:X}",
                            vehicleNumber, sensorData.location, sensorData.sid);
                    }
#endif
                    log.DebugFormat("Json={0}",
                    sJson.Replace("\r\n", ""));

                }
                succesfull = response.IsSuccessful && succesfull;
            }
            masterdataOk = succesfull;
            return succesfull;
        }

    }
}
