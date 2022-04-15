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

        public VehicleWithSendQueue GetOrAdd(int vehicleNumber, BlobContainerClient containerClient, CancellationToken ct)
        {
            lock (this)
            {
                VehicleWithSendQueue result = this.Find(V => V.vehicleNumber == vehicleNumber.ToString());
                if (result == null)
                {
                    result = new VehicleWithSendQueue(ct)
                    {
                        vehicleNumber = vehicleNumber.ToString(),
                        containerClient = containerClient,
                        useTimestampNow = this.useTimestampNow
                    };
                    this.Add(result);
                }
                return result;
            }
        }

    }
}
