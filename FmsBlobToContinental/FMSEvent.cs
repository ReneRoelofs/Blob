using Newtonsoft.Json;
using System;

namespace FmsBlobToContinental
{
    public class FMSEvent
    {
        /// <summary>
        /// TimeStamp
        /// </summary>
        public DateTime? ts { get; set; }
        /// <summary>
        /// latitude
        /// </summary>
        public float lat { get; set; }
        /// <summary>
        /// longitude
        /// </summary>
        public float? lon { get; set; }
        /// <summary>
        /// heading in degrees
        /// </summary>
        public float? dir { get; set; }
        /// <summary>
        /// velocity in km/h over GPS
        /// </summary>
        public float? vgps { get; set; }
        /// <summary>
        /// Driver id
        /// </summary>
        public string drvid { get; set; }
        /// <summary>
        /// velocity in km/h over FMS
        /// </summary>
        public float? vfms { get; set; }
        /// <summary>
        /// Numerical gear, -1 reverse, 0 neutral
        /// </summary>
        public int? gear { get; set; }
        /// <summary>
        /// Engine revolutions/min
        /// </summary>
        public float? rpm { get; set; }
        /// <summary>
        /// Engine torque % of max
        /// </summary>
        public float? torq { get; set; }
        /// <summary>
        /// Coolant temperature in celsius
        /// </summary>
        public float? tcool { get; set; }
        /// <summary>
        /// Ambient temperature in celsius
        /// </summary>
        public float? tamb { get; set; }
        /// <summary>
        /// Odometer in km
        /// </summary>
        public float? dist { get; set; }
        /// <summary>
        /// Overall door info - 0 all doors closed, 1 at leastone door open
        /// </summary>
        public byte? dinfo { get; set; }
        /// <summary>
        /// Door state (1 means open) counting from front -door controller 2 (DC2) not always available
        /// </summary>
        public byte[] doors;

        [JsonIgnore]
        public string sDoors
        {
            get
            {
                string s = "{";
                foreach (byte b in doors)
                {
                    if (s != "{")
                    {
                        s += ",";
                    }
                    s += b.ToString();
                }
                s += "}";
                return s;
            }
        }


        /// <summary>
        /// Fuel level in litres
        /// </summary>
        public int? fuell { get; set; }
        /// <summary>
        /// Fuel rate in l/h
        /// </summary>
        public float? fuelr { get; set; }
        /// <summary>
        /// Overall fuel consumption in l
        /// </summary>
        public float? fuelc { get; set; }
        /// <summary>
        /// AdBlue level in %
        /// </summary>
        public byte? adbleu { get; set; }
        /// <summary>
        /// Accelerator pedal position in %
        /// </summary>
        public byte? peacc { get; set; }
        /// <summary>
        /// Break pedal position in %
        /// </summary>
        public byte? pebrk { get; set; }
        /// <summary>
        /// Actual retarder %
        /// </summary>
        public byte? reta { get; set; }
        public Raw raw { get; set; }


    }

    public class Raw
    {
        public string[] FEF4 { get; set; }
        public string[] FC42 { get; set; }
    }
}


public class Rootobject
{
    public DateTime ts { get; set; }
    public float lat { get; set; }
    public float lon { get; set; }
    public int dir { get; set; }
    public int vgps { get; set; }
    public string drvid { get; set; }
    public int vfms { get; set; }
    public int gear { get; set; }
    public int rpm { get; set; }
    public object torq { get; set; }
    public int tamb { get; set; }
    public int tcool { get; set; }
    public int dist { get; set; }
    public object engine { get; set; }
    public int dinfo { get; set; }
    public int[] doors { get; set; }
    public object fuell { get; set; }
    public int fuelr { get; set; }
    public int fuelc { get; set; }
    public int adblue { get; set; }
    public int peacc { get; set; }
    public object pebrk { get; set; }
    public int reta { get; set; }
    public Raw raw { get; set; }
    public class Raw
    {
        public string[] FEF4 { get; set; }
        public string[] FC42 { get; set; }
    }
}