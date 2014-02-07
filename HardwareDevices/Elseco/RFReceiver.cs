using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using HardwareDevices.Schenck.Disomat.RS232;
using HWB.NETSCALE.BOEF;

namespace HardwareDevices.Elseco
{   // Das könnte man mal auf Observer Pattern ändern
    public class RFReceiver : INotifyPropertyChanged
    {
        public string ErrorMessage;
        private DateTime LastRfcontact;
        public string LastKfzId;
        private Lokaleeinstellungen oLE;
        public event PropertyChangedEventHandler PropertyChanged;
        private string _kfzid;
        private PortCom oPortListener;
        //  public string ButtonNr;

        public string Kfzid
        {
            get { return _kfzid; }
            set
            {
                _kfzid = value;
                TelegrammChange();
            }
        }

        public RFReceiver(bool aktiv)
        {
            if (aktiv == true)
            {
                //TODO In die Prog-Einstellungen übernehmen
                oLE = new Lokaleeinstellungen();
                oLE = oLE.Load();
                //   oPortListener = new PortCom();
                string PORT = oLE.FUNKMODULCOMPORT;
                string BAUD_POLL = "9600";
                string BIT_POLL = "8";
                try
                {
                    oPortListener = new PortCom(PORT, int.Parse(BAUD_POLL), Convert.ToInt32(BIT_POLL),
                                                System.IO.Ports.Parity.None,
                                                System.IO.Ports.StopBits.One);

                    oPortListener.Open();
                    oPortListener.DataReceived += serialPort_DataReceived;
                    oPortListener.ReceivedBytesThreshold = 2;
                    ErrorMessage = "";
                }
                catch (Exception e)
                {
                    ErrorMessage = "Funkmodul - Falsche Einstellunge im Programm-Setup -->" + e.Message.ToString();
                }
            }
        }

        private void TelegrammChange()
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs("Kfzid"));
        }

        private void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //   System.Threading.Thread.Sleep(100);
            byte[] message = new byte[2];

            oPortListener.Read(message, 0, 2);
            if ((LastRfcontact > DateTime.Now) && (message[0].ToString() == LastKfzId))
                return;

            //TODO: Den 60 Sekundenwert einstelbar machen
            LastRfcontact = DateTime.Now.AddSeconds(60);

            Kfzid = message[0].ToString();
            ; // Damit der Event feuert
            LastKfzId = Kfzid;

            //  ButtonNr = Convert.ToInt32(message[1]).ToString();
        }

        public void Close()
        {
            if (oPortListener != null)
                oPortListener.Close();
        }
    }
}