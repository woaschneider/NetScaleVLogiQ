using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HWB.NETSCALE.BOEF;
using Microsoft.Windows.Controls;

namespace HWB.NETSCALE.FRONTEND.WPF.Import.KindOfGoodsImport
{
    class ImportKindsOfGoods
    {
        private HWB.NETSCALE.BOEF.Warenarten boW;
        private BOEF.Warenarten boWE;

        public bool Import(string FullQualifiedFileName)
        {
            try
            {

                KindOfGoodsImport.RootObject oK =
                    FullQualifiedFileName.CreateFromJsonFile<KindOfGoodsImport.RootObject>();
                boW = new BOEF.Warenarten();

                foreach (KindOfGood obj in oK.kindOfGoods)
                {
                    if (obj.id != null)
                    {
                        boWE = boW.GetById(obj.id);
                    }
                }


            }

            catch (Exception e)
            {
                MessageBox.Show(e.Message.ToString());
            }


            return true;
        }
    }
}