using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace HardwareDevices.Systec
{
    public class SystecTcp2Adm : IWaagenSchnittstelle
    {
        private const int Bufsize = 250; // Laut Systec können die Datensätze bis 250 Zeichen lang sein.

        public SystecTcp2Adm(string ip, string port)
        {
            try
            {
                Client = new TcpClient(ip, Convert.ToInt16(port));
                // Get a client stream for reading and writing.
                //  Stream stream = oTCP.GetStream();
                Client.GetStream();
                Connected = true;
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

        public bool PollStop { get; set; }


        public TcpClient Client { set; get; }

        public NetworkStream NetStream { set; get; }

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
        public bool Connected { get; set; }
        public decimal DemoWeight { get; set; }

        public Weight GetPollGewicht(string wnr)
        {
            var rcvBuffer = new byte[Bufsize]; // TCP Buffer
            var rcvBuffer2 = new byte[Bufsize]; // TCP Buffer
            var oW = new Weight();
            var Telegramm = Chr(60) + "RM" + Chr(62);
            /////////////////////////////////////////////////
            var cpolltelegramm = Encoding.ASCII.GetBytes(Telegramm); // <--------
            /////////////////////////////////////////////////
            NetStream = Client.GetStream();
            var dummy = new byte[250];
            NetStream.Write(cpolltelegramm, 0, cpolltelegramm.Length);

            var stopTime = DateTime.Now.AddMilliseconds(3000);

            var ii = 0;
            NetStream.Read(rcvBuffer, 0, rcvBuffer.Length);
            while (stopTime > DateTime.Now)
            {
                if (rcvBuffer[ii] == 62) // ">"
                {
                    Thread.Sleep(500);
                    NetStream.Read(rcvBuffer2, 0, rcvBuffer2.Length); // <- Hier sind dasCR / LN
                    var enc = new ASCIIEncoding();
                    var Test = enc.GetString(rcvBuffer);

                    var fehler = enc.GetString(rcvBuffer).Substring(0, 4);
                    if (fehler.Substring(0, 1) == "<" && fehler.Substring(3, 1) == ">")
                    {
                        Statusanzeigen(fehler.Substring(1, 2));
                        oW.WeightValue = Convert.ToDecimal("99,99");
                        NetStream.Read(rcvBuffer, 0, rcvBuffer.Length); // Buffer leer machen
                        return oW;
                    }


                    var gewicht = enc.GetString(rcvBuffer).Substring(25, 6);
                    var sGewicht =
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
            var w = "1";
            switch (wnr)
            {
                case "01":
                    break;
                case "02":
                    w = "1";
                    break;
                case "03":
                    w = "2";
                    break;
                case "04":
                    w = "3";
                    break;
            }
            var rcvBuffer = new byte[Bufsize]; // TCP Buffer
            var rcvBuffer2 = new byte[Bufsize]; // TCP Buffer
            var sendBuffer = new byte[Bufsize]; // TCP Buffer
            var oRW = new RegisterWeight();
            var Telegramm = Chr(60) + "RN" + w + Chr(62);
            /////////////////////////////////////////////////
            var cEichtelegramm = Encoding.ASCII.GetBytes(Telegramm); // <--------
            /////////////////////////////////////////////////
            NetStream = Client.GetStream();
            var dummy = new byte[250];
            NetStream.Write(cEichtelegramm, 0, cEichtelegramm.Length);

            var stopTime = DateTime.Now.AddMilliseconds(3000);

            var ii = 0;
            NetStream.Read(rcvBuffer, 0, rcvBuffer.Length);
            while (stopTime > DateTime.Now)
            {
                if (rcvBuffer[ii] == 62) // ">"
                {
                    Thread.Sleep(500);
                    NetStream.Read(rcvBuffer2, 0, rcvBuffer2.Length); // <- Hier sind dasCR / LN
                    var enc = new ASCIIEncoding();
                    var Test = enc.GetString(rcvBuffer);
                    var fehler = enc.GetString(rcvBuffer).Substring(0, 4);
                    if (fehler.Substring(0, 1) == "<" && fehler.Substring(3, 1) == ">")
                    {
                        Statusanzeigen(fehler.Substring(1, 2));


                        oRW.weight = Convert.ToDecimal("99,99");
                        NetStream.Read(rcvBuffer, 0, rcvBuffer.Length); // Buffer leer machen
                        return oRW;
                    }
                    var ret = enc.GetString(rcvBuffer);
                    if (ret.Substring(1, 4) == "0000")
                        oRW.Status = "80";
                    var sDatum = ret.Substring(5, 8);
                    oRW.Date = Convert.ToDateTime(sDatum);
                    var sZeit = ret.Substring(13, 5);
                    oRW.Time = Convert.ToDateTime(sZeit);
                    oRW.Ln = ret.Substring(18, 4);


                    var gewicht = enc.GetString(rcvBuffer).Substring(25, 6);
                    var sGewicht =
                        gewicht.Replace("k", "").Replace("<", "").Replace(">", "").Replace("g", "").Replace(".", ",");


                    Statusanzeigen("00");
                    oRW.weight = Convert.ToDecimal(sGewicht);

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

        public string WaageAufschalten(string wnr)
        {
            var w = "1";
            switch (wnr)
            {
                case "01":
                    break;
                case "02":
                    w = "1";
                    break;
                case "03":
                    w = "2";
                    break;
                case "04":
                    w = "3";
                    break;
            }


            var rcvBuffer = new byte[Bufsize]; // TCP Buffer
            var sendBuffer = new byte[Bufsize]; // TCP Buffer

            var Telegramm = Chr(60) + "SS" + w + Chr(62);
            /////////////////////////////////////////////////
            var cTelegramm = Encoding.ASCII.GetBytes(Telegramm); // <--------
            /////////////////////////////////////////////////
            NetStream = Client.GetStream();
            var dummy = new byte[250];
            NetStream.Write(cTelegramm, 0, cTelegramm.Length);

            Thread.Sleep(500);
            NetStream.Read(rcvBuffer, 0, rcvBuffer.Length);


            return "";
        }

        public void ReadAllContacts()
        {
        }

        public void SetContact(int k)
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