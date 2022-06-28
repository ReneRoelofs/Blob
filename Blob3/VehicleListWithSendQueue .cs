using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using BlobHandler;
using FmsBlobToContinental;
using RestSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Statics = FmsBlobToContinental.Statics;

namespace Blob3
{

    public class VehicleSenderList : List<VehicleWithSendQueue>
    {
        Boolean useTimestampNow = false;
        public void SetUpdateNow(Boolean useTimestampNow)
        {
            this.useTimestampNow = useTimestampNow;
            foreach (VehicleWithSendQueue v in this)
            {
                v.useTimestampNow = this.useTimestampNow;
            }
        }

        public TestProd _testProd = TestProd.Test;

        /// <summary>
        /// Get Testprod. 
        /// Set Testprod en thus TestProd for all vehicles in this list.
        /// </summary>
        public TestProd TestProd
        {
            get
            {
                return _testProd;
            }
            set
            {
                _testProd = value;
                foreach (VehicleWithSendQueue vehicleWithSendQueue in this)
                {
                    vehicleWithSendQueue.testProd = value;
                }
            }
        }

        //public VehicleWithSendQueue GetOrAdd(int vehicleNumber, BlobContainerClient containerClient, CancellationToken ct)
        //{
        //    lock (this)
        //    {
        //        VehicleWithSendQueue result = this.Find(V => V.vehicleNumber == vehicleNumber.ToString());
        //        if (result == null)
        //        {
        //            result = new VehicleWithSendQueue(ct, this.TestProd)
        //            {
        //                vehicleNumber = vehicleNumber.ToString(),
        //                containerClient = containerClient,
        //                useTimestampNow = this.useTimestampNow
        //            };
        //            this.Add(result);
        //        }
        //        return result;
        //    }
        //}
        public VehicleWithSendQueue GetOrAdd(CCVehicle vehicle, BlobContainerClient containerClient, CancellationToken ct)
        {
            lock (this)
            {
                VehicleWithSendQueue result = this.Find(V => V.vehicleNumber == vehicle.vehicleNumber.ToString());
                if (result == null)
                {
                    result = new VehicleWithSendQueue(vehicle, containerClient,ct);
                    result.useTimestampNow = this.useTimestampNow;
                    this.Add(result);
                }
                return result;
            }
        }

        /*
         * 
            //+
            //--- create a vehiclesender per vehicle is not known allready. 
            //-
            if (vehicleSenderList.Find(V => V.vehicleNumber == vehicle.vehicleNumber) == null)
            {
                lock (vehicleSenderList)
                {
                    VehicleWithSendQueue vehicleSender = (VehicleWithSendQueue)vehicle;
                    vehicleSender.StartSenderTasks(ct);
                    vehicleSender.testProd = this.TestProd;

                    //VehicleWithSendQueue vehicleSender2 = vehicleSenderList.GetOrAdd(blobFilename.vehicleNumber, containerClient, ct);
                    //-
                    //--- enqueue the blobitem to the vehicleSender
                    //-
                    vehicleSender.EnqueueBlobItemForDownload(blobItem);
                }
            }
        */

    }
}
