using System;


namespace HardwareDevices
{
    internal interface IWaagenSchnittstelle
    {
        bool PollStop { get; set; }
        String Status { get; set; }
        bool Connected { get; set; }

        decimal DemoWeight { get; set; } // Hilfsproperty für den Demo Modus -Slider schreibt da rein

        Weight GetPollGewicht(string wnr);
        RegisterWeight RegisterGewicht(string wnr);

        void Close();
    }
}