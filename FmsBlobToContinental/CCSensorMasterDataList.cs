using BlobHandler;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace FmsBlobToContinental
{
    public class SensorMasterDataList 
    {
        public List<SensorMasterData> theList = new List<SensorMasterData>();
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [XmlIgnore]
        public CCVehicle vehicle;

        private Boolean _needsResending = false; // needs to be resent to Continental
        public DateTime ResendTimeStamp  { get; set; }
        public string resendReason = "";

        public SensorMasterDataList()
        {
        }

        public SensorMasterDataList(CCVehicle vehicle)
        {
            this.vehicle = vehicle;
            ResendTimeStamp = DateTime.Now.AddMinutes(-1 * Statics.SendMDAtLeastEveryMinute);
        }

        public void SetNeedResending(Boolean value, string reason)
        {
            _needsResending = value;
            resendReason = reason;
        }
        public Boolean NeedsResending()
        {
            TimeSpan ts = DateTime.Now.Subtract(ResendTimeStamp);
            if (ts.TotalMinutes > Statics.SendMDAtLeastEveryMinute && EnoughSensors()) // elke zoveel minuten
            {
                resendReason = String.Format("Refresh was {0:0} min ago ", ts.TotalMinutes);
                _needsResending = true;
            }
            return _needsResending;
        }

        public Boolean EnoughSensors()
        {
            return (this.theList.Count >= 4);
        }
        public SensorMasterData GetOrAddSensorMasterData(uint sid, string graphicalPosition, out Boolean somethingChanged)
        {
            if (graphicalPosition.Length <= 1)
            {
                graphicalPosition = "0" + graphicalPosition;
            }
            somethingChanged = false;
            SensorMasterData result = this.theList.Find(S => S.ttmId == sid);
            if (result == null)
            {
                //+
                //--- sensor met dit ID is niet gevonden.
                //--- gooi zonodig sensor op hetzelfde positie weg
                //-
                RemoveSensor(graphicalPosition);
                //+
                //--- nieuwe sensor toevoegen.
                //-
                result = new SensorMasterData { position = graphicalPosition, ttmId = sid };
                //+
                //--- let op alleen goede sensoren met een juist ttm id toe te voegen,
                //-
                if (result.ttmId.ToString().Length >= 9)
                {
                    this.theList.Add(result);
                    somethingChanged = true;
                    //opnieuw sturen als we wel genoeg sensoren kennen.
                    SetNeedResending(EnoughSensors(), "NewSensorFound");
                }
            }
            if (result.position != graphicalPosition)
            {
                //opnieuw sturen als we wel genoeg sensoren kennen.
                SetNeedResending( EnoughSensors(),"SensorPosistionChanged");
                somethingChanged = true;
            }
            return result;
        }

        /// <summary>
        /// Remove the sensor from the list if it exists
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="graphicalPosition"></param>
        public SensorMasterData RemoveSensor(string graphicalPosition)
        {
            SensorMasterData sensor = this.theList.Find(S => S.position == graphicalPosition);
            if (sensor != null)
            {
                this.theList.Remove(sensor);
            }
            return sensor;
        }

        /// <summary>
        /// Remove the sensor from the list if it exists
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="graphicalPosition"></param>
        public SensorMasterData RemoveSensor(uint sid)
        {
            SensorMasterData sensor = this.theList.Find(S => S.ttmId == sid);
            if (sensor != null)
            {
                this.theList.Remove(sensor);
            }
            return sensor;
        }


        CCRestClient _clientMd;
        public CCRestClient clientMd()
        {
            string path = "vehiclemd/vehicles/";
            string url = string.Format("{0}{1}", path, vehicle.vehicleNumber);
            if (_clientMd == null || (_clientMd.BaseUrl.ToString().EndsWith(url)))
            {
                _clientMd = new CCRestClient(vehicle.testProd, url);
            }
            return _clientMd;

        }

        public IRestResponse SendMD(Boolean alwaysLog = false, string updateReason = "")
        {
            if (updateReason != "")
            {
                this.resendReason = updateReason;
            }

            var request = new CCRestRequest(Method.PUT, vehicle.testProd);
            theList.Sort();
            VehicleMD vehicleMd = new VehicleMD();
            vehicleMd.axleNumber = 2; // voor 2 banden en achter 4 banden.
            //vehicleMd.ccuId = vehicleNumber + "_FC55"; // ccuid in hex 
            vehicleMd.ccuId = vehicle.agentSerialNumber; // agentSerialNumber;
            vehicleMd.highTemperatureThreshold = 80; // alarm bij 80 *C
            vehicleMd.lowPressureThreshold = 20;  // alarm bij 20% afwijking
            vehicleMd.veryLowPressureThreshold = 50; // alarm bij 50% afwijking

            vehicleMd.sensors = this.theList.ToArray();
            vehicleMd.ttmNumber = this.theList.Count;

            if (vehicleMd.ccuId != null)
            {
                // put the Data as json in the request.
                string sJson = vehicleMd.Json();
                request.AddParameter("application/json", sJson, ParameterType.RequestBody);

                // see what happens.
                IRestResponse response = clientMd().Execute(request);
                _needsResending = false;

                if (Statics.DetailedContiLogging || alwaysLog)
                {
                    log.InfoFormat("Veh={0,4} MasterDataUpdate ccuId={1} Url={2} Status={3} ResendReason={4}",
                        vehicle.vehicleNumber,
                        vehicleMd.ccuId,
                        clientMd().BaseUrl,
                        response.StatusCode,
                        resendReason);

                    resendReason = "";


                    if (!response.IsSuccessful)
                    {
                        log.InfoFormat("                Json={0}", sJson.Replace("\r\n", ""));
                    }
                    else
                    {
                        log.InfoFormat("SUCCES Veh={0}:{1} ",vehicle.vehicleNumber, sJson);
                    }


                    Statics.FeedbackResponse(clientMd(), response);
#if TTMDATA  //defined in 1st line of this document

                foreach (TTMData ttmData in this)
                {
                    log.DebugFormat("Veh={0,4} pos={1,2} Sid={2} SidHex={2:X}",
                        vehicle.vehicleNr, ttmData.tireLocation, ttmData.TTMID);
                }
#else
                    foreach (SensorMasterData md in this.theList)
                    {
                        log.DebugFormat("Veh={0,4} pos={1,2} Sid={2} SidHex={2:X}",
                            vehicle.vehicleNumber, md.position, md.ttmId);
                    }
#endif
                    log.DebugFormat("Json={0}",
                    sJson.Replace("\r\n", ""));
                }
                ResendTimeStamp = DateTime.Now;
                return response;
                //Statics.WriteFeedbackToConsole(clientMd, response);
            }
            else // if (vehicleMd.ccuId == null)
            {
                log.ErrorFormat("CCUid is unknown for vehicle {0}", vehicle.vehicleNumber);
                ResendTimeStamp = DateTime.Now;
                return null;
            }
        }

    }
}
