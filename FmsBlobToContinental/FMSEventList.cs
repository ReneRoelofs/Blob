using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FmsBlobToContinental
{
    public class FMSEventList
    {
        public List<FMSEvent> theList;
        public List<SensorData> tmpSensors = new List<SensorData>();
        /// <summary>
        /// Create list of events.
        /// </summary>
        public void Create()
        {
            theList = new List<FMSEvent>();
            for (int i = 0; i<10; i++)
            {
                FMSEvent newOne = FMSEventTest.Create();
                theList.Add(newOne);
            }
        }

        /// <summary>
        /// Save the list of events to a file.
        /// </summary>
        public void Save()
        {
            RR_Serialize.JSON.ToFile<List<FMSEvent>>(theList, "FmsEventList.json", Indented: true);
        }

        /// <summary>
        /// Load a list of events from a file
        /// </summary>
        public void Load(string filename)
        {
            theList = RR_Serialize.JSON.FromFile<List<FMSEvent>>(Path.Combine(Properties.Settings.Default.DataDir, filename));
            tmpSensors.Clear();
            
            DateTime now = DateTime.Now;
            foreach (FMSEvent evnt in theList)
            {
                CCRaw raw = new CCRaw(null, null);
                if (evnt.raw != null)
                {
                    foreach (string s in evnt.raw.FEF4)
                    {
                        raw.ReadTireCondition(s, now); // read raw from FILE (not important for real blob stuff)
                    }
                    foreach (string s in evnt.raw.FC42)
                    {
                        raw.ReadTireCondition2(s, now);// read raw from FILE (not important for real blob stuff)
                    }
                }
          //      Console.WriteLine(raw.vehicle.Text());
                foreach (SensorData sensorData in raw.vehicle.sensorsDataList)
                {
                    tmpSensors.Add(sensorData);
                }
            }
        }
    }   
}
