using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FmsBlobToContinental
{
    static class FMSEventTest
    {
        /// <summary>
        /// create a test item
        /// </summary>
        static public FMSEvent Create()
        {
            FMSEvent testItem = new FMSEvent();
            testItem.adbleu = 75;
            testItem.dinfo = 1;
            testItem.dir = 70;
            testItem.dist = 100000;
            testItem.doors = new byte[] { 1, 0, 0 };
            testItem.drvid = "12345";
            testItem.fuelc = 3.3f;
            testItem.fuell = 250;
            testItem.fuelr = 1.2f;
            testItem.gear = 3;
            testItem.lat = 52.2f;
            testItem.lon = 4.2f;
            testItem.peacc = 50;
            testItem.pebrk = 0;
            testItem.reta = 25;
            testItem.rpm = 4000;
            testItem.tamb = 11;
            testItem.tcool = 90;
            testItem.torq = 33.3f;
            testItem.ts = DateTime.Now;
            testItem.vfms = 36.5f;
            testItem.vgps = 36.7f;
            return testItem;
        }
    }
}
