using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.ServiceModel.Configuration;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using HWB.NETSCALE.BOEF;

using HWB.NETSCALE.POLOSIO.ArticleImport;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OakLeaf.MM.Main;
using OakLeaf.MM.Main.WPF;



namespace HWB.NETSCALE.FRONTEND.WPF.Forms
{
    /// <summary>
    /// AttributeForArticleFrm Class
    /// </summary>
    public partial class AttributeForArticleFrm : mmBusinessWindow
    {
        /// <summary>
        /// Die ist entweder die vom Orderitemservice oder die aus dem Artikelstamm
        /// lOrder legt fest ob OI oder Artikelstamm
        /// </summary>
        private WaegeEntity _boWe;

    
        public AttributeForArticleFrm(string id, bool lOrder ,WaegeEntity boWe)

        {
            _boWe = boWe; 
            this.InitializeComponent();
            GetAttrForArticle(id);
            if (boWe.attributes_as_json != null)
            {
              GetAttrValuesFromWaege();
            }
        }

        private void GetAttrForArticle( string id )
        {
            Artikel oA = new Artikel();
            ArtikelEntity oAe = oA.GetById(id);
            if (oAe != null)
            {
                string json = oAe.attributes_as_json;


                var array = (JArray)JsonConvert.DeserializeObject(json);
                List<object> attributList = array.ToList<object>();
              //  rO = new RootObject();

                // 

                for (int i = 0; i < attributList.Count; i++)
                {
                    stackPanel1.Children.Add(CreateStackPanel(attributList[i].ToString()));
                    stackPanel1.Height = double.NaN;
                    this.Height = double.NaN;
                }             
            }

          
        }

         private void GetAttrValuesFromWaege()
        {
             try
             {
          
            
                 var attrobj = JsonConvert.DeserializeObject<RootObject>(_boWe.attributes_as_json);
                 var PropertyInfos = attrobj.attributes.GetType().GetProperties();
                 foreach (PropertyInfo prop in PropertyInfos)
                 {
                     string propName = prop.Name;
                     object propValue = prop.GetValue(attrobj.attributes, null);
                     if (propValue != null)
                     {
                         object t = this.FindName("txt" + propName);
                         if (t != null)
                         {
                             if (t.GetType() == typeof (TextBox))
                             {
                                 ((TextBox) t).Text = propValue.ToString();
                             }
                         }

                     }
                 }
             }
             catch (Exception e)
             {
                 MessageBox.Show(e.Message);

             }
           
        }

        private Label CreateLabel(string labelName)
        {
            Label lbl = new Label();
            lbl.Name = "lbl" + labelName;
            lbl.Width = 250;
            lbl.Content = labelName;
            lbl.Margin = new Thickness(10);
            return lbl;
        }

        private TextBox CreateTextBox(string attributName)
        {
            TextBox txt = new TextBox();
            txt.Name = "txt" + attributName;
            txt.Width = 150;
            txt.Margin = new Thickness(10);
            RegisterName(txt.Name,txt);
            return txt;
        }

        private StackPanel CreateStackPanel(string attributName )
        {
            StackPanel sp = new StackPanel();
            sp.Orientation= Orientation.Horizontal;
            sp.Name = "sp" + attributName;

            sp.Children.Add(CreateLabel(attributName));
            sp.Children.Add(CreateTextBox(attributName));

            return sp;
        }

        public class Attributes
        {
            //public Attributes(List<object> lo)
            //{
            //    if (lo != null)
            //    {
            //        for (int i = 0; i < lo.Count; i++)
            //        {

            //            string s = lo[i].ToString();

            //            var x = new ExpandoObject() as IDictionary<string, Object>;
            //            x.Add(s, string.Empty);

            //        }
            //    }
            //}

            public string SERIAL_NUMBER { get; set; }
            public string BATCH { get; set; }
            public string ORIGIN { get; set; }
            public string GRADE { get; set; }
            public string ORIGINAL_NUMBER { get; set; }
            public string LENGTH { get; set; }
            public string WIDTH { get; set; }
            public string HEIGHT { get; set; }
            public string STORAGE_AREA_REFERENCE { get; set; }
            public string DIAMETER { get; set; }
            public string ORIGINAL_MARKING { get; set; }
            public string STORAGE_AREA_REFERENCE_NUMBER { get; set; }
            public string DIMENSION { get; set; }
        }

        public class RootObject
        {

            public Attributes attributes { get; set; }
            public RootObject()
            {
                attributes = new Attributes();
            }


        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            WriteJsonToWaege();
        }

        private void WriteJsonToWaege()
        {
            string jsonObject_Start = "{attributes:{";
            string jsonObject_End = "}}";
            string jsonData = null;
         
            foreach (TextBox tb in FindVisualChildren<TextBox>(this))
            {
                if (!String.IsNullOrWhiteSpace(tb.Text))
                {
                    jsonData = jsonData + tb.Name + ":" + '\u0022' + tb.Text + '\u0022'+ ",";
                    this.Hide();
                }
            }

            if (jsonData != null)
            {
                _boWe.attributes_as_json = (jsonObject_Start + jsonData + jsonObject_End).Replace("txt","");
            }

            else
            {
                _boWe.attributes_as_json = null;
            }
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }
    }
}