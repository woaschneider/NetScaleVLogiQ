using System;
using System.Net.Sockets;
using System.Threading;
using Modbus.Device;

// Die Klasse statisch gemacht

namespace HardwareDevices.Schenck.Disomat.MODBUSTCP
{
    public class TersusOpusModBusTcp : IWaagenSchnittstelle
    {
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

        public bool PollStop { get; set; }

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
            return "";
        }


        public Weight GetPollGewicht(string wnr)
        {
            var oW = new Weight();


            RegistersMessWerteFloat1 = Master.ReadHoldingRegisters(SlaveId, 1880, 2);
            var bs1 = Convert.ToString(RegistersMessWerteFloat1[0], 2).PadLeft(16, '0');
            var bs2 = Convert.ToString(RegistersMessWerteFloat1[1], 2).PadLeft(16, '0');
            var bs3 = bs1 + bs2;
            var z = StringToFloat(bs3);
            oW.WeightValue = (decimal) z;
            return oW;
        }

        public RegisterWeight RegisterGewicht(string wnr)
        {
            var oRw = new RegisterWeight();


            // Alte LN auslesen
            RegistersMessWerteFloat1 = Master.ReadHoldingRegisters(SlaveId, 1830, 2);
            var bs1 = Convert.ToString(RegistersMessWerteFloat1[0], 2).PadLeft(16, '0');
            var bs2 = Convert.ToString(RegistersMessWerteFloat1[1], 2).PadLeft(16, '0');
            var bs3 = bs1 + bs2;
            var oldLn = StringToFloat(bs3);
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
            var newLn = StringToFloat(bs3);
            // ****************

            // Status auslesen

            RegistersMessWerteFloat1 = Master.ReadHoldingRegisters(SlaveId, 4864, 2);
            bs1 = Convert.ToString(RegistersMessWerteFloat1[0], 2).PadLeft(16, '0');
            Convert.ToString(RegistersMessWerteFloat1[1], 2).PadLeft(16, '0');
            var status = string.Format("{0:X2}", Convert.ToUInt64(bs1, 2));


            if (newLn > oldLn) // LN hat hochgezählt, EW erfolgreich + Status prüfen
            {
                RegistersMessWerteFloat1 = Master.ReadHoldingRegisters(SlaveId, 1826, 2);
                bs1 = Convert.ToString(RegistersMessWerteFloat1[0], 2).PadLeft(16, '0');
                bs2 = Convert.ToString(RegistersMessWerteFloat1[1], 2).PadLeft(16, '0');
                bs3 = bs1 + bs2;
                var z = StringToFloat(bs3);

                oRw.weight = (decimal) z;

                oRw.Status = "80";

                oRw.Ln = ((int) newLn).ToString();

                // Zeit
                RegistersMessWerteFloat1 = Master.ReadHoldingRegisters(SlaveId, 2310, 2);
                bs1 = Convert.ToString(RegistersMessWerteFloat1[0], 2).PadLeft(16, '0');
                bs2 = Convert.ToString(RegistersMessWerteFloat1[1], 2).PadLeft(16, '0');


                var sekunden = (GetIntegerFromBinaryString(bs1, 8)/1000).ToString();
                var minuten = GetIntegerFromBinaryString(bs2.Substring(0, 8), 8).ToString().PadLeft(2, '0');
                var stunden = GetIntegerFromBinaryString(bs2.Substring(8, 8), 8).ToString().PadLeft(2, '0');
                oRw.Time = Convert.ToDateTime(stunden + ':' + minuten + ':' + sekunden);


                // Datum
                RegistersMessWerteFloat1 = Master.ReadHoldingRegisters(SlaveId, 2308, 2);
                bs1 = Convert.ToString(RegistersMessWerteFloat1[0], 2).PadLeft(16, '0');
                bs2 = Convert.ToString(RegistersMessWerteFloat1[1], 2).PadLeft(16, '0');

                var tag = GetIntegerFromBinaryString(bs1.Substring(0, 8), 8).ToString().PadLeft(2, '0');
                var monat = GetIntegerFromBinaryString(bs1.Substring(8, 8), 8).ToString().PadLeft(2, '0');
                var jahr = GetIntegerFromBinaryString(bs2.Substring(0, 8), 8).ToString().PadLeft(2, '0');


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

        public void ReadAllContacts()
        {
        }

        public void SetContact(int k)
        {
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
            var i = Convert.ToInt32(value, 2);
            return *(float*) &i;
        }

        private static int GetIntegerFromBinaryString(string binary, int bitCount)
        {
            if (binary.Length == bitCount && binary[0] == '1')
                return Convert.ToInt32(binary.PadLeft(32, '1'), 2);
            return Convert.ToInt32(binary, 2);
        }
    }
}