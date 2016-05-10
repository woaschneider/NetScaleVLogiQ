using System;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using HWB.Logging;

namespace HardwareDevices.Schenck.Disomat.RS232
{
    //  Schenk Pollprozedure 8785 


    public class ComTersusOpus : IWaagenSchnittstelle
    {
        public const char Stx = (char) 2;
        public const char Etx = (char) 3;
        public const char DC1 = (char) 17;
        private readonly Parity _p;
        private readonly StopBits _s;

        private readonly PortCom oCom;


        //************************************************************************************************************************
        public ComTersusOpus(string comPort, int baud, int databit, string sP, string sS)
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
            catch (Exception e)
            {
                Log.Instance.Error(e.Source + " " + e.Message);
                //     oCom.Open();
                Connected = false;
            }
        }

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

        public string WaageAufschalten(string wnr)
        {
            var tg1 = wnr + "#WN#" + Etx;

            // String senden
            var stopTime = DateTime.Now.AddMilliseconds(3000);
            oCom.WriteLine(Stx + tg1 + Bcc(tg1));

            while (oCom.BytesToRead < 12)
            {
                // Stoptime prüfen
                if (DateTime.Now > stopTime) // Timeout
                {
                    return oCom.ReadExisting();
                }
            }
            return oCom.ReadExisting();
        }

        public Weight GetPollGewicht(string wnr)
        {
            oCom.ReadExisting();
            Weight oPW = new Weight();
            if (oCom.IsOpen)
            {
                string clear = oCom.ReadExisting();
                // Pollstring zusammenbauen
                string tg = Stx + "01#TG#" + Etx + DC1;
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


                if (ret.Substring(4, 2) == "TG")
                {
                    string sGewicht = ret.Substring(7, 7);
                    sGewicht = sGewicht.Replace(".", ",");
                    oPW.Status = ret.Substring(31, 2).ToUpper();

                    var BitString = Convert.ToString(Convert.ToInt32(oPW.Status, 16), 2);

                    if (BitString != null)
                    {
                        if (BitString.Length == 8)
                        {
                            oPW.S0 = Convert.ToBoolean(Convert.ToInt16(BitString.Substring(7, 1))); // Unterbereich
                            oPW.S1 = Convert.ToBoolean(Convert.ToInt16(BitString.Substring(6, 1))); // Überbereich
                            oPW.S2 = Convert.ToBoolean(Convert.ToInt16(BitString.Substring(5, 1))); // Tara errechnet 
                            oPW.S3 = Convert.ToBoolean(Convert.ToInt16(BitString.Substring(4, 1))); // Genau null
                            oPW.S4 = Convert.ToBoolean(Convert.ToInt16(BitString.Substring(3, 1))); // 
                            oPW.S5 = Convert.ToBoolean(Convert.ToInt16(BitString.Substring(2, 1))); // Gewicht ungültig
                            oPW.S6 = Convert.ToBoolean(Convert.ToInt16(BitString.Substring(1, 1)));  // Tara gesetzt
                            oPW.S7 = Convert.ToBoolean(Convert.ToInt16(BitString.Substring(0, 1))); // Stillstand
                        }
                    }

                    if (oPW.S5)
                    {
                        oPW.WeightValue = (decimal)99.99;
                        return oPW;
                    }

                    oPW.WeightValue = Convert.ToDecimal(sGewicht);

                }
            }
            oCom.ReadExisting();
            return oPW;
        }

        public RegisterWeight RegisterGewicht(string wnr)
        {
            var oRW = new RegisterWeight();
            try
            {
                if (oCom.IsOpen)
                {
                    var tg1 = wnr + "#DR#3#" + Etx;


                    // String senden
                    var stopTime = DateTime.Now.AddMilliseconds(3000);
                    oCom.WriteLine(Stx + tg1 + Bcc(tg1));
                    while (oCom.BytesToRead < 61) // 63 eigentlich
                    {
                        // Stoptime prüfen
                        if (DateTime.Now > stopTime) // Timeout
                        {
                            var buffer = new byte[63];
                            Log.Instance.Error("Timeout (3000 ms) bei Eichwaegung");
                            oCom.Read(buffer, 0, oCom.BytesToRead);
                            Log.Instance.Info("RS232  Buffer - Disomat : " +
                                              Encoding.UTF8.GetString(buffer, 0, buffer.Length));
                            Thread.Sleep(1000);
                            var x = oCom.ReadExisting();


                            return oRW;
                        }
                    }

                    var ret = oCom.ReadExisting();


                    oRW.Status = ret.Substring(22, 2);
                    if (oRW.Status != "80")
                    {
                        Log.Instance.Error("Status bei Eichwaegung " + oRW.Status);
                        oRW.Status = "00";
                        return oRW;
                    }

                    var sDatum = ret.Substring(25, 8);
                    oRW.Date = Convert.ToDateTime(sDatum);
                    var sZeit = ret.Substring(34, 5);
                    oRW.Time = Convert.ToDateTime(sZeit);
                    oRW.Ln = ret.Substring(40, 6).Replace("#", "");

                    var sGewicht =
                        ret.Substring(48, 6)
                            .Replace("t", "")
                            .Replace("k", "")
                            .Replace("<", "")
                            .Replace(">", "")
                            .Replace(
                                "g", "").Replace(".", ",");
                    oRW.weight = Convert.ToDecimal(sGewicht);
                    Thread.Sleep(500);
                    Log.Instance.Info(ret);
                    var dummy = oCom.ReadExisting();
                }
            }
            catch (Exception e)
            {
                Log.Instance.Error(e.Source + " " + e.Message);
            }

            return oRW;
        }


        public void ReadAllContacts()
        {
            Connected = oCom.IsOpen;
            try
            {
                if (Connected == false)
                {
                    return;
                }
                var tg1 = "01#TK#" + Etx;

                // String senden

                oCom.WriteLine(Stx + tg1 + Bcc(tg1));
                var stopTime = DateTime.Now.AddMilliseconds(3000);
                while (oCom.BytesToRead < 59)
                {
                    // Stoptime prüfen
                    if (DateTime.Now > stopTime) // Timeout
                    {
                        // kontakte=  oCom.ReadExisting();
                        Log.Instance.Error("Timeout (3000 ms) bei der Tersuskontaktabfrage");
                        return;
                    }
                }
                oCom.ReadTimeout = 3000;
                var kontakte = oCom.ReadExisting();
                if (kontakte != null)
                {
                    X1 = kontakte.Substring(7, 1) == "1" ? true : false;
                    X2 = kontakte.Substring(9, 1) == "1" ? true : false;
                    X3 = kontakte.Substring(11, 1) == "1" ? true : false;
                    X4 = kontakte.Substring(13, 1) == "1" ? true : false;

                    X5 = kontakte.Substring(15, 1) == "1" ? true : false;
                    X6 = kontakte.Substring(17, 1) == "1" ? true : false;
                    X7 = kontakte.Substring(19, 1) == "1" ? true : false;
                    X8 = kontakte.Substring(21, 1) == "1" ? true : false;

                    X9 = kontakte.Substring(23, 1) == "1" ? true : false;
                    X10 = kontakte.Substring(25, 1) == "1" ? true : false;
                    X11 = kontakte.Substring(27, 1) == "1" ? true : false;
                    X12 = kontakte.Substring(29, 1) == "1" ? true : false;

                    X13 = kontakte.Substring(31, 1) == "1" ? true : false;
                    X14 = kontakte.Substring(33, 1) == "1" ? true : false;
                }
            }
            catch (Exception e)
            {
                Log.Instance.Error(e.Source + " " + e.Message);
            }
        }

        public void Close()
        {
            oCom.Close();
        }

        public void SetContact(int k)
        {
            try
            {
                var teiltelgramm = "00#00#00#00#";
                switch (k)
                {
                    case 1:
                        teiltelgramm = "01#00#00#00#";
                        break;
                    case 2:
                        teiltelgramm = "00#01#00#00#";
                        break;
                    case 3:
                        teiltelgramm = "00#00#01#00#";
                        break;
                    case 4:
                        teiltelgramm = "00#00#00#01#";
                        break;
                }
                var tg1 = "01#EK#" + teiltelgramm + Etx;

                // String senden

                oCom.WriteLine(Stx + tg1 + Bcc(tg1));
            }
            catch (Exception e)
            {
                Log.Instance.Error("Fehler beim Kontakte setzen");
                Log.Instance.Error(e.Source + " " + e.Message);
            }
        }

        public string Bcc(string s)
        {
            var checksum = s.Aggregate(0, (current, c) => current ^ Convert.ToByte(c));
            var hex = checksum.ToString("X2");
            var result = "";

            for (var ss = 0; ss < hex.Length/2; ss++)
            {
                result += (char) Convert.ToUInt16(hex.Substring(ss*2, 2), 16);
            }

            return result;
        }

        #region Kontakte

        //        Alle Kontakte lesen TK WN#TK# WN#TK#x1#x2#x3#x4#x5#x6#x7#x8#x9#x10#x11#x12#x
        //13#x14#a1#a2#
        //X1..x4: Stellung der Eingangskontakte 1-4
        //x5..x10: Stellung der Ausgangskontakte 1-6
        //x11..x14: Stellung der virtuellen EDV-Kontakte 1-4
        //a1,a2: Analoge EDV-Ausgänge (10 stellig,
        //rechtsbündig, 3 Nachkommastellen)
        //(Kontakt gesetzt = 1, Kontakt nicht gesetzt = 0)

        #endregion
    }
}