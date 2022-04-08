

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FmsBlobToContinental
{
    public class VehicleMD : CCBaseDataWithJson
    {

        /// <summary>
        /// Number of axles on the vehicle.
        /// Minimum value : 1
        /// Maximum value : 8
        /// </summary>
        public Int32 axleNumber;

        /// <summary>
        /// Id of the ccu. 
        /// Length : 0 - 12
        /// </summary>
        public string ccuId;

        /// <summary>
        /// Threshold for high temperature alerts in *C.
        /// Minimum value : 5
        /// Maximum value : 300
        /// </summary>
        public Int64 highTemperatureThreshold;

        /// <summary>
        /// Threshold for low pressure alerts in % of pressure.
        /// Minimum value : 5
        /// Maximum value : 100
        /// </summary>
        public Int64 lowPressureThreshold;

        /// <summary>
        /// 
        /// </summary>
        public SensorMasterData[] sensors;

        /// <summary>
        /// Number of sensors(ttm) on the vehicle.
        /// Minimum value : 0
        /// Maximum value : 64
        /// </summary>
        public Int32 ttmNumber;

        /// <summary>
        /// Threshold for very low pressure alerts in % of pressure.
        /// Minimum value : 5
        /// Maximum value : 100
        /// </summary>
        public Int64 veryLowPressureThreshold;

    }
}
