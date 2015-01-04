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
using HWB.NETSCALE.BOEF;
using OakLeaf.MM.Main;
using OakLeaf.MM.Main.Collections;
using OakLeaf.MM.Main.WPF;
using YeomanExport;


namespace HWB.NETSCALE.FRONTEND.WPF.Forms
{
    /// <summary>
    /// ExportYeoman2XlsFrm Class
    /// </summary>
    public partial class ExportYeoman2XlsFrm : mmBusinessWindow
    {
        private mmBindingList<WaegeEntity> oTaabYeo;
        /// <summary>
        /// Constructor
        /// </summary>
        public ExportYeoman2XlsFrm()
        {
            this.InitializeComponent();
            this.PreviewKeyDown += new KeyEventHandler(HandleKey);
            datePicker.SelectedDate = DateTime.Today;
            txtAuftraggeber.Text = "YEOWIL";
        IniInfoBox();


        }
        private void HandleKey(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Hide();

        }
        private void cmdExportXls_Click(object sender, RoutedEventArgs e)
        {
            oTaabYeo = new Waege().GetYeomanTaabListe(datePicker.SelectedDate.Value, txtAuftraggeber.Text);
            WriteTaabToExcel oWTE = new WriteTaabToExcel();
            int uRet =  oWTE.Export2Xls(oTaabYeo);
            txtInfo.AppendText("Fertig: Es wurden "+ uRet.ToString()+ " Datensätze in die Exceltabelle geschrieben!" + " \r\n" );
        }

        private void MenuItemClose_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void IniInfoBox()
        {
            oTaabYeo = new Waege().GetYeomanTaabListe(datePicker.SelectedDate.Value, txtAuftraggeber.Text);
            if(oTaabYeo!=null)
            {
                txtInfo.AppendText("Anzahl der Datensätze die zum Export bereit stehen: " + oTaabYeo.Count.ToString()+ "\r\n");
            }
        }
    }
}