
using System.Globalization;

using System.Windows;
using System.Windows.Controls;

using System.Windows.Input;
using System.Windows.Markup;

using HWB.NETSCALE.BOEF;


using OakLeaf.MM.Main.Business;
using OakLeaf.MM.Main.WPF;

namespace HWB.NETSCALE.FRONTEND.WPF.Forms
{
    /// <summary>
    /// CFEditFrm Class
    /// </summary>
    public partial class CFEditFrm : mmBusinessWindow
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public string uRet;

        private Fahrzeuge _boCF = new Fahrzeuge();
        private FahrzeugeEntity _boCFE = new FahrzeugeEntity();

        // KfzID direkt in die Wägemaske zurückgeben, wenn Fahrzeug aus der Wägemaske neu angelegt wird.

     

        public CFEditFrm(int pk, bool New, string caption)
        {
            this.InitializeComponent();
        


            if (New == true)
            {
                this.Title = "Neues " + caption + " anlegen";
                _boCFE = _boCF.NewEntity();

            }
            else
            {
                this.Title = caption + " bearbeiten";
                _boCFE = _boCF.GetByPk(pk);
                uRet = null;
            }
            DataContext = _boCFE;

            this.Language = XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag);
      
            this.PreviewKeyDown += new KeyEventHandler(HandleKey);
        }

        private void HandleKey(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                uRet = null;


                Hide();
            }
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            tb_CatchFocus.Focus();
         mmSaveDataResult sdr =   _boCF.SaveEntity(_boCFE);
            // Focus auf eine nicht sichtbare Textbox setzen. Sonst werden die Änderungen an der letzen TB nicht übernommen. Trick!

        }

        private void cmdDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult uRet = MessageBox.Show("Wollen Sie diesen Datensatzwirklich löschen? ", "ACHTUNG",
                                                    MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (uRet == MessageBoxResult.Yes)
            {
                _boCF.DeleteEntity(_boCFE);
                this.Close();
            }
        }

        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            _boCF.CancelEntity(_boCFE);
            Hide();
        }

        private void MenuItemClose_Click(object sender, RoutedEventArgs e)
        {

            Close();
        }



  










        protected override void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowExtensions.HideCloseButton(this);
        }

        private void buttonLookUpFrachtmittel_Click(object sender, RoutedEventArgs e)
        {
            FrachtmittelListFrm oFFrm = new FrachtmittelListFrm();
            oFFrm.ShowDialog();
            int uRet = oFFrm.uRet;
            if(uRet!=0)
            {
                Frachtmittel _boF = new Frachtmittel();
                FrachtmittelEntity _boFE = _boF.GetFrachtmittelByPK(uRet);
                if(_boFE!=null)
                {
                   _boCFE.Kennung = _boFE.Kennung;
                   _boCFE.Bezeichnung = _boFE.Bezeichnung;
                }
            }
            oFFrm.Close();
        }

        private void buttonLookUpFrachtführer_Click(object sender, RoutedEventArgs e)
        {
            AdressenListeFrm oAFrm = new AdressenListeFrm();
            oAFrm.ShowDialog();
            int uRet = oAFrm.uRet;
            if(uRet != 0)
            {
                Adressen _boA = new Adressen();
                AdressenEntity _boAE = _boA.GetByPk(uRet);
                if(_boAE!=null)
                {
                    _boCFE.Frachtfuehrer = _boAE.businessIdentifier;
                }
            }
        }



    }
}