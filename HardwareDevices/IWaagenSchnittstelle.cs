namespace HardwareDevices
{
    internal interface IWaagenSchnittstelle
    {
        string Status { get; set; }
        bool Connected { get; set; }
        bool X1 { get; set; }
        bool X2 { get; set; }
        bool X3 { get; set; }
        bool X4 { get; set; }
        bool X5 { get; set; }
        bool X6 { get; set; }
        bool X7 { get; set; }
        bool X8 { get; set; }
        bool X9 { get; set; }
        bool X10 { get; set; }
        bool X11 { get; set; }
        bool X12 { get; set; }
        bool X13 { get; set; }
        bool X14 { get; set; }

        decimal DemoWeight { get; set; } // Hilfsproperty für den Demo Modus -Slider schreibt da rein

        Weight GetPollGewicht(string wnr);
        RegisterWeight RegisterGewicht(string wnr);
        string WaageAufschalten(string wnr);
        void ReadAllContacts();
        void SetContact(int k);

        void Close();
    }
}