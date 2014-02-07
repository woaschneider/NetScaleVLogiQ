using System;
using System.Windows;
using System.Windows.Input;
using HWB.NETSCALE.BOEF;

namespace HWB.NETSCALE.FRONTEND.WPF.Forms
{
    /// <summary>
    /// AbruflisteFrm Class
    /// </summary>
    public partial class AbruflisteFrm
    {
        /// <summary>
        /// Constructor
        /// </summary>
        private Abruf boAbruf;

        public int URet;

        public AbruflisteFrm()
        {
            boAbruf = new Abruf();
            InitializeComponent();
            dataGrid1.SelectedValuePath = "PK";
            dataGrid1.ItemsSource = boAbruf.GetAllAbruf();

            PreviewKeyDown += HandleKey;
        }

        private void HandleKey(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Hide();
            if (e.Key == Key.Return)
            {
                CmdSelectClick(cmdSelect, e);
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


        private void MenuItemCloseClick(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void CmdSelectClick(object sender, RoutedEventArgs e)
        {
            URet = Convert.ToInt32(dataGrid1.SelectedValue);
            Hide();
        }

        private new void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (dataGrid1.Items.Count > 0)
            {
                dataGrid1.SelectedItem = dataGrid1.Items[0];
                dataGrid1.Focus();
                KBDown();
                KBDown();
            }
            WindowExtensions.HideCloseButton(this);
        }
    }
}