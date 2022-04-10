using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FmsBlobToContinental
{

    public class SensorMasterData
    {
        /// <summary>
        /// Graphical position of the sensor on the vehicle
        /// </summary>
        public string position;

        /// <summary>
        /// Recommended Pressure of the Tire in Pa.
        /// </summary>
        public double recommendedPressure = 875000;

        /// <summary>
        /// Numeric Sensor ID.
        /// </summary>
        public string ttmId;

        public string Text()
        {
            return string.Format("pos={0} recomPres={1} ttmId={2}", position, recommendedPressure, ttmId);
        }

    }

    public class SensorData : CCBaseData
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);



        [JsonIgnore]
        public Boolean doSendData = false; // should the sensordata be send to continental asap
        [JsonIgnore]
        public String why; // if true why?
        [JsonIgnore]
        public DateTime timestampUploaded { get; set; } // when whas is last uploaded.


        //+
        //--- every field with 3 or 4 letter abbreviations is the encoding
        //--- used to send (in JSON) to continental
        //--- some of those fields are filled by the PGN 65268 (FEF4) messages “Tire Condition” of SAE J1939
        //--- some of those fields are filled by the PGN 64578 (FE42) messages “Tire Condition 2” of SAE J1939
        //--- via the properties below, starting with property 'location' those abbreviations are set .
        //--- abbriviations with a get/set are shown in the debug-form grid.
        //-

        /// <summary>
        /// Sensor field (state encoded)  electrical fault
        /// </summary>
        public Int64 flt { get; set; } // electrical fault   (not:   attrib location)

        /// <summary>
        /// Tire leakage rate in Pa/s
        /// </summary>
        public float lkrt { get; set; } //attrib leakageRate

        /// <summary>
        /// Operating hours integer (
        /// </summary>
        public Int64 oph;

        /// <summary>
        /// Tire pressure threshold detect (state encoded)
        /// </summary>
        public Int64 ptd = 0; // attrib pressureTresholdDetection

        /// <summary>
        /// Signal Strength Indicator 1
        /// </summary>
        public Int64 rssi1 = 0;

        /// <summary>
        /// Signal Strength Indicator 2
        /// </summary>
        public Int64 rssi2 = 0;

        /// <summary>
        /// Signal Strength Indicator 3
        /// </summary>
        public Int64 rssi3 = 0;

        /// <summary>
        /// Signal Strength Indicator 4
        /// </summary>
        public Int64 rssi4 = 0;

        /// <summary>
        /// Signal Strength Indicator 5
        /// </summary>
        public Int64 rssi5 = 0;

        /// <summary>
        /// Signal Strength Indicator 6
        /// </summary>
        public Int64 rssi6 = 0;

        /// <summary>
        /// Sensor enable status
        /// </summary>
        public Int64 ses { get; set; } // attrib sensorEnabledStatus

        /// <summary>
        /// Sensor hit rate
        /// </summary>
        public Int64 shr = 0;

        /// <summary>
        /// SensorID
        /// </summary>
        public string sid { get; set; }

        [JsonIgnore]
        public string sidHex { get; set; }

        /// <summary>
        /// Sensor type
        /// </summary>
        public Int64 styp = 1;

        /// <summary>
        /// Tire pressure in Pa
        /// </summary>
        public float tprs; // attrib pressure

        /// <summary>
        /// Timestamp in Base class
        /// </summary>
        //   public Int64 ts;

        /// <summary>
        /// Tire status (state encoded)
        /// </summary>
        public Int64 tst;

        /// <summary>
        /// Tire temperature in *C
        /// </summary>
        public float ttmpr;


        [JsonIgnore]
        public uint location { get; set; }
        [JsonIgnore]
        public uint pressure { get { return (uint)tprs; } set { tprs = value; } }
        [JsonIgnore]
        public uint temperature { get { return (uint)ttmpr; } set { ttmpr = value; } }
        [JsonIgnore]
        public uint sensorEnabledStatus { get { return (uint)ses; } set { ses = value; } }
        [JsonIgnore]
        public uint CtiTireStatus { get { return (uint)tst; } set { tst = value; } }
        [JsonIgnore]

        public uint ctiTireElectricalFault { get; set; }  // no abbriviations yet??
        [JsonIgnore]
        public uint extendedTirePressureSupport { get; set; }  // no abbriviations yet??
        [JsonIgnore]
        public uint airLeakageRate { get { return (uint)lkrt; } set { lkrt = value; } }
        public uint pressureTresholdDetection { get { return (uint)ptd; } set { ptd = value; } }

        /// <summary>
        /// TrireCondition2.TirePressureExtendedRange
        /// </summary>
        [JsonIgnore]
        public uint pressure2 { get; set; }

        /// <summary>
        /// TrireCondition2.RequireTirePressure
        /// </summary>
        [JsonIgnore]
        public uint requiredPressure2 { get; set; }

        [JsonIgnore]
        public string rawFEF4 { get; set; }
        [JsonIgnore]
        public string rawFC42 { get; set; }

        public string Text()
        {
            return string.Format(
    "{0} location {1,2} {2}  {3,3}KPa-1 {4,3}KPa-2 (>={5,3}Kpa) {6,3}*C  enabled={7} CtiTireStatus={8} electFlt={9} leakageRage={10} treasholdDetection={11} tirestatus={12} uploaded={13}",
     timestamp, location, sidHex, pressure, pressure2, requiredPressure2, temperature, sensorEnabledStatus, CtiTireStatus, flt, lkrt, ptd, tst, timestampUploaded);
            //    0        1         2         3          4                  5            6                    7              8    9     10   11

        }

        [JsonIgnore]
        public bool FromFEF4
        {
            get { return !string.IsNullOrEmpty(rawFEF4); }
        }
        [JsonIgnore]
        public bool FromFC42
        {
            get { return !string.IsNullOrEmpty(rawFC42); }
        }


        /// <summary>
        /// Have a look if thius sensordata is young in the first placed.
        /// </summary>
        /// <returns></returns>
        public Boolean UpToDate()
        {
            Boolean result = true;
            if (timestampUploaded.AddMinutes(Statics.MinitesForIgnoreSensorDataBeforeDeserializing) < DateTime.Now)
            {
                //not up todate because latest send data was too long ago
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Compare this sensordata with the previous
        /// </summary>
        /// <param name="Prev"></param>
        /// <returns></returns>
        public Boolean SignificantChange(SensorData Prev, out string why)
        {
            if (Prev.timestamp >= this.timestamp || this.timestampUploaded >= this.timestamp)
            {
                // never update an older one; or one that has already been updated.
                why = "outdated";
                return false;
            }

            if (this.flt != Prev.flt) { why = "flt"; return true; } // electrical fault value changed;
            if (this.lkrt != Prev.lkrt) { why = "lkrt"; return true; } // leakage rate value changed;
            if (this.ptd != Prev.ptd) { why = "ptd"; return true; } // pressure threashold detection changed
            if (this.ses != Prev.ses) { why = "ses"; return true; } //  sensor enanbled status changed;
            if (this.tst != Prev.tst) { why = "tst"; return true; } //  tire status changed;

            if (changePercentage(this.pressure, Prev.pressure) > 10) { why = "pres"; return true; } // more then 10 percent in pressure changed
            if (changePercentage(this.temperature, Prev.temperature) > 10) { why = "temp"; return true; } // more hten 10 percent in temperature changed.
            why = "not";

            return false;
        }

        public void CopyDataFromPrev(SensorData Prev)
        {
            if (Prev == null)
            {
                return;
            }

            //+
            //--- kopieer alles van de voorgaande en zet de conditions opnieuw.
            //-
            this.flt = Prev.flt;
            this.lkrt = Prev.lkrt;
            this.ptd = Prev.ptd;
            this.ses = Prev.ses;
            this.tst = Prev.tst;
            this.pressure2 = Prev.pressure2;
            this.requiredPressure2 = Prev.requiredPressure2;
            this.SetConditionFromFC42(rawFC42);
            this.SetConditionFromFEF4(rawFEF4);

            this.timestampUploaded = Prev.timestampUploaded;

        }


        public double changePercentage(uint A, uint B)
        {
            if (B == 0) return 0;
            double result = Math.Abs(((1.0 * A) / (1.0 * B)) * 100) - 100;
            return result;
        }

        /// <summary> 
        /// PGN FEF4 == 65268 “Tire Condition” of SAE J1939  in CPC-3rd-Party-Connection_v0160.pdf
        /// </summary>
        /// <param name="value"></param>
        public void SetConditionFromFEF4(string value, Boolean debug = false)
        {
            uint tmp;


            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            //17T093647553;1;18fef433;00e100244100005f
            //                        00e100244100005f
            //                        00
            //                          e1
            //                            0024
            //                                41

            if (value == "00C9E023410000C0")
            {
                int debugx = 0;
                debugx++;
            }
            if (debug)
            {
                //log.InfoFormat("--- FEF4  Sensordata from '{0}'", value);
                log.Info(Statics.GetBinarystring(value));
            }
            location = Statics.GetBitsAt(value, 0, 8, true, debug, "Location");

            tmp = Statics.GetBitsAt(value, 8, 8, true, debug, "pressure");
            if (tmp < 0xFA)
            {
                pressure = 4000 * tmp; // valid range? then tire pressure in KPa  4 KPa per bit
            }
            else
            {
                int xdebug = 0;
                xdebug++;
            }

            if (debug) { log.InfoFormat("Pressure = {0}  using 4Kpa per bit", pressure); }

            tmp = Statics.GetBitsAt(value, 16, 16, true, debug, "temperature");
            if (tmp <= 0xFAFF)
            {
                temperature = (uint)((0.03125 * tmp - 273)); //0024 -> 2400   valid range? then 0.03125K per bit -273 offset
            }
            else
            {
                int xdebug = 0;
                xdebug++;
            }

            if (debug) { log.InfoFormat("Temperature = {0} using 0.03125K per bit -273 offset", temperature); }
            if (debug) { log.Info(Statics.GetBinarystring(value)); }

            sensorEnabledStatus = Statics.GetBitsAt(value, 32, 2, true, debug, "sensor Enabled Status"); //10
            CtiTireStatus = Statics.GetBitsAt(value, 34, 2, true, debug, "CtiTireStatus");
            ctiTireElectricalFault = Statics.GetBitsAt(value, 36, 2, true, debug, "ctiTireElectricalFault");
            extendedTirePressureSupport = Statics.GetBitsAt(value, 38, 2, true, debug, "extendedTirePressureSupport");
            airLeakageRate = (uint)(0.1) * Statics.GetBitsAt(value, 40, 16, true, debug, "airLeakageRate");  //0.1 pa/s

            Statics.GetBitsAt(value, 56, 5, true, debug, "reserved");

            pressureTresholdDetection = Statics.GetBitsAt(value, 61, 3, true, debug, "pressureTresholdDetection");
            rawFEF4 = value;
        }


        /// <summary>
        /// FC42 == PGN 64578 “Tire Condition 2” of SAE J1939  CPC-3rd-Party-Connection_v0160.pdf
        /// </summary>
        /// <param name="value"></param>
        public void SetConditionFromFC42(string value, Boolean debug = false)
        {
            uint tmp;
            if (string.IsNullOrEmpty(value))
            {
                return;
            }
            tmp = Statics.GetBitsAt(value, 8, 16, LittleEndian: true, debug, "Pressure2");
            if (tmp <= 0xFAFF)
            {
                pressure2 = tmp;
            }
            if (debug) { log.InfoFormat("Pressure2 = {0}  using 1Kpa per bit", pressure2); }

            requiredPressure2 = Statics.GetBitsAt(value, 24, 16, LittleEndian: true, debug, "RequiredPressure2");
            if (debug) { log.InfoFormat("RequiredPressure = {0}  using 1Kpa per bit", requiredPressure2); }
            if (pressure == 0)
            {
                pressure = pressure2 * 1000;
            }
            rawFC42 = value;
        }

        public void EnrichWithTTM(List<TTMData> ttmDataList)
        {
            TTMData ttmData = ttmDataList.Find(T => T.tireLocation == this.location);
            if (ttmData != null)
            {
                this.sid = ttmData.TTMID.ToString();
                this.sidHex = ttmData.TTMID.ToString("X");
                this.temperature = ttmData.TTMTemperature;
                if (ttmData.TTMAlarmWarning != 0x04)
                {
                    this.ses = 1;
                }
                else
                {
                    this.ses = 0;
                }
                if (this.temperature == 0)
                {
                    this.temperature = ttmData.TTMTemperature;
                }
            }
        }

      
    }

    public class TTMData
    {

        public uint systemId { get; set; }
        public uint tireId { get; set; }
        public uint tireLocation { get; set; }
        public UInt32 TTMID { get; set; }


        public uint TTMPressure { get; set; }
        public uint TTMTemperature { get; set; }

        public string rawFF02 { get; set; }
        public string rawFF04 { get; set; }

        public uint TTMState { get; set; }
        public uint TTMAlarmWarning { get; set; }
        public uint BatteryFlag { get; set; }
        public uint TTMDefective { get; set; }
        public uint TTMLoosedetection { get; set; }

        public uint GraphicalPosition { get; set; }


        public TTMData()
        {
        }

        public void SetTTMValue(string value)
        {
            // systemID and tireId already set as primary key.
            //  systemId = Statics.GetBitsAt(value, 0, 2);
            //  tireId = Statics.GetBitsAt(value, 2, 5);
            TTMPressure = (4706 * Statics.GetBitsAt(value, 8, 8)); // -4706; // 4706 kPa/bit -4706 kPa offset maar offset doen we toch maar niet.
            TTMPressure = (uint)(TTMPressure / 1000);
            TTMTemperature = (uint)((1 * Statics.GetBitsAt(value, 16, 8)) - 50); // 1gr C/bit -50K offset
            TTMState = Statics.GetBitsAt(value, 32, 8);
            TTMAlarmWarning = Statics.GetBitsAt(value, 40, 8);
            BatteryFlag = Statics.GetBitsAt(value, 49, 1);
            TTMDefective = Statics.GetBitsAt(value, 50, 1);
            TTMLoosedetection = Statics.GetBitsAt(value, 51, 2);
            rawFF02 = value;
        }


        public void SetTTMPosition(string value)
        {
            GraphicalPosition = Statics.GetBitsAt(value, 8, 8);
            tireLocation = Statics.GetBitsAt(value, 16, 8);
            TTMID = Statics.GetBitsAt(value, 32, 32);



            rawFF04 = value;

        }
    }
}
