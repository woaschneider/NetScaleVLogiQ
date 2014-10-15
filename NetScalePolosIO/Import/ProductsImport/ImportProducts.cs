using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HWB.NETSCALE.BOEF;
using Microsoft.Windows.Controls;

namespace HWB.NETSCALE.FRONTEND.WPF.Import.ProductsImport
{
    public class ImportProducts
    {
        private Produkte boP;
        private ProdukteEntity boPE;

        public bool Import(string FullQualifiedFileName)
        {
            try
            {

                ProduktRootObject oP =
                    FullQualifiedFileName.CreateFromJsonFile<ProduktRootObject>();
               boP = new Produkte(); 

                foreach (Product obj in oP.products)
                {
                    if (obj.id != null)
                    {
                        boPE = boP.GetById(obj.id);
                    }
                    if(boPE==null)
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
                MessageBox.Show(e.Message.ToString());
            }


            return true;
        }
    }
    
}
