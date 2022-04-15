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

        [JsonIgnore]
        private uint TTMWarning = Statics.UNKNOWN;

        [JsonIgnore]
        private long TTMStatus = Statics.UNKNOWN;

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
        /// 
        private Int64 _ses= Statics.UNKNOWN;  // 254 betekend dat we het niet weten, dus dat hij ook niet als change gezien wordt
        public Int64 ses
        { // attrib sensorEnabledStatus
            get
            {
                return _ses;
            }
            set
            {
                _ses = value;
#if DEBUG
                if (value == 2)
                {
                    int d = 0;
                    d++;
                }
#endif
            }
        }

        /// <summary>
        /// Sensor hit rate
        /// </summary>
        public Int64 shr = 0;

        /// <summary>
        /// SensorID
        /// </summary>
        public string sid { get; set; }

        [JsonIgnore]
        public uint tireId { get; set; }

        public Boolean SidOk()
        {
            if (String.IsNullOrEmpty(sid)) return false;
            if (sid.Trim() == "0") return false;
            if (sid.Trim() == "1") return false;
            return true;
        }


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


        private uint _location;

        [JsonIgnore]
        public uint location
        {
            get
            {
                return _location;
            }
            set
            {
                if (_location != 0 && _location != value)
                {
                    int debug = 1;
                }
                _location = value;
            }
        }
        [JsonIgnore]
        public string src { get; set; }
        [JsonIgnore]
        public uint pressure { get { return (uint)tprs; } set { tprs = value; } }
        [JsonIgnore]
        public uint temperature { get { return (uint)ttmpr; } set { ttmpr = value; } }
        [JsonIgnore]
        public uint sensorEnabledStatus { get { return (uint)ses; } set { ses = value; } }
        [JsonIgnore]
        public uint CtiTireStatus { get { return (uint)tst; } set { tst = value; } }
        [JsonIgnore]

        public uint ctiTireElectricalFault { get { return (uint)flt; } set { flt = (long)value; } }
        [JsonIgnore]
        public uint extendedTirePressureSupport { get; set; }  // no abbriviations yet??
        [JsonIgnore]
        public uint airLeakageRate { get { return (uint)lkrt; } set { lkrt = value; } }
        [JsonIgnore]
        public uint pressureTresholdDetection
        {
            get { return (uint)ptd; }
            set
            {
                ptd = value;
#if DEBUG
                if (ptd == 9)
                {
                    int debug = 0;
                }
#endif
            }
        }

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

        [JsonIgnore]
        TTMData ttmData = null;
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

            if (this.ses == Statics.UNKNOWN)
            {
                why = "ses missing";
                return false; // record is incomplete so dont send anyway.
            }

            if (this.flt != Prev.flt) { why = string.Format("flt {0} to {1}", Prev.flt, this.flt); return true; } // electrical fault value changed;
            if (this.lkrt != Prev.lkrt) { why = string.Format("lkrt {0} to {1}", Prev.lkrt, this.lkrt); return true; } // leakage rate value changed;
            if (this.ptd != Prev.ptd) { why = string.Format("ptd {0} to {1}", Prev.ptd, this.ptd); return true; } // pressure threashold detection changed
          
            /// SES is geen significant change want die zwappert te veel
            /// 
            /// if (this.ses != Prev.ses) { why = string.Format("ses {0} to {1}", Prev.ses, this.ses); return true; } //  sensor enanbled status changed;
            if (this.tst != Prev.tst) { why = string.Format("tst {0} to {1}", Prev.tst, this.tst); return true; } //  tire status changed;

            if (changePercentage(this.pressure, Prev.pressure) > 10) { why = string.Format("pres {0} to {1}", Prev.pressure, this.pressure); return true; } // more then 10 percent in pressure changed
            if (changePercentage(this.temperature, Prev.temperature) > 10) { why = string.Format("temp {0} to {1}", Prev.temperature, this.temperature); return true; } // more hten 10 percent in temperature changed.
            why = "not";

            return false;
        }

        /// <summary>
        /// kopieer waarden uit het vorige record.
        /// en dan opnieuw uit dit record. maar daarbij wordt de TTM genegeerd dus die moet ook nog daarna gedaan worden.
        /// </summary>
        /// <param name="Prev"></param>
        public void CopyDataFromPrev(SensorData Prev)
        {
            if (Prev == null)
            {
                return;
            }

            //+
            //--- kopieer alles van de voorgaande en zet de conditions opnieuw.
            //-
            //this.flt = Prev.flt;
            //this.lkrt = Prev.lkrt;
            //this.ptd = Prev.ptd;

            /*

            if (!this.FromFEF4)
            {
                this.ses = Prev.ses;
            }
            this.tst = Prev.tst;
            this.pressure2 = Prev.pressure2;
            this.requiredPressure2 = Prev.requiredPressure2;
            this.SetConditionFromFC42(rawFC42);
            this.SetConditionFromFEF4(rawFEF4);

            this.timestampUploaded = Prev.timestampUploaded;

            this.sid = Prev.sid;
            this.sidHex = Prev.sidHex;
            */
        }


        public double changePercentage(uint A, uint B)
        {
            if (B == 0) return 0;
            double result = Math.Abs(((1.0 * A) / (1.0 * B)) * 100) - 100;
            return result;
        }
        public void ProcessTTMData(string value)
        {
            pressure = (4706 * Statics.GetBitsAt(value, 8, 8)); // -4706; // 4706 kPa/bit -4706 kPa offset maar offset doen we toch maar niet.
            pressure2 = (uint)(pressure / 1000);
            temperature = (uint)((1 * Statics.GetBitsAt(value, 16, 8)) - 50); // 1gr C/bit -50K offset

            TTMStatus = Statics.GetBitsAt(value, 32, 8); // TTM State ignored.
             
            TTMWarning = Statics.GetBitsAt(value, 40, 8); // TTM Alarm+warning
            ptd = 0;

            switch (TTMWarning)
            {
                case 0x00: ptd = 0b010; break; // No warning
                case 0x01: ptd = 0b011; break; // under-inflation warning 
                case 0x02: ptd = 0b100; break; // under-inflation alarm
                case 0x04: ptd = 0b110; break; // mute->sensorerror
                case 0x09: ptd = 0b010; break; // pressure difference warning -> no warning
            }
            flt = Statics.GetBitsAt(value, 49, 1); // TTM Battery Flag
            uint TTMDefective = Statics.GetBitsAt(value, 50, 1);

            //+
            //--- alleen als hij WEL kapot is zetten we de ses op 2.
            //
            if (TTMDefective == 1)
            {
                ses = 2;
            }
            //ses = 1; // TTM Defective  loopt niet lekker synchroon.  opposite of ses therefor ZERO MINUS:
            uint TTMLoosedetection = Statics.GetBitsAt(value, 51, 2);
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
#if DEBUG
            long tmpx = TTMStatus;

            if (sensorEnabledStatus == 2 && debug == false)
            {
                 //      SetConditionFromFEF4(value, true);// extra debug
                //_ = Statics.GetBitsAt(value, 32, 2, true, true, "sensor Enabled Status"); // extra debug
            }
#endif
            uint TmpCtiTireStatus = Statics.GetBitsAt(value, 34, 2, true, debug, "CtiTireStatus");
            if (TmpCtiTireStatus != CtiTireStatus && TTMStatus != Statics.UNKNOWN)
            {
                CtiTireStatus = TmpCtiTireStatus;
            }





            ctiTireElectricalFault = Statics.GetBitsAt(value, 36, 2, true, debug, "ctiTireElectricalFault");
            extendedTirePressureSupport = Statics.GetBitsAt(value, 38, 2, true, debug, "extendedTirePressureSupport");
            airLeakageRate = (uint)(0.1) * Statics.GetBitsAt(value, 40, 16, true, debug, "airLeakageRate");  //0.1 pa/s

            Statics.GetBitsAt(value, 56, 5, true, debug, "reserved");

            uint tmpPtd = Statics.GetBitsAt(value, 61, 3, true, debug, "pressureTresholdDetection");
            if (tmpPtd != ptd && TTMWarning != Statics.UNKNOWN)
            {
                // een sensor error kan nooit de eerder gelezen waarde uit de TTI overschijven.
                ptd = tmpPtd;
            }
            uint x = TTMWarning;//DEBUG
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
            ttmData = ttmDataList.Find(T => T.tireLocation == this.location);
            EnrichWithTTM();
        }

        public void EnrichWithTTM()
        {
            if (ttmData != null)
            {
                this.sid = ttmData.TTMID.ToString();
                this.sidHex = ttmData.TTMID.ToString("X");
                //if (ttmData.TTMAlarmWarning != 0x04)
                //{
                //    this.ses = 1;
                //}
                //else
                //{
                //    this.ses = 0;
                //}
                if (this.temperature == 0)
                {
                    if (ttmData.TTMState != 255)
                    {
                        this.temperature = ttmData.TTMTemperature;
                    }
                }
            }
        }

        /// <summary>
        /// Process the GraphicalPosition or FF04 PGN
        /// setting the TireID, Location and Sid. the graphicalposition is ignored so far.
        /// </summary>
        /// <param name="value"></param>
        public void ProcessGraphicalPosition(string value)
        {
            tireId = Statics.GetBitsAt(value, 2, 5);

            // tireId += 1000;  // DEBUG

            location = Statics.GetBitsAt(value, 16, 8); //+1000 //DEBUG

            
            uint iSid = Statics.GetBitsAt(value, 32, 32);
            sid = iSid.ToString();
            sidHex = iSid.ToString("X");
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


        public void SetGraphicalPositionAndTTMId(string value)
        {
            GraphicalPosition = Statics.GetBitsAt(value, 8, 8);
            tireLocation = Statics.GetBitsAt(value, 16, 8);
            TTMID = Statics.GetBitsAt(value, 32, 32);
            rawFF04 = value;

        }
    }
}
