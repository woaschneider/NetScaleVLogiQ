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
using System.Globalization;

namespace HWB.NETSCALE.FRONTEND.WPF
{
    /// <summary>
    /// UserWindow Class
    /// </summary>
    public partial class LanguageWindow : mmBusinessWindow
    {
        /// <summary>
        /// User business object
        /// </summary>
        //protected Language LanguageController;
        /// <summary>
        /// Constructor
        /// </summary>
        public LanguageWindow()
        {
            // Instantiate and register business objects
            //this.LanguageController = (Language)this.RegisterPrimaryBizObj(new Language());

            this.InitializeComponent();

            // Insert code required on object creation below this point.
            this.FocusOnCancelElement = this.btnNew;
            this.FocusOnLoadElement = this.btnNew;
            this.FocusOnSaveElement = this.btnNew;
            this.FocusOnNewElement = this.txtLanguage;

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
            //this.DataContext = this.LanguageController.GetAllEntities();

            CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
            this.cboCulture.ItemsSource = cultures;


            // Hide the new item row place holder in the Order Detail DataGrid
            ((IEditableCollectionView) CollectionViewSource.GetDefaultView(this.grdUsers.ItemsSource)).
                NewItemPlaceholderPosition =
                NewItemPlaceholderPosition.None;
        }
    }
}