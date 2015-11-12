using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HardwareDevices
{
    internal class DemoDevice : IWaagenSchnittstelle
    {
        private bool _x1;
        private bool _x2;
        private bool _x3;
        private bool _x4;
        private bool _x5;
        private bool _x6;
        private bool _x7;
        private bool _x8;
        private bool _x9;
        private bool _x10;
        private bool _x11;
        private bool _x12;
        private bool _x13;
        private bool _x14;
        public bool X1
        {
            get { return _x1; }
            set { _x1 = value; }
        }

        public bool X2
        {
            get { return _x2; }
            set { _x2 = value; }
        }

        public bool X3
        {
            get { return _x3; }
            set { _x3 = value; }
        }

        public bool X4
        {
            get { return _x4; }
            set { _x4 = value; }
        }

        public bool X5
        {
            get { return _x5; }
            set { _x5 = value; }
        }

        public bool X6
        {
            get { return _x6; }
            set { _x6 = value; }
        }

        public bool X7
        {
            get { return _x7; }
            set { _x7 = value; }
        }

        public bool X8
        {
            get { return _x8; }
            set { _x8 = value; }
        }

        public bool X9
        {
            get { return _x9; }
            set { _x9 = value; }
        }

        public bool X10
        {
            get { return _x10; }
            set { _x10 = value; }
        }

        public bool X11
        {
            get { return _x11; }
            set { _x11 = value; }
        }

        public bool X12
        {
            get { return _x12; }
            set { _x12 = value; }
        }

        public bool X13
        {
            get { return _x13; }
            set { _x13 = value; }
        }

        public bool X14
        {
            get { return _x14; }
            set { _x14 = value; }
        }

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