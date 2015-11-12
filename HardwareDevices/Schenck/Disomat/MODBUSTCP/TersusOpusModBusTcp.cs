using System;
using Modbus.Device;
using System.Net.Sockets;
using System.Threading;


// Die Klasse statisch gemacht

namespace HardwareDevices.Schenck.Disomat.MODBUSTCP
{
    public class TersusOpusModBusTcp : IWaagenSchnittstelle
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

        public bool PollStop { get; set; }
        public String Status { get; set; }
        public bool Connected { get; set; }
        public decimal DemoWeight { get; set; }
        public static ModbusIpMaster Master;
        public static TcpClient MasterTcpClient;
        public static byte SlaveId;
        public static ushort[] RegistersMessWerteFloat1;

        public TersusOpusModBusTcp(string ip)
        {
            var uRet = SetUp(1, ip, 502);
            if (uRet == "OK")
                Connected = true;
            else
            {
                Connected = false;
            }
        }
        public string WaageAufschalten(string wnr)
        {
            return "";
        }
        public static string SetUp(byte slaveId, string ip, int port)
        {
            SlaveId = slaveId;
            var uRet = CreateModBusTcpConnection(ip, port);

            return uRet;
        }

        public static void SetTare()
        {
            ushort startAddress = 16;
            ushort SetValue = 1; // Angezeigte Waage
            Master.WriteSingleRegister(SlaveId, startAddress, SetValue);
        }

        public static void DelTara()
        {
            ushort startAddress = 16;
            ushort SetValue = 2;
            Master.WriteSingleRegister(SlaveId, startAddress, SetValue);
        }

        public static void SetNull()
        {
            ushort startAddress = 16;
            ushort SetValue = 3;
            Master.WriteSingleRegister(SlaveId, startAddress, SetValue);
        }


        public Weight GetPollGewicht(string wnr)
        {
            Weight oW = new Weight();


            RegistersMessWerteFloat1 = Master.ReadHoldingRegisters(SlaveId, 1880, 2);
            string bs1 = Convert.ToString(RegistersMessWerteFloat1[0], 2).PadLeft(16, '0');
            string bs2 = Convert.ToString(RegistersMessWerteFloat1[1], 2).PadLeft(16, '0');
            string bs3 = bs1 + bs2;
            float z = StringToFloat(bs3);
            oW.WeightValue = (decimal) z;
            return oW;
        }

        public RegisterWeight RegisterGewicht(string wnr)
        {
            RegisterWeight oRw = new RegisterWeight();


            // Alte LN auslesen
            RegistersMessWerteFloat1 = Master.ReadHoldingRegisters(SlaveId, 1830, 2);
            string bs1 = Convert.ToString(RegistersMessWerteFloat1[0], 2).PadLeft(16, '0');
            string bs2 = Convert.ToString(RegistersMessWerteFloat1[1], 2).PadLeft(16, '0');
            string bs3 = bs1 + bs2;
            float oldLn = StringToFloat(bs3);
            // ****************

            // EW auslösen
            // Register muss erst auf Null gesetzt werden.
            // Ansonsten wird die Ausführung ignoriert!!!

            const ushort startAddress = 16;
            ushort setValue = 0; // Angezeigte Waage
            Master.WriteSingleRegister(SlaveId, startAddress, setValue); // Register auf 0 setzen
            Thread.Sleep(500);

            setValue = 80; // Angezeigte Waage
            Master.WriteSingleRegister(SlaveId, startAddress, setValue); // EW auslösen
            Thread.Sleep(1000);
            // ****************

            // Neue LN auslesen
            RegistersMessWerteFloat1 = Master.ReadHoldingRegisters(SlaveId, 1830, 2);
            bs1 = Convert.ToString(RegistersMessWerteFloat1[0], 2).PadLeft(16, '0');
            bs2 = Convert.ToString(RegistersMessWerteFloat1[1], 2).PadLeft(16, '0');
            bs3 = bs1 + bs2;
            float newLn = StringToFloat(bs3);
            // ****************

            // Status auslesen

            RegistersMessWerteFloat1 = Master.ReadHoldingRegisters(SlaveId, 4864, 2);
            bs1 = Convert.ToString(RegistersMessWerteFloat1[0], 2).PadLeft(16, '0');
            Convert.ToString(RegistersMessWerteFloat1[1], 2).PadLeft(16, '0');
            string status = String.Format("{0:X2}", Convert.ToUInt64(bs1, 2));


            if (newLn > oldLn) // LN hat hochgezählt, EW erfolgreich + Status prüfen
            {
                RegistersMessWerteFloat1 = Master.ReadHoldingRegisters(SlaveId, 1826, 2);
                bs1 = Convert.ToString(RegistersMessWerteFloat1[0], 2).PadLeft(16, '0');
                bs2 = Convert.ToString(RegistersMessWerteFloat1[1], 2).PadLeft(16, '0');
                bs3 = bs1 + bs2;
                float z = StringToFloat(bs3);

                oRw.weight = (decimal) z;

                oRw.Status = "80";

                oRw.Ln = ((int) newLn).ToString();

                // Zeit
                RegistersMessWerteFloat1 = Master.ReadHoldingRegisters(SlaveId, 2310, 2);
                bs1 = Convert.ToString(RegistersMessWerteFloat1[0], 2).PadLeft(16, '0');
                bs2 = Convert.ToString(RegistersMessWerteFloat1[1], 2).PadLeft(16, '0');


                string sekunden = (GetIntegerFromBinaryString(bs1, 8)/1000).ToString();
                string minuten = GetIntegerFromBinaryString(bs2.Substring(0, 8), 8).ToString().PadLeft(2, '0');
                string stunden = GetIntegerFromBinaryString(bs2.Substring(8, 8), 8).ToString().PadLeft(2, '0');
                oRw.Time = Convert.ToDateTime(stunden + ':' + minuten + ':' + sekunden);


                // Datum
                RegistersMessWerteFloat1 = Master.ReadHoldingRegisters(SlaveId, 2308, 2);
                bs1 = Convert.ToString(RegistersMessWerteFloat1[0], 2).PadLeft(16, '0');
                bs2 = Convert.ToString(RegistersMessWerteFloat1[1], 2).PadLeft(16, '0');

                string tag = GetIntegerFromBinaryString(bs1.Substring(0, 8), 8).ToString().PadLeft(2, '0');
                string monat = GetIntegerFromBinaryString(bs1.Substring(8, 8), 8).ToString().PadLeft(2, '0');
                string jahr = GetIntegerFromBinaryString(bs2.Substring(0, 8), 8).ToString().PadLeft(2, '0');


                oRw.Date = Convert.ToDateTime(tag + '.' + monat + '.' + jahr);

                return oRw;
            }
            oRw.Status = status.Substring(1, 2);
            oRw.Date = DateTime.Today;
            oRw.Time = DateTime.Now;
            return oRw;
        }

        public void Close()
        {
        }

        private static string CreateModBusTcpConnection(string ip, int port)
        {
            string cRet;
            // ModbusTCP Master erzeugen
            try
            {
                MasterTcpClient = new TcpClient(ip, port);
                Master = ModbusIpMaster.CreateIp(MasterTcpClient);
                cRet = "OK";
            }
            catch (Exception e)
            {
                cRet = e.ToString();
            }
            return cRet;
        }

        private static unsafe float StringToFloat(string value)
        {
            int i = Convert.ToInt32(value, 2);
            return *((float*) &i);
        }

        private static int GetIntegerFromBinaryString(string binary, int bitCount)
        {
            if (binary.Length == bitCount && binary[0] == '1')
                return Convert.ToInt32(binary.PadLeft(32, '1'), 2);
            return Convert.ToInt32(binary, 2);
        }

        public void ReadAllContacts()
        {
            
        }
        public void SetContact(int k)
        {
        }
    }
}