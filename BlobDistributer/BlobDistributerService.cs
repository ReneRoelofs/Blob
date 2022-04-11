using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BlobHandler;

namespace BlobDistributer
{

    /*
     * ik kreeg opeens een fout
     * Kan bestand of assembly System.ValueTuple, Version=4.0.3.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51 
     * 
     * opgelost door onderstaande te verwijderen uit de configuratie
     
      <dependentAssembly>
        <assemblyIdentity name = "System.ValueTuple" publicKeyToken="cc7b13ffcd2ddd51" culture="neutral" />
        <bindingRedirect oldVersion = "0.0.0.0-4.0.3.0" newVersion="4.0.3.0" />
      </dependentAssembly>


    */

    /// <summary>
    /// The BlobDistributerService distributes all blobs into subdirectories for yyyy/MM/dd on the blobstorage
    /// this is the first step in the Continental blob tire pressure flow.
    /// After that the payloads of the blobs can be send to continental.
    /// The blobs in the distributer are loaded in a completely random order, not in any time-based order whatsover.
    /// therefore this feed cannot be used to update continental directly. That is done in ContiTirePressureUpdater, handling the
    /// time of the payloads.
    /// </summary>
    public partial class BlobDistributerService : ServiceBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        BlobHandler.BlobDistributer distributer;

        public BlobDistributerService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                log.Info("**************** START  *************     Versie   " + RR.RR_Assembly.AssemblyVersionPlusBuildDateTimeEXE);

                //+
                //--- prepare the test prod and create the distributer
                //-
                TestProd testProd = Properties.Settings.Default.TestProd.ToLower() == "prod" ? TestProd.Prod : TestProd.Test;
                BlobHandler.Statics.detailedDistributeLogging = Properties.Settings.Default.DetailedLogging;
                BlobHandler.Statics.detailedDistributeLoggingFilter = Properties.Settings.Default.DetailedLoggingFilter;
                BlobHandler.Statics.DelayWhenNothingFound = Properties.Settings.Default.DelayWhenNothingFound;
                log.InfoFormat("Detailed logging = {0}", BlobHandler.Statics.detailedDistributeLogging);
                log.InfoFormat("Detailed logging if BlobName contains = {0}", BlobHandler.Statics.detailedDistributeLoggingFilter);
                log.InfoFormat("TestProd = {0}", testProd.ToString());
                log.InfoFormat("Src URL = {0}", BlobHandler.Statics.GetConnectionString(testProd));
                log.InfoFormat("Delay when no (more) blobs found = {0} sec", BlobHandler.Statics.DelayWhenNothingFound);
                log.InfoFormat("Target URL = Src+yyyy/MM/dd", "");

                distributer = new BlobHandler.BlobDistributer(testProd, DistributeFinished);

                BlobHandler.BlobDistributer.CancelationTokenSource = new CancellationTokenSource();

                //+
                //--- run the distributer 'forever'
                //-
                Task.Run(() => distributer.DistributeAllNow(
                    BlobHandler.BlobDistributer.CancelationTokenSource.Token, doLoop: true));
            }
            catch (Exception ex)
            {
                log.Error("", ex);
            }
        }

        protected override void OnStop()
        {
            if (distributer != null && distributer.running)
            {
                //+
                //-- cancel old process.
                //-
                if (BlobHandler.BlobDistributer.CancelationTokenSource != null)
                {
                    BlobHandler.BlobDistributer.CancelationTokenSource.Cancel();
                }
                //+
                //-- Wait for distributer to end and react to the cancelation.
                //-
                while (distributer.running)
                {
                    log.Info("Wait for distributer to cancel");
                    Thread.Sleep(TimeSpan.FromSeconds(3));
                }
            }
            log.Info("**************** STOPPED   *************     Versie   " + RR.RR_Assembly.AssemblyVersionPlusBuildDateTimeEXE);
        }

        /// <summary>
        /// callback from the distributer
        /// </summary>
        private void DistributeFinished()
        {
            log.Info("BlobDistributerService finished");
        }
    }
}
