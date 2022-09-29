using BlobHandler;
using RestSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
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

    public class CCVehicle : IComparable<CCVehicle>
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

        [OnSerialized]
        public void onSerialized()
        {
            if (sensorsMasterDataList != null)
            {
                sensorsMasterDataList.vehicle = this;
            }
            else
            {
                sensorsMasterDataList = new SensorMasterDataList(this);
            }
        }

        public int CompareTo(CCVehicle obj)
        {
            return iVehicleNumber.CompareTo(obj.iVehicleNumber);
        }

        public string agentSerialNumber { get; set; }
        [XmlIgnore]
        public GpsData gpsData;
        //[XmlIgnore]
        public SensorMasterDataList sensorsMasterDataList;
        [XmlIgnore]
        public List<SensorData> sensorsDataList = new List<SensorData>();
        public TestProd testProd = TestProd.Test;
        public DateTime timestampSendToContinental { get; set; } = DateTime.MinValue;
        public DateTime timestampFileSeen { get; set; } = DateTime.MinValue;

        public Boolean IsUpToDate
        {
            get
            {
                return timestampSendToContinental >= DateTime.Now.AddDays(-4);
            }
        }

        public List<SimpleSensorInfo> simpleSensorInfo = new List<SimpleSensorInfo>();

        /// <summary>
        ///  constructor for serializing
        /// </summary>
        public CCVehicle()
        {
            sensorsMasterDataList = new SensorMasterDataList(this);
        }

        public CCVehicle(string vehicleNumber = "0000", TestProd testProd = TestProd.Prod)
        {
            this.vehicleNumber = vehicleNumber;
            // create some dummy gps data.
            SetLatLon(51.976, 4.612, DateTime.Now);
            sensorsMasterDataList = new SensorMasterDataList(this);
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
                string time = string.Format("{0:HH:mm}", si.Updated);
                string date = string.Format("{0:dd-MM}", si.Updated);
                if (si.Updated.Date == this.timestampFileSeen.Date)
                {
                    date = "";
                }
                return string.Format("{0} {1} gr {2:0.0} bar   {3} {4}", si.HexId, si.Temp, si.Pressure/100000.0, time, date);
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
        public SensorMasterData GetOrAddSensorMasterData(uint sid, string graphicalPosition)
        {
            SensorMasterData result= sensorsMasterDataList.GetOrAddSensorMasterData(sid, graphicalPosition, out Boolean somethingChanged);
            if (somethingChanged)
            {
                Statics.SensorFoundInVehicle(this, sid, graphicalPosition);
            }
            return result;
        }

        public void SetLatLon(double? lat, double? lon, DateTime? ts)
        {
            if (gpsData == null)
            {
                gpsData = new GpsData();
            }
            gpsData.lat = (double)lat;
            gpsData.lon = (double)lon;
            gpsData.setTimestamp((DateTime)ts);
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
                Statics.FeedbackResponse(clientGps(), response);
            }
            return response;
        }

        public IRestResponse SendMD(Boolean alwaysLog = false, string updateReason = "")
        {
            if (sensorsMasterDataList.vehicle == null)
            {
                sensorsMasterDataList.vehicle = this;
            }
            IRestResponse result = sensorsMasterDataList.SendMD(alwaysLog, updateReason);
            return result;
        }



        public string Text2()
        {
            return string.Format("{0} {1} {2:yyyy-MM-dd HH:mm:ss}", vehicleNumber, agentSerialNumber, timestampSendToContinental);
        }

        public string Text()
        {
            string result = "";
            result += string.Format("Vehicle {0}  {1} {2}\r\n", vehicleNumber, clientGps().BaseUrl, sensorsMasterDataList.clientMd().BaseUrl);
            foreach (SensorMasterData sensorMd in sensorsMasterDataList.theList)
            {
                SensorData sensorData = sensorsDataList[sensorsMasterDataList.theList.IndexOf(sensorMd)];
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
            timestampSendToContinental = UpdatedAtContinental;
        }
    }
}
