using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using combit.ListLabel17;
using HWB.NETSCALE.BOEF;
using HWB.NETSCALE.GLOBAL;
using OakLeaf.MM.Main;
using OakLeaf.MM.Main.Collections;
using OakLeaf.MM.Main.WPF;


namespace HWB.NETSCALE.FRONTEND.WPF.Forms
{
    /// <summary>
    /// WiegelisteFrm Class
    /// </summary>
    public partial class WiegelisteFrm : mmBusinessWindow
    {
        /// <summary>
        /// Constructor
        /// </summary>
        private Waege boW;


        public int uRet;

        public WiegelisteFrm()
        {
            this.InitializeComponent();

            boW = new Waege();
            vondatePicker.SelectedDate = System.DateTime.Now;
            bisdatePicker.SelectedDate = System.DateTime.Now;
            cbFelder1.Items.Add("Kunde / Lieferant");
            cbFelder1.Items.Add("Kfz");
            cbFelder1.Items.Add("Sorten-Nr.");
            cbFelder1.Items.Add("Sortenbezeichnung 1");
            cbFelder1.Items.Add("Baustellenbezeichnung 1");

            cbFelder2.Items.Add("Kunde / Lieferant");
            cbFelder2.Items.Add("Kfz");
            cbFelder2.Items.Add("Sorten-Nr.");
            cbFelder2.Items.Add("Sortenbezeichnung 1");
            cbFelder2.Items.Add("Baustellenbezeichnung 1");

            cbFelder3.Items.Add("Kunde / Lieferant");
            cbFelder3.Items.Add("Kfz");
            cbFelder3.Items.Add("Sorten-Nr.");
            cbFelder3.Items.Add("Sortenbezeichnung 1");
            cbFelder3.Items.Add("Baustellenbezeichnung 1");

            cbEditls.Items.Add("LS-Liste Allgemein");
            cbEditls.Items.Add("LS-Liste Kunde");
            cbEditls.Items.Add("LS-Liste Kfz");
            if(goApp.acessLevel=="1")
                ControlGroup.Visibility = Visibility.Visible;

            else
            {
                ControlGroup.Visibility = Visibility.Collapsed;

            }
            PartialSetGrid();
            FillGrid();
            this.PreviewKeyDown += new KeyEventHandler(HandleKey);
        }

        private void HandleKey(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Hide();
            if (e.Key == Key.Return)
            {
                cmdSelect_Click(cmdSelect, e);
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

        private void cmdSelect_Click(object sender, RoutedEventArgs e)
        {
            uRet = Convert.ToInt32(dataGrid1.SelectedValue);
            this.Hide();
        }

        private void MenuItemClose_Click(object sender, RoutedEventArgs e)
        {
            uRet = 0;
            Hide();
        }

        private void FillGrid()
        {
            boW = new Waege();
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

            var oWaegeliste = boW.GetLSListe(vondatePicker.SelectedDate.Value, bisdatePicker.SelectedDate.Value, cFeld1,
                                             tbMatch1.Text,cFeld2,tbMatch2.Text,cFeld3,tbMatch3.Text);
            dataGrid1.ItemsSource = oWaegeliste;

            decimal?  summe = 0;
            var count = oWaegeliste.Count;
            for (int i = 0; i < count;i++)
            {
                summe = summe + oWaegeliste[i].Nettogewicht;
            }
            tbSumme.Text = summe.ToString();
            // L&L

        }


        protected override void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowExtensions.HideCloseButton(this);
            dataGrid1.SelectedValuePath = "PK";
            ((IEditableCollectionView) CollectionViewSource.GetDefaultView(dataGrid1.ItemsSource)).
                NewItemPlaceholderPosition = NewItemPlaceholderPosition.None;
            dataGrid1.Focus();
            KBDown();
            KBDown();
        }

        // Lieferschein
        private void cmdPrint_Click(object sender, RoutedEventArgs e)
        {
            uRet = Convert.ToInt32(dataGrid1.SelectedValue);
            Waege boW = new Waege();
            WaegeEntity boWE = boW.GetWaegungByID(uRet);
            Lokaleeinstellungen oLE = new Lokaleeinstellungen();
            oLE = oLE.Load();
            PrinterLS oPLS = new PrinterLS();
            oPLS.DoPrintLS(oLE, boWE, true);
        }

        // Wiegeliste Allgemein
        private void cmdPrintListe_Click(object sender, RoutedEventArgs e)
        {
            Lokaleeinstellungen oLE = new Lokaleeinstellungen();
            oLE = oLE.Load();

            ListLabel LL = new ListLabel();
            LL.LicensingInfo = "wOGzEQ";

            boW = new Waege();
            dataGrid1.SelectedValuePath = "PK";
            string cFeld1 = "";
            if (cbFelder1.SelectedValue == null)
                cFeld1 = "";
            else
            {
                cFeld1 = cbFelder1.SelectedValue.ToString();
            }

            string cFeld2 = "";
            if (cbFelder2.SelectedValue == null)
                cFeld2 = "";
            else
            {
                cFeld2 = cbFelder2.SelectedValue.ToString();
            }

               string cFeld3 = "";
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
            var oWaegeliste = boW.GetLSListe(vondatePicker.SelectedDate.Value, bisdatePicker.SelectedDate.Value, cFeld1,
                                             tbMatch1.Text,cFeld2,tbMatch2.Text,cFeld3,tbMatch3.Text);


            LL.DataSource = oWaegeliste;
            LL.AutoProjectType = LlProject.List;
            LL.AutoProjectFile = oLE.WIEGELISTFILE ;
            LL.AutoShowSelectFile = false;
            LL.AutoShowPrintOptions = false;
            try
            { 
                LL.Print(oLE.LISTENDRUCKER);
            }
            catch (ListLabelException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
       

        private void cmdEditListe_Click(object sender, RoutedEventArgs e)
        {
            openLsFile();
        }

        private void openLsFile()
        {
           
            mmBindingList<WaegeEntity> oWL;
             ListLabel LL = new ListLabel();
            LL.LicensingInfo = "wOGzEQ";
   
        
         
                    LL.AutoProjectFile = "*allgemein*.lst";
                    oWL = boW.GetLSListe(vondatePicker.SelectedDate.Value, bisdatePicker.SelectedDate.Value, "","","","","","");
                
              
              
         
            
             
            LL.DataSource = oWL;
            
            try
            {
                
                LL.Design();
            }
            catch (ListLabelException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void vondatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            FillGrid();
        }

        private void bisdatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            FillGrid();
        }

        private void cmdFilterOff_Click(object sender, RoutedEventArgs e)
        {
            vondatePicker.SelectedDate = System.DateTime.Today;
            bisdatePicker.SelectedDate = System.DateTime.Today;
            cbFelder1.SelectedValue = null;
            tbMatch1.Text = "";
            cbFelder2.SelectedValue = null;
            tbMatch2.Text = "";
              cbFelder3.SelectedValue = null;
            tbMatch3.Text = "";
            FillGrid();
        }

        private void cbFilterOn_Click(object sender, RoutedEventArgs e)
        {
            FillGrid();
        }

        private void PartialSetGrid()
        {
            // Die sechste Spalte = Nettogewicht muss zur Laufzeit geändert werden
            DataGridTextColumn GC6 = new DataGridTextColumn(); // Column erzeugen 
            Binding BindingGC6 = new Binding("Nettogewicht"); // Binding erzeugen 

            if (goApp.MengenEinheit == "t")
                BindingGC6.StringFormat = "F2";

            if (goApp.MengenEinheit == "kg")
                BindingGC6.StringFormat = "F0";

            GC6.Header = "Netto in " + goApp.MengenEinheit; // Header beschriften
            GC6.Binding = BindingGC6; // Binding mit Column verbinden
            this.dataGrid1.Columns.Add(GC6);
            GC6.Width = 100;
        }

       

        
       
    }
}