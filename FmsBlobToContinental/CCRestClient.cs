using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using BlobHandler;

namespace FmsBlobToContinental
{

    public class CCRestClient : RestClient
    {
        public CCRestClient(TestProd testProd, string path)
            : base()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            Timeout = -1;
            if (testProd == TestProd.Test)
            {
                this.BaseUrl = new Uri(Properties.Settings.Default.hostNameTest + path);
            }
            else
            {
                this.BaseUrl = new Uri(Properties.Settings.Default.hostNameProd + path);
            }
        }
    }

    public class CCRestRequest : RestRequest
    {
        public CCRestRequest(Method method, TestProd testProd)
            : base(method)
        {
            /// It is needed to set the securityProtocol.
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            if (testProd == TestProd.Test)
            {
                AddHeader("Authorization", "Bearer " + Properties.Settings.Default.certificateStringTest);
            }
            else
            {
                AddHeader("Authorization", "Bearer " + Properties.Settings.Default.certificateStringTest);
            }
            AddHeader("Content-Type", "application/json");
        }

        public void SetBody(string sJson)
        {
            AddParameter("application/json", sJson, ParameterType.RequestBody);
        }
    }

  
}
