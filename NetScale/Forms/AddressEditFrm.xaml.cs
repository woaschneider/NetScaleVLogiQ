using System;
using System.ComponentModel;
using System.Drawing.Printing;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using HWB.NETSCALE.BOEF;

using NetScaleGlobal;
using OakLeaf.MM.Main;
using OakLeaf.MM.Main.WPF;

namespace HWB.NETSCALE.FRONTEND.WPF.Forms
{
    /// <summary>
    /// AddressEditFrm Class
    /// </summary>
    public partial class AddressEditFrm : mmBusinessWindow
    {
        /// <summary>
        /// Constructor
        /// </summary>
        private Adressen boAddress;

        private AdressenEntity boAPE;

        public AddressEditFrm(int PK, bool New, string _rolle)
        {
            boAddress = new Adressen();
            this.InitializeComponent();
            PopulateInstalledPrintersCombo();

            if (New == true)
            {
                this.Title = Partnerrollen.GetRollenBezeichnung(_rolle) + " - Neue Adresse anlegen";
                boAPE = boAddress.NewEntity();


            }
            else
            {
                this.Title = Partnerrollen.GetRollenBezeichnung(_rolle) + " - Adresse bearbeiten";
                //   this.Title = "Empfänger bearbeiten!";
               boAPE = boAddress.GetByPk(PK);
            }
            this.DataContext = boAPE;
            this.PreviewKeyDown += new KeyEventHandler(HandleKey);

      
        }

        private void HandleKey(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Hide();
        }

        private void MenuItemClose_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            tb_CatchFocus.Focus();
            // Focus auf eine nicht sichtbare Textbox setzen. Sonst werden die Änderungen an der letzen TB nicht übernommen. Trick!
            boAddress.SaveEntity(boAPE);
            this.Hide();

        }

        private void cmdDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult uRet = MessageBox.Show("Wollen Sie diesen Datensatzwirklich löschen? ", "ACHTUNG",
                                                    MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (uRet == MessageBoxResult.Yes)
            {
                boAddress.DeleteEntity();
                this.Hide();
            }
        }

        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            boAddress.Cancel();
            this.Hide();
        }

        protected override void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowExtensions.HideCloseButton(this);
        }

        private void ButtonLsVorlage_OnClick(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog

            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();


            // Set filter for file extension and default file extension
            dlg.DefaultExt = ".lbl";

            dlg.Filter = "Text documents (.lbl)|*.lbl";


            // Display OpenFileDialog by calling ShowDialog method

            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox

            if (result == true)
            {
                // Open document

                string filename = dlg.FileName;

                boAPE.Lieferscheinvorlage = filename;
            
               
            }
        }
        private void PopulateInstalledPrintersCombo()
        {
      
            // Add list of installed printers found to the combo box.
            // The pkInstalledPrinters string will be used to provide the display string.
            String pkInstalledPrinters;
            for (int i = 0; i < PrinterSettings.InstalledPrinters.Count; i++)
            {
                pkInstalledPrinters = PrinterSettings.InstalledPrinters[i];

                combo_LISTE_InstalledPrinters.Items.Add(pkInstalledPrinters);
            }
          
        }
    }
}