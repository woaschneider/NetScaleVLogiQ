using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using HWB.Logging;


namespace HardwareDevices.Schenck.Disomat.UDP
{
    public class UDPTersus : IWaagenSchnittstelle
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
        public const char Stx = (char)2;
        public const char Etx = (char)3;

        private UdpClient _client;
        private string Ip;
        private int Port;

        private const int Bufsize = 250; // Laut Systec können die Datensätze bis 250 Zeichen lang sein.


        public UDPTersus(string ip, int port)
        {
            Ip = ip;
            Port = port;
            _client = new UdpClient(Ip, Port);
            Connected = _client.Client.Connected;
            // Client.Connect(ip,port);
        }


        public Weight GetPollGewicht(string wnr)
        {
            byte[] rcvBuffer = new byte[Bufsize]; // TCP Buffer
            byte[] rcvBuffer2 = new byte[Bufsize]; // TCP Buffer
            Weight oW = new Weight();
            oW.Status = "00";
            Weight oPW = new Weight();

            {
                // Pollstring zusammenbauen
                string tg1 = wnr + "#TG#" + Etx;


                // String senden
                DateTime stopTime = DateTime.Now.AddMilliseconds(1000);

                byte[] datagram = Encoding.ASCII.GetBytes(Stx + tg1 + Bcc(tg1));
                _client.Send(datagram, datagram.Length);

                var tokens = GetTokens(ReadWithTimeout(_client, TimeSpan.FromSeconds(3)));
                if ((tokens.Count() >= 5) && (tokens[1] == "TG"))
                {
                    oPW.WeightValue = Decimal.Parse(tokens[2].Replace('.', ','));

                    oPW.Status = tokens[5];
                    int status = int.Parse(tokens[5], System.Globalization.NumberStyles.HexNumber);

                    oPW.Stillstand = (status & 0x80) > 0 ? true : false;
                    oPW.TaraGesetzt = (status & 0x40) > 0 ? true : false;
                    oPW.GewichtUngueltig = (status & 0x20) > 0 ? true : false;
                    oPW.GenauNull = (status & 0x08) > 0 ? true : false;
                    oPW.TaraErrechnet = (status & 0x04) > 0 ? true : false;
                    oPW.Ueberbereich = (status & 0x02) > 0 ? true : false;
                    oPW.Unterbereich = (status & 0x01) > 0 ? true : false;


                }

                return oPW;
            }
        }

        public RegisterWeight RegisterGewicht(string wnr)
        {
            RegisterWeight oRW = new RegisterWeight();
            try
            {
                if (_client.Client.Connected)
                {

                    //Generate Request string
                    string command = wnr + "#DR#3#" + Etx;
                    string stringToSend = Stx + command + GetChecksum(command);


                    //Send request
                    _client.Send(Encoding.ASCII.GetBytes(stringToSend), Encoding.ASCII.GetBytes(stringToSend).Length);

                    //Get both replies
                    ReadWithTimeout(_client, TimeSpan.FromSeconds(5));
                    var tokens = GetTokens(ReadWithTimeout(_client, TimeSpan.FromSeconds(5)));
                    if (tokens.Count() >= 8)
                    {
                        oRW.Status = tokens[4];
                        oRW.Time = oRW.Date = DateTime.Parse(tokens[5] + " " + tokens[6]);
                        oRW.Ln = tokens[7];
                        oRW.weight = Decimal.Parse(tokens[8].Replace('.', ',').Replace('B', ' ').Replace('N', ' ').Replace('t', ' ').Replace("kg", " ").Replace("g", " ").Replace("<", " ").Replace(">", " ").Trim());
                        Log.Instance.Info("Eichwiegung: " + oRW.Status + " " + oRW.Time + " " + oRW.weight);
                    }

                }
            }
            catch (Exception e)
            {
                Log.Instance.Error(e.Source + " " + e.Message);
            }

            return oRW;
        }

        private string ReadWithTimeout(UdpClient client, TimeSpan timeout)
        {
            //Wait for Data with timeout
            DateTime stopTime = DateTime.Now + timeout;

            while (client.Available < 1)
            {
                System.Threading.Thread.Sleep(TimeSpan.FromMilliseconds(10));
                if (DateTime.Now > stopTime)
                {
                    Log.Instance.Error("Timeout waiting for response!");
                    return "";
                }
            }
            IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
            string reply = "";

            try
            {
                reply = Encoding.ASCII.GetString(client.Receive(ref RemoteIpEndPoint));
            }
            catch (SocketException)
            {

            }
            return reply;
        }

        /// <summary>
        /// Entfernt das erste und die letzten beiden Zeichen (Stx, Etx und Checksum) und zerlegt den String in tokens
        /// </summary>
        /// <param name="telegram"></param>
        /// <returns></returns>
        private string[] GetTokens(string telegram)
        {
            if (telegram.Length > 3)
            {
                return telegram.Remove(telegram.Length - 2, 2).Remove(0, 1).Split('#');
            }
            else
            {
                return new string[0];
            }
        }

        // LLC - Längs-Parität-Berechnung
        private static char GetChecksum(string s)
        {
            int checksum = 0;
            foreach (char c in s)
            {
                checksum ^= Convert.ToByte(c);
            }
            return (char)checksum;
        }
        public void Close()
        {
        }

        public static char Chr(int nAnsiCode)
        {
            return (char)nAnsiCode;
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
            string tg1 = wnr + "#WN#" + Etx;

            byte[] datagram = Encoding.ASCII.GetBytes(Stx + tg1 + Bcc(tg1));
            _client.Send(datagram, datagram.Length);
            // String senden
            DateTime stopTime = DateTime.Now.AddMilliseconds(500);


            while (_client.Client.Available < 12)
            {
                // Stoptime prüfen
                if (DateTime.Now > stopTime) // Timeout
                {
                    return "";
                }
            }
            IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
            Byte[] receiveBytes = _client.Receive(ref RemoteIpEndPoint);

            string ret = Encoding.ASCII.GetString(receiveBytes);
            return ret;
        }

        public void ReadAllContacts()
        {
        }

        public void SetContact(int k)
        {
        }

        public string Bcc(string s)
        {
            int checksum = s.Aggregate(0, (current, c) => current ^ Convert.ToByte(c));
            string hex = checksum.ToString("X2");
            string result = "";

            for (int ss = 0; ss < hex.Length / 2; ss++)
            {
                result += (char)Convert.ToUInt16(hex.Substring(ss * 2, 2), 16);
            }

            return result;
        }
    }
}