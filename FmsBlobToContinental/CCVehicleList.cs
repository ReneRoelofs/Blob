using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FmsBlobToContinental
{
    public class VehicleList : List<CCVehicle>
    {
        public CCVehicle GetOrAdd(int vehicleNumber, string agentSerialNumber)
        {
            CCVehicle result = this.Find(V=>V.vehicleNumber == vehicleNumber.ToString());
            if (result == null)
            {
                result = new CCVehicle(vehicleNumber.ToString());
                   this.Add(result);
            }
            result.agentSerialNumber = agentSerialNumber;
            return result;
        }
    }
}
