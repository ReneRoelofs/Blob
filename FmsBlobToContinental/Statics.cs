using BlobHandler;
using RestSharp;
using RR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
[assembly: log4net.Config.XmlConfigurator(Watch = true)]


namespace FmsBlobToContinental
{
    public static class Statics
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        static public int MinutesForMainLoop = 10; // After getblobs() wait for so long to try again

#if !DEBUG
        static public int MinutesForIgnoreSensorDataAfterDeserializing = 20; // data only taken into account if more then 20 minutes has passed (per sensor)\
        static public int MinitesForIgnoreSensorDataBeforeDeserializing = 0; // even niet op voorhand al skippen.
#else
        static public int MinutesForIgnoreSensorDataAfterDeserializing = 5; // data only taken into account if more then 20 minutes has passed (per sensor)
        static public int MinitesForIgnoreSensorDataBeforeDeserializing = 0; // even niet op voorhand al skippen.
#endif

        static public CleverIntList VehiclesShowingDownloadTrace = new CleverIntList();
        /*
         * 
         * 
         * 
         * 
         * 
         System::Net::ServicePointManager::SecurityProtocol = SecurityProtocolType::Tls12;
WebRequestHandler ^ clientHandler = gcnew WebRequestHandler();
X509Certificates::X509Certificate2^ modCert = gcnew X509Certificates::X509Certificate2("Dev.pfx", "test");
clientHandler->ClientCertificates->Add(cerInter);
clientHandler->AuthenticationLevel = System::Net::Security::AuthenticationLevel::MutualAuthRequested;
clientHandler->ClientCertificateOptions = ClientCertificateOption::Manual;
httpClient = gcnew HttpClient(clientHandler);
HttpContent ^ httpContent = gcnew ByteArrayContent(state->postBody);
httpContent->Headers->ContentType = gcnew MediaTypeHeaderValue("application/octet-stream");
resultTask = httpClient->PostAsync(state->httpRequest, httpContent);
         */

        static public Boolean DetailedContiLogging = false;
        static public Boolean ContiLoggingOnUpdates = true;


        static private VehicleList _vehicleList;//= new FmsBlobToContinental.VehicleList();
        static public VehicleList vehicleList
        {
            get
            {
                if (_vehicleList == null)
                {
                    try
                    {
                        _vehicleList = RR_Serialize.Xml.FromXMLFile<VehicleList>(Path.Combine(Properties.Settings.Default.DataDir, "Vehicles.xml"));
                    }
                    catch
                    {
                        _vehicleList = new VehicleList();
                    }
                }
                return _vehicleList;
            }
        }

        public static void SaveVehicleList()
        {
            lock (vehicleList)
            {
                RR_Serialize.Xml.ToXMLFile<VehicleList>(Statics.vehicleList, Path.Combine(Properties.Settings.Default.DataDir, "Vehicles.xml"));
            }
        }

        static private List<SensorData> _sensorList;//= new FmsBlobToContinental.VehicleList();
        static private Dictionary<string, SensorData> _sensorDict;
        static object MyLock = new object();
        static private Dictionary<string, SensorData> sensorDict
        {
            get
            {
                if (_sensorList == null)
                {
                    try
                    {
                        _sensorList = RR_Serialize.Xml.FromXMLFile<List<SensorData>>(Path.Combine(Properties.Settings.Default.DataDir, "Sensors.xml"));
                        _sensorDict = new Dictionary<string, SensorData>();
                        foreach (SensorData sensordata in _sensorList)
                        {
                            if (!_sensorDict.ContainsKey(sensordata.sid))
                            {
                                _sensorDict.Add(sensordata.sid, sensordata);
                            }
                        }
                    }
                    catch
                    {
                        _sensorList = new List<SensorData>();
                        _sensorDict = new Dictionary<string, SensorData>();
                    }
                }
                return _sensorDict;
            }
        }

        static public SensorData getSensor(SensorData src)
        {
            try
            {
                lock (MyLock)
                {
                    if (sensorDict.ContainsKey(src.sid))
                    {
                        return sensorDict[src.sid];
                    }
                    else
                    {
                        sensorDict.Add(src.sid, src);
                        return src;
                    }
                }
            }
            catch
            {
                return src;
            }
        }

        static public void ReplaceSensorInList(SensorData oldOne, SensorData newOne)
        {
            lock (MyLock)
            {
                if (sensorDict.ContainsKey(oldOne.sid))
                {
                    sensorDict.Remove(oldOne.sid);
                }
                sensorDict.Add(newOne.sid, newOne);
            }
            // dit kost te veel performance : SaveSensorList();
        }

        public static void SaveSensorList()
        {
            lock (MyLock)
            {
                if (_sensorDict != null)
                {
                    List<SensorData> theList = _sensorDict.Values.ToList();
                    RR_Serialize.Xml.ToXMLFile<List<SensorData>>(theList, (Path.Combine(Properties.Settings.Default.DataDir, "Sensors.xml")));
                }
            }
        }



        public static TestProd StaticsTestProd = TestProd.Test;

        //public static Int64 GetBitsAt64(string value, int start, int len)
        //{
        //    // location = value 0, 7      // first byte
        //    // pressure = value 8, 8      // second byte
        //    // temperature = value 16, 16 /// third and fourth byte


        //    // the entire value-hex-string as bits
        //    string binarystring = String.Join(String.Empty,
        //                value.Select(
        //                    c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')
        //            )
        //            );
        //    // cut the piece we need
        //    string mybits = binarystring.Substring(start, len);
        //    if (mybits.Length > 8)
        //    {
        //        string s1 = mybits.Substring(0, 8);
        //        string s2 = mybits.Substring(8);
        //        mybits = s2 + s1;
        //    }

        //    // convert to decimal
        //    Int64 result = Convert.ToInt64(mybits, 2);

        //    string hex = result.ToString("X");
        //    return result;

        //}




        //private static string GetHex(byte[] rawData, ref int pointerOctet, int length)
        //{
        //    pointerOctet += length;
        //    string result;
        //    result =  String.Concat(Array.ConvertAll(rawData.Skip(pointerOctet - length).Take(length).Reverse().ToArray(), x => x.ToString("X2")));
        //    return result;
        //}
        //private static UInt32 GetUInt32(byte[] rawData, ref int pointerOctet)
        //{
        //    return Convert.ToUInt32(GetHex(rawData, ref pointerOctet, 4), 16);
        //}

        public static string GetBinarystring(string value)
        {
            return String.Join(String.Empty,
                    value.Select(
                        c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')
                )
           );
        }

#if DEBUG
        static private string Spaceout(string s)
        {
            int i = 0;
            string s2 = "";
            foreach (char c in s)
            {
                s2 += c;
                if (++i % 8 == 0)
                {
                    s2 += " ";
                }

            }
            return s2;
        }
#endif

        /// <summary>
        /// Als 8 (een veelvoud van 8) of meer bits gevraagd worden krijg je gewoon die bits binnen
        /// anders juist de bits in de betreffende byte-binairy-string tellend van rechts naar links.
        /// </summary>
        /// <param name="binairyString"></param>
        /// <param name="start"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        static private string GetmybitsAt(string binairyString, int start, int len)
        {
            //binairyString = 00001111-00110011-00111001-11000110
            //string b2 = Spaceout(binairyString);  
            string result = "";

            if (len >= 8) // && len %8 == 0
            {
                return binairyString.Substring(start, len);
            }
            int byteIndex = (int)(start / 8);
            string myByte = binairyString.Substring(byteIndex * 8, 8);
            // startIndex : start 0 t/m 7 -> 1e byte   dus mybyte = 00001111
            //              start 8 t/m 15 -> 2e byte  dus mybyte = 00110011
            //              start 16 t/m 23 -> 3e byte dus mybyte = 00111001
            //              start 24 t/m 32 -> 4e byte dus mybyte = 11000110
            // start b.v 34 -> startindex 4 and rest 2
            
            int rest = start - (byteIndex * 8);
            
            result = myByte.Substring(8 - rest - len, len);
            // b.v. start 34 len 2 -> 11000110 substring 4, 2-> 01;
            // result are the rightmost bits in the byte
            return result;
        }

        /// <summary>
        /// Get the uInt value from bits in a HexString
        /// </summary>
        /// <param name="value">Hexadecimal string</param>
        /// <param name="start">index of first bit, start with 1</param>
        /// <param name="len">number of bits needed</param>
        /// <param name="LittleEndian">if true and len>8, the first bit is least important and the second byte the most important</param>
        /// <param name="Debug">put some extra debugging in the log4net</param>
        /// <param name="tag">tag for the extra debugging</param>
        /// <returns></returns>
        public static uint GetBitsAt(string value, int start, int len, Boolean LittleEndian = true, Boolean Debug = false, string tag = "")
        {
            // location = value 0, 7      // first byte
            // pressure = value 8, 8      // second byte
            // temperature = value 16, 16 /// third and fourth byte

            // the entire value-hex-string as bits
            string binarystring = GetBinarystring(value);
            //DEBUG: string b2 = Spaceout(binarystring);

            // int L = binarystring.Length;
            // cut the piece we need
#if DEBUG
            if (value == "00C9E02361000040")
            {
                int debug2 = 0;
                debug2++;
            }
            if (value == "00C9E023410000C0")
            {
                int debug2 = 0;
                debug2++;
            }
#endif      
            string mybits = binarystring.Substring(start, len);
            mybits = GetmybitsAt(binarystring, start, len);

            if (LittleEndian)
            {
                //mybits = 44 AC FA 6E binair.

                if (mybits.Length > 16)
                {
                    string s1 = mybits.Substring(0, 8);  //44
                    string s2 = mybits.Substring(8, 8);  //AC
                    string s3 = mybits.Substring(16, 8); //FA
                    string s4 = mybits.Substring(24, 8); //6E
                    mybits = s4 + s3 + s2 + s1;

                }
                else if (len == 16)
                {
                    //+
                    //--- example Temperature in 
                    //--- in position 16 len 16
                    //--- 00C9E02341000040
                    //---          0    0    C    9    E    0    2    3    4    1    0    0    0    0    4    0
                    //--- mybits = 0000 0000 1100 1001 1110 0000 0010 0011 0100 0001 0000 0000 0000 0000 0100 0000
                    //--- pos      1    4    8    12   16   20   24   28   32   36   40   44   48   52   56   60
                    //--- s1                           1110 0000
                    //--- s2                                     0010 0011
                    //---
                    //--- mybits (littleEndian)        0010 0011 1110 0000
                    //--- Decimal value = 9184 == c5456Temperature = 14 deg C using 0.03125K per bit -273 offset
                    string s1 = mybits.Substring(0, 8);
                    string s2 = mybits.Substring(8, 8);
                    mybits = s2 + s1;
                }
            }
            /**/
            // convert to decimal
            uint result = Convert.ToUInt32(mybits, 2);

            string hex = result.ToString("X");


            if (Debug)
            {
                string filler = "                                                                                  ".Substring(0, start);
                string filler2 = "                                                                                  ".Substring(0, 64 - (start + len));
                log.InfoFormat("{0}{1}{2} {3,-27}  start={4,2} len={5,2} result={6} hex={7}", filler, mybits, filler2, tag, start, len, result, hex);
            }



            return result;

        }



    }
}

