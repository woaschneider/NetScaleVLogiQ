using System;

namespace HardwareDevices
{
    // "Hilfs"-Klassen
    public class Weight
    {
        // public decimal WeightValue = 0.00m;
        public decimal WeightValue { get; set; }

        public string Status { get; set; }
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