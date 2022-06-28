using Blob3;
using BlobHandler;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlobContinentalUpdater
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

    /*
     * Kill a service manually: 
     *  1.       Click the Start menu
        2.       Click Run or in the search bar type services.msc
        3.       Press Enter
        4.       Look for the service and check the Properties and identify its service name
        5.       Once found, open a command prompt. Type sc queryex [servicename].
                  For EDS Server       type: sc queryex “EDS Server”
        6.       Press Enter
        7.       Identify the PID
                  In my example the PID was 5476, see above screenshot
        8.       In the same command prompt type taskkill /pid [pid number] /f
        ***/



    public partial class BlobContinentalUpdaterService : ServiceBase
    {


        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        CancellationTokenSource cts = new CancellationTokenSource();
        ContinentalUpdater continentalUpdater = new ContinentalUpdater();

        public BlobContinentalUpdaterService()
        {
            InitializeComponent();

            //CanShutdown = true;
            CanHandlePowerEvent = true;
            //CanHandleSessionChangeEvent = true;
            //Microsoft.Win32.SystemEvents.SessionEnded += new Microsoft.Win32.SessionEndedEventHandler(SystemEvents_SessionEnded);

            log.Info("CanHandlePowerEvent set");
        }

        protected override void OnStart(string[] args)
        {
            try
            {  // test voor git
                log.Info("**************** START  *************     Versie   " + RR.RR_Assembly.AssemblyVersionPlusBuildDateTimeEXE);




                //+
                //--- prepare the test prod and create the distributer
                //-
                TestProd testProd = Properties.Settings.Default.TestProd.ToLower() == "prod" ? TestProd.Prod : TestProd.Test;

                //BlobHandler.Statics.detailedDistributeLogging = Properties.Settings.Default.DetailedLogging;
                //BlobHandler.Statics.detailedDistributeLoggingFilter = Properties.Settings.Default.DetailedLoggingFilter;

                FmsBlobToContinental.Statics.DetailedContiLogging = Properties.Settings.Default.DetailedLogging;
                FmsBlobToContinental.Statics.DetailedContiLoggingFilter = Properties.Settings.Default.DetailedLoggingFilter;

                BlobHandler.Statics.DelayWhenNothingFound = Properties.Settings.Default.DelayWhenNothingFound;
                log.InfoFormat("Detailed logging = {0}", FmsBlobToContinental.Statics.DetailedContiLogging);
                log.InfoFormat("Detailed logging for vehicles = {0}", FmsBlobToContinental.Statics.DetailedContiLoggingFilter);
                log.InfoFormat("Delay when no (more) blobs found = {0} sec (Not used!)", BlobHandler.Statics.DelayWhenNothingFound);


                //+
                //--- run the continental updater in a loop
                //-
                continentalUpdater = new ContinentalUpdater();
                cts = new CancellationTokenSource();
                Task.Run(() =>
                {
                    string url = "";
                    if (testProd == TestProd.Prod)
                    {
                        url = FmsBlobToContinental.Properties.Settings.Default.hostNameProd;
                    }
                    else
                    {
                        url = FmsBlobToContinental.Properties.Settings.Default.hostNameTest;
                    }
                    log.InfoFormat("Starting DoNowInALoop testProd={0} url={1}", testProd.ToString(), url);
                    
                    continentalUpdater.DoNowInALoop(testProd, onlyVehicle: "", cts.Token);
                    if (cts.Token.IsCancellationRequested)
                    {
                        log.Info("DoNowInALoop Finished after cancelation");
                    }
                    else
                    {
                        log.Error("DoNowInALoop Finished (should not happen)");
                    }
                }
                );

                //log.InfoFormat("Started DoNowInALoop");
            }
            catch (Exception ex)
            {
                log.Error("", ex);
            }

        }

        //protected override void OnShutdown()
        //{
        //    log.Info("SHUT DOWN.....");
        //    Thread.Sleep(TimeSpan.FromSeconds(3));
        //    OnStop();
        //    base.OnShutdown();//Don't forget to call ServiceBase OnShutdown()
        //}

        //protected override void OnSessionChange(SessionChangeDescription changeDescription)
        //{
        //    log.Info("OnSessionChange.....");
        //    Thread.Sleep(TimeSpan.FromSeconds(3));
        //    Do something
        //    base.OnSessionChange(changeDescription);
        //}

        protected override bool OnPowerEvent(PowerBroadcastStatus powerStatus)
        {
            log.Info("OnPowerEvent....." + powerStatus.ToString());
            //if (powerStatus == PowerBroadcastStatus.Suspend)
            //{
            //    //cts.Cancel();
            //    ////Thread.Sleep(TimeSpan.FromSeconds(3));
            //    //base.RequestAdditionalTime(1000 * 5);
            //    log.Info("Power suspend, call OnStop()");
            //    OnStop();
            //    log.Info("OnPowerEvent2.....");
            //}
            //else if (powerStatus == PowerBroadcastStatus.ResumeSuspend)
            //{
            //    log.Info("Power resumeSuspend, call OnStart()");
            //    //cts.Cancel(); // voor de zekerheid even de oude tasks cancelen.
            //    OnStart(null);
            //}
            return base.OnPowerEvent(powerStatus);
        }
        //void SystemEvents_SessionEnded(object sender, Microsoft.Win32.SessionEndedEventArgs e)
        //{
        //    log.Info("SystemEvents_SessionEnded.....");
        //    Thread.Sleep(TimeSpan.FromSeconds(3));
        //    //your code here
        //}
        protected override void OnStop()
        {
            log.Info("STOPPING.....");
            if (continentalUpdater != null && continentalUpdater.running)
            {
                //+
                //-- cancel old process.
                //-
                cts.Cancel();
                //+
                //-- Wait for distributer to end and react to the cancelation.
                //-
                while (continentalUpdater.running)
                {
                    log.Info("Wait for updater to cancel");
                    Thread.Sleep(TimeSpan.FromSeconds(3));
                }
            }
            log.Info("**************** STOPPED   *************     Versie   " + RR.RR_Assembly.AssemblyVersionPlusBuildDateTimeEXE);
        }
    }
}
