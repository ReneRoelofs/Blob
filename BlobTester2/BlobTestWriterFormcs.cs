using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Blob3;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using Azure;
using BlobHandler;
using BlobHandler;

namespace BlobTester
{
    //https://docs.microsoft.com/en-us/azure/storage/blobs/storage-quickstart-blobs-dotnet
    public partial class BlobTestWriterFormcs : Form
    {
        Progress<string> progress = new Progress<string>();

        public BlobTestWriterFormcs()
        {
            InitializeComponent();
        }
        private void BlobTestWriterFormcs_Load(object sender, EventArgs e)
        {

            progress.ProgressChanged += Feedback;


        }
        private async void bDownload_Click(object sender, EventArgs e)
        {

            TestProd testProd = rbTestBlob.Checked ? TestProd.Test : TestProd.Prod;
            int i = 0;

            try
            {
                textBox1.AppendText("connectionstring = " + Statics.GetConnectionString(testProd) + "\r\n");
                BlobContainerClient containerClient = Statics.GetContainerClient(testProd); // get container client for 'bandenspanning'
                if (containerClient != null)
                {
                    //+
                    //--- we hebben een containerClient.. eens even kijken wat daar in zit, in de 'arriva' map;
                    //-
                    textBox1.AppendText("get blobs from  = " + Statics.prefix + "\r\n");
                    Azure.Pageable<BlobItem> allBlobs = containerClient.GetBlobs(BlobTraits.None, BlobStates.None, Statics.prefix);
                    textBox1.AppendText("got blobs from  = " + Statics.prefix + " going to enumerate \r\n");

                    //DIT DUURT LANG:    List<BlobItem> allBlobsInList = containerClient.GetBlobs(BlobTraits.None, BlobStates.None, "2021").ToList();
                    //+
                    //--- try to enumerate through the blobs
                    //-
                    Boolean enumerateOK = false;
                    try
                    {
                        if (true)
                        {
                            Stopwatch sw = new Stopwatch();
                            sw.Start();
                            IEnumerator<BlobItem> enumerator = allBlobs.GetEnumerator();
                            while (enumerator.MoveNext())
                            {
                                i++;
                                BlobItem blobItem = enumerator.Current;  // get one blobItem
                                textBox1.AppendText(string.Format("{0} {1} {2}\r\n", RR.Lib.Elapsed(sw), i, blobItem.Name));

                                BlobFileName blobFileName = new BlobFileName();
                                blobFileName.Name = blobItem.Name; // set vehicle and datetime;
                                string newBlobName = string.Format("{0}/{1}", blobFileName.DirForStorage, blobFileName.Name); //  /2021/10/15/9876/......
                                string fullPath = Path.Combine(BlobHandler.Properties.Settings.Default.DownloadDir, newBlobName);

                                BlobClient blobClientSrc = containerClient.GetBlobClient(blobItem.Name); // we need a blobClient do download the blob
                                RR.Lib.CreateDirForFile(fullPath);  // prepare the directory
                                blobClientSrc.DownloadTo(fullPath); // download the blob

                                DateTimeOffset lastModified = (DateTimeOffset) blobItem.Properties.LastModified;

                            }
                            enumerateOK = true; // stays false in exception
                        }
                    }
                    catch (Exception ex)
                    {
                        textBox1.AppendText("Enumerate blobItems Failed \r\n" + ex.Message);
                    }
                    if (!enumerateOK)
                    {
                        //+
                        //--- try to get blobs as a list
                        //-
                        try
                        {
                            List<BlobItem> allBlobsList = containerClient.GetBlobs().ToList();
                        }
                        catch (Exception ex2)
                        {
                            textBox1.AppendText("Get blobs as list failed \r\n" + ex2.Message);
                        }

                    }
                }
            }

            catch (Exception ex)
            {
            }
            textBox1.AppendText("got blobs " + i.ToString() + " blobs \r\n");


        }

        private void button2_Click(object sender, EventArgs e)
        {
            TestProd testProd = rbTestBlob.Checked ? TestProd.Test : TestProd.Prod;
            BlobDistributer distributer = new BlobDistributer(testProd,null);
            Task.Factory.StartNew(() => distributer.DistributeAllNow(BlobDistributer.CancelationTokenSource.Token,false));
        }

        public void Feedback(object sender, string s)
        {
            Feedback(s);
        }

        public void Feedback(string s)
        {
            if (this.InvokeRequired)
            {
                if (this.InvokeRequired)
                {
                    this.BeginInvoke(new MethodInvoker(() => Feedback(s)));
                    return;
                }
            }
            else
            {
                textBox1.AppendText(s);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Feedback("\r\n");
            string prefix = "2021/10/19";
            BlobContainerClient containerClient = Statics.GetContainerClient(rbTestBlob.Checked ? TestProd.Test : TestProd.Prod);
            List<BlobItem> allBlobsInList = containerClient.GetBlobs(BlobTraits.None, BlobStates.None, prefix).ToList();

            foreach (BlobItem blobItem in allBlobsInList)
            {
                //  BlobClient blobClientSrc = containerClient.GetBlobClient(blobItem.Name);
                //  BlobDownloadResult x = blobClientSrc.DownloadContent();
                //  string s = x.ToString();
                //  int len = s.Length;
                int len = 0;

                Feedback(string.Format("{0} {1} \r\n", blobItem.Name, len));
            }
            int i = allBlobsInList.Count;
            Feedback(string.Format("{0} items found for '{1}'", i, prefix));
        }

        private void button4_Click(object sender, EventArgs e)
        {

            Feedback("\r\n");
            string prefix;
            prefix = textBox2.Text;
            BlobContainerClient containerClient = Statics.GetContainerClient(rbTestBlob.Checked ? TestProd.Test : TestProd.Prod);

            Stopwatch sw = new Stopwatch();
            sw.Start();
            Feedback(string.Format("Checking '{0}'   ", prefix));

            List<BlobItem> allBlobsInList = containerClient.GetBlobs(BlobTraits.None, BlobStates.None, prefix).ToList();

            //foreach (BlobItem blobItem in allBlobsInList)
            //{
            ////    //  BlobClient blobClientSrc = containerClient.GetBlobClient(blobItem.Name);
            ////    //  BlobDownloadResult x = blobClientSrc.DownloadContent();
            ////    //  string s = x.ToString();
            ////    //  int len = s.Length;
            //    int len = 0;

            //    Feedback(string.Format("{0} {1} \r\n", blobItem.Name, len));
            //}
            int i = allBlobsInList.Count;
            sw.Stop();
            Feedback(string.Format("{0} items found for '{1}' in {2}", i, prefix, RR.Lib.Elapsed(sw)));
        }

   
    }
}
