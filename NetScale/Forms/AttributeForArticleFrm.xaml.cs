using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using HWB.NETSCALE.BOEF;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace HWB.NETSCALE.FRONTEND.WPF.Forms
{
    /// <summary>
    /// AttributeForArticleFrm Class
    /// </summary>
    public partial class AttributeForArticleFrm
    {
   

        /// <summary>
        /// Die ist entweder die vom Orderitemservice oder die aus dem Artikelstamm
        /// lOrder legt fest ob OI oder Artikelstamm
        /// </summary>
        private readonly WaegeEntity _boWe;


        public AttributeForArticleFrm(string id, WaegeEntity boWe)

        {
        
            _boWe = boWe;
            InitializeComponent();
            GetAttrForArticle(id);
            if (boWe.attributes_as_json != null)
            {
                GetAttrValuesFromWaege();
            }


            // Automatically resize height and width relative to content
            SizeToContent = SizeToContent.WidthAndHeight;
        }

   

        private void GetAttrForArticle(string id)
        {
            var oA = new Artikel();
            var oAe = oA.GetById(id);
            if (oAe != null)
            {
                var json = oAe.attributes_as_json;


                var array = (JArray) JsonConvert.DeserializeObject(json);
                var attributList = array.ToList<object>();

                foreach (var t in attributList)
                {
                    stackPanel1.Children.Add(CreateStackPanel(t.ToString()));
                    stackPanel1.Height = double.NaN;
                    Height = double.NaN;
                }
            }
        }

        private void GetAttrValuesFromWaege()
        {
            try
            {
                var obj = JObject.Parse(_boWe.attributes_as_json);
                foreach (var pair in obj)
                {
                    var propName = pair.Key;
                    var propValue = pair.Value.ToString();

                    var t = FindName("txt" + propName);
                    if (t != null)
                    {
                        if (t.GetType() == typeof (TextBox))
                        {
                            ((TextBox) t).Text = propValue;
                        }
                    }
                }
            }
            catch (Exception e)
            {
              // MessageBox.Show(e.Message);
            }
        }

        private Label CreateLabel(string labelName)
        {
            var lbl = new Label
            {
                Name = "lbl" + labelName,
                Width = 250,
                Content = labelName,
                Margin = new Thickness(10)
            };
            return lbl;
        }

        private TextBox CreateTextBox(string attributName)
        {
            var txt = new TextBox
            {
                Name = "txt" + attributName,
                Width = 250,
                Margin = new Thickness(10)
            };
            RegisterName(txt.Name, txt);
            return txt;
        }

        private StackPanel CreateStackPanel(string attributName)
        {
            var sp = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Name = "sp" + attributName
            };

            sp.Children.Add(CreateLabel(attributName));
            sp.Children.Add(CreateTextBox(attributName));

            return sp;
        }


        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            WriteJsonToWaege();
        }

        private void WriteJsonToWaege()
        {
            var jsonObject_Start = "{";
            var jsonObject_End = "}";
            string jsonData = null;

            foreach (var tb in FindVisualChildren<TextBox>(this))
            {
                if (!String.IsNullOrWhiteSpace(tb.Text))
                {
                    if (!String.IsNullOrEmpty(tb.Text))
                    {
                        if (jsonData == null) // Dann Komma
                        {
                            jsonData = jsonData + tb.Name + ":" + '\u0022' + tb.Text + '\u0022' ;
                        }
                        else
                        {
                            jsonData = jsonData +","+ tb.Name + ":" + '\u0022' + tb.Text + '\u0022' ;
                        }
                    }
                }
            }
            // Entferne das rechte Komma
         
            _boWe.attributes_as_json = jsonData != null ? (jsonObject_Start + jsonData + jsonObject_End).Replace("txt", "") : null;
            Hide();
        }

        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (var i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    var child = VisualTreeHelper.GetChild(depObj, i);
                    var children = child as T;
                    if (children != null)
                    {
                        yield return children;
                    }

                    foreach (var childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        protected override void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.HideCloseButton();
        }
    }
}