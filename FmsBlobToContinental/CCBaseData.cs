using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FmsBlobToContinental
{

    abstract public class CCBaseDataWithJson
    {
        public string Json()
        {
            return RR_Serialize.JSON.ToString(this);
        }
    }

    abstract public class CCBaseData : CCBaseDataWithJson
    {

        public CCBaseData()
        {
            setTimestamp(DateTime.Now);
        }

        /// <summary>
        /// Timestamp
        /// </summary>
        public Int64 ts { get; set; }
        
        [JsonIgnore]
        public DateTime timestamp
        {
            get
            {
                DateTime result = new DateTime(1970, 1, 1);
                result = result.AddMilliseconds(ts);
                return result.ToLocalTime();
            }
        }

        public void setTimestamp(DateTime value)
        {
            ts = (Int64)value.ToUniversalTime().Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
        }

    }

 

}
