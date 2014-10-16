using System;
using HWB.NETSCALE.BOEF;
using Xceed.Wpf.Toolkit;

namespace HWB.NETSCALE.POLOSIO.ProductsImport
{
    public class ImportProducts
    {
        private Produkte boP;
        private ProdukteEntity boPE;

        public bool Import(string FullQualifiedFileName)
        {
            try
            {
                var oP =
                    FullQualifiedFileName.CreateFromJsonFile<ProduktRootObject>();
                boP = new Produkte();

                foreach (Product obj in oP.products)
                {
                    if (obj.id != null)
                    {
                        boPE = boP.GetById(obj.id);
                    }
                    if (boPE == null)
                    {
                        boPE = boP.NewEntity();
                    }
                    boPE.id = obj.id;
                    boPE.description = obj.description;
                    boPE.shortdescirption = obj.description;
                    boP.SaveEntity(boPE);
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