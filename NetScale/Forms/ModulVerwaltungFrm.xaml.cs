using System.Windows;
using System.Windows.Controls;
using HWB.NETSCALE.BOEF;
using OakLeaf.MM.Main.WPF;

namespace HWB.NETSCALE.FRONTEND.WPF.Forms
{
    /// <summary>
    /// ModulVerwaltungFrm Class
    /// </summary>
    public partial class ModulVerwaltungFrm : mmBusinessWindow
    {
        /// <summary>
        /// Constructor
        /// </summary>
        private HardewareInfo oHI = new HardewareInfo();

        private NETSCALE.BOEF.Lokaleeinstellungen oLE;


        public ModulVerwaltungFrm()
        {
            this.InitializeComponent();
            HardewareInfo oHI = new HardewareInfo();
            tb_ser.Text = oHI.GetVolumeSerialNumber2();
            oLE = new Lokaleeinstellungen();
            oLE = oLE.Load();
            FillFrm();
        }

        private void FillFrm()
        {
            tb_WaegemodulFreischaltcode.Text = oLE.LI_WAAGE;
            tb_FunkmodulFreischaltcode.Text = oLE.LI_FUNK;
            tb_KartenlesermodulFreischaltcode.Text = oLE.LI_KARTEN;
            tb_FernanzeigeFreischaltcode.Text = oLE.LI_FERNANZEIGE;
        }

        private void MenuItemClose_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            oLE.LI_WAAGE = tb_WaegemodulFreischaltcode.Text;
            oLE.LI_FUNK = tb_FunkmodulFreischaltcode.Text;
            oLE.LI_KARTEN = tb_KartenlesermodulFreischaltcode.Text;
            oLE.LI_FERNANZEIGE = tb_FernanzeigeFreischaltcode.Text;
            oLE.Save(oLE);
        }

        private void TbWaegemodulFreischaltcodeTextChanged(object sender, TextChangedEventArgs e)
        {
            if (Md5Stuff.VerifyMd5Hash(tb_ser.Text + "W1789A", tb_WaegemodulFreischaltcode.Text) == true)
            {
                cb_modulWaege.IsChecked = true;
            }
            else
            {
                cb_modulWaege.IsChecked = false;
            }
        }

        private void TbFunkmodulFreischaltcodeTextChanged(object sender, TextChangedEventArgs e)
        {
            cb_Funkmodul.IsChecked = Md5Stuff.VerifyMd5Hash(tb_ser.Text + "F0687B", tb_FunkmodulFreischaltcode.Text);
        }

        private void TbKartenlesermodulFreischaltcodeTextChanged(object sender, TextChangedEventArgs e)
        {
            cb_Kartenleser.IsChecked = Md5Stuff.VerifyMd5Hash(tb_ser.Text + "K9785C",
                                                              tb_KartenlesermodulFreischaltcode.Text);
        }

        protected override void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowExtensions.HideCloseButton(this);
        }

        private void tb_FernanzeigeFreischaltcode_TextChanged(object sender, TextChangedEventArgs e)
        {
            cb_Fernanzeige.IsChecked = Md5Stuff.VerifyMd5Hash(tb_ser.Text + "FERN523D",
                                                              tb_FernanzeigeFreischaltcode.Text);
        }
    }
}