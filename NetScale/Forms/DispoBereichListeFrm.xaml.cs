using System;

using System.Windows;

using HWB.NETSCALE.BOEF;



namespace HWB.NETSCALE.FRONTEND.WPF.Forms
{
    /// <summary>
    /// DispoBereichListeFrm Class
    /// </summary>
    public partial class DispoBereichListeFrm
    {
        public int URet;

        /// <summary>
        /// Constructor
        /// </summary>
        public DispoBereichListeFrm()
        {
            InitializeComponent();
            // PrimaryBizObj = 
            var boP = new Planningdivision();
            dataGrid.ItemsSource = boP.GetAll();
            dataGrid.SelectedValuePath = "PK";
        }

        private void CmdSelect_OnClick(object sender, RoutedEventArgs e)
        {
            URet = Convert.ToInt32(dataGrid.SelectedValue);
            Hide();
        }

        private void MenuItemClose_OnClick(object sender, RoutedEventArgs e)
        {
       
        }

        private void CmdNeu_OnClick(object sender, RoutedEventArgs e)
        {
           
        }

        private void CmdEdit_OnClick(object sender, RoutedEventArgs e)
        {
       
        }

         void  Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.HideCloseButton();
        }
    }
}