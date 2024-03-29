﻿using Azure.Storage.Blobs;
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
    public class BlobItemList
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        OnHandleBlob OnHandleBlob;
        DateTime lastesMondified = DateTime.MinValue;
        BlobContainerClient containerClient;

        public BlobItemList(OnHandleBlob onHandleBlob = null)
        {
            this.OnHandleBlob = onHandleBlob;
        }


        /// <summary>
        /// Get a list of blobitems from the blobstorage, day by day
        /// </summary>
        /// <param name="testProd"></param>
        /// <param name="sinceUTC">Since a specific timestamp in UTC for oldest blob</param>
        /// <param name="untilUTC">Since a specific timestamp in UTC latest blob (null means NOW)</param>
        /// <param name="onlyVehicle">Only for a specific vehicle</param>
        /// <param name="downloadToFile">Download the blob to local download dir 'onlyVehicle' is obliged</param>
        /// <returns></returns>
        public async Task<int> GetBlobItemsASync(
            TestProd testProd,
            DateTime sinceUTC,
            DateTime? untilUTC,
            bool downloadToFile,
            string onlyVehicle,
            CancellationToken ct
            )
        {
            int nItemsFound = 0;
            //List<BlobItem> result = new List<BlobItem>();
            await Task.Run(() =>
              {
                 nItemsFound = GetBlobItems(testProd, sinceUTC, untilUTC, downloadToFile, onlyVehicle, ct);
              }
            );
            log.InfoFormat("GetBlobs() finished");
            //return result;
            return nItemsFound;
        }


        //public List<BlobItem> GetBlobItems(
        public int GetBlobItems(
            TestProd testProd,
            DateTime sinceUTC,
            DateTime? untilUTC,
            bool downloadToFile,
            string onlyVehicle,
            CancellationToken ct)
        {
            containerClient = Statics.GetContainerClient(testProd);
            int nItemsFound = 0;
            //+
            //--- get all blobs in prefix '2021/12/08'
            //-
            int i = 1;
            BlobFileName blobFilename = new BlobFileName();
            //niet meer onthouden vanwege memory..
            //List<BlobItem> result = new List<BlobItem>();
            Boolean UntilNow = false;
            if (untilUTC == null)
            {
                UntilNow = true;
                untilUTC = DateTime.Now;
            }

            while (sinceUTC < untilUTC)
            {

                string myPrefix = Statics.DirForStorage(sinceUTC);
                // {
                //   if (onlyRecent)
                // {
                if (!string.IsNullOrEmpty(onlyVehicle))
                {
                    log.InfoFormat("Mem={0} GetBlobs() since {1:u} up to {2:u} prefix {3} only for vehicle {4}",
                        RR.Lib.MemoryUsageStringShort(),
                        sinceUTC, untilUTC, myPrefix, onlyVehicle);
                }
                else
                {
                    log.InfoFormat("Mem={0} GetBlobs() since {1:u} up to {2:u} prefix {2}",
                            RR.Lib.MemoryUsageStringShort(),
                            sinceUTC, untilUTC, myPrefix);
                }

                //+
                //--- get the blobs en enumerate through them
                //-
                Azure.Pageable<BlobItem> allBlobs = containerClient.GetBlobs(BlobTraits.None, BlobStates.None, myPrefix);
                IEnumerator<BlobItem> enumerator = allBlobs.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (ct.IsCancellationRequested)
                    {
                        break;
                    }
                    nItemsFound++;
                    BlobItem blobItem = enumerator.Current;
                    blobFilename.Name = blobItem.Name; // thus get the vehiclenumber and date from the filename.
                    Boolean addToResult = true;

                    //+
                    //--- add them to result if conditions are met
                    //-
                    if (blobFilename.timeStamp <= sinceUTC)
                    {
                        addToResult = false;
                    }
                    if (blobFilename.timeStamp > untilUTC)
                    {
                        addToResult = false;
                    }

                    if (!string.IsNullOrEmpty(onlyVehicle) && !blobItem.Name.Contains("/" + onlyVehicle + "_"))
                    {
                        addToResult = false;
                    }
                    if (addToResult)
                    {
                        // niet meer onthouden vanwege memory
                        // result.Add(blobItem);
                        //  log.DebugFormat("#blobsSeen={0} #blobsSelected={1} blobName={2} Dated={3} ", 
                        //      i, result.Count, blobItem.Name, blobItem.Properties.LastModified);
                        if (OnHandleBlob != null)
                        {
                            OnHandleBlob(blobItem);
                        }
                        if (downloadToFile && blobFilename.vehicleNumber.ToString() == onlyVehicle)
                        {
                            DownloadToFile(blobItem, Properties.Settings.Default.DownloadDir);
                        }
                    }
                    i++;
                    blobItem = null;// dispose needed??
                }
                log.InfoFormat("Mem={0} GotBlobs   since {1:u} up to {2:u} ==> Total={3}", RR.Lib.MemoryUsageStringShort(), sinceUTC, untilUTC, nItemsFound);


                sinceUTC = sinceUTC.AddDays(1);
                if (UntilNow)
                {
                    // update timestamp to keep getting the blobs even if it took a while
                    untilUTC = DateTime.Now;
                }

                if (ct.IsCancellationRequested)
                {
                    break;
                }
            }// While dateloop.
             //+
             //--- sort them by lastmodified to be sure to handle them in the correct order
             //-

            //result.Sort((x, y) => ((DateTimeOffset)x.Properties.LastModified).CompareTo((DateTimeOffset)y.Properties.LastModified));
            //return result;
            return nItemsFound;
        }


        /// <summary>
        /// Download all items in the itemslist to directory dir
        /// </summary>
        /// <param name="items"></param>
        /// <param name="dir"></param>
        public void DownloadToFile(List<BlobItem> items, string dir)
        {
            int index = 0;
            foreach (BlobItem blobItem in items)
            {
                index++;
                DownloadToFile(blobItem, dir);
            }
        }

        /// <summary>
        /// Download all items in the itemslist to directory dir
        /// </summary>
        /// <param name="items"></param>
        /// <param name="dir"></param>
        public void DownloadToFile(BlobItem blobItem, string dir)
        {
            BlobClient blobClient = containerClient.GetBlobClient(blobItem.Name);
            //+
            //--- download to file
            //-
            string dest = Path.Combine(dir, blobItem.Name);
            RR.Lib.CreateDirForFile(dest);
            if (File.Exists(dest))
            {
                int i = 0;
                string dest2 = dest + "_" + i++.ToString();
                while (File.Exists(dest2))
                {
                    dest2 = dest + "_" + i++.ToString();
                }
                File.Move(dest, dest2);
            }
            log.InfoFormat("DownloadAsync to file {0}", dest);
            blobClient.DownloadToAsync(dest);
        }




    }// class
}// namespace
