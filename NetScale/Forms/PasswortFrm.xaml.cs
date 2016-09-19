using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HWB.NETSCALE.FRONTEND.WPF.Forms
{
    /// <summary>
    /// Interaktionslogik für PasswortFrm.xaml
    /// </summary>
    public partial class PasswortFrm : Window
    {
        public bool PWOk = false;
        public PasswortFrm()
        {
            InitializeComponent();
        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            if (passwordBox1.Password == "235711")
            {
                PWOk = true;
                this.Hide();
            }
            else
            {
                PWOk = false;
                this.Hide();
            }
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            PWOk = false;
            this.Hide();
        }
    }
}
