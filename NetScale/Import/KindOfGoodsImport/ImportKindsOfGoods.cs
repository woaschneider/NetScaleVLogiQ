using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HWB.NETSCALE.BOEF;
using Microsoft.Windows.Controls;

// Imports Kind of Goods nach Warenarten

namespace HWB.NETSCALE.FRONTEND.WPF.Import.KindOfGoodsImport
{
    public class ImportKindsOfGoods
    {
        private Warenarten boW;
        private WarenartenEntity boWE;

        public bool Import(string FullQualifiedFileName)
        {
            try
            {
                KindOfGoodsImport.KindOfGoodsImportRootObject oK =
                    FullQualifiedFileName.CreateFromJsonFile<KindOfGoodsImport.KindOfGoodsImportRootObject>();
                boW = new BOEF.Warenarten();

                foreach (KindOfGood obj in oK.kindOfGoods)
                {
                    if (obj.id != null)
                    {
                        boWE = boW.GetById(obj.id);
                    }
                    if (boWE == null)
                    {
                        boWE = boW.NewEntity();
                    }
                    boWE.id = obj.id;
                    boWE.description = obj.description;
                    boWE.baseunit_shortdescription = obj.baseUnit.shortDescription;
                    boW.SaveEntity(boWE);
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