using System;
using System.Text;
using System.Net.Sockets;

namespace HardwareDevices.Systec
{
    public class SystecTcp1Adm : IWaagenSchnittstelle
    {
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

            Weight oW = new Weight();
            string Telegramm = Chr(60) + "RM1" + wnr + Chr(62);
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
                if (rcvBuffer[ii] == 62) // "]"
                {
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
                    NetStream.Read(rcvBuffer, 0, rcvBuffer.Length); // Buffer leer machen

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
    }
}