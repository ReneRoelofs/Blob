using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobHandler
{

    /// <summary>
    /// Class to parse vehicleNumber, Filedate from the blobfilename.
    /// </summary>
    public class BlobFileName
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        private int _vehicle = -1;
        private DateTime _timeStamp = DateTime.MinValue;
        public string AgentSerialNumber;

        public int vehicleNumber
        {
            get
            {
                if (_vehicle == -1)
                {

                }
                return _vehicle;
            }
        }

        public DateTime timeStamp
        {
            get
            {
                return _timeStamp;
            }
            set
            {
            }
        }

        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            // = C:\Klanten\Arriva\Blob3\Blob3\bin\Debug\arriva-nl\cloud-fms\9605_cloudfms1-2_20211103093958_2232864_1635932822473.json
            //                                                               vvvv             yyyymmddhhMMss_ppppppp_???
            set
            {
                try
                {
                    FullPath = value;
                    string filename = Path.GetFileName(value);
                    // = 9605_cloudfms1-2_20211103093958_2232864_1635932822473.json
                    string[] tokens = filename.Split('_');

                    _vehicle = RR.Lib.Str2AnyInt(tokens[0]); //9605
                    if (_vehicle == 0)
                    {
                        log.WarnFormat("Vehicle is 0 in filename {0}",filename);
                    }

                    CultureInfo provider = CultureInfo.InvariantCulture;
                    _name = filename;
                    _timeStamp = DateTime.ParseExact(tokens[2], "yyyyMMddHHmmss", provider); //20211030134212
                    if (_timeStamp.Year < 2021) // catch the 1970 date stuff.
                    {
                        _timeStamp = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, _timeStamp.Hour, _timeStamp.Minute, _timeStamp.Second);
                    }

                    if (tokens.Length >= 4)
                    {
                        AgentSerialNumber = tokens[3];
                    }
                }
                catch (Exception ex)
                {
                    log.Error(string.Format("Set Name {0}", value), ex);
                }
            }
        }

        public string DirForStorage
        {
            get
            {
                string result = string.Format("{0:yyyy/MM/dd}/{1}", timeStamp, vehicleNumber);
                //+
                //--- somehow this is now yyyy-MM-dd so replace it
                //-
                result = result.Replace("-", "/");
                return result;
            }
        }

        public string FullPath { get; set; }
    }

   
}
