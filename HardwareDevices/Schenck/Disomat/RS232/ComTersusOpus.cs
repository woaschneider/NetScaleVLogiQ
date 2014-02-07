using System;
using System.IO.Ports;


namespace HardwareDevices.Schenck.Disomat.RS232
{
    // Zunöchst Quick and Dirty Schenk Pollprozedure 8785
    // 14.6.2011 Änderung LRC berechnet u. Beizeichnen bei EW

    public class ComTersusOpus : IWaagenSchnittstelle
    {
        public bool PollStop { get; set; }
        public String Status { get; set; }
        public bool Connected { get; set; }

        public decimal DemoWeight { get; set; }

        private PortCom oCom;
        private readonly Parity _p;
        private readonly StopBits _s;

        public ComTersusOpus(String comPort, int baud, int databit, String sP, String sS)
        {
            if (sP == "N" || sP == "n")
            {
                _p = Parity.None;
            }
            if (sP == "O" || sP == "o")
            {
                _p = Parity.Odd;
            }
            if (sP == "E" || sP == "e")
            {
                _p = Parity.Even;
            }


            if (sS == "0")
            {
                _s = StopBits.None;
            }
            if (sS == "1")
            {
                _s = StopBits.One;
            }

            if (sS == "2")
            {
                _s = StopBits.Two;
            }

            oCom = new PortCom(comPort, baud, databit, _p, _s);
            try
            {
                oCom.Open();
                Connected = true;
            }
            catch (Exception)
            {
                oCom.Open();
                Connected = false;
            }
        }

        public Weight GetPollGewicht(string wnr)
        {
            Weight oPW = new Weight();
            if (oCom.IsOpen)
            {
                // Pollstring zusammenbauen
                string tg = Chr(2) + "01#TG#" + Chr(3) + Chr(17);
                // String senden
                DateTime stopTime = DateTime.Now.AddMilliseconds(3000);
                oCom.WriteLine(tg);
                while (oCom.BytesToRead < 36)
                {
                    // Stoptime prüfen
                    if (DateTime.Now > stopTime) // Timeout
                    {
                        return oPW; // Wegen Timeout
                    }
                }

                string ret = oCom.ReadExisting();
                string abc = ret.Substring(4, 2);
                var tt = abc;
                if (ret.Substring(4, 2) == "TG")
                {
                    string sGewicht = ret.Substring(7, 7);
                    sGewicht = sGewicht.Replace(".", ",");

                    oPW.WeightValue = Convert.ToDecimal(sGewicht);
                    if (oPW.WeightValue == 0)
                    {
                        int x = 1;
                        int y = x;
                    }
                }
            }
            return oPW;
        }

        public RegisterWeight RegisterGewicht(string wnr)
        {
            RegisterWeight oRW = new RegisterWeight();
            if (oCom.IsOpen)
            {
                string tg1 = "01#DR#3#" + Chr(3);
                string bccHex = GetChecksum(tg1);
                string bcc = Hex2String(bccHex);
                string tg2 = Chr(2) + tg1 + bcc;

                // String senden
                DateTime stopTime = DateTime.Now.AddMilliseconds(2000);
                oCom.WriteLine(tg2);
                while (oCom.BytesToRead < 61)
                {
                    // Stoptime prüfen
                    if (DateTime.Now > stopTime) // Timeout
                    {
                        return oRW;
                    }
                }

                string ret = oCom.ReadExisting();


                oRW.Status = ret.Substring(22, 2);

                string sDatum = ret.Substring(25, 8);
                oRW.Date = Convert.ToDateTime(sDatum);
                string sZeit = ret.Substring(34, 5);
                oRW.Time = Convert.ToDateTime(sZeit);
                oRW.Ln = ret.Substring(40, 5);
                string sGewicht =
                    ret.Substring(48, 6).Replace("t", "").Replace("k", "").Replace("<", "").Replace(">", "").Replace(
                        "g", "").Replace(".", ",");
                oRW.weight = Convert.ToDecimal(sGewicht);
                System.Threading.Thread.Sleep(500);
                var dummy = oCom.ReadExisting();
            }
            return oRW;
        }

        public RegisterWeight RegisterGewicht(string wnr, string bz)
        {
            RegisterWeight oRW = new RegisterWeight();

            // BEIZEICHEN
            string tg1 = "0" + wnr + "#DR#6#HALLO WAAGE#" + bz + Chr(3);
            string bccHex = GetChecksum(tg1);
            string bcc = Hex2String(bccHex);
            string tg2 = Chr(2) + tg1 + bcc;

            DateTime stopTime = DateTime.Now.AddMilliseconds(2000);
            oCom.WriteLine(tg2);
            while (oCom.BytesToRead < 61) // 67
            {
                // Stoptime prüfen
                if (DateTime.Now > stopTime) // Timeout
                {
                    return oRW;
                }
            }

            string ret = oCom.ReadExisting();


            oRW.Status = ret.Substring(22, 2);
            string sDatum = ret.Substring(25, 10);
            oRW.Date = Convert.ToDateTime(sDatum);
            string sZeit = ret.Substring(36, 5);
            oRW.Time = Convert.ToDateTime(sZeit);
            oRW.Ln = ret.Substring(42, 5);
            string sGewicht =
                ret.Substring(50, 6).Replace("k", "").Replace("<", "").Replace(">", "").Replace("g", "").Replace(".",
                                                                                                                 ",");
            oRW.weight = Convert.ToDecimal(sGewicht)*1000;
            return oRW;
        }

        public void Close()
        {
            oCom.Close();
        }

        // LLC - Längs-Parität-Berechnung
        private static string GetChecksum(string s)
        {
            int checksum = 0;
            foreach (char c in s)
            {
                checksum ^= Convert.ToByte(c);
            }
            return checksum.ToString("X2");
        }

        public static string Hex2String(string hex)
        {
            string result = "";
            int count = hex.Length/2;
            int s;

            for (s = 0; s < count; s++)
            {
                string zeichen = hex.Substring(s*2, 2);
                result += (char) Convert.ToUInt16(zeichen, 16);
            }

            return result;
        }


        // Meine alte Foxprofunktion
        public static char Chr(int nAnsiCode)
        {
            return (char) nAnsiCode;
        }
    }
}