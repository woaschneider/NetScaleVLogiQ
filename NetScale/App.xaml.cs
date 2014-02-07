using System;
using System.Windows;
using System.Data;
using System.Windows.Controls;
using System.Xml;
using System.Configuration;

namespace HWB.NETSCALE.FRONTEND.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            EventManager.RegisterClassHandler(typeof (TextBox),
                                              TextBox.GotFocusEvent,
                                              new RoutedEventHandler(TextBox_GotFocus)
                );
            base.OnStartup(e);
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            tb.Focus();
            tb.SelectAll();
        }
    }
}