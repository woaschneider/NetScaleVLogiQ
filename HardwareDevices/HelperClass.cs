using System;

namespace HardwareDevices
{
    // "Hilfs"-Klassen
    public class Weight
    {

        private decimal _weightValue;
        private string _status;

        public bool S0
        {
            get { return _s0; }
            set { _s0 = value; }
        }

        public bool S1
        {
            get { return _s1; }
            set { _s1 = value; }
        }

        public bool S2
        {
            get { return _s2; }
            set { _s2 = value; }
        }

        public bool S3
        {
            get { return _s3; }
            set { _s3 = value; }
        }

        public bool S4
        {
            get { return _s4; }
            set { _s4 = value; }
        }

        public bool S5
        {
            get { return _s5; }
            set { _s5 = value; }
        }

        public bool S6
        {
            get { return _s6; }
            set { _s6 = value; }
        }

        public bool S7
        {
            get { return _s7; }
            set { _s7 = value; }
        }

        // Status
        private bool _s0;
        private bool _s1;
        private bool _s2;
        private bool _s3;
        private bool _s4;
        private bool _s5;
        private bool _s6;
        private bool _s7;

        // public decimal WeightValue = 0.00m;
        public decimal WeightValue
        {
            get { return _weightValue; }
            set
            {
                _weightValue = value;

            }
        }

        public string Status
        {
            get { return _status; }
            set
            {
                _status = value;

            }
        }

    }

    public class RegisterWeight
    {
        public DateTime Date;
        public string Ln;
        public string Status; // 80
        public DateTime Time;
        public decimal weight;
    }
}