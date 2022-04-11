using Blob3;
using BlobHandler;
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
            BlobHandler.Statics.detailedDistributeLogging = Properties.Settings.Default.DetailedLogging;
            BlobHandler.Statics.detailedDistributeLoggingFilter = Properties.Settings.Default.DetailedLoggingFilter;
            BlobHandler.Statics.DelayWhenNothingFound = Properties.Settings.Default.DelayWhenNothingFound;
            log.InfoFormat("Detailed logging = {0}", BlobHandler.Statics.detailedDistributeLogging);
            log.InfoFormat("Detailed logging if BlobName contains = {0}", BlobHandler.Statics.detailedDistributeLoggingFilter);
            log.InfoFormat("Delay when no (more) blobs found = {0} sec (Not used!)", BlobHandler.Statics.DelayWhenNothingFound);


            //+
            //--- run the continental updater in a loop
            //-
            continentalUpdater = new ContinentalUpdater();
            Task.Run(() =>
            {
                log.InfoFormat("Starting DoNowInALoop");
                continentalUpdater.DoNowInALoop(testProd,  onlyVehicle: "",cts.Token);
            }
            );

            log.InfoFormat("Started DoNowInALoop");
        }
        catch (Exception ex)
        {
            log.Error("", ex);
        }

    }

    protected override void OnStop()
    {
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
