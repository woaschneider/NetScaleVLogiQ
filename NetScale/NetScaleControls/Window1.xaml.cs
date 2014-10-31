
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace HWB.NETSCALE.FRONTEND.WPF.NetScaleControls
{
    /// <summary>
    /// Interaktionslogik für Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }
    }

    public class ImageButton : Button
    {
        public ImageSource Source
        {
            get { return base.GetValue(SourceProperty) as ImageSource; }
            set { base.SetValue(SourceProperty, value); }
        }
        public static readonly DependencyProperty SourceProperty =
          DependencyProperty.Register("Source", typeof(ImageSource), typeof(ImageButton));
    }
}
