using System;
using System.Text;
using System.Net.Sockets;

namespace HardwareDevices.Systec
{
    public class SystecTcp1Adm : IWaagenSchnittstelle
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
        public bool Connected { get; set; }
        public decimal DemoWeight { get; set; }

        private TcpClient _client;
        private NetworkStream _netStream;
        private const int Bufsize = 250; // Laut Systec können die Datensätze bis 250 Zeichen lang sein.


        public TcpClient Client
        {
            set { _client = value; }
            get { return _client; }
        }

        public NetworkStream NetStream
        {
            set { _netStream = value; }
            get { return _netStream; }
        }

        public SystecTcp1Adm(string ip, string port)
        {
            try
            {
                _client = new TcpClient(ip, Convert.ToInt16(port));
                // Get a client stream for reading and writing.
                //  Stream stream = oTCP.GetStream();
                _client.GetStream();
                this.Connected = true;
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
        }

        public Weight GetPollGewicht(string wnr)
        {
            byte[] rcvBuffer = new byte[Bufsize]; // TCP Buffer
            byte[] rcvBuffer2 = new byte[Bufsize]; // TCP Buffer
            Weight oW = new Weight();
            string Telegramm = Chr(60) + "RM" + Chr(62);
            /////////////////////////////////////////////////
            byte[] cpolltelegramm = Encoding.ASCII.GetBytes(Telegramm); // <--------
            /////////////////////////////////////////////////
            _netStream = _client.GetStream();
            byte[] dummy = new byte[250];
            _netStream.Write(cpolltelegramm, 0, cpolltelegramm.Length);

            DateTime stopTime = DateTime.Now.AddMilliseconds(3000);

            int ii = 0;
            NetStream.Read(rcvBuffer, 0, rcvBuffer.Length);
            while (stopTime > DateTime.Now)
            {
                if (rcvBuffer[ii] == 62) // ">"
                    
                {
                    System.Threading.Thread.Sleep(500);
                    NetStream.Read(rcvBuffer2, 0,rcvBuffer2.Length ); // <- Hier sind dasCR / LN
                    ASCIIEncoding enc = new System.Text.ASCIIEncoding();
                    string Test = enc.GetString(rcvBuffer);

                    string fehler = enc.GetString(rcvBuffer).Substring(0, 4);
                    if (fehler.Substring(0, 1) == "<" && fehler.Substring(3, 1) == ">")
                    {
                        Statusanzeigen(fehler.Substring(1, 2));
                        oW.WeightValue = Convert.ToDecimal("99,99");
                        NetStream.Read(rcvBuffer, 0, rcvBuffer.Length); // Buffer leer machen
                        return oW;
                    }


                    

                    string gewicht = enc.GetString(rcvBuffer).Substring(25, 6);
                    string sGewicht =
                        gewicht.Replace("k", "").Replace("<", "").Replace(">", "").Replace("g", "").Replace(".", ",");


                    Statusanzeigen("00");
                    oW.WeightValue = Convert.ToDecimal(sGewicht);
                   // NetStream.Read(rcvBuffer, 0, rcvBuffer.Length); // Buffer leer machen

                    return oW;
                }
                ii = ii + 1;
            }
            oW.WeightValue = 0;
            return oW;
        }

        public RegisterWeight RegisterGewicht(string wnr)
        {
            byte[] rcvBuffer = new byte[Bufsize]; // TCP Buffer
            byte[] sendBuffer = new byte[Bufsize]; // TCP Buffer
            RegisterWeight oRW = new RegisterWeight();
            string Telegramm = Chr(60) + "RN" + wnr + Chr(62);
            /////////////////////////////////////////////////
            byte[] cEichtelegramm = Encoding.ASCII.GetBytes(Telegramm); // <--------
            /////////////////////////////////////////////////
            _netStream = _client.GetStream();
            byte[] dummy = new byte[250];
            _netStream.Write(cEichtelegramm, 0, cEichtelegramm.Length);

            DateTime stopTime = DateTime.Now.AddMilliseconds(3000);

            int ii = 0;
            NetStream.Read(rcvBuffer, 0, rcvBuffer.Length);
            while (stopTime > DateTime.Now)
            {
                if (rcvBuffer[ii] == 62) // "]"
                {
                    ASCIIEncoding enc = new System.Text.ASCIIEncoding();
                    string Test = enc.GetString(rcvBuffer);
                    string fehler = enc.GetString(rcvBuffer).Substring(0, 4);
                    if (fehler.Substring(0, 1) == "<" && fehler.Substring(3, 1) == ">")
                    {
                        Statusanzeigen(fehler.Substring(1, 2));


                        oRW.weight = Convert.ToDecimal("99,99");
                        NetStream.Read(rcvBuffer, 0, rcvBuffer.Length); // Buffer leer machen
                        return oRW;
                    }
                    string ret = enc.GetString(rcvBuffer);
                    if (ret.Substring(1, 4) == "0000")
                        oRW.Status = "80";
                    string sDatum = ret.Substring(5, 8);
                    oRW.Date = Convert.ToDateTime(sDatum);
                    string sZeit = ret.Substring(13, 5);
                    oRW.Time = Convert.ToDateTime(sZeit);
                    oRW.Ln = ret.Substring(19, 5);


                    string gewicht = enc.GetString(rcvBuffer).Substring(25, 6);
                    string sGewicht =
                        gewicht.Replace("k", "").Replace("<", "").Replace(">", "").Replace("g", "").Replace(".", ",");


                    Statusanzeigen("00");
                    oRW.weight = Convert.ToDecimal(sGewicht);
                    NetStream.Read(rcvBuffer, 0, rcvBuffer.Length); // Buffer leer machen
                    return oRW;
                }
                ii = ii + 1;
            }
            oRW.weight = 0;
            NetStream.Read(rcvBuffer, 0, rcvBuffer.Length); // Buffer leer machen
            return oRW;
        }

        public void Close()
        {
        }

        public static char Chr(int nAnsiCode)
        {
            return (char) nAnsiCode;
        }

        public void Statusanzeigen(string fehlerCode)
        {
            switch (fehlerCode)
            {
                case "00":
                    Status = "...";
                    break;
                case "11":
                    Status = "Allgemeiner Waagenfehler";
                    break;
                case "12":
                    Status = "Überlast";
                    break;
                case "13":
                    Status = "Waage in Bewegung";
                    break;
                case "14":
                    Status = "Waage nicht verfügbar";
                    break;
                case "31":
                    Status = "Übertragungsfehler";
                    break;
                case "32":
                    Status = "Ungültiger Befehl";
                    break;
                case "33":
                    Status = "Ungültiger Parameter";
                    break;
            }
        }

        public string WaageAufschalten(string wnr)
        {
            return "";
        }

        public void ReadAllContacts()
        {
            
        }
        public void SetContact(int k)
        {
        }
    }

}