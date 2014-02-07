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
using HWB.NETSCALE.BOEF;
using HWB.NETSCALE.BOEF.User;
using OakLeaf.MM.Main;
using OakLeaf.MM.Main.WPF;

namespace HWB.NETSCALE.FRONTEND.WPF
{
    /// <summary>
    /// UserWindow Class
    /// </summary>
    public partial class UserWindow : mmBusinessWindow
    {
        /// <summary>
        /// User business object
        /// </summary>
        protected User User;

        /// <summary>
        /// Constructor
        /// </summary>
        public UserWindow()
        {
            // Instantiate and register business objects
            this.User = (User) this.RegisterPrimaryBizObj(new User());

            this.InitializeComponent();

            // Insert code required on object creation below this point.
            this.FocusOnCancelElement = this.btnNew;
            this.FocusOnLoadElement = this.btnNew;
            this.FocusOnSaveElement = this.btnNew;
            this.FocusOnNewElement = this.txtFirstName;

            this.NavigationControl = this.grdUsers;
        }

        /// <summary>
        /// Retrieves the user entities
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void Window_Loaded(object sender, RoutedEventArgs e)
        {
            base.Window_Loaded(sender, e);

            // Get the user entities
            this.DataContext = this.User.GetAllEntities();

            // Hide the new item row place holder in the Order Detail DataGrid
            //   ((IEditableCollectionView)CollectionViewSource.GetDefaultView(this.grdUsers.ItemsSource)).NewItemPlaceholderPosition =
            //     NewItemPlaceholderPosition.None;
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}