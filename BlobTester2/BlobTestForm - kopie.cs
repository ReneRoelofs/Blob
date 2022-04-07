using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Blob3;
using BlobHandler;
using DevExpress.XtraGrid.Columns;
using FmsBlobToContinental;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using TestProd = BlobHandler.TestProd;

namespace BlobTester
{

    /// <summary>
    ///  nummer van PilotFish Continental is 2233193
    /// </summary>
    public partial class BlobTestForm : Form
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        Boolean loading { get; set; } = true;

        List<Payload> payloadList = new List<Payload>();
        List<String> filesInPayloadList = new List<string>();
        CCVehicle vehicle;
        ContinentalUpdater continentalUpdater;
        RR.Tracer tracer;
        DateTime recenteDateTime;

        /*
         * 
         * willekeurig voorbeeld:
         * BlobEndpoint=https://storagesample.blob.core.windows.net;
SharedAccessSignature=sv=2015-04-05&sr=b&si=tutorial-policy-635959936145100803&sig=9aCzs76n0E7y5BpEi2GvsSv433BZa22leDOZXX%2BXXIU%3D
         * https://docs.microsoft.com/en-us/azure/storage/blobs/storage-quickstart-blobs-dotnet
         * 
         * https://docs.microsoft.com/en-us/azure/storage/blobs/storage-blobs-list?tabs=dotnet
         * 
         * https://www.thecodebuzz.com/getting-latest-modified-file-azure-blob-storage-net/
         * 
         * https://docs.microsoft.com/en-us/azure/storage/blobs/storage-performance-checklist#sas-and-cors
         */


        public BlobTestForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            log.Info("**************** START  *************     Versie   " + RR.RR_Assembly.AssemblyVersionPlusBuildDateTimeEXE);

            tracer = new RR.Tracer(RR.RR_Assembly.AssemblyCallingName);
            tracer.LogDir = "D:\\temp\\blob\\logging\\";
            //  tracer.IncrementFileNumber();
            //RR.Lib.CreateDirForFile(tracer.Filename);
            tracer.trace("Ja");


            //    DevExpress.Data.CurrencyDataController.DisableThreadingProblemsDetection = true;
            rbProdBlob.Checked = RR.MyRegistry.GetBoolean(rbProdBlob.Name, false);

            dtSince.Value = RR.MyRegistry.GetDate(dtSince.Name, DateTime.Now.AddDays(-7));
            dtSinceTime.Value = dtSince.Value;

            cbSince.Checked = RR.MyRegistry.GetBoolean(cbSince.Name);
            cbDownload.Checked = RR.MyRegistry.GetBoolean(cbDownload.Name);
            cbAppend.Checked = RR.MyRegistry.GetBoolean(cbAppend.Name);
            cbUseRecentDate.Checked = RR.MyRegistry.GetBoolean(cbUseRecentDate.Name);
            cbDetailedContiLogging.Checked = RR.MyRegistry.GetBoolean(cbDetailedContiLogging.Name);

            //+
            //--- recenteDateTime wordt alleen gebruikt om hier op het scherm te zetten
            //--- de werkelijke waarde worde in de ContinentalUpdater uit de registry gehaald.
            //-
            recenteDateTime = RR.MyRegistry.GetDate("latestModified", new DateTime(2021, 12, 11));
            lbSince.Text = string.Format("{0:yyyy-MM-dd HH:mm:ss}", recenteDateTime);

            string s = RR.MyRegistry.GetString(tbFilename.Name);
            if (!String.IsNullOrEmpty(s))
            {
                tbFilename.Text = s;
            }

            continentalUpdater = new ContinentalUpdater();


            bsVehiclesAndSensors.DataSource = FmsBlobToContinental.Statics.vehicleList;
            RRXtra.LibForXtragrid.SetMyDefaultXtraGridOptions(gvVehiclesAndSensors);
            RRXtra.LibForXtragrid.SetDateTimeFormat(gvVehiclesAndSensors, "timestampSendToContinental", true);
            RRXtra.LibForXtragrid.SetDateTimeFormat(gvVehiclesAndSensors, "timestampFileSeen", true);
            

            //GridColumn col = gvVehiclesAndSensors.Columns["timestampSendToContinental"];
            //if (col != null)
            //{
            //    col.DisplayFormat.FormatString = "yyyy-MM-dd HH:mm:ss";
            //    col.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            //}
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            loading = false;
        }

        private async void bGetBlobs_Click(object sender, EventArgs e)
        {
       
            DateTime? targetDate;
            if (!cbSince.Checked)
            {
                if (cbUseRecentDate.Checked)
                {
                    targetDate = DateTime.MinValue;
                }
                else
                {
                    targetDate = null;
                }
            }
            else
            {
                targetDate = dtSince.Value.Date;
                targetDate = ((DateTime)targetDate).AddHours(dtSinceTime.Value.Hour).AddMinutes(dtSinceTime.Value.Minute);
            }
            //+
            //--- start background task to get the items in a loop or not
            //-
            await continentalUpdater.GetItemsAndUpdateContinentalAsync(
                testProd: rbTestBlob.Checked ? TestProd.Test : TestProd.Prod,
                onlyRecent: cbUseRecentDate.Checked,
                paramSince: cbUseRecentDate.Checked ? null : targetDate,
                onlyBus: tbBusFilter.Text,
                doLoop: false);

            bsVehicleInQueue.DataSource = continentalUpdater.vehicleSenderList;
        }


        private void rbProd_CheckedChanged(object sender, EventArgs e)
        {
            RR.MyRegistry.PutBoolean(rbProdBlob.Name, rbProdBlob.Checked);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFile(tbFilename.Text, cbAppend.Checked);
        }

        private void OpenFile(string fullPath, Boolean append)
        {
            try
            {
                CloudFmsRootObject rootobject = RR_Serialize.JSON.FromFile<CloudFmsRootObject>(fullPath);
                vehicle = FmsBlobToContinental.Statics.vehicleList.GetOrAdd(rootobject.vehicle.vehicleNo, rootobject.agentSerial);
                if (!append)
                {
                    filesInPayloadList.Clear();
                    payloadList = rootobject.payload.ToList().FindAll(p => p.HasSensorData);
                    filesInPayloadList.Add(fullPath);

                    foreach (Payload p in payloadList)
                    {
                        p.vehicle = vehicle;
                    }
                }
                else
                {
                    if (!filesInPayloadList.Contains(fullPath))
                    {
                        foreach (Payload pl in rootobject.payload.ToList().FindAll(p => p.HasSensorData))
                        {
                            payloadList.Add(pl);
                            pl.vehicle = vehicle;
                        }
                        filesInPayloadList.Add(fullPath);

                        payloadList.Sort((x, y) => ((DateTime)x.ts).CompareTo((DateTime)y.ts));
                    }
                }

                lbVehicle.Text = vehicle.Text2();

                bsPayload.DataSource = payloadList;
                //bindingSource1.DataSource = sdList;
                gcPayload.RefreshDataSource();

                SetPayloadFieldNames();

                RRXtra.LibForXtragrid.SetDateTimeFormat(gvPayload, "ts", true);

                //GridColumn col = gvPayload.Columns["ts"];
                //col.DisplayFormat.FormatString = "yyyy-MM-dd HH:mm:ss";
                //col.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;

            }
            catch (Exception ex)
            {
                log.Error("", ex);
            }
        }

        private void SetPayloadFieldNames()
        {
            GridColumn col;
            col = gvPayload.Columns["FEF4"]; if (col != null) { col.Caption = "FEF4 Tire Condition with Temperature"; }
            col = gvPayload.Columns["FC42"]; if (col != null) { col.Caption = "FC42 Tire Condition 2"; }
            col = gvPayload.Columns["FF00"]; if (col != null) { col.Caption = "FF00 CPC SystemConfiguration"; }
            col = gvPayload.Columns["FF01"]; if (col != null) { col.Caption = "FF01 CPC System Status"; }
            col = gvPayload.Columns["FF02"]; if (col != null) { col.Caption = "FF02 CPC TTM Data"; }
            col = gvPayload.Columns["FF04"]; if (col != null) { col.Caption = "FF04 CPC Graphical Position Configuration"; }
        }

        private void bsPayLoad_CurrentChanged(object sender, EventArgs e)
        {
            try
            {
                Payload payload = (Payload)bsPayload.Current;
                if (payload != null)
                {
                    payload.EnrichSensorWithTTM();
                    bindingSource2.DataSource = payload.sensorsDataList;
                    gcTirePressure.RefreshDataSource();
                    //    RR.LibForXtragrid.SetMyDefaultXtraGridOptions(gridView2);

                    bindingSource3.DataSource = payload.ttmDataList;
                    gcValues.RefreshDataSource();
                    //    RR.LibForXtragrid.SetMyDefaultXtraGridOptions(gridView3);

                    GridColumn col = gvPayload.Columns["FC42"];
                    if (col != null)
                    {
                        col.VisibleIndex = 3;
                    }
                    col = gvPayload.Columns["FF02"];
                    if (col != null)
                    {
                        col.VisibleIndex = 4;
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }


        private async void bOpenFile_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = tbFilename.Text;
            openFileDialog1.InitialDirectory = Path.GetDirectoryName(tbFilename.Text);
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                foreach (string s in openFileDialog1.FileNames)
                {
                    tbFilename.Text = s;
                    Thread.Sleep(TimeSpan.FromMilliseconds(1)); // refresh screen.
                    OpenFile(s, cbAppend.Checked);
                }
            }
        }



        List<BlobFileName> BlobFiles = new List<BlobFileName>();

        private void button4_Click(object sender, EventArgs e)
        {
            BlobFiles.Clear();
            List<string> files = Directory.GetFiles(Path.GetDirectoryName(tbFilename.Text)).ToList();
            foreach (string s in files)
            {
                BlobFiles.Add(new BlobFileName { Name = s });
            }
            bsBlobFiles.DataSource = BlobFiles;
            gcFiles.RefreshDataSource();
        }

        private void tbFilename_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(tbFilename.Text))
            {
                RR.MyRegistry.PutString(tbFilename.Name, tbFilename.Text);
            }
        }


        private void button3_Click(object sender, EventArgs e)
        {
            tbFeedback2.Clear();
        }

        private void bSendOne_Click(object sender, EventArgs e)
        {
            if (rbTestConti.Checked)
            {
                vehicle.testProd = TestProd.Test;
            }
            else
            {
                vehicle.testProd = TestProd.Prod;
            }

            Boolean succesfull = true;

            List<Payload> tmpList = payloadList.FindAll(P => P.NewEnoughToSend(vehicle, cbUseTimestampNow.Checked));
            log.InfoFormat("Sending {0} payload items", tmpList.Count);

            Task.Factory.StartNew(() =>
            {
                int index = 0;
                foreach (Payload payload in tmpList)
                {
                    log.InfoFormat("{0}/{1}", index++.ToString(), tmpList.Count);
                    if (payload != null)
                    {
                        List<SensorData> sensorsInThisPayload;
                        succesfull = succesfull &&
                            payload.SendToContinental(
                                    cbUseTimestampNow.Checked,
                                    out sensorsInThisPayload);
                    }
                    if (!succesfull)
                    {
                        break;
                    }
                }
            });

            if (tmpList.Count == 0)
            {
                log.Warn("Nothing new enough to send");
            }
            else
            {
                FmsBlobToContinental.Statics.SaveVehicleList();
            }
        }


        private void rbTestConti_CheckedChanged(object sender, EventArgs e)
        {
            if (rbProdConti.Checked)
            {
                FmsBlobToContinental.Statics.StaticsTestProd = TestProd.Prod;
            }
            else
            {
                FmsBlobToContinental.Statics.StaticsTestProd = TestProd.Test;
            }
        }

        private void bindingSource4_CurrentChanged(object sender, EventArgs e)
        {
            BlobFileName blobFilename = (BlobFileName)bsBlobFiles.Current;
            if (blobFilename != null)
            {
                tbFilename.Text = blobFilename.FullPath;
                OpenFile(tbFilename.Text, cbAppend.Checked);
            }
        }

        private void cbSince_CheckedChanged(object sender, EventArgs e)
        {
            if (!loading)
            {
                RR.MyRegistry.PutDate(dtSince.Name, dtSince.Value);
                RR.MyRegistry.PutBoolean(cbSince.Name, cbSince.Checked);
            }
            dtSince.Enabled = cbSince.Checked;
            dtSinceTime.Enabled = cbSince.Checked;
            cbUseRecentDate.Checked = !cbSince.Checked;
        }

        private void cbDownload_CheckedChanged(object sender, EventArgs e)
        {
            if (!loading)
            {
                RR.MyRegistry.PutBoolean(cbDownload.Name, cbDownload.Checked);
            }
        }

        private void cbAppend_CheckedChanged(object sender, EventArgs e)
        {
            if (!loading)
            {
                RR.MyRegistry.PutBoolean(cbAppend.Name, cbAppend.Checked);
            }
        }

        private void tbBusFilter_TextChanged(object sender, EventArgs e)
        {
            if (tbBusFilter.Text == "")
            {
                cbDownload.Checked = false;
                cbDownload.Enabled = false;
            }
            else
            {
                cbDownload.Enabled = true;
            }
        }

        private void timerShowQueueLen_Tick(object sender, EventArgs e)
        {
            lbBlobQueue.Text = continentalUpdater.Text();
            FmsBlobToContinental.Statics.SaveSensorList();
            FmsBlobToContinental.Statics.SaveVehicleList();
            //gcVehicleInQueue.RefreshDataSource();

        }

        private void cbUseRecentDate_CheckedChanged(object sender, EventArgs e)
        {
            RR.MyRegistry.PutBoolean(cbUseRecentDate.Name, cbUseRecentDate.Checked);
            cbSince.Checked = !cbUseRecentDate.Checked;
            lbSince.Enabled = cbSince.Enabled;
        }

        private void cbDetailedContiLogging_CheckedChanged(object sender, EventArgs e)
        {
            RR.MyRegistry.PutBoolean(cbDetailedContiLogging.Name, cbDetailedContiLogging.Checked);
            FmsBlobToContinental.Statics.DetailedContiLogging = cbDetailedContiLogging.Checked;

        }

        private void richEditControl1_Click(object sender, EventArgs e)
        {

        }


        public void onException(Exception ex)
        {
        }

        public async void onCompleted()
        {
            //TestProd testProd = rbTestBlob.Checked ? TestProd.Test : TestProd.Prod;
            //await continentalUpdater.GetItemsAndUpdateContinentalParallel(
            // testProd: testProd,
            // onlyRecent: true,
            // since: null,
            // onlyBus: "",
            // doLoop: false);

        }

        private async void bDistribute_Click(object sender, EventArgs e)
        {
            TestProd testProd = rbTestBlob.Checked ? TestProd.Test : TestProd.Prod;
            BlobDistributer distributer = new BlobDistributer(testProd);
            await Task.Run(() => distributer.DistributeAllNow());
            //Task.Factory.StartNew(() => distributer.DistributeAllNow());
        }


        BindingList<BlobItem> itemsOnForm = new BindingList<BlobItem>();
        /// <summary>
        /// receive some blobs an put htem in ItemsOnForm and show them in the grid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void bViewData_Click(object sender, EventArgs e)
        {
            DateTime? targetDate;
            if (!cbSince.Checked)
            {
                if (cbUseRecentDate.Checked)
                {
                    targetDate = DateTime.MinValue;
                }
                else
                {
                    targetDate = null;
                }
            }
            else
            {
                targetDate = dtSince.Value.Date;
                targetDate = ((DateTime)targetDate).AddHours(dtSinceTime.Value.Hour).AddMinutes(dtSinceTime.Value.Minute);
            }
            BlobHandler.BlobItemList blobItemlist = new BlobHandler.BlobItemList(AddBlobToItemsOnForm);

            List<BlobItem> items = new List<BlobItem>();
            bsBlobItems.DataSource = itemsOnForm;
            gcFiles.DataSource = bsBlobItems;
            bsPayload.DataSource = payloadList;
            SetPayloadFieldNames();

            itemsOnForm.Clear();

            _ = await blobItemlist.GetBlobItemsASync(
                testProd: rbTestBlob.Checked ? TestProd.Test : TestProd.Prod,
                (DateTime)targetDate,
                ((DateTime)targetDate).AddDays((int)spDays.Value),
                cbDownload.Checked,
                onlyVehicle: tbBusFilter.Text
                );

            containerClient = null;// reset when showing the payload.
        }

        private void AddBlobToItemsOnForm(BlobItem blobItem)
        {
            this.BeginInvoke(new MethodInvoker(() => itemsOnForm.Add(blobItem)));

        }


        BlobContainerClient containerClient;

        private void ShowPayload(BlobItem blobItem)
        {
            if (blobItem == null)
            {
                return;
            }
            this.payloadList.Clear();
            if (containerClient == null)
            {
                containerClient = BlobHandler.Statics.GetContainerClient(testProd: rbTestBlob.Checked ? TestProd.Test : TestProd.Prod);
            }
            BlobClient blobClient = containerClient.GetBlobClient(blobItem.Name);
            BlobDownloadResult x = blobClient.DownloadContent();
            BlobFileName blobFilename = new BlobFileName();
            blobFilename.Name = blobItem.Name;
            string str = x.Content.ToString();
            str = str.Replace("{\"ts\"", "\r\n{\"ts\"");
            try
            {
                CloudFmsRootObject rootobject = RR_Serialize.JSON.FromString<CloudFmsRootObject>(str);
                CCVehicle vehicle = FmsBlobToContinental.Statics.vehicleList.GetOrAdd(rootobject.vehicle.vehicleNo, rootobject.agentSerial);

                foreach (Payload payload in rootobject.payload)
                {
                    payload.vehicle = vehicle;
                    this.payloadList.Add(payload);
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }

            gcPayload.RefreshDataSource();
            RRXtra.LibForXtragrid.SetDateTimeFormat(gvPayload, "ts", true);


        }

        private void bsBlobItems_CurrentChanged(object sender, EventArgs e)
        {
            ShowPayload((BlobItem)bsBlobItems.Current);
        }

        private void gcPayload_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void lbBlobQueue_Click(object sender, EventArgs e)
        {

        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void dtSinceTime_ValueChanged(object sender, EventArgs e)
        {

        }

        private void dtSince_ValueChanged(object sender, EventArgs e)
        {

        }

        private void lbVehicle_TextChanged(object sender, EventArgs e)
        {

        }

        private void lbVehiclelabel_Click(object sender, EventArgs e)
        {

        }

        private void cbUseTimestampNow_CheckedChanged(object sender, EventArgs e)
        {
            continentalUpdater.vehicleSenderList.SetUpdateNow(cbUseTimestampNow.Checked);
        }

        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void splitContainer4_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }

        private void tbFeedback2_TextChanged(object sender, EventArgs e)
        {

        }

        private void bindingNavigator4_RefreshItems(object sender, EventArgs e)
        {

        }

        private void bindingNavigatorAddNewItem_Click(object sender, EventArgs e)
        {

        }

        private void bindingNavigatorCountItem1_Click(object sender, EventArgs e)
        {

        }

        private void bindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {
            BlobItem blobItem = (BlobItem)bsBlobItems.Current;
        }

        private void bindingNavigatorMoveFirstItem1_Click(object sender, EventArgs e)
        {

        }

        private void bindingNavigatorMovePreviousItem1_Click(object sender, EventArgs e)
        {

        }

        private void bindingNavigatorSeparator2_Click(object sender, EventArgs e)
        {

        }

        private void bindingNavigatorPositionItem1_Click(object sender, EventArgs e)
        {

        }

        private void bindingNavigatorSeparator3_Click(object sender, EventArgs e)
        {

        }

        private void bindingNavigatorMoveNextItem1_Click(object sender, EventArgs e)
        {

        }

        private void bindingNavigatorMoveLastItem1_Click(object sender, EventArgs e)
        {

        }

        private void bindingNavigatorSeparator4_Click(object sender, EventArgs e)
        {

        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void gridControl4_Click(object sender, EventArgs e)
        {

        }

        private void bsVehicleInQueue_CurrentChanged(object sender, EventArgs e)
        {

        }

        private void splitContainer2_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }

        private void gridControlFiles_Click(object sender, EventArgs e)
        {

        }

        private void bindingNavigator1_RefreshItems(object sender, EventArgs e)
        {

        }

        private void bindingNavigatorCountItem_Click(object sender, EventArgs e)
        {

        }

        private void bindingNavigatorMoveFirstItem_Click(object sender, EventArgs e)
        {

        }

        private void bindingNavigatorMovePreviousItem_Click(object sender, EventArgs e)
        {

        }

        private void bindingNavigatorSeparator_Click(object sender, EventArgs e)
        {

        }

        private void bindingNavigatorPositionItem_Click(object sender, EventArgs e)
        {

        }

        private void bindingNavigatorSeparator1_Click(object sender, EventArgs e)
        {

        }

        private void bindingNavigatorMoveNextItem_Click(object sender, EventArgs e)
        {

        }

        private void bindingNavigatorMoveLastItem_Click(object sender, EventArgs e)
        {

        }

        private void splitContainer3_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }

        private void gridControl2_Click(object sender, EventArgs e)
        {

        }

        private void bindingSource2_CurrentChanged(object sender, EventArgs e)
        {

        }

        private void bindingNavigator2_RefreshItems(object sender, EventArgs e)
        {

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripLabel1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {

        }

        private void toolStripSeparator1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripTextBox1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripSeparator2_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {

        }

        private void toolStripSeparator3_Click(object sender, EventArgs e)
        {

        }

        private void gridControl3_Click(object sender, EventArgs e)
        {

        }

        private void bindingSource3_CurrentChanged(object sender, EventArgs e)
        {

        }

        private void bindingNavigator3_RefreshItems(object sender, EventArgs e)
        {

        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {

        }

        private void toolStripLabel2_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton8_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton9_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton10_Click(object sender, EventArgs e)
        {

        }

        private void toolStripSeparator4_Click(object sender, EventArgs e)
        {

        }

        private void toolStripTextBox2_Click(object sender, EventArgs e)
        {

        }

        private void toolStripSeparator5_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton11_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton12_Click(object sender, EventArgs e)
        {

        }

        private void toolStripSeparator6_Click(object sender, EventArgs e)
        {

        }

        private void statusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void lbVoortgang_Click(object sender, EventArgs e)
        {

        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void cbDetailedDistributeLogging_CheckedChanged(object sender, EventArgs e)
        {
            BlobHandler.Statics.detailedDistributeLogging = cbDetailedDistributeLogging.Checked;
        }

        
    }
}
