using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using HardwareDevices.LedIt;
using HardwareDevices.Schenck.Disomat.MODBUSTCP;
using HardwareDevices.Schenck.Disomat.RS232;
using HWB.NETSCALE.GLOBAL;
using NetScaleGlobal;
using OakLeaf.MM.Main;
using OakLeaf.MM.Main.WPF;
using HWB.NETSCALE.BOEF;


namespace HardwareDevices
{
    public delegate void WeightChangedHandler();



    /// <summary>
    /// Interaction logic for mmUserControl.xaml
    /// </summary>
    public partial class NetScaleView : mmUserControl
    {
//******************************************************************
// Observer Pattern
// Diese Klasse ist das Subject (oder Observable)

        private event WeightChangedHandler _onChange;

        public event WeightChangedHandler OnWeightChanged
        {
            add { _onChange += value; }
            remove { _onChange -= value; }
        }


        public bool Stillstand;

        private decimal _gewicht;

        public decimal Gewicht
        {
            get { return _gewicht; }
            set
            {
                _gewicht = value;
                WeightChangeNotify();
            }
        }

        private void WeightChangeNotify()
        {
            if (_onChange != null)
                _onChange();
        }

        private void tb_gewicht_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (oW != null)
                    Gewicht = oW.WeightValue;
            }
            catch (Exception)
            {
                throw;
            }
        }

// *****************************************************************
        private bool _poll = false;


        public bool Poll
        {
            get { return _poll; }
            set { _poll = value; }
        }

        private string _einheit = "";

        public string Einheit
        {
            get { return _einheit; }
            set { _einheit = value; }
        }

        public bool SliderInvisible
        {
            set { slider1.Visibility = Visibility.Hidden; }
        }

        private Einstellungen oE;
        private EinstellungenEntity oEe;
        public LedIt.Wid100 oWF;
        private Waageneinstellungen oWE;

        private IWaagenSchnittstelle _w1;
        private IWaagenSchnittstelle _w2;
        private DispatcherTimer DT;
        public Weight oW;

        private bool _wStoerung;
        private bool _toogle;

      

        public bool WStoerung
        {
            get { return _wStoerung; }
            set
            {
                _wStoerung = value;
                if (_wStoerung == true)
                {
                    if (DT.Interval != System.TimeSpan.Parse("1000"))
                        DT.Interval = new TimeSpan(0, 0, 5);
                }
                else
                {
                    if (DT.Interval != System.TimeSpan.Parse("1000"))
                        DT.Interval = new TimeSpan(0, 0, 1);
                }
            }
        }

        // Da erst zur Laufzeit entschieden wird welches Waggen Object geladen wird
        private int _activeScale;

        public NetScaleView()
        {
            InitializeComponent();
            if (goApp.FernanzeigeAktive)
            {
                oE = new Einstellungen();
                oEe = oE.GetEinstellungen();


                var oLe = new Lokaleeinstellungen().Load();
                oWF = new Wid100(oLe.FERNANZEIGECOMPORT);
            }
        }

        public int ActiveScale
        {
            get { return _activeScale; }
            set
            {
                 
               
                _activeScale = value;

                WaageAufschalten();
                if (_activeScale == 1)
                {
                    tb_waagennr.Text = oWE.W1_WAAGENNAME;
                    tb_e.Text = oWE.W1_e;
                    tb_min.Text = oWE.W1_min;
                    tb_max.Text = oWE.W1_max;
                }
                if (_activeScale == 2)
                {
                    tb_waagennr.Text = oWE.W2_WAAGENNAME;
                    tb_e.Text = oWE.W2_e;
                    tb_min.Text = oWE.W2_min;
                    tb_max.Text = oWE.W2_max;
                }

                if (_activeScale == 3)
                {
                
                }
            }
        }

   
        public bool X1
        {
            get
            {
                if (_w1 != null)
                    return _w1.X1;
                else
                {
                    return false;
                }
            }

           
            set
            {
                _w1.X1 = value;
            

            }
     
        }

        public bool X2
        {
            get { return _w1.X2; }
            set
            {
                _w1.X2 = value;
           
            }
        }

        public bool X3
        {
            get { return _w1.X3; }
            set
            {
                _w1.X3 = value;
              
            }
            
        }

        public bool X4
        {
            get { return _w1.X4; }
            set
            {
                _w1.X4 = value;
              
            }

        }

        public bool X5
        {
            get { return  _w1.X5; }
            set
            {
                _w1.X5 = value;

            }
        }

        public bool X6
        {
            get { return _w1.X6; }
            set
            {
                _w1.X6 = value;

            }
        }

        public bool X7
        {
            get { return _w1.X7; }
            set
            {
                _w1.X7 = value;

            }
         
        }

        public bool X8
        {
            get { return _w1.X8; }
            set
            {
                _w1.X8 = value;

            }
        }

        public bool X9
        {
            get { return _w1.X9; }
            set
            {
                _w1.X9 = value;

            }
        }

        public bool X10
        {
            get { return _w1.X10; }
            set
            {
                _w1.X10 = value;

            }
        }

        public bool X11
        {
            get { return _w1.X11; }
            set
            {
                _w1.X11 = value;

            }
        }

        public bool X12
        {
            get { return _w1.X12; }
            set
            {
                _w1.X12 = value;

            }
        }

        public bool X13
        {
            get { return _w1.X13; }
            set
            {
                _w1.X13 = value;

            }
        }

        public bool X14
        {
            get { return _w1.X14; }
            set
            {
                _w1.X14 = value;

            }
          
        }

        private void WaageAufschalten()
        {
            if (oWE.MESSKREISE == "2")
            {
                switch (_activeScale)
                {
                    case 1: // Waage 1
                        _w1.WaageAufschalten("02");
                        break;
                    case 2: // Waage 2
                        _w1.WaageAufschalten("03");
                        break;

                    case 3: // Verbund
                        _w1.WaageAufschalten("04");
                        break;
                }
            }
        }

        public void SetUp(bool Demo)
        {
            if (!Demo) // Normalfall
            {
                DT = new DispatcherTimer();
                DT.Tick += new EventHandler(Poll_TICK);
                DT.Interval = TimeSpan.FromMilliseconds(2000);
                DT.Start();

                oWE = new Waageneinstellungen();
                oWE = oWE.Load();

                // Ein oder zwei Auswerte ? 
                int Anz = int.Parse(oWE.SCALES);
                if (Anz == 1)
                {
                    SetUpW(1, oWE);
                    if (_w1.Connected == false)
                    {
                        _poll = false;
                        MessageBox.Show("Es konnte keine Verbindung zur Wägeelektronik aufgebaut werden!", "ACHTUNG",
                                        MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        _poll = true;
                    }
                    if (oWE.MESSKREISE == "2")
                    {
                    }
                }
                else
                {
                    SetUpW(1, oWE);
                    if (_w1.Connected == false)
                    {
                        _poll = false;
                        MessageBox.Show("Es konnte keine Verbindung zur Wägeelektronik aufgebaut werden!", "ACHTUNG",
                                        MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                    else
                    {
                        _poll = true;
                    }

                    SetUpW(2, oWE);
                    if (_w2.Connected == false)
                    {
                        _poll = false;
                        MessageBox.Show("Es konnte keine Verbindung zur Wägeelektronik aufgebaut werden!", "ACHTUNG",
                                        MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                    else // Damit wird der Fall abgefangen, das Waage 1 keine Verbindung hat
                    {
                        // In so einem Fall werden beide Waage nicht gepollt
                        if (_w1.Connected == true)
                            _poll = true;
                        else
                        {
                            _poll = false;
                        }
                    }
                }
            }
            else // Demo
            {
                oWE = new Waageneinstellungen();
                oWE = oWE.Load();
                DT = new DispatcherTimer();
                DT.Tick += new EventHandler(Poll_TICK);
                DT.Interval = TimeSpan.FromMilliseconds(1000);
                DT.Start();
                _w1 = new DemoDevice();
                // Demo
            }
        }

        public void Close()
        {
            if (oWE.SCALES == "1")
            {
                if (_w1 != null)
                    _w1.Close();
            }
            if (oWE.SCALES == "2")
            {
                if (_w1 != null)
                    _w1.Close();

                if (_w2 != null)
                    _w2.Close();
            }
            DT.Stop();
            if (oWF != null)
                oWF.Close(); // Schließ den seriellen Port
        }

        private void Poll_TICK(object sender, EventArgs e)
        {
            if (_w1.Connected == false)
            {   oWE = new Waageneinstellungen();
                oWE = oWE.Load();
                SetUpW(1,oWE);
            }

            if(_poll)
            poll();
        }


        private void SetUpW(int Wnr, Waageneinstellungen oWE)
        {
            if (Wnr == 1)
            {
                switch (oWE.W1_WAAGENID)
                {
                    case "1": // Disomat Bplus RS232
                        _w1 = new ComTersusOpus(oWE.W1_COM, int.Parse(oWE.W1_BAUD),
                                                int.Parse(oWE.W1_DATA_BIT), oWE.W1_PARITY_BIT,
                                                oWE.W1_STOP_BIT);
                        break;
                    case "10": // OPUS RS232
                        _w1 = new ComTersusOpus(oWE.W1_COM, int.Parse(oWE.W1_BAUD),
                                                int.Parse(oWE.W1_DATA_BIT), oWE.W1_PARITY_BIT,
                                                oWE.W1_STOP_BIT);
                        break;
                    case "20": // Tersus RS232
                        _w1 = new ComTersusOpus(oWE.W1_COM, int.Parse(oWE.W1_BAUD),
                                                int.Parse(oWE.W1_DATA_BIT), oWE.W1_PARITY_BIT,
                                                oWE.W1_STOP_BIT);
                        break;
                    case "30": // OPUS Modbus/TCP
                        _w1 = new TersusOpusModBusTcp(oWE.W1_IP_NUMMER);

                        break;
                    case "40":
                        _w1 = new TersusOpusModBusTcp(oWE.W1_IP_NUMMER);
                        break; // Tersus Modbus/Tcp

                    case "50": // Systec RS232
                        break;
                    case "60": // Systec TCP  
                        _w1 = new Systec.SystecTcp1Adm(oWE.W1_IP_NUMMER, "1234");
                        break;

                    case "70": // Systec TCP  
                        _w1 = new Systec.SystecTcp2Adm(oWE.W1_IP_NUMMER, "1234");
                        break;
                }
            }

            if (Wnr == 2)
            {
                switch (oWE.W2_WAAGENID)
                {
                    case "1": // DisomatBplus RS232
                        _w2 = new ComTersusOpus(oWE.W2_COM, int.Parse(oWE.W2_BAUD),
                                                int.Parse(oWE.W2_DATA_BIT), oWE.W2_PARITY_BIT,
                                                oWE.W2_STOP_BIT);
                        break;
                    case "10": // OPUS RS232
                        _w2 = new ComTersusOpus(oWE.W2_COM, int.Parse(oWE.W2_BAUD),
                                                int.Parse(oWE.W2_DATA_BIT), oWE.W2_PARITY_BIT,
                                                oWE.W2_STOP_BIT);
                        break;
                    case "20": // Tersus RS232
                        _w2 = new ComTersusOpus(oWE.W2_COM, int.Parse(oWE.W2_BAUD),
                                                int.Parse(oWE.W2_DATA_BIT), oWE.W2_PARITY_BIT,
                                                oWE.W2_STOP_BIT);
                        break;
                    case "30": // OPUS Modbus/TCP
                        _w2 = new TersusOpusModBusTcp(oWE.W2_IP_NUMMER);
                        break;
                    case "40":
                        _w2 = new TersusOpusModBusTcp(oWE.W2_IP_NUMMER);
                        break; // Tersus Modbus/Tcp

                    case "50": // Systec RS232
                        break;
                    case "60": // Systec TCP
                        _w2 = new Systec.SystecTcp1Adm(oWE.W2_IP_NUMMER, "1234");
                        break;
                }
            }
        }

        public void poll()
        {
            var x = _poll;


            // Erst mal nur auf _w1
         //   _w1.ReadAllContacts();
            X1 = _w1.X1;
            cbx1.IsChecked = X1;

            X2 = _w1.X2;
            cbx2.IsChecked = X2;

            X3 = _w1.X3;
            cbx3.IsChecked = X3;

            X4 = _w1.X4;
            cbx4.IsChecked = X4;

            X5 = _w1.X5;
            cbx5.IsChecked = X5;

            X6 = _w1.X6;
            cbx6.IsChecked = X6;


            X7 = _w1.X7;
            cbx7.IsChecked = X7;

            X8 = _w1.X8;
            cbx8.IsChecked = X8;

            X9 = _w1.X9;
            cbx9.IsChecked = X9;

            X10 = _w1.X10;
            cbx10.IsChecked = X10;

            X11 = _w1.X11;
            cbx11.IsChecked = X11;

            X12 = _w1.X12;
            cbx12.IsChecked = X12;


            X13 = _w1.X13;
            cbx13.IsChecked = X13;

            X14 = _w1.X14;
            cbx14.IsChecked = X14;

         

            if (_activeScale == 0)
                return;

            switch (_activeScale)
            {
                case 1: // Waage 1
                    
                        oW = _w1.GetPollGewicht("01");
                    if (oW.Status != null)
                    {
                        if (oW.Status.Substring(0, 1) == "0")
                        {
                            tb_status.Text = "Kein Stillstand";
                            Stillstand = false;
                        }
                    }
                    else
                    {
                        tb_status.Text = "";
                        Stillstand = true;
                    }
                    //    tb_status.Text = _w1.Status;
                    break;
                case 2: // Waage 2
                    if (oWE.MESSKREISE == "2")
                    {

                        oW = _w1.GetPollGewicht("03");
                        tb_status.Text = _w1.Status;
                    }

                    if (oWE.MESSKREISE == "1")
                    {

                        oW = _w2.GetPollGewicht("01");
                        tb_status.Text = _w2.Status;
                    }
                    break;
                case 3: // Verbund

                    oW = _w1.GetPollGewicht("04");
                    tb_status.Text = _w1.Status;
                    break;
            }
            if (oWF != null)
            {
                if (oW.WeightValue <= oEe.Schwellwertampel && _wStoerung == false)
                {
                    oWF.SetAllLightOff();
                }


                if (_wStoerung && _toogle)
                {
                    oWF.WriteToLed("Probl.");
                    System.Threading.Thread.Sleep(1250);
                    oWF.WriteToLed("Waage");
                    System.Threading.Thread.Sleep(1250);
                    oWF.WriteToLed("noch");
                    System.Threading.Thread.Sleep(1250);
                    oWF.WriteToLed("mal");
                    System.Threading.Thread.Sleep(1250);
                }
                else
                {
                    oWF.WriteToLed(oW.WeightValue.ToString());
                }
                _toogle = _toogle ? false : true;
            }

            if (_poll)
            {
                tb_gewicht.Text = oW.WeightValue.ToString() + _einheit;
            }
        }

        public RegisterWeight RegisterGewicht()
        {
            
            RegisterWeight oRW;
            _poll = false;
            switch (_activeScale)
            {
                case 1:
                    _poll = false;
                    oRW = _w1.RegisterGewicht("01");
                    SaveClearWAlarm(oRW);
                    _poll = true;


                    return oRW;

                case 2:
                    if (oWE.MESSKREISE == "2")

                    {
                        _poll = false;
                        oRW = _w1.RegisterGewicht("03");
                        SaveClearWAlarm(oRW);
                        _poll = true;
                        return oRW;
                    }
                    if (oWE.MESSKREISE == "1")
                    {
                        _poll = false;
                        oRW = _w2.RegisterGewicht("01");
                        SaveClearWAlarm(oRW);
                        _poll = true;
                        return oRW;
                    }
                    else
                    {
                        // TODO:

                        RegisterWeight odummy = new RegisterWeight();
                        return odummy;
                    }


                 

                case 3:
                    _poll = false;
                    oRW = _w1.RegisterGewicht("04");
                    SaveClearWAlarm(oRW);
                    _poll = true;
                    return oRW;
                 
                default:

                    RegisterWeight dummy = new RegisterWeight();
                    dummy.Status = "00";
                    return dummy;
            }
        }

        private void SaveClearWAlarm(RegisterWeight oRW)
        {
            if (oRW.Status == "80")
                WStoerung = false;
            else
            {
                WStoerung = true;
            }
            if (oWF != null)
                oWF.WStoerung = WStoerung;
        }

        public void ResetAlarm()
        {
            WStoerung = false;
            if (oWF != null)
            {
                oWF.WStoerung = WStoerung;
            }
        }

        private void slider1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _w1.DemoWeight = Convert.ToDecimal(slider1.Value);
            tb_gewicht.Text = slider1.Value.ToString();
        }

        public void SetContact(int k)
        {
            _w1.SetContact(k);
        }

        private void expanderKontakte_Collapsed(object sender, RoutedEventArgs e)
        {
            this.Height = 200;
        }

        private void expanderKontakte_Expanded(object sender, RoutedEventArgs e)
        {
            this.Height = 260;
        }


    }
}