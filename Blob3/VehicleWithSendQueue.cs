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
    public class VehicleWithSendQueue
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private ConcurrentStack<BlobItem> blobItemStack = new ConcurrentStack<BlobItem>(); // the main queue of incoming blobs, handled one by one. in DownloadBlobItem

        //private ConcurrentStack<BlobItem> blobItemStack = new ConcurrentStack<BlobItem>();  // the main stack of incoming blobs. lastest pushed is popped first.

        public int nFailed, nDownload, nSucces, nTotal;
        public Boolean useTimestampNow;
        public BlobContainerClient containerClient;
        public List<SensorData> sensorDataListSend = new List<SensorData>(); // List of all that sensors actualy have been send (or failed to send) to continental;
        public List<SensorData> sensorDataListAll = new List<SensorData>();  // List of all sensors. Refreshed with payload data for one blob, and then analysed for timeouts

        private CCVehicle vehicle;

        private DateTime _updateFailed = DateTime.MinValue;
        private const int RetryAfterFail = 60;//this number of minutes after a fail it's tried again
        private BlobFileName blobFilename = new BlobFileName();
        public DateTime timestampOfLatestBlob = DateTime.MinValue;

        //+
        //--- wrappertjes rond vehicle
        //-

        public string vehicleNumber { get { return vehicle.vehicleNumber; } }
        public int iVehicleNumber { get { return vehicle.iVehicleNumber; } }
        public string agentSerialNumber { get { return vehicle.agentSerialNumber; } }
        public TestProd testProd { get { return vehicle.testProd; } set { vehicle.testProd = value; } }
        public GpsData gpsData { get { return vehicle.gpsData; } set { vehicle.gpsData = value; } }

        ///// <summary>
        ///// constructor
        ///// </summary>
        ///// <param name="ct"></param>
        //public VehicleWithSendQueue(CancellationToken ct, TestProd testProd)
        //{
        //    this.testProd = testProd;
        //    StartSenderTasks(ct);
        //}

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="ct"></param>
        public VehicleWithSendQueue(CCVehicle vehicle, BlobContainerClient containerClient, CancellationToken ct)
        {
            this.vehicle = vehicle;
            this.containerClient = containerClient;
            StartSenderTasks(ct);

        }

        private void LogMemUsage(string tag)
        {
            string s = RR_Serialize.JSON.ToString<VehicleWithSendQueue>(this);
            log.InfoFormat("Mem usage Veh={0} {1} : {2} blobitemqueueLen={3} sensorDataListSendLen={4} sensorDataListAll={5}",
                vehicleNumber, tag, s.Length, blobItemStack.Count, sensorDataListSend.Count, sensorDataListAll.Count);
        }


        Task senderTask;
        public void StartSenderTasks(CancellationToken ct)
        {
            if (senderTask == null || senderTask.IsCompleted)
            {
                senderTask = Task.Factory.StartNew(() => DownloadBlobItem(ct));
            }
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
                return blobItemStack.Count;
            }
        }

        /// <summary>
        /// Enqueue one blobitem to be downlaoded later.
        /// </summary>
        /// <param name="blobItem"></param>        
        public void EnqueueBlobItemForDownload(BlobItem blobItem)
        {
            //blobItemStack.Enqueue(blobItem);
            blobItemStack.Push(blobItem);
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

                //if (blobItemStack.TryDequeue(out blobItem))
                if (blobItemStack.TryPop(out blobItem))
                {

                    blobFilename.Name = blobItem.Name; // set the filename and filedate
                    if (blobFilename.timeStamp > timestampOfLatestBlob)
                    {
                        timestampOfLatestBlob = blobFilename.timeStamp;
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
                }
                else
                {
                    // log.DebugFormat("Vehicle {0} I'm asleep {1} items in queue", vehicleNumber,blobItemQueueCount);
                    Thread.Sleep(TimeSpan.FromSeconds(10));
                }
            }
            if (ct.IsCancellationRequested)
            {
                //flush the queue
                log.DebugFormat("Vehicle {0} I'm Canceled. Flushing {1} queued items", vehicleNumber, blobItemQueueCount);
                FlushQueue();
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
                    blobItemStack.Count);
            }
            try
            {
                nDownload++;
                BlobClient blobClient = containerClient.GetBlobClient(blobItem.Name);
                BlobDownloadResult blobDownloadResult = blobClient.DownloadContent();

                string str = blobDownloadResult.Content.ToString();
                str = str.Replace("{\"ts\"", "\r\n{\"ts\"");

                CloudFmsRootObject rootobject = RR_Serialize.JSON.FromString<CloudFmsRootObject>(str);
                //TODO : VehicleNo moet uit AQAD komen en niet uit het rootobject.


                if (vehicle.agentSerialNumber != rootobject.agentSerial && !string.IsNullOrEmpty(rootobject.agentSerial))
                {
                    vehicle.agentSerialNumber = rootobject.agentSerial;
                    vehicle.sensorsMasterDataList.SetNeedResending(vehicle.sensorsMasterDataList.EnoughSensors(), "agentSerialNumberChanged");
                }

                Boolean ok = true;

                List<Payload> payloadList = rootobject.payload.ToList().FindAll(P => P.HasSensorData && P.HasPressureInfo());
                //List<Payload> payloadList = rootobject.payload.ToList(); //  niet alleeh HasPressure want we willen ook gps data  // .FindAll(P => P.HasSensorData && P.HasPressureInfo());

                //+
                //--- Get every payload (having sensordata) update now if change or alarm detected.
                //--- update via timeout afterwards.
                //-
                foreach (Payload payload in payloadList)
                {
                    SendMasterDataToContinental(useTimestampNow);  // should happen max 1x per blobitem i.e. per payloadlist.
                    if (UpdateFailedRecently())  // code right below
                    {
                        break;
                    }

                    payload.vehicle = this.vehicle;
                    UpdateSensorDataListViaPayload(payload);  // and thus update the masterdatalist if needed.
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
                //+
                //--- memory cleanup needed??
                //-
                blobDownloadResult = null;
                blobClient = null;
                str = null;
                rootobject = null;
                payloadList = null;
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
            if (payLoad.lat != 0 && payLoad.lat != null)
            {
                vehicle.SetLatLon(payLoad.lat, payLoad.lon, payLoad.ts);
            }

            //   LogMemUsage("1");
            foreach (SensorData sensorData in payLoad.sensorsDataList)  // sensorsInThisPayload)
            {
                //+
                //--- replace sensorDataList with this one
                //-
                SensorData existingSensorinAll = this.sensorDataListAll.Find(S => S.location == sensorData.location);
                if (existingSensorinAll != null)
                {
                    this.sensorDataListSend.Remove(existingSensorinAll);
                    this.sensorDataListAll.Remove(existingSensorinAll);
                }
                this.sensorDataListAll.Add(sensorData);

                //+
                //--- update the sensorMasterDatalist and set the 
                //--- NeedsResending flag if something changed there
                //-

                vehicle.GetOrAddSensorMasterData(sensorData.uSid, sensorData.graphicalPosition.ToString("X"));

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
            }

            // LogMemUsage("2");
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
            //--- Do send gps data to continental
            //-
            if (vehicle.gpsData != null)
            {
                SendGPSData(useTimeStampNow, sensorData.timestamp, vehicle.gpsData.lat, vehicle.gpsData.lon);
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
                vehicle.UpdateSimpleInfo(sensorData.location, sensorData.sidHex, sensorData.timestamp, sensorData.temperature, sensorData.pressure);
                log.DebugFormat("Mem={0} Veh={1,4} Loc={2,2} Time={3} Url={4} SensorHex={5} Status={6} Why={7,4} ",
                    RR.Lib.MemoryUsageStringShort(),
                    this.vehicleNumber,
                    sensorData.location,
                    sensorData.timestamp,
                    client.BaseUrl,
                    sensorData.sidHex,
                    response.StatusCode,
                    sensorData.why
                );
                if (Statics.IsVehicleDebugged(vehicleNumber))
                {
                    log.DebugFormat("                Json={0}",
                        sJson.Replace("\r\n", ""));
                    //log.DebugFormat("                Text={0}",
                    //    sensorData.Text());
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
                    Statics.FeedbackResponse(client, response);
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
            if (agentSerialNumber != null)
            {
                Boolean succesfull = true;
                if (vehicle.sensorsMasterDataList.NeedsResending())
                {
                    //+
                    //---  send sensor master data, only if changes in sensores were made
                    //-
                    IRestResponse response = vehicle.SendMD();
                    succesfull = response.IsSuccessful && succesfull;
                    FmsBlobToContinental.Statics.SaveVehicleList();
                }
                if (!succesfull)
                {
                    updateFailed = DateTime.Now;
                }
                return succesfull;
            }
            else
            {
                log.WarnFormat("AgentSerialNumber unknown for vehicle {0}", vehicleNumber);
                //opnieuw sturen als we wel genoeg sensoren kennen.
                vehicle.sensorsMasterDataList.SetNeedResending(vehicle.sensorsMasterDataList.EnoughSensors(), "AgentSerialNumber unknown");
                updateFailed = DateTime.Now;
                return false;
            }
        }


        private Boolean SendGPSData(Boolean useTimeStampNow, DateTime? ts, double lat, double lon)
        {
            Boolean succesfull = true;
            string sJson;
            //+
            //--- create and send GPS data
            //-
            if (gpsData == null)
            {
                gpsData = new GpsData();
            }

            if (lat != null && lon != null)
            {
                gpsData.lat = (double)lat;
                gpsData.lon = (double)lon;
                gpsData.setTimestamp((DateTime)ts);
                if (useTimeStampNow)
                {
                    gpsData.setTimestamp(DateTime.Now);
                }
            }
            IRestResponse responsegps = vehicle.SendGPS();
            if (Statics.DetailedContiLogging && responsegps.StatusCode != System.Net.HttpStatusCode.OK)
            {
                Statics.FeedbackResponse(vehicle.clientGps(), responsegps);
            }
            succesfull = responsegps.IsSuccessful && succesfull;
            return succesfull;
        }
        private Boolean SendGPSData(Boolean useTimeStampNow)
        {
            Boolean succesfull = true;
            if (useTimeStampNow)
            {
                vehicle.gpsData.setTimestamp(DateTime.Now);
            }
            IRestResponse responsegps = vehicle.SendGPS();
            if (Statics.DetailedContiLogging && responsegps.StatusCode != System.Net.HttpStatusCode.OK)
            {
                Statics.FeedbackResponse(vehicle.clientGps(), responsegps);
            }
            succesfull = responsegps.IsSuccessful && succesfull;
            return succesfull;
        }

        public string DebugQueueInfo(int total)
        {
            return string.Format("VehSender Veh={0} blobItemQueueCount={1} Sensormsg={2} Tot={3} Fail={4} Succ={5} Downl={6} tastState={7} blobItemQueueCountGrandTot={8}",
                    vehicleNumber, blobItemQueueCount, sensorDataListAll.Count,
                    nTotal, nFailed, nSucces, nDownload,
                    senderTask.Status.ToString(),
                    total); ;
        }

        public void FlushQueue()
        {
            while (blobItemStack.TryPop(out BlobItem blobItem))
            {
                ;
            }
        }
    }
}
