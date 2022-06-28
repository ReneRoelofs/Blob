using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FmsBlobToContinental
{
    public class SensorMasterData : IComparable<SensorMasterData>
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
        public uint ttmId;

        [JsonIgnore]
        public string hexId
        {
            get
            {
                return ttmId.ToString("X");
            }
            set
            {

            }
        }
        public int CompareTo(SensorMasterData obj)
        {
            return position.CompareTo(obj.position);
        }

        public string Text()
        {
            return string.Format("pos={0} recomPres={1} ttmId={2}", position, recommendedPressure, ttmId);
        }



    }
}

