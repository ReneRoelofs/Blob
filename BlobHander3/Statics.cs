using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace BlobHandler
{
    /// <summary>
    /// Contains all stuff to get and manage the blobfiles from the blobstorage
    /// </summary>
    /// 

    public enum TestProd { Test, Prod }

    //public delegate void Onprogress.Report(string s);
    public delegate void OnException(Exception ex);
    public delegate void OnCompleted();
    public delegate void OnHandleBlob(BlobItem blobItem);


    static public class Statics
    {
        static public string urlTest = "BlobEndpoint=https://jupitertstexternal.blob.core.windows.net";
        static public string urlProd = "BlobEndpoint=https://jupiterprdexternal.blob.core.windows.net";

       // static public string connectionStringProd = "SharedAccessSignature=se=2022-10-22T00%3A00%3A00Z&sp=rl&spr=https&sv=2018-11-09&sr=c&sig=tXeAwjGvuInzAwtxsCoQ0v0NYLcumwuLPdK9q3X/c/Q%3D";
       // static public string connectionStringTest = "SharedAccessSignature=se=2022-09-21&sp=rl&spr=https&sv=2018-11-09&sr=c&sig=jUdMymIq6CBVHIEUKPy4fW7uS4hVmLHPWqZuZkSBffo%3D";

        static public string connectionStringTestWithWrite = "SharedAccessSignature=se=2022-11-13T00%3A00%3A00Z&sp=rwdl&spr=https&sv=2018-11-09&sr=c&sig=Kq5Vit6/sc%2BTaQUYtNucW7hqWaxGCMQtJbYx01ZoIcQ%3D";
        static public string connectionStringProdWithWrite = "SharedAccessSignature=se=2022-11-13T00%3A00%3A00Z&sp=rwdl&spr=https&sv=2018-11-09&sr=c&sig=MLGNR1RNveN7hlw8UhRsyrnstev5rqNh9gCEDQ6A1tc%3D";

        static public string containerName = "bandenspanning";
        static public string prefix = "arriva-nl/cloud-fms/";


        static public int DelayWhenNothingFound = 5; // delay in seconds when no more blobs found
        static public Boolean detailedDistributeLogging;
        static private string _detailedDistributeLoggingFilter;
        static private List<string> _detailedDistributeLoggingFilterList = new List<string>();


        static public string detailedDistributeLoggingFilter
        {
            get
            {
                return _detailedDistributeLoggingFilter;
            }
            set
            {
                _detailedDistributeLoggingFilter = value;
                _detailedDistributeLoggingFilterList.Clear();
                _detailedDistributeLoggingFilterList = _detailedDistributeLoggingFilter.Split(';').ToList();
            }
        }

        static public Boolean detailedDistributeLoggingNeeded(string blobName)
        {
            if (!detailedDistributeLogging) return false;
            else
            {
                foreach (string s in _detailedDistributeLoggingFilterList)
                {
                    if (blobName.Contains(s))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Get the connectionstring to the blob depending on mode test or prod
        /// </summary>
        /// <param name="testProd"></param>
        /// <returns></returns>
        static public string GetConnectionString(TestProd testProd)
        {
            string result;
            //+
            //-- build the url using test or prod
            //-
            if (testProd == TestProd.Test)
            {
                result = urlTest + ";" + connectionStringTestWithWrite;
            }
            else
            {
                result = urlProd + ';' + connectionStringProdWithWrite;
            }
            return result;
        }

        /// <summary>
        /// Get the serviceclient depending on mode test or prod
        /// </summary>
        /// <param name="testProd"></param>
        /// <returns></returns>
        static private BlobServiceClient GetServiceClient(TestProd testProd)
        {
            return  new BlobServiceClient(GetConnectionString(testProd));
        }

        /// <summary>
        /// Get the containerClient depending on mode test or prod
        /// </summary>
        /// <param name="testProd"></param>
        /// <returns></returns>
        static public BlobContainerClient GetContainerClient(TestProd testProd)
        {
            BlobServiceClient serviceClient = GetServiceClient(testProd);
            return serviceClient.GetBlobContainerClient(containerName);
        }

        /// <summary>
        /// Build a string for yyyy mm dd to be put or get a normalized blob
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        public static string DirForStorage(DateTime timeStamp)
        {
            string result = string.Format("{0:yyyy/MM/dd}/", timeStamp);
            //+
            //--- somehow this is now yyyy-MM-dd so replace it
            //-
            result = result.Replace("-", "/");
            return result;

        }




    }
}
