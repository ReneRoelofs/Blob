using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FmsBlobToContinental
{

   
    public class GpsData : CCBaseData
    {
        /// <summary>
        /// GPS altitude
        /// </summary>
        public double alt { get; set; }

        /// <summary>
        /// Geometric dilution of precision
        /// </summary>
        public double gdp { get; set; }

        /// <summary>
        /// GPS heading
        /// </summary>
        public double hdg { get; set; }
        
        /// <summary>
        /// Horizontal dilution of precision
        /// </summary>
        public double hdp { get; set; }

        /// <summary>
        /// GPS latitude
        /// </summary>
        public double lat { get; set; }

        /// <summary>
        /// GPS longitude
        /// </summary>
        public double lon { get; set; }

        /// <summary>
        /// GPS speed
        /// </summary>
        public double spd { get; set; }

       

    }


}
