using System;
using System.IO.Ports;
using System.Linq;
using System.Text;
using HWB.Logging;
using NLog.Config;
using NLog;


namespace HardwareDevices.Schenck.Disomat.RS232
{
    //  Schenk Pollprozedure 8785 


    public class ComTersusOpus : IWaagenSchnittstelle
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
        public bool Connected { get; set; }

        public decimal DemoWeight { get; set; }

        private PortCom oCom;
        private readonly Parity _p;
        private readonly StopBits _s;
        public const char Stx = (char) 2;
        public const char Etx = (char) 3;


        //************************************************************************************************************************
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
            catch (Exception e)
            {
                Log.Instance.Error(e.Source+" "+ e.Message);
           //     oCom.Open();
                Connected = false;
            }
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

        public string WaageAufschalten(string wnr)
        {
            string tg1 = wnr + "#WN#" + Etx;

            // String senden
            DateTime stopTime = DateTime.Now.AddMilliseconds(3000);
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
            Weight oPW = new Weight();

            try
            {
                if (oCom.IsOpen)
                {
                    string clear = oCom.ReadExisting();
                    // Pollstring zusammenbauen
                    string tg1 = wnr + "#TG#" + Etx;


                    // String senden
                    DateTime stopTime = DateTime.Now.AddMilliseconds(3000);
               //     oCom.WriteTimeout = 1000;
                    oCom.WriteLine(Stx + tg1 + Bcc(tg1));
                    while (oCom.BytesToRead < 36)
                    {
                        // Stoptime prüfen
                        if (DateTime.Now > stopTime) // Timeout
                        {
                            Log.Instance.Error("Timeout (3000 ms) bei der Tersuspollabfrage");
                            return oPW; // Wegen Timeout
                        }
                    }

                    string ret = oCom.ReadExisting();


                    if (ret.Substring(4, 2) == "TG")
                    {
                        string sGewicht = ret.Substring(7, 7);
                        sGewicht = sGewicht.Replace(".", ",");

                        oPW.WeightValue = Convert.ToDecimal(sGewicht);
                    }
                 
                }
                else
                {
                    Log.Instance.Error(oCom.PortName + " " + "ist geschlossen!");
                    oCom.Open();
                }
            }
            catch (Exception e)
            {

                Log.Instance.Error(e.Source + " " +  e.InnerException +" "+ e.Message);
            }
           
            return oPW;
        }

        public RegisterWeight RegisterGewicht(string wnr)
        {
            RegisterWeight oRW = new RegisterWeight();
            try
            {
                if (oCom.IsOpen)
                {
                    string tg1 = wnr + "#DR#3#" + Etx;


                    // String senden
                    DateTime stopTime = DateTime.Now.AddMilliseconds(3000);
                    oCom.WriteLine(Stx + tg1 + Bcc(tg1));
                    while (oCom.BytesToRead < 61) // 63 eigentlich
                    {
                        // Stoptime prüfen
                        if (DateTime.Now > stopTime) // Timeout
                        {
                            byte[] buffer = new byte[63];
                            Log.Instance.Error("Timeout (3000 ms) bei Eichwaegung");
                            oCom.Read(buffer, 0, oCom.BytesToRead);
                            Log.Instance.Info("RS232  Buffer - Disomat : " + Encoding.UTF8.GetString(buffer, 0, buffer.Length));
                            System.Threading.Thread.Sleep(1000);
                            var x = oCom.ReadExisting();
                            return oRW;
                        }
                    }

                    string ret = oCom.ReadExisting();


                    oRW.Status = ret.Substring(22, 2);
                    if (oRW.Status != "80")
                    {
                        oRW.Status = "00";
                        return oRW;
                    }

                    string sDatum = ret.Substring(25, 8);
                    oRW.Date = Convert.ToDateTime(sDatum);
                    string sZeit = ret.Substring(34, 5);
                    oRW.Time = Convert.ToDateTime(sZeit);
                    oRW.Ln = ret.Substring(40, 6).Replace("#","");

                    string sGewicht =
                        ret.Substring(48, 6).Replace("t", "").Replace("k", "").Replace("<", "").Replace(">", "").Replace(
                            "g", "").Replace(".", ",");
                    oRW.weight = Convert.ToDecimal(sGewicht);
                    System.Threading.Thread.Sleep(500);
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
                string tg1 = "01#TK#" + Etx;

                // String senden

                oCom.WriteLine(Stx + tg1 + Bcc(tg1));
                DateTime stopTime = DateTime.Now.AddMilliseconds(3000);
                while (oCom.BytesToRead < 59)
                {
                    // Stoptime prüfen
                    if (DateTime.Now > stopTime) // Timeout
                    {
                        // kontakte=  oCom.ReadExisting();
                        Log.Instance.Error( "Timeout (3000 ms) bei der Tersuskontaktabfrage");
                        return;
                    }
                }
              oCom.ReadTimeout = 3000;
                var kontakte = oCom.ReadExisting();
                if (kontakte != null)
                {
                    X1 = (kontakte.Substring(7, 1) == "1") ? true : false;
                    _x2 = (kontakte.Substring(9, 1) == "1") ? true : false;
                    _x3 = (kontakte.Substring(11, 1) == "1") ? true : false;
                    _x4 = (kontakte.Substring(13, 1) == "1") ? true : false;

                    X5 = (kontakte.Substring(15, 1) == "1") ? true : false;
                    _x6 = (kontakte.Substring(17, 1) == "1") ? true : false;
                    _x7 = (kontakte.Substring(19, 1) == "1") ? true : false;
                    _x8 = (kontakte.Substring(21, 1) == "1") ? true : false;

                    X9 = (kontakte.Substring(23, 1) == "1") ? true : false;
                    _x10 = (kontakte.Substring(25, 1) == "1") ? true : false;
                    _x11 = (kontakte.Substring(27, 1) == "1") ? true : false;
                    _x12 = (kontakte.Substring(29, 1) == "1") ? true : false;

                    X13 = (kontakte.Substring(31, 1) == "1") ? true : false;
                    _x14 = (kontakte.Substring(33, 1) == "1") ? true : false;
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

        public string Bcc(string s)
        {
            int checksum = s.Aggregate(0, (current, c) => current ^ Convert.ToByte(c));
            string hex = checksum.ToString("X2");
            string result = "";

            for (int ss = 0; ss < hex.Length/2; ss++)
            {
                result += (char) Convert.ToUInt16(hex.Substring(ss*2, 2), 16);
            }

            return result;
        }

        public void SetContact(int k)
        {
            try
            {
                string teiltelgramm = "00#00#00#00#";
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
                string tg1 = "01#EK#" + teiltelgramm + Etx;

                // String senden

                oCom.WriteLine(Stx + tg1 + Bcc(tg1));
            }
            catch (Exception e)
            {
                Log.Instance.Error("Fehler beim Kontakte setzen");
                Log.Instance.Error(e.Source + " " +  e.Message);
                
            }
       
            
        }
    }
}