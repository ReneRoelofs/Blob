#define TTMDATAX
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

#if TTMDATA  //defined in 1st line of this document
    private List<TTMData> ttmDataList = new List<TTMData>();
#endif


    /// <summary>
    /// OnDeserializedMethod is only called after Json deserialization
    /// </summary>
    /// <param name="context"></param>
    [OnDeserialized()]

    public void OnDeserializedMethod(StreamingContext context)
    {
        if (raw != null)
        {
            if (raw.FF04 != null)
            {
                //+
                //--- 1:  create the sensors based on the GraphicalPosition PGN (parameter group number) FF04
                //-
                foreach (string s in raw.FF04)
                {
                    uint location = Statics.GetBitsAt(s, 16, 8);

                    // location += 1000; // DEBUG

                    SensorData sd = GetOrAddSensorDataByLocation(location, "FF04 new", s);
                    sd.ProcessGraphicalPosition(s);   // thus set the location, and tireId and TTMid which is equals Sid.
                    sd.setTimestamp((DateTime)this.ts);

                    //niet meer nodig want we bouwen de ttddatalijst een paar regels hieronder op:
                    //raw.ReadTTMPositionAndTTMId(this, s, (DateTime)ts);
                }
            }
            if (raw.FF02 != null)
            {
                //+
                //--- 2: process the TTM data 
                foreach (string s in raw.FF02)
                {
                    uint tireId = Statics.GetBitsAt(s, 2, 5);

                    //tireId += 1000; // DEBUG

                    SensorData sd = GetSensorTireId(tireId, "FF02 new", s);
                    if (sd != null)
                    {
                        sd.ProcessTTMData(s);
                    }
                    //niet meer nodig want we bouwen de ttddatalijst een paar regels hieronder op:
                    //raw.ReadTTMData(this, s, (DateTime)ts);
                }
            }

            if (raw.FEF4 != null)
            {
                foreach (string s in raw.FEF4) //PGN FEF4 == 65268 “Tire Condition” of SAE J1939
                {
                    uint location = Statics.GetBitsAt(s, 0, 8);

                    // location += 1000; // DEBUG

                    SensorData sd = GetSensorDataByLocation(location, "FEF4 new", s);
                    if (sd != null)
                    {
                        sd.SetConditionFromFEF4(s);
                    }
                }
            }

            if (raw.FC42 != null)
            {
                foreach (string s in raw.FC42)
                {
                    uint location = Statics.GetBitsAt(s, 0, 8);

                    //location += 1000; // DEBUG

                    SensorData sd = GetSensorDataByLocation(location, "FC42 new", s);
                    if (sd != null)
                    {
                        sd.SetConditionFromFC42(s);
                    }
                }
            }

#if TTMDATA
            // ttddatalijst bouwen
            foreach (SensorData sd in sensorsDataList)
            {
                if (sd.ttmData != null)
                {
                    if (this.ttmDataList.Find(T=>T.TTMID == sd.ttmData.TTMID) == null)  
                    {
                        this.ttmDataList.Add(sd.ttmData);
                    }
                }
            }
#endif
        }
    }

    private SensorData GetOrAddSensorDataByLocation(uint location, string srcType, string src)
    {
        SensorData sd = sensorsDataList.Find(D => D.location == location);
        if (sd == null)
        {
            sd = new SensorData();
            sd.location = location;
            sensorsDataList.Add(sd);
            sd.src = srcType + " : " + src;
        }
        return sd;
    }
    private SensorData GetSensorDataByLocation(uint location, string srcType, string src)
    {
        SensorData sd = sensorsDataList.Find(D => D.location == location);
        return sd;
    }
    private SensorData GetSensorTireId(uint tireId, string srcType, string src)
    {
        SensorData sd = sensorsDataList.Find(D => D.tireId == tireId);
        return sd;
    }
    public Boolean HasSensorData
    {
        get
        {
            return sensorsDataList.Count > 0;
        }
    }

    private Boolean SendGPSData(Boolean useTimeStampNow)
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

    /// <summary>
    /// Send the masterdata to continental is something chnanged, and update the vehice.masterdataok boolean.
    /// See SendMasterDataToContinentalBasedOnTTM below for old routine
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
        foreach (SensorData sensorData in this.sensorsDataList)  // was eerder  //.FindAll(S => S.ttmData != null))
        {
            SensorMasterData smd = vehicle.GetOrAddSensorMasterData(sensorData.sid.ToString(), sensorData.graphicalPosition.ToString("X"),
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
            IRestResponse response = vehicle.SendMD(out sJson);
            //1111

            if (Statics.DetailedContiLogging)
            {
                log.DebugFormat("Veh={0,4} MasterDataUpdate  Url={1} Status={2} {3}",
                    vehicleNr,
                    vehicle.clientMd().BaseUrl,
                    response.StatusCode, why);

#if TTMDATA  //defined in 1st line of this document

                foreach (TTMData ttmData in this.ttmDataList)
                {
                    log.DebugFormat("Veh={0,4} pos={1,2} Sid={2} SidHex={2:X}",
                        vehicleNr, ttmData.tireLocation, ttmData.TTMID);
                }
#else
                foreach (SensorData sensorData in this.sensorsDataList)
                {
                    log.DebugFormat("Veh={0,4} pos={1,2} Sid={2} SidHex={2:X}",
                        vehicleNr, sensorData.location, sensorData.sid);
                }
#endif
                log.DebugFormat("Json={0}",
                sJson.Replace("\r\n", ""));

            }
            succesfull = response.IsSuccessful && succesfull;
        }
        vehicle.masterdataOk = succesfull;
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


#if TTMDATA  //defined in 1st line of this document


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
        public void ReadTTMPositionAndTTMId(Payload payload, string value, DateTime baseDateTime)
        {
            uint systemId = Statics.GetBitsAt(value, 0, 2);
            uint tireId = Statics.GetBitsAt(value, 2, 5);

            TTMData sd = GetOrAddTTMData(payload, systemId, tireId);

            sd.SetGraphicalPositionAndTTMId(value);
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
#endif




































































































    }
}
