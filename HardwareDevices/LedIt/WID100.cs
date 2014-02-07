using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Windows;

// Fernanzeige 
// Die Erzeugung und Steuerung erfolgt über NetScaleView ! 

namespace HardwareDevices.LedIt
{
    public class Wid100
    {
        private int _ampel;

        private SerialPort sp;
        private bool _wStoerung;

        public Wid100(string comport)
        {
            _ampel = 48; // ASCII 0
            sp = new SerialPort(comport);
            SetPort();
        }

        public bool WStoerung
        {
            get { return _wStoerung; }
            set
            {
                _wStoerung = value;
                if (_wStoerung == false)
                    SetAllLightOff();
            }
        }


        private void SetPort()
        {
            try
            {
                if (!sp.IsOpen)
                    //   sp.PortName = p;

                    sp.BaudRate = 9600;
                sp.DataBits = 8;
                sp.Parity = Parity.None;
                sp.StopBits = StopBits.One;

                if (!sp.IsOpen)
                    sp.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void WriteToLed(string message)

        {
            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            Byte[] aAmpelStatus = new byte[] {(byte) _ampel};
            Byte[] aCr = new byte[] {13};
            Byte[] aM = new byte[message.Length];
            Byte[] aTg = new byte[message.Length + 2];
            Array.Copy(enc.GetBytes(message), aM, message.Length);
            aTg = Utils.Combine(aAmpelStatus, aM, aCr);

            if (!sp.IsOpen)
                sp.Open();
            sp.Write(Utils.Combine(aAmpelStatus, aM, aCr), 0, message.Length + 2);
        }

        public void Close()
        {
            sp.Close();
        }


        public void SetGreenLight()
        {
            _ampel = 50;
        }

        public void SetRedLight()
        {
            _ampel = 49;
        }

        public void SetAllLightOff()
        {
            _ampel = 48;
        }
    }
}