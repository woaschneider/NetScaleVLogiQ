using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using OakLeaf.MM.Main;
using OakLeaf.MM.Main.WPF;

namespace HWB.NETSCALE.FRONTEND.WPF.Forms
{
    /// <summary>
    /// PortListFrm Class
    /// </summary>
    public partial class PortListFrm : mmBusinessWindow
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public string uRet;

        public PortListFrm()
        {
            this.InitializeComponent();
      // Entfernen ?     int  nCount = 0;
            string[] theSerialPortNames = System.IO.Ports.SerialPort.GetPortNames();
            foreach (object name in theSerialPortNames)
            {
                 listBox1.Items.Add((string) name);
              
            }

            // Die Einträge sortieren
            SortDescription sd = new SortDescription();
            sd.Direction = ListSortDirection.Ascending ;
            listBox1.Items.SortDescriptions.Add(sd);


        }

        private new void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void listBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            uRet = listBox1.SelectedValue.ToString();
        }

        private void cmdSelect_Click(object sender, RoutedEventArgs e)
        {
            uRet = listBox1.SelectedValue.ToString();
            this.Hide();
        }

        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            uRet = "";
            this.Hide();
        }

        private void cmdCancel_Click_1(object sender, RoutedEventArgs e)
        {
        }

        private void MenuItemClose_Click(object sender, RoutedEventArgs e)
        {
            uRet = "";
            this.Hide();
        }
    }
}