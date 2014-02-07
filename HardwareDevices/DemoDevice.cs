using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HardwareDevices
{
    internal class DemoDevice : IWaagenSchnittstelle
    {
        public String Status { get; set; }
        public bool PollStop { get; set; }
        public decimal DemoWeight { get; set; }
        public bool Connected { get; set; }

        public Weight GetPollGewicht(string WNR)
        {
            Weight oW = new Weight();
            oW.WeightValue = DemoWeight;
            return oW;
        }

        public RegisterWeight RegisterGewicht(string WNR)
        {
            RegisterWeight oRW = new RegisterWeight();
            DateTime x = DateTime.Now;

            oRW.Status = "80";
            oRW.Date = DateTime.Today ;
            oRW.Time = DateTime.Now ;
            oRW.weight = DemoWeight;

            return oRW;
        }


        public void Close()
        {
        }
    }
}