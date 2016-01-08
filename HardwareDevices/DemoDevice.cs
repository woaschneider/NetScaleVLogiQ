using System;

namespace HardwareDevices
{
    internal class DemoDevice : IWaagenSchnittstelle
    {
        public bool PollStop { get; set; }

        public bool X1 { get; set; }

        public bool X2 { get; set; }

        public bool X3 { get; set; }

        public bool X4 { get; set; }

        public bool X5 { get; set; }

        public bool X6 { get; set; }

        public bool X7 { get; set; }

        public bool X8 { get; set; }

        public bool X9 { get; set; }

        public bool X10 { get; set; }

        public bool X11 { get; set; }

        public bool X12 { get; set; }

        public bool X13 { get; set; }

        public bool X14 { get; set; }

        public string Status { get; set; }
        public decimal DemoWeight { get; set; }
        public bool Connected { get; set; }

        public Weight GetPollGewicht(string WNR)
        {
            var oW = new Weight();
            oW.WeightValue = DemoWeight;
            return oW;
        }

        public RegisterWeight RegisterGewicht(string WNR)
        {
            var oRW = new RegisterWeight();
            var x = DateTime.Now;

            oRW.Status = "80";
            oRW.Date = DateTime.Today;
            oRW.Time = DateTime.Now;
            oRW.weight = DemoWeight;

            return oRW;
        }

        public string WaageAufschalten(string wnr)
        {
            return "";
        }

        public void Close()
        {
        }

        public void ReadAllContacts()
        {
        }

        public void SetContact(int k)
        {
        }
    }
}