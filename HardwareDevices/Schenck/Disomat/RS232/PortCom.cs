using System;
using System.IO.Ports;

namespace HardwareDevices.Schenck.Disomat.RS232
{
    public class PortCom : SerialPort
    {
        public PortCom()
        {
            PortName = "COM2";
            BaudRate = 9600;
            Parity = Parity.None;
            DataBits = 8;
            StopBits = StopBits.One;
        }

        public PortCom(string cPortName)
        {
            PortName = cPortName;
            BaudRate = 9600;
            Parity = Parity.None;
            DataBits = 8;
            StopBits = StopBits.One;
        }

        public PortCom(string cPortName, int iBaudRate, int iDataBit, Parity pParity, StopBits sStopbit)
        {
            PortName = cPortName;
            BaudRate = iBaudRate;
            DataBits = iDataBit;
            Parity = pParity;

            StopBits = sStopbit;
        }


        public string PortOpen()
        {
            try
            {
                Open();
                return "Port " + PortName + "wurde geöffnet";
            }
            catch (Exception s)
            {
                //throw (s);                
                return s.Message;
            }
        }

        // Hier kommen die Methoden rein
        public void BaseConstructorCall()
        {
            PortOpen();
        }

        //
    }
}