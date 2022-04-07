using FmsBlobToContinental;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Blob3
{
    class CloudFmsFile
    {
    }
}



public class CloudFmsRootObject
{
    public string agentSerial { get; set; }
    public string batch { get; set; }
    public string eventType { get; set; }
    public Customer customer { get; set; }
    public Vehicle vehicle { get; set; }
    public Payload[] payload { get; set; }
}

public class Customer
{
    public int companyId { get; set; }
    public string company { get; set; }
    public string customer { get; set; }
}

public class Vehicle
{
    public int vehicleNo { get; set; }
}
