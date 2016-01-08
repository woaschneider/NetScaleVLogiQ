using System;
using System.ComponentModel;
using System.IO.Ports;
using HardwareDevices.Schenck.Disomat.RS232;
using HWB.NETSCALE.BOEF;

namespace HardwareDevices.Elseco
{ // Das könnte man mal auf Observer Pattern ändern

    public class RFReceiver : INotifyPropertyChanged
    {
        private string _kfzid;
 
        private string LastKfzId;
        private DateTime LastRfcontact;
        private readonly Lokaleeinstellungen oLE;
        private readonly PortCom oPortListener;

        public RFReceiver(bool aktiv)
        {
            if (aktiv)
            {
                //TODO In die Prog-Einstellungen übernehmen
                oLE = new Lokaleeinstellungen();
                oLE = oLE.Load();
                //   oPortListener = new PortCom();
                var PORT = oLE.FUNKMODULCOMPORT;
                var BAUD_POLL = "9600";
                var BIT_POLL = "8";
                try
                {
                    oPortListener = new PortCom(PORT, int.Parse(BAUD_POLL), Convert.ToInt32(BIT_POLL),
                        Parity.None,
                        StopBits.One);

                    oPortListener.Open();
                    oPortListener.DataReceived += serialPort_DataReceived;
                    oPortListener.ReceivedBytesThreshold = 2;
             
                }
                catch (Exception e)
                {
                 
                }
            }
        }

        //  public string ButtonNr;

        private string Kfzid
        {
            get { return _kfzid; }
            set
            {
                _kfzid = value;
                TelegrammChange();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void TelegrammChange()
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs("Kfzid"));
        }

        private void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //   System.Threading.Thread.Sleep(100);
            var message = new byte[2];

            oPortListener.Read(message, 0, 2);
            if ((LastRfcontact > DateTime.Now) && (message[0].ToString() == LastKfzId))
                return;

            //TODO: Den 60 Sekundenwert einstelbar machen
            LastRfcontact = DateTime.Now.AddSeconds(60);

            Kfzid = message[0].ToString();
           // Damit der Event feuert
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