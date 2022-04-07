using BlobHandler;
using FmsBlobToContinental;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Statics = FmsBlobToContinental.Statics;

public class Payload
{
    private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    [JsonIgnore]
    public CCVehicle vehicle;
    [JsonIgnore]
    public string vehicleNr
    {
        get
        {
            if (vehicle != null)
            {
                return vehicle.vehicleNumber;
            }
            else
            {
                return "";
            }
        }
    }

    public DateTime? ts { get; set; }

    [JsonIgnore]
    public string FEF4 { get { if (raw != null) return raw.sFEF4; else return ""; } }
    [JsonIgnore]
    public string FC42 { get { if (raw != null) return raw.sFC42; else return ""; } }
    [JsonIgnore]
    public string FF00 { get { if (raw != null) return raw.sFF00; else return ""; } }
    [JsonIgnore]
    public string FF01 { get { if (raw != null) return raw.sFF01; else return ""; } }
    [JsonIgnore]
    public string FF02 { get { if (raw != null) return raw.sFF02; else return ""; } }
    [JsonIgnore]
    public string FF04 { get { if (raw != null) return raw.sFF04; else return ""; } }


    //public Boolean Has

    public Boolean HasPressureInfo()
    {
        if (raw == null) 
            return false;
        return raw.HasPressureInfo();
    }

    public Boolean HasFEF4()
    {
        return raw.HasFEF4();
    }

    public Boolean HasFC42()
    {
        return raw.HasFC42();
    }

    public float? lat { get; set; }
    public float? lon { get; set; }
    public int? dir { get; set; }
    public float? vgps { get; set; }
    public float? hdop { get; set; }
    public int? satcnt { get; set; }
    public int? fix { get; set; }
    public object drvid { get; set; }
    public int? vfms { get; set; }
    public int? gear { get; set; }
    public int? rpm { get; set; }
    public int? torq { get; set; }
    public float? tamb { get; set; }
    public float? tcool { get; set; }
    public int? dist { get; set; }
    public int? dinfo { get; set; }
    public object doors { get; set; }
    public int? fuell { get; set; }
    public float? fuelr { get; set; }
    public int? fuelc { get; set; }
    public int? adblue { get; set; }
    public int? peacc { get; set; }
    public int? pebrk { get; set; }
    public bool? swbrake { get; set; }
    public bool? pbrake { get; set; }
    public bool? cctrl { get; set; }
    public int? reta { get; set; }
    public int? reti { get; set; }
    public int? retd { get; set; }
    public int? rets { get; set; }
    public float[] accm { get; set; }

    public string sAccm
    {
        get
        {
            string result = "";
            foreach (float fl in accm)
            {
                result += string.Format("{0:0.000};", fl);
            }
            return result;
        }
    }
    public int? agrade { get; set; }
    public float? volt { get; set; }
    public int[] digin { get; set; }

    public string sDigin
    {
        get
        {
            string result = "";
            foreach (int i in accm)
            {
                result += string.Format("{0};", i);
            }
            return result;
        }
    }

    public Raw raw { get; set; }

    // sensordatalist voor payload is er niet meer want we gebruiken altijd 
    public List<SensorData> sensorsDataList = new List<SensorData>();
    public List<TTMData> ttmDataList = new List<TTMData>();


    /// <summary>
    /// OnDeserializedMethod is only called after Json deserialization
    /// </summary>
    /// <param name="context"></param>
    [OnDeserialized()]
    internal void OnDeserializedMethod(StreamingContext context)
    {
        if (raw != null)
        {
            if (raw.FC42 != null)
            {
                foreach (string s in raw.FC42)
                {
                    raw.ReadTireCondition2(this, s, (DateTime)ts);
                }
            }
            if (raw.FEF4 != null)
            {
                foreach (string s in raw.FEF4) //PGN FEF4 == 65268 “Tire Condition” of SAE J1939
                {
                    raw.ReadTireCondition(this, s, (DateTime)ts);
                }
            }
            if (raw.FF02 != null)
            {
                foreach (string s in raw.FF02)
                {
                    raw.ReadTTMData(this, s, (DateTime)ts);
                }
            }
            if (raw.FF04 != null)
            {
                foreach (string s in raw.FF04)
                {
                    raw.ReadTTMPosition(this, s, (DateTime)ts);
                }
            }
        }
    }


    //public string sSensorData
    //{
    //    get
    //    {
    //        string result = "";
    //        foreach (SensorData sd in sensorsDataList)
    //        {
    //            result += string.Format("Loc={0} Pres1={1} Pres2={2} Temp={3}   ", sd.location, sd.pressure, sd.pressure2, sd.temperature);
    //        }
    //        return result;
    //    }
    //}

    public Boolean HasSensorData
    {
        get
        {
            return sensorsDataList.Count > 0;
        }
    }




    public void EnrichSensorWithTTM()
    {
        foreach (SensorData sensorData in this.sensorsDataList)
        {
            sensorData.EnrichWithTTM(this.ttmDataList);
        }
    }


    /// <summary>
    /// the main routine to send payloaddata to continental
    /// </summary>
    /// <param name="vehicle"></param>
    /// <param name="useTimeStampNow"></param>
    /// <returns>SuccesFull</returns>
    public Boolean SendToContinentalIfNeeded(Boolean useTimeStampNow, out List<SensorData> sensorsInThisPayload)
    {
        sensorsInThisPayload = new List<SensorData>();
        Boolean succesfull = true;
        if (ts != null)
        {
            //+
            //--- 1: Send the metadata to continental. This contains a check if meta data has changed since last time
            //--- no no redundant data is send.
            //-
            // RENE MASTER EVEN OVERSLAAN??
            succesfull = succesfull & SendMasterDataToContinental(useTimeStampNow);
            if (succesfull)
            {                //+
                //--- 2: Send the SensorData to continental
                //-
                succesfull = succesfull & SendSensorDataContinental(useTimeStampNow, out sensorsInThisPayload, out Boolean AnythingSend);
                //+
                //--- 3: send GPS data only if Sensordata has been send
                //-
                if (AnythingSend)
                {
                    SentGPSData(useTimeStampNow);
                }
            }
        }

        if (vehicle.timestampSendToContinental < (DateTime)ts)
        {
            vehicle.timestampSendToContinental = (DateTime)ts;
        }
        return succesfull;
    }

    private Boolean SentGPSData(Boolean useTimeStampNow)
    {
        Boolean succesfull = true;
        string sJson;
        //+
        //--- create and send GPS data
        //-
        if (vehicle.gpsData == null)
        {
            vehicle.gpsData = new GpsData();
        }

        if (lat != null && lon != null)
        {
            vehicle.gpsData.lat = (double)lat;
            vehicle.gpsData.lon = (double)lon;
            vehicle.gpsData.setTimestamp((DateTime)ts);
            if (useTimeStampNow)
            {
                vehicle.gpsData.setTimestamp(DateTime.Now);
            }
        }
        IRestResponse responsegps = vehicle.SendGPS();
        if (Statics.DetailedContiLogging && responsegps.StatusCode != System.Net.HttpStatusCode.OK)
        {
            FeedbackResponse(vehicle.clientGps(), responsegps);
        }
        succesfull = responsegps.IsSuccessful && succesfull;
        return succesfull;
    }
    private Boolean SendMasterDataToContinental(Boolean useTimeStampNow)
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
        foreach (TTMData ttmData in this.ttmDataList)
        {
            SensorMasterData smd = vehicle.GetOrAddSensorMasterData(ttmData.TTMID, ttmData.GraphicalPosition.ToString("X"),
                out Boolean tmpMetaDataChanged, out string tmpWhy);
            metadataChanged = metadataChanged || tmpMetaDataChanged;
            if (tmpMetaDataChanged)
            {
                why += ttmData.GraphicalPosition + " " + tmpWhy + ";";
            }

        }

        if (metadataChanged)
        {
            //+
            //---  send sensor master data, only if changes in sensores were made
            //-
            IRestResponse response = vehicle.SendMD(out sJson);
            //1111

            if (Statics.DetailedContiLogging)
            {
                log.DebugFormat("Veh={0,4} Why={1,4} Url={2} Status={3} ",
                    vehicleNr,
                    why,
                    vehicle.clientMd().BaseUrl,
                    response.StatusCode);
                log.DebugFormat("Json={0}",
                    sJson.Replace("\r\n", ""));

            }
            succesfull = response.IsSuccessful && succesfull;
        }


        return succesfull;
    }


    /// <summary>
    /// Send the sensordata to Continental IF
    ///     - useTimeStampNow is true
    /// OR  - it's more then Statics.MinutesForIgnoreSensorDataAfterDeserializing minutes ago since something was send 
    /// OR  - compared to the previous sensordata, something significant has changed.
    /// </summary>
    /// <param name="useTimeStampNow"></param>
    /// <returns></returns>
    /// 
    private Boolean SendSensorDataContinental(Boolean useTimeStampNow, out List<SensorData> sensorsInThisPayload, out Boolean SendAnyThing)
    {
        Boolean succesfull = true;
        SendAnyThing = false;
        sensorsInThisPayload = new List<SensorData>();

        //+
        //---  Get al info from sensor data and ttm data.
        //-
        EnrichSensorWithTTM();

        //+
        //---  Send sensordata
        //-
        foreach (SensorData sensorData in this.sensorsDataList)
        {
            if (!String.IsNullOrEmpty(sensorData.sid) && sensorData.sid != "0" && sensorData.sid != "1")
            {
                SensorData prevSend = Statics.getSensor(sensorData);
                if (prevSend == sensorData)
                {
                    string sX = "No prev found";
                }
                DateTime prevSendTs = prevSend.timestamp;
                DateTime thisTs = sensorData.timestamp;
                sensorsInThisPayload.Add(sensorData);

                Boolean doSendData = false;
                sensorData.CopyDataFromPrev(prevSend);
                string why;


                if (useTimeStampNow)
                {
                    why = "TIMENOW";
                }

                if (sensorData.SignificantChange(prevSend, out why))
                {
                    // log.DebugFormat("NEW {1} {0}", sensorData.Text(),why);
                    // log.DebugFormat("OLD {1} {0}", prevSend.Text(),why);
                    doSendData = true;
                }
                if (prevSend == sensorData)
                {
                    // log.Debug("NEW");
                    why = "NEW";
                    doSendData = true;
                }
                if (prevSend.timestamp.AddMinutes(Statics.MinutesForIgnoreSensorDataAfterDeserializing) < sensorData.timestamp)
                {
                    why = "TIME";
                    //log.DebugFormat("TIME :{0} vs {1}",prevSend.timestamp, sensorData.timestamp);
                    doSendData = true;
                }

                if (useTimeStampNow || doSendData == true)
                ///|| prevSend == sensorData
                //|| prevSend.timestamp.AddMinutes(Statics.MinutesForIgnoreSensorDataAfterDeserializing) < sensorData.timestamp
                //|| sensorData.SignificantChange(prevSend))
                {
                    //+
                    //--- deze sensor is nog nooit verzonden of langer dan 20 (MinutesForIgnoreSensorData) minuten geleden verzonden
                    //-
                    sensorData.timestampUploaded = sensorData.timestamp;

                    var request = new CCRestRequest(Method.PUT, vehicle.testProd);
                    RestClient client = new CCRestClient(vehicle.testProd, "sensorad/sensors/" + sensorData.sid);
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

                    if (response.IsSuccessful)// && sensorData.enabled ==1) // little feedback including url ans statuscode.
                    {
                        SendAnyThing = true;
                        Statics.ReplaceSensorInList(prevSend, sensorData);
                        vehicle.UpdateSimpleInfo(sensorData.location, sensorData.sidHex, sensorData.timestamp, sensorData.temperature, sensorData.pressure);
                        log.InfoFormat("Veh={0,4} Loc={1,2} Why={2,4} Time={3} Url={4} Status={5} ",
                            vehicleNr,
                            sensorData.location,
                            why,
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
                        //+
                        //--- ook als het mislukt is updaten we toch de timestamp van deze sensor anders blijven we aan de gang
                        //-
                        Statics.ReplaceSensorInList(prevSend, sensorData);

                        log.WarnFormat("Veh={0,4} Loc={1,2} Why={2,4} Time={3} Url={4} SensorHex={5} Status={6} Response={7}",
                            vehicleNr,
                            sensorData.location,
                            why,
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
                    succesfull = response.IsSuccessful && succesfull;
                }
                else // else for if (useTimeStampNow || doSendData == true)
                {
                    if (Statics.DetailedContiLogging)
                    {
                        localFeedback(string.Format("Sensor {0}  ts {1} has been send allready on {2}", sensorData.sidHex, sensorData.timestamp, prevSend.timestamp));
                    }
                }
            }
        }
        return succesfull;
    }



    public void FeedbackResponse(RestClient client, IRestResponse response)
    {
        if (response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            log.Warn("Feedback from WebClient:");
            log.Warn("URL: " + client.BaseUrl);
            log.Warn("error:   " + response.ErrorMessage);
            log.Warn("content: " + response.Content);
            log.Warn("status:  " + response.StatusCode);
        }
        else
        {
            log.Debug("URL: " + client.BaseUrl + " " + response.StatusCode);
            //log.Debug("error:   " + response.ErrorMessage);
            //log.Debug("content: " + response.Content);
            //log.Debug("status:  " + response.StatusCode);
        }
    }


    private void localFeedback(string s)
    {
        string logfile = string.Format("D:\\temp\\blob\\logging\\{0}.txt", vehicleNr);
        using (StreamWriter sw = new StreamWriter(logfile, append: true))
        {
            sw.WriteLine(DateTime.Now.ToString() + " " + s.Replace("\r\n", ""));
        }
    }



    public Boolean NewEnoughToSend(CCVehicle vehicle, Boolean selectAll)
    {
        if (selectAll)
        {
            return true;
        }
        else
        {
            return (ts > vehicle.timestampSendToContinental);
        }
    }



    //public string sRaw
    //{
    //    get
    //    {
    //        if (raw != null)
    //        {
    //            return raw.sRaw;
    //        }
    //        else
    //        {
    //            return "N/A";
    //        }
    //    }
    //}

    public class Raw
    {

        [JsonIgnore]
        public string sFEF4 { get { return sArray(FEF4); } }
        [JsonIgnore]
        public string sFC42 { get { return sArray(FC42); } }
        [JsonIgnore]
        public string sFF00 { get { return sArray(FF00); } }
        [JsonIgnore]
        public string sFF01 { get { return sArray(FF01); } }
        [JsonIgnore]
        public string sFF02 { get { return sArray(FF02); } }
        [JsonIgnore]
        public string sFF04 { get { return sArray(FF04); } }

        public string[] FEF4 { get; set; }  //  dec:65.268   Tire Condition with Temperature 
        public string[] FC42 { get; set; }  //  dec:64.578   Tire Condition 2
        public string[] FF00 { get; set; }  //      65.280   CPC SystemConfiguration
        public string[] FF01 { get; set; }  //      65.281   CPC System Status
        public string[] FF02 { get; set; }  //      65.282   CPC TTM Data
        public string[] FF04 { get; set; }  //      65.284   CPC Graphical Position Configuration

        [JsonIgnore]
        public string sRaw
        {
            get
            {
                return RR_Serialize.JSON.ToString<Raw>(this);
            }
        }

        private string sArray(string[] a)
        {
            string result = "";
            if (a != null)
            {
                foreach (string s in a)
                {
                    result += s + ";";
                }
            }
            return result;
        }


        public Boolean HasPressureInfo()
        {
            return (HasFEF4() || HasFC42());
        }

        public Boolean HasFEF4()
        {
            return (FEF4 != null && FEF4.Length > 0);
        }

        public Boolean HasFC42()
        {
            return (FC42 != null && FC42.Length > 0);
        }


        /// <summary>
        /// PGN FEF4 == 65268 “Tire Condition” of SAE J1939
        /// </summary>
        /// <param name="value"></param>
        /// <param name="baseDateTime"></param>
        public void ReadTireCondition(Payload payload, string value, DateTime baseDateTime)
        {
            //17T093647553;1;18fef433;00e100244100005f
            //                        00e100244100005f
            //                        00
            //                          e1
            //                            0024
            //                                41

            uint location = Statics.GetBitsAt(value, 0, 8); //00
            SensorData sd = GetOrAddSensorData(payload, location);
            sd.setTimestamp(baseDateTime);
            sd.SetConditionFromFEF4(value);
            //  Console.WriteLine(string.Format("'{0}'  {1}", value, sd.Text()));
        }


        /// <summary>
        ///  PGN FC42 == 64578 “Tire Condition 2” of SAE J1939
        /// </summary>
        /// <param name="value"></param>
        /// <param name="baseDateTime"></param>
        public void ReadTireCondition2(Payload payload, string value, DateTime baseDateTime)
        {
            uint location = Statics.GetBitsAt(value, 0, 8);
            SensorData sd = GetOrAddSensorData(payload, location);
            sd.setTimestamp(baseDateTime);
            sd.SetConditionFromFC42(value);
            //   Console.WriteLine(string.Format("'{0}'  {1}", value, sd.Text()));
        }

        /// <summary>
        /// FF02 CPC TTM Data
        /// </summary>
        /// <param name="value"></param>
        /// <param name="baseDateTime"></param>
        public void ReadTTMData(Payload payload, string value, DateTime baseDateTime)
        {
            uint systemId = Statics.GetBitsAt(value, 0, 2);
            uint tireId = Statics.GetBitsAt(value, 2, 5);

            TTMData sd = GetOrAddTTMData(payload, systemId, tireId);
            sd.SetTTMValue(value);
        }

        /// <summary>
        /// FF04 CPC TTM Position
        /// </summary>
        /// <param name="value"></param>
        /// <param name="baseDateTime"></param>
        public void ReadTTMPosition(Payload payload, string value, DateTime baseDateTime)
        {
            uint systemId = Statics.GetBitsAt(value, 0, 2);
            uint tireId = Statics.GetBitsAt(value, 2, 5);

            TTMData sd = GetOrAddTTMData(payload, systemId, tireId);

            sd.SetTTMPosition(value);
        }


        private SensorData GetOrAddSensorData(Payload payload, uint location)
        {
            SensorData sd = payload.sensorsDataList.Find(D => D.location == location);
            if (sd == null)
            {
                sd = new SensorData();
                sd.location = location;
                payload.sensorsDataList.Add(sd);
            }
            return sd;
        }

        private TTMData GetOrAddTTMData(Payload payload, uint ttmId, uint tireId)
        {
            TTMData ttm = payload.ttmDataList.Find(D => D.tireId == tireId /* &&  D.systemId == ttmId   /**/);
            if (ttm == null)
            {
                ttm = new TTMData();
                ttm.systemId = ttmId;
                if (ttm.systemId == 2)
                {
                    ttm.systemId = 0;  //not supported => Truck
                }
                ttm.tireId = tireId;
                payload.ttmDataList.Add(ttm);
            }
            return ttm;
        }

    }
}
