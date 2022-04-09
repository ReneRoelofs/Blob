using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using BlobHandler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlobHandler
{
    public class BlobDistributer
    {



        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        static public CancellationTokenSource CancelationTokenSource;



        TestProd testProd;
        public int MaxRecs = 5000;
        BlobContainerClient containerClient;
        public Boolean running;
        OnCompleted completed;
        //  int nProcesses = 0;

        public BlobDistributer(TestProd testProd, OnCompleted onCompleted)
        {
            this.testProd = testProd;
            this.completed = onCompleted;
            containerClient = Statics.GetContainerClient(testProd); // get container client for 'bandenspanning'
        }


        int nMoved, nDeleted, nTotal, nGrandTotal = 0, nError = 0, nNonExistened;
        /// <summary>
        /// Pick-up all items from  'arriva' and move them to the yyyy/mm/dd subfolders
        /// </summary>
        public async Task DistributeAllNow(CancellationToken ct, Boolean doLoop)
        {
            bool foundAtLeastOne = true;
            List<Task> allTasks = new List<Task>();
            running = true;
            do // if !DoLoop do it once otherwise loop via the while doLoop at the bottom of this statement
            {
                while (foundAtLeastOne)
                {
                    foundAtLeastOne = false;
                    nMoved = 0;
                    nDeleted = 0;
                    nTotal = 0;
                    nError = 0;
                    nNonExistened = 0;
                    //+
                    //--- haal alle items op uit de 'arriva' directory
                    //-
                    log.InfoFormat("get {0} blobs from {1}", MaxRecs, Statics.prefix);

                    Azure.Pageable<BlobItem> allBlobs = containerClient.GetBlobs(BlobTraits.None, BlobStates.None, Statics.prefix);

                    // NIET DIT DUS: List<BlobItem> allBlobsInList = containerClient.GetBlobs(BlobTraits.None, BlobStates.None, "test").ToList();

                    //+
                    //--- try to enumerate through the blobs, download them en upload them to the container in another subdirectory
                    //-
                    try
                    {
                        Stopwatch sw = new Stopwatch();
                        sw.Start();

                        //+
                        //--- move all blobs, and fill the list of backgroundtasks allTask
                        //--- this list is used to wait for all tasks to complete
                        //-
                        int i = await MoveSomeBlobs(allBlobs, sw, allTasks, MaxRecs, ct); // and set nTotal, nMoved and nDeleted
                        if (i > 0)
                        {
                            foundAtLeastOne = true;
                        }
                        sw.Stop();
                    }
                    catch (Exception ex)
                    {
                        log.Error("Enumerate blobItems Failed", ex);
                    }
                    if (ct.IsCancellationRequested)
                    {
                      //  log.Info("Canceled.");
                        break;
                    }
                } // while at least one found
                log.InfoFormat("no (more) blobs found", MaxRecs, Statics.prefix);
                if (doLoop && !ct.IsCancellationRequested)
                {
                    foundAtLeastOne = true;// make sure to go in to the innerloop above.
        
                    
                    log.InfoFormat("Loop: Waiting for {0} seconds",Statics.DelayWhenNothingFound);
                    for (int i = 0; i < 10; i++)
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(Statics.DelayWhenNothingFound / 10)); // 
                        if (ct.IsCancellationRequested)
                        {
                            break;
                        }
                    }
               }
            } while (doLoop && !ct.IsCancellationRequested); 

            running = false;
            log.InfoFormat("BlobDistributer finished", MaxRecs, Statics.prefix);
            if (completed != null)
            {
                completed();
            }
        }

        /// <summary>
        /// Move al blobs to yyyy/mm/dd destination
        /// </summary>
        /// <param name="allBlobs"></param>
        /// <returns>Number of blobs found in src</returns>
        private async Task<int> MoveSomeBlobs(Azure.Pageable<BlobItem> allBlobs, Stopwatch sw, List<Task> allTasks, int MaxRecs, CancellationToken ct)
        {
            IEnumerator<BlobItem> enumerator = allBlobs.GetEnumerator();
            //try
            // {
            int i = 0;
            //  int nMoved = 0;
            while (enumerator.MoveNext())
            {
                if (ct.IsCancellationRequested)
                {
                    log.Info("Canceled");
                    break;
                }

                BlobItem blobItem = enumerator.Current;
                i++;
                nGrandTotal++;
                nTotal++;
                //+
                //--- kopieer de inhoud van src naar target;
                //-
                Boolean upload = false;
                try
                {
                    Boolean useTask = true;
                    if (useTask)
                    {
                        Task task = MoveOneBlob(blobItem);
                        allTasks.Add(task);
                    }
                    else
                    {
                        await MoveOneBlob(blobItem);
                    }

                    upload = true;
                }
                catch (Exception ex)
                {
                    log.Error("", ex);
                }

                if (i % 500 == 0)
                {
                    GiveFeedback();
                }
                //+
                //--- not to much in one go
                //-
                if (i > MaxRecs)
                {
                    break;
                    //Thread.Sleep(TimeSpan.FromSeconds(10));
                }
            } // while next blob...


            // wachten met feedback..
            await Task.Run(() => AllTasksDone(allTasks));



            GiveFeedback();

            return i;
        }

        private void GiveFeedback()
        {
            string s = string.Format("#total={0} #blobs handled this run ={1} moved={2} onlyDeleted={3} nonExistent={4} error={5}",
                nGrandTotal, nTotal, nMoved, nDeleted, nNonExistened, nError);
            log.InfoFormat(s);
        }

        /// <summary>
        /// Move one blobs to yyyy/mm/dd destination
        /// this operations waits for te move to complete. so it should be called in a task.
        /// </summary>
        /// <param name="blobItem"></param>
        private async Task MoveOneBlob(BlobItem blobItem)
        {
            //+
            //--- download the content an upload to the new name
            //-
            BlobFileName blobFileName = new BlobFileName();

            //+
            //--- get the name, date etc for this blobItem
            //-

            blobFileName.Name = blobItem.Name; // set vehicle and datetime;
            //+
            //--- create a new name with subdirectories
            //-
            string newBlobName = string.Format("{0}/{1}", blobFileName.DirForStorage, blobFileName.Name);

            if (Statics.detailedDistributeLogging && Statics.detailedDistributeLoggingNeeded(blobItem.Name))// || blobItem.Name.Contains("_2233193_"))  // speciale debug log voor Pilotfish bij Continental Duitsland.
            {
                log.DebugFormat("Move blob {0} to {1}", blobItem.Name, newBlobName);
            }

            BlobClient blobClientSrc = containerClient.GetBlobClient(blobItem.Name);
            BlobClient blobClientTarget = containerClient.GetBlobClient(newBlobName);

            //FmsBlobToContinental.Statics.vehicleList;

            try
            {
                string fullPath = "";
                if (DateTime.Now.Subtract(blobFileName.timeStamp) <= TimeSpan.FromDays(30))
                {
                    if (blobClientSrc.Exists())
                    {
                        await blobClientTarget.StartCopyFromUriAsync(blobClientSrc.Uri);
                        nMoved++;
                    }
                    else
                    {
                        nNonExistened++;
                    }
                }
                else
                {
                    nDeleted++;
                    DownloadABlobfile(blobClientSrc);
                }
                containerClient.DeleteBlob(blobItem.Name);
            }
            catch (Exception ex)
            {
                log.ErrorFormat("Distribute from {0} to {1} : error :{2}", blobItem.Name, newBlobName, ex.Message);
            }
        }

        private void DownloadABlobfile(BlobClient blobClientSrc)
        {
            string fullPath = Path.Combine(Properties.Settings.Default.DownloadDir, blobClientSrc.Name);
            RR.Lib.CreateDirForFile(fullPath);
            blobClientSrc.DownloadTo(fullPath);
        }




        ///*  download to local if file exists, to compare in... not needed, they are the same i found out.
        //if (blobClientTarget.Exists())
        //{
        //    progress.Report(string.Format("Distribute Target exists {0} ", newBlobName));
        //    fullPath = Path.Combine(Properties.Settings.Default.DownloadDir, newBlobName);
        //    RR.Lib.CreateDirForFile(fullPath);
        //    blobClientTarget.DownloadTo(fullPath);
        //    RR.Lib.CreateDirForFile(fullPath);
        //    blobClientSrc.DownloadTo(fullPath + ".new.json");
        //}
        //*/

        ////+
        ////--- debug.. download some blobs
        ////-
        ///*
        //if (blobItem.Name.Contains("/97"))
        //{
        //    fullPath = Path.Combine(Properties.Settings.Default.DownloadDir, newBlobName);
        //    RR.Lib.CreateDirForFile(fullPath);
        //    blobClientSrc.DownloadTo(fullPath);
        //}
        //*/
        //timeStamp
        //await blobClientTarget.StartCopyFromUriAsync(blobClientSrc.Uri);
        ////+
        ////---  en dan gooien we het origineel wel even weg.
        ////-
        //containerClient.DeleteBlob(blobItem.Name);
        //log.DebugFormat("Moved {0} to {1}", blobItem.Name, newBlobName);




        /// <summary>
        /// Wait until all tasks are done, cleanup the tasklist in de meantime
        /// </summary>
        /// <param name="allTasks"></param>
        private void AllTasksDone(List<Task> allTasks)
        {
            while (allTasks.Count > 0)
            {
                //log.InfoFormat("Waiting for tasks to complete #Tasks={0} ", allTasks.Count);
                List<Task> done = new List<Task>();
                foreach (Task task in allTasks)
                {
                    if (task.IsCompleted)
                    {
                        done.Add(task);
                    }
                }
                //+
                //--- remove completed tasks
                //-
                foreach (Task task in done)
                {
                    allTasks.Remove(task);
                }
                Thread.Sleep(TimeSpan.FromMilliseconds(1000));
            }

        }

    } //class
}//ns
