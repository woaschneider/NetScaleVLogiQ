using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using combit.ListLabel20;
using HWB.NETSCALE.BOEF;

using HWB.NETSCALE.GLOBAL;
using OakLeaf.MM.Main;
using OakLeaf.MM.Main.Collections;
using OakLeaf.MM.Main.WPF;
using Xceed.Wpf.DataGrid.FilterCriteria;


namespace HWB.NETSCALE.FRONTEND.WPF.Forms
{
    /// <summary>
    /// WiegelisteFrm Class
    /// </summary>
    public partial class WiegelisteFrm
    {
        /// <summary>
        /// Constructor
        /// </summary>
        private Waege _boW;


        public int uRet;

        public WiegelisteFrm()
        {
            this.InitializeComponent();

            _boW = new Waege();
            vondatePicker.SelectedDate = System.DateTime.Now;
            bisdatePicker.SelectedDate = System.DateTime.Now;

            cbFelder1.Items.Add("Lieferschein-Nr.");
            cbFelder1.Items.Add("Auftraggeber");
            cbFelder1.Items.Add("Produkt");
            cbFelder1.Items.Add("Fahrzeug");

            cbFelder2.Items.Add("Lieferschein-Nr.");
            cbFelder2.Items.Add("Auftraggeber");
            cbFelder2.Items.Add("Produkt");
            cbFelder2.Items.Add("Fahrzeug");

            cbFelder3.Items.Add("Lieferschein-Nr.");
            cbFelder3.Items.Add("Auftraggeber");
            cbFelder3.Items.Add("Produkt");
            cbFelder3.Items.Add("Fahrzeug");

            if (goApp.acessLevel == "1")

            {
            }
            else
            {
              
            }
            PartialSetGrid();

            DeserializeFilter();
            FillGrid();
            PreviewKeyDown += new KeyEventHandler(HandleKey);
        }

        private void HandleKey(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                SerializeFilter();
                Hide();
            if (e.Key == Key.Return)
            {
                CmdSelectClick(cmdSelect, e);
                SerializeFilter();
                Hide();
            }
        }

        private void KBDown()
        {
            KeyEventArgs args = new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0,
                                                 Key.Down);
            args.RoutedEvent = Keyboard.KeyDownEvent;
            InputManager.Current.ProcessInput(args);
            args.RoutedEvent = Keyboard.KeyDownEvent;
            InputManager.Current.ProcessInput(args);
        }

        private void CmdSelectClick(object sender, RoutedEventArgs e)
        {
            SerializeFilter();
            uRet = Convert.ToInt32(dataGrid1.SelectedValue);
            Hide();
        }

        private void MenuItemCloseClick(object sender, RoutedEventArgs e)
        {
            SerializeFilter();
            uRet = 0;
            Hide();
        }

        private void FillGrid()
        {
            _boW = new Waege();
            dataGrid1.SelectedValuePath = "PK";
            string cFeld1 = "";
            string cFeld2 = "";
            string cFeld3 = "";
            if (cbFelder1.SelectedValue == null)
                cFeld1 = "";
            else
            {
                cFeld1 = cbFelder1.SelectedValue.ToString();
            }

            if (cbFelder2.SelectedValue == null)
                cFeld2 = "";
            else
            {
                cFeld2 = cbFelder2.SelectedValue.ToString();
            }

            if (bisdatePicker.SelectedDate == null) // Das passiert beim Instanzierien der Maske. 
            {
                return;
            }

            if (cbFelder3.SelectedValue == null)
                cFeld3 = "";
            else
            {
                cFeld3 = cbFelder3.SelectedValue.ToString();
            }

            if (bisdatePicker.SelectedDate == null) // Das passiert beim Instanzierien der Maske. 
            {
                return;
            }

            if (vondatePicker.SelectedDate != null)
            {
               // var oWaegeListe = _boW.GetLSListe(vondatePicker.SelectedDate.Value, bisdatePicker.SelectedDate.Value);
                var oWaegeListe = _boW.GetLsListe(vondatePicker.SelectedDate.Value, bisdatePicker.SelectedDate.Value, cFeld1,
                                                 tbMatch1.Text, cFeld2, tbMatch2.Text, cFeld3, tbMatch3.Text);
                dataGrid1.ItemsSource = oWaegeListe;

                decimal? summe = 0;
                var count = oWaegeListe.Count;
                for (int i = 0; i < count; i++)
                {
                    summe = summe + oWaegeListe[i].Nettogewicht;
                }
                tbSumme.Text = summe.ToString();
            }
            // L&L
        }

        protected override void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.HideCloseButton();
            dataGrid1.SelectedValuePath = "PK";
            //((IEditableCollectionView) CollectionViewSource.GetDefaultView(dataGrid1.ItemsSource)).
            //    NewItemPlaceholderPosition = NewItemPlaceholderPosition.None;
            dataGrid1.Focus();
            KBDown();
            KBDown();
        }

        // Lieferschein
        private void CmdPrintClick(object sender, RoutedEventArgs e)
        {
            uRet = Convert.ToInt32(dataGrid1.SelectedValue);
            Waege boW = new Waege();
            WaegeEntity boWE = boW.GetWaegungByPk(uRet);
            Lokaleeinstellungen oLE = new Lokaleeinstellungen();
            oLE = oLE.Load();
            PrinterLs oPLS = new PrinterLs();
            oPLS.DoPrintLs(oLE, boWE, true);
        }

        private void VondatePickerSelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            FillGrid();
        }

        private void BisdatePickerSelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            FillGrid();
        }

        private void CmdFilterOffClick(object sender, RoutedEventArgs e)
        {
            vondatePicker.SelectedDate = DateTime.Today;
            bisdatePicker.SelectedDate = DateTime.Today;
            cbFelder1.SelectedValue = null;
            tbMatch1.Text = "";
            cbFelder2.SelectedValue = null;
            tbMatch2.Text = "";
            cbFelder3.SelectedValue = null;
            tbMatch3.Text = "";
            FillGrid();
        }

        private void CbFilterOnClick(object sender, RoutedEventArgs e)
        {
            FillGrid();
        }

        private void PartialSetGrid()
        {
            // Die siebte Spalte = Nettogewicht muss zur Laufzeit geändert werden
            DataGridTextColumn gc7 = new DataGridTextColumn(); // Column erzeugen 
            Binding bindingGc7 = new Binding("Nettogewicht"); // Binding erzeugen 

            if (goApp.MengenEinheit == "t")
                bindingGc7.StringFormat = "F2";

            if (goApp.MengenEinheit == "kg")
                bindingGc7.StringFormat = "F0";

            gc7.Header = "Netto in " + goApp.MengenEinheit; // Header beschriften
            gc7.Binding = bindingGc7; // Binding mit Column verbinden
            dataGrid1.Columns.Add(gc7);
            gc7.Width = 100;
        }

        private void DataGrid1_OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            uRet = Convert.ToInt32(dataGrid1.SelectedValue);
            Waege boW = new Waege();

            WaegeEntity boWE = boW.GetWaegungByPk(uRet);
            if (boWE != null)
            {
                if (boWE.taab != true)
                    if (
                        MessageBox.Show("Auftragsnummer entfernen um diesen Datensatz als initierten Auftrag zu senden?", "Frage", MessageBoxButton.YesNo,
                            MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        boWE.identifierOItem = null;
                        boWE.number = null;
                        boWE.sequence = null;
                        boW.SaveEntity(boWE);
                    }

            }
        }

        private void SerializeFilter()
        {
            FilterObject oFO = new FilterObject();
            oFO.cb1 = cbFelder1.Text;
            oFO.cb2 = cbFelder2.Text;
            oFO.cb3 = cbFelder3.Text;

            oFO.mc1 = tbMatch1.Text;
            oFO.mc2 = tbMatch2.Text;
            oFO.mc3 = tbMatch3.Text;

            oFO.DatumVon = vondatePicker.DisplayDate;
            oFO.DatumBis = bisdatePicker.DisplayDate;
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream("MyFile.bin", FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, oFO);
            stream.Close();
        }

        private void DeserializeFilter()
        {
            
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream("MyFile.bin", FileMode.Open, FileAccess.Read, FileShare.Read);
            FilterObject oFo = (FilterObject)formatter.Deserialize(stream);
            stream.Close();

            cbFelder1.Text = oFo.cb1;
            cbFelder2.Text = oFo.cb2;
            cbFelder3.Text = oFo.cb3;

            tbMatch1.Text = oFo.mc1;
            tbMatch2.Text = oFo.mc2;
            tbMatch3.Text = oFo.mc3;

            vondatePicker.SelectedDate = oFo.DatumVon;
            bisdatePicker.SelectedDate = oFo.DatumBis;
        }
    }
    [Serializable]
    public class FilterObject
    {
        
        
        public string cb1 = "";
        public string cb2 = "";
        public string cb3 = "";

        public string mc1 = "";
        public string mc2 = "";
        public string mc3 = "";

        public DateTime DatumVon = System.DateTime.Now;
        public DateTime DatumBis = System.DateTime.Now; 
    }

}