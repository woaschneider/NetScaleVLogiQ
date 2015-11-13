using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace HardwareDevices
{
    // "Hilfs"-Klassen
    public class Weight
    {
        private string _status;
        private decimal _weightValue;
        
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
            set { _status = value; }
        }
    }

    public class RegisterWeight
    {
        public decimal weight;
        public string Status; // 80
        public DateTime Date;
        public DateTime Time;
        public string Ln;
    }
}