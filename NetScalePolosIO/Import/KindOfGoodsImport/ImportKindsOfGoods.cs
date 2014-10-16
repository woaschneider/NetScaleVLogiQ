using System;
using HWB.NETSCALE.BOEF;
using Xceed.Wpf.Toolkit;

// Imports Kind of Goods nach Warenarten

namespace HWB.NETSCALE.POLOSIO.KindOfGoodsImport
{
    public class ImportKindsOfGoods
    {
        private Warenarten boW;
        private WarenartenEntity boWE;

        public bool Import(string FullQualifiedFileName)
        {
            try
            {
                var oK =
                    FullQualifiedFileName.CreateFromJsonFile<KindOfGoodsImportRootObject>();
                boW = new Warenarten();

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
                MessageBox.Show(e.Message);
            }


            return true;
        }
    }
}