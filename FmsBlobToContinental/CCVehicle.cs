using BlobHandler;
using RestSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FmsBlobToContinental
{
    public class SimpleSensorInfo
    {
        public uint location;
        public string HexId;
        public DateTime Updated;
        public uint Temp;
        public uint Pressure;
    }

    public class CCVehicle
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private string _vehicleNumber;

        public int iVehicleNumber;

        public string vehicleNumber
        {
            get
            {
                return _vehicleNumber;
            }
            set
            {
                _vehicleNumber = value;
                iVehicleNumber = RR.Lib.Str2AnyInt(value);

                /* sensorsMasterDataList.Clear();
                 sensorsMasterDataList.Add(new SensorMasterData { position = "03", ttmId = vehicleNumber + "001" });  //left front
                 sensorsMasterDataList.Add(new SensorMasterData { position = "0B", ttmId = vehicleNumber + "002" });  //right front
                 sensorsMasterDataList.Add(new SensorMasterData { position = "53", ttmId = vehicleNumber + "003" });  //left-left rear
                 sensorsMasterDataList.Add(new SensorMasterData { position = "55", ttmId = vehicleNumber + "004" });  //etc
                 sensorsMasterDataList.Add(new SensorMasterData { position = "59", ttmId = vehicleNumber + "005" });
                 sensorsMasterDataList.Add(new SensorMasterData { position = "5B", ttmId = vehicleNumber + "006" });
                 */
            }
        }

        public string agentSerialNumber { get; set; }

        [XmlIgnore]
        public GpsData gpsData;
        [XmlIgnore]
        public List<SensorMasterData> sensorsMasterDataList = new List<SensorMasterData>();
        [XmlIgnore]
        public Boolean masterdataOk = true;
        [XmlIgnore]
        public List<SensorData> sensorsDataList = new List<SensorData>();
        public TestProd testProd = TestProd.Test;
        public DateTime timestampSendToContinental { get; set; } = DateTime.MinValue;
        public DateTime timestampFileSeen { get; set; } = DateTime.MinValue;

        public List<SimpleSensorInfo> simpleSensorInfo = new List<SimpleSensorInfo>();

        public CCVehicle()
        {
            //+
            //--- constructor for serializing
            //-
        }

        public CCVehicle(string vehicleNumber = "0000")
        {
            this.vehicleNumber = vehicleNumber;
            // create some dummy gps data.
            SetLatLon(51.976, 4.612);
        }

     
        public string P0 { get { return SensorOnPosition(0); } }
        public string P1 { get { return SensorOnPosition(1); } }
        public string P16 { get { return SensorOnPosition(16); } }
        public string P17 { get { return SensorOnPosition(17); } }
        public string P18 { get { return SensorOnPosition(18); } }
        public string P19 { get { return SensorOnPosition(19); } }

        private string SensorOnPosition(uint location)
        {
            string result = "";
            SimpleSensorInfo si = simpleSensorInfo.Find(S => S.location == location);
            if (si != null)
            {
                return string.Format("{0} {1} {2} Degr {3} Bar",si.HexId, si.Updated, si.Temp, si.Pressure);
            }
            else
            {
                return "-";
            }
        }



        /// <summary>
        /// Find a sensorMasterdata by ttmId, add if not found yet
        /// </summary>
        /// <param name="ttmId"></param>
        /// <param name="graphicalPosition"></param>
        /// <param name="somethingChanged">true if not already known, false otherwise</param>
        /// <returns></returns>
        public SensorMasterData GetOrAddSensorMasterData(string sid, string graphicalPosition, out Boolean somethingChanged, out string why)
        {
            if (graphicalPosition.Length <= 1)
            {
                graphicalPosition = "0" + graphicalPosition;
            }
            somethingChanged = false;
            why = "NoChange";
            SensorMasterData result = sensorsMasterDataList.Find(S => S.ttmId == sid);
            if (result == null)
            {
                result = new SensorMasterData { position = graphicalPosition, ttmId = sid };
                //+
                //--- let op alleen goede sensoren met een juist ttm id toe te voegen,
                //-
                if (result.ttmId.Length >= 9)
                {
                    sensorsMasterDataList.Add(result);
                    somethingChanged = true;
                    why = "NewSensorFound";
                }
            }
            if (result.position != graphicalPosition)
            {
                why = "SensorPosistionChanged";
                somethingChanged = true;
            }
            return result;
        }

        public void SetLatLon(double lat, double lon)
        {
            if (gpsData == null)
            {
                gpsData = new GpsData();
            }
            gpsData.lat = lat;
            gpsData.lon = lon;
        }

        CCRestClient _clientGps;
        public CCRestClient clientGps()
        {
            string path = "vehiclead/gps/";
            string url = string.Format("{0}{1}", path, vehicleNumber);
            if (_clientGps == null || (_clientGps.BaseUrl.ToString().EndsWith(url)))
            {
                _clientGps = new CCRestClient(this.testProd, url);
            }
            return _clientGps;

        }

        CCRestClient _clientMd;
        public CCRestClient clientMd()
        {
            string path = "vehiclemd/vehicles/";
            string url = string.Format("{0}{1}", path, vehicleNumber);
            if (_clientMd == null || (_clientMd.BaseUrl.ToString().EndsWith(url)))
            {
                _clientMd = new CCRestClient(this.testProd, url);
            }
            return _clientMd;

        }


        public IRestResponse SendGPS()
        {

            var request = new CCRestRequest(Method.PUT, this.testProd);

            /// create dummy gpsData from Json proviced by Continental-example
            gpsData.setTimestamp(DateTime.Now);
            // put the gpsData as json in the request.
            string sJson = gpsData.Json();
            //request.AddParameter("application/json",sJson, ParameterType.RequestBody);
            request.SetBody(sJson);

            // see what happens.
            IRestResponse response = clientGps().Execute(request);
            if (Statics.DetailedContiLogging && response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                FeedbackResponse(clientGps(), response);
            }
            return response;
        }

        public IRestResponse SendMD(out string sJson)
        {
            var request = new CCRestRequest(Method.PUT, this.testProd);

            VehicleMD vehicleMd = new VehicleMD();
            vehicleMd.axleNumber = 2; // voor 2 banden en achter 4 banden.
            //vehicleMd.ccuId = vehicleNumber + "_FC55"; // ccuid in hex 
            vehicleMd.ccuId = agentSerialNumber;
            vehicleMd.highTemperatureThreshold = 80; // alarm bij 80 *C
            vehicleMd.lowPressureThreshold = 20;  // alarm bij 20% afwijking
            vehicleMd.veryLowPressureThreshold = 50; // alarm bij 50% afwijking

            vehicleMd.sensors = sensorsMasterDataList.ToArray();
            vehicleMd.ttmNumber = sensorsMasterDataList.Count;


            // put the Data as json in the request.
            sJson = vehicleMd.Json();
            request.AddParameter("application/json", sJson, ParameterType.RequestBody);

            // see what happens.
            IRestResponse response = clientMd().Execute(request);
            return response;
            //Statics.WriteFeedbackToConsole(clientMd, response);

        }

        /* public void SendSensors()  ==> dit zit nu in SendSensorDataContinental in CloudFmsPayload.cs*/
        // goto: CloudFmsPayload.cd/payload.SendSensorDataContinental
            /*
         {
             // SensorData sd = new SensorData();
             string path = "sensorad/sensors/";
             CCRestClient client;


             foreach (SensorData sensorData in sensorsDataList)
             {
                 sensorData.setTimestamp(DateTime.Now);
                 SensorMasterData sensorMd = sensorsMasterDataList[sensorsDataList.IndexOf(sensorData)];
                 sensorData.sid = sensorMd.ttmId;
                 //  sensorData.sidHex = sensor.
                 // 'random' temperatures
                 // sensorData.temperature = (DateTime.Now.Second);

                 client = new CCRestClient(this.testProd, string.Format("{0}{1}", path, sensorData.sid));
                 var request = new CCRestRequest(Method.PUT, this.testProd);

                 // put the Data as json in the request.
                 request.SetBody(sensorData.Json());
                 // send to continental
                 IRestResponse response = client.Execute(request);
                 if (Statics.DetailedContiLogging)
                 {
                     FeedbackResponse(client, response);
                 }
                 //break; // even alleen het eerste wiel
             }
         }*/


        public string Text2()
        {
            return string.Format("{0} {1} {2:yyyy-MM-dd HH:mm:ss}", vehicleNumber, agentSerialNumber, timestampSendToContinental);
        }


        public string Text()
        {
            string result = "";
            result += string.Format("Vehicle {0}  {1} {2}\r\n", vehicleNumber, clientGps().BaseUrl, clientMd().BaseUrl);
            foreach (SensorMasterData sensorMd in sensorsMasterDataList)
            {
                SensorData sensorData = sensorsDataList[sensorsMasterDataList.IndexOf(sensorMd)];
                result += string.Format("{0} {1}\r\n", sensorMd.Text(), sensorData.Text());
            }
            return result;
        }


        public void UpdateSimpleInfo(uint Location, string SensorIdHex, DateTime UpdatedAtContinental, uint temp, uint pressure)
        {
            SimpleSensorInfo si = simpleSensorInfo.Find(S => S.location == Location);
            {
                if (si == null)
                {
                    si = new SimpleSensorInfo { location = Location };
                    simpleSensorInfo.Add(si);
                }
                si.HexId = SensorIdHex;
                si.Updated = UpdatedAtContinental;
                si.Temp = temp;
                si.Pressure = pressure;
            }
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
                /*
                log.Debug("Feedback from WebClient:");
                log.Debug("URL: " + client.BaseUrl);
                log.Debug("error:   " + response.ErrorMessage);
                log.Debug("content: " + response.Content);
                log.Debug("status:  " + response.StatusCode);
                */
            }
        }

    }
}
