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
using HWB.NETSCALE.BOEF.JoinClasses;
using OakLeaf.MM.Main;
using OakLeaf.MM.Main.WPF;


namespace HWB.NETSCALE.FRONTEND.WPF.Forms
{


    /// <summary>
    /// AuftragEditFrm Class
    /// </summary>
    public partial class AuftragEditFrm : mmBusinessWindow
    {
        public int uRet;
        private int kk_pk;
   
    //    private Auftragsdetailliste boADLE;
        /// <summary>
        /// Constructor
        /// </summary>
        public AuftragEditFrm(int kkpk)
        {
            kk_pk = kkpk;
            this.InitializeComponent();
       
           

          

            // 
            // Nichts in der Liste. Liste gleich schließen.
            
           
            this.PreviewKeyDown += new KeyEventHandler(HandleKey);
          


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
        private void HandleKey(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Hide();
                e.Handled = true;
            }
            if (e.Key == Key.Return)
            {

                cmdSelect_Click(cmdSelect, e);
                Hide();
                e.Handled = true;
            }
        }

        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
        }

        private void cmdDelete_Click(object sender, RoutedEventArgs e)
        {
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
        }

        private void cmdSelect_Click(object sender, RoutedEventArgs e)
        {
            uRet = Convert.ToInt32(dataGrid1.SelectedValue);
            this.Hide();

        }

        private void MenuItemClose_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

       new private  void  Window_Loaded(object sender, RoutedEventArgs e)
        {
            dataGrid1.SelectedItem = dataGrid1.Items[0];
            dataGrid1.Focus();

            KBDown();
            KBDown();
            WindowExtensions.HideCloseButton(this);
        }

        private void cmdAddPos_Click(object sender, RoutedEventArgs e)
        {
       
                
  
            
  

        }
        private void  FillGrid(int kkpk)
        {  
       
         
        }
    }
}