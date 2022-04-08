using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FmsBlobToContinental
{
    public class CCRaw
    {
        string file = @"C:\klanten\arriva\blob3\data\9725.txt";
        public CCVehicle vehicle;

        public CCRaw(CCVehicle vehicle, string file = null)
        {
            this.vehicle = vehicle;
            if (this.vehicle == null)
            {
                this.vehicle = new CCVehicle();
            }
            if (!string.IsNullOrEmpty(file))
            {
                this.file = file;
            }
        }
    
        private DateTime GetTimeStamp(string sLine, DateTime baseDateTime)
        {
            //# Time: 20210517T093525
            //17T093525245
            DateTime result = baseDateTime;
            string[] tokens = sLine.Split(' ');
            if (tokens.Length == 3)
            {
                if (tokens[1] == "Time:")
                {
                    //# Time: 20210517T093525
                    int y = int.Parse(tokens[2].Substring(0, 4));
                    int m = int.Parse(tokens[2].Substring(4, 2));
                    int d = int.Parse(tokens[2].Substring(6, 2));
                    int hour = int.Parse(tokens[2].Substring(9, 2));
                    int min = int.Parse(tokens[2].Substring(11, 2));
                    int sec = int.Parse(tokens[2].Substring(13, 2));
                    result = new DateTime(y, m, d, hour, min, sec);

                }
            }
            if (tokens.Length == 1)
            {
                //17T093525245
                int y = baseDateTime.Year;
                int m = baseDateTime.Month;
                int d = int.Parse(tokens[0].Substring(0, 2));
                int hour = int.Parse(tokens[0].Substring(3, 2));
                int min = int.Parse(tokens[0].Substring(5, 2));
                int sec = int.Parse(tokens[0].Substring(7, 2));
                int mil = int.Parse(tokens[0].Substring(9, 3));
                result = new DateTime(y, m, d, hour, min, sec, mil);
            }

            return result;
        }

        private SensorData GetSensor(uint location)
        {
            SensorData sd = vehicle.sensorsDataList.Find(D => D.location == location);
            if (sd == null)
            {
                sd = new SensorData();
                sd.location = location;
                vehicle.sensorsDataList.Add(sd);
            }
            return sd;
        }

        /// <summary>
        /// FEF4
        /// </summary>
        /// <param name="value"></param>
        /// <param name="baseDateTime"></param>
        public void ReadTireCondition(string value, DateTime baseDateTime)
        {
            //Github test vrijdag 8-4

            //17T093647553;1;18fef433;00e100244100005f
            //                        00e100244100005f
            //                        00
            //                          e1
            //                            0024
            //                                41

            uint location = Statics.GetBitsAt(value, 0, 8); //00
            SensorData sd = GetSensor(location);
            sd.setTimestamp(baseDateTime);
            sd.SetConditionFromFEF4(value);   // read raw from FILE (not important for real blob stuff)

            //  sd.pressure = 4000 * Statics.GetBitsAt(value, 8, 8); //e1  tire pressure in KPa  4 KPa per bit
            //  sd.temperature = (uint)((0.03125 * Statics.GetBitsAt(value, 16, 16)) - 273); //0024 -> 2400    0.03125K per bit -273 offset
            //  sd.enabled = Statics.GetBitsAt(value, 32, 2); //10
            //  sd.contition = Statics.GetBitsAt(value, 34, 2);
            ////  Console.WriteLine(string.Format("'{0}'  {1}", value, sd.Text()));
        }

        /// <summary>
        /// PGN FC42 == 64578 “Tire Condition 2” of SAE J1939
        /// </summary>
        /// <param name="value"></param>
        /// <param name="baseDateTime"></param>
        public void ReadTireCondition2(string value, DateTime baseDateTime)
        {
            uint location = Statics.GetBitsAt(value, 0, 8);
            SensorData sd = GetSensor(location);
            sd.SetConditionFromFC42(value);
            sd.setTimestamp(baseDateTime);
              // Console.WriteLine(string.Format("'{0}'  {1}", value, sd.Text()));
        }

    }
}
