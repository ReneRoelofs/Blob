using System;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Configuration;
using System.IO;
using RestSharp;
using System.Net;
using System.Collections.Generic;
using BlobHandler;


/*

ContiConnect Qual Portal - https://qa.conticonnect.com/
Username: ArrivaNL_Test
Password: Welcome2021
 
ContiConnect Qual Portal - https://qa.conticonnect.com/
Username: arriva_nl
Password: Welcome2021
 
  
link to vehicle   
https://qa.conticonnect.com/fleetanalytics/?ref=1377702#/vehicle/4F1A5C49EDB54AA8979E5A31935B22B4
 * 
*/


namespace FmsBlobToContinental
{
    public class ContiClient
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public void DemoGPS()
        {
            var client = new CCRestClient(TestProd.Test,  "vehiclead/gps/X-A654");
         //   client.Timeout = -1;
            var request = new CCRestRequest(Method.PUT,TestProd.Test);

            /// create dummy gpsData from Json proviced by Continental-example
            GpsData gpsData = RR_Serialize.JSON.FromString<GpsData>(
"{\r\n    \"ts\": 1585766325000,\r\n    \"lat\": 52.37052,\r\n    \"lon\": 4.23322,\r\n    \"tdst\": 1100,\r\n    \"alt\": 300,\r\n    \"spd\": 10,\r\n    \"hdp\": 0,\r\n    \"gdp\": 0,\r\n    \"hdg\": 90\r\n}");
            // set the timestamp to 'now'
            gpsData.setTimestamp(DateTime.Now);
            // put the gpsData as json in the request.
            string sJson = gpsData.Json();
            //request.AddParameter("application/json",sJson, ParameterType.RequestBody);
            request.SetBody(sJson);

            // see what happens.
            IRestResponse response = client.Execute(request);
            //Console.WriteLine("URL: " + client.BaseUrl);
            //Console.WriteLine("error:   " + response.ErrorMessage);
            //Console.WriteLine("content: " + response.Content);
            //Console.WriteLine("status:  " + response.StatusCode);

            if (Statics.DetailedContiLogging)
            {
                FeedbackResponse(client, response);
            }

        }

        public void DemoSensor()
        {

            var request = new CCRestRequest(Method.PUT,TestProd.Test);
          
            SensorData sd = new SensorData();
            sd.sid = "11";
            RestClient client;
            client = new CCRestClient(TestProd.Test, "sensorad/sensors/" + sd.sid);
            client.Timeout = -1;

            sd.rssi1 = 111;
            sd.rssi2 = 112;
            sd.rssi3 = 113;
            sd.tprs = 850000;
            sd.ttmpr = 90;
            sd.ses = 1;
            sd.shr = 3;
            sd.tst = 0;
            sd.ptd = 2; // alert //SPN 2587
            // put the Data as json in the request.
            request.SetBody(sd.Json());

            // see what happens.
            IRestResponse response = client.Execute(request);
            if (Statics.DetailedContiLogging)
            {
                FeedbackResponse(client, response);
            }

        }

        public void DemoVehicle()
        {
            var client = new RestClient("https://qa.c2tires.conti.de/vehiclemd/vehicles/X-A655");
            client.Timeout = -1;

            var request = new RestRequest(Method.PUT);
            request.AddHeader("Authorization", "Bearer " + Properties.Settings.Default.certificateStringTest);
            request.AddHeader("Content-Type", "application/json");


            VehicleMD vehicleMd = new VehicleMD();
            vehicleMd.axleNumber = 2; // voor 2 banden en achter 4 banden.
            vehicleMd.ccuId = "FC55"; // ccuid in hex 
            vehicleMd.highTemperatureThreshold = 80; // alarm bij 80 *C
            vehicleMd.lowPressureThreshold = 20;  // alarm bij 20% afwijking
            vehicleMd.veryLowPressureThreshold = 50; // alarm bij 50% afwijking

            List<SensorMasterData> tmpList = new List<SensorMasterData>();

            tmpList.Add(new SensorMasterData { position = "03", ttmId = "11" });  //left front
            tmpList.Add(new SensorMasterData { position = "0B", ttmId = "12" });  //right front
            tmpList.Add(new SensorMasterData { position = "53", ttmId = "13" });  //left-left rear
            tmpList.Add(new SensorMasterData { position = "55", ttmId = "14" });   //etc
            tmpList.Add(new SensorMasterData { position = "59", ttmId = "15" });
            tmpList.Add(new SensorMasterData { position = "5B", ttmId = "16" });

            vehicleMd.sensors = tmpList.ToArray();
            vehicleMd.ttmNumber = tmpList.Count;


            // put the Data as json in the request.
            string sJson = vehicleMd.Json();
            request.AddParameter("application/json", sJson, ParameterType.RequestBody);

            // see what happens.
            IRestResponse response = client.Execute(request);
            if (Statics.DetailedContiLogging)
            {
                FeedbackResponse(client, response);
            }
        }

        public void FeedbackResponse(RestClient client, IRestResponse response)
        {
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                log.Warn("Feedback from WebClient:");
                log.Warn("URL: " + client.BaseUrl);
                log.Warn("error:   " + response.ErrorMessage);
                log.Warn("content: " + response.Content);
                log.Warn("status:  " + response.StatusCode);
            }
            else
            {
                //log.Debug("Feedback from WebClient:");
                //log.Debug("URL: " + client.BaseUrl);
                //log.Debug("error:   " + response.ErrorMessage);
                //log.Debug("content: " + response.Content);
                //log.Debug("status:  " + response.StatusCode);
            }
        }


        private void OldStuff()
        {
            // wrong URL containing wwww
            //var client = new RestClient("https://www.qa.c2tires.conti.de/vehiclead/gps/X-A654");

            // correct url?
            //var client = new RestClient("https://qa.c2tires.conti.de/vehiclead/gps/X-A654");
            //var client = new RestClient("https://qa.c2tires.conti.de/vehiclead/gps/X-A655");
            // another try for url...
            //  client = new RestClient("https://qa.conticonnect.com/vehiclead/gps/X-A654");  
            //
            // request.AddHeader("Authorization", "Bearer " + Properties.Settings.Default.certificateString);
            // request.AddHeader("Content-Type", "application/json");
        }
    }
}


