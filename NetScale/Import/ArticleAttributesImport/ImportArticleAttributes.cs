using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HWB.NETSCALE.BOEF;

namespace HWB.NETSCALE.FRONTEND.WPF.Import.ArticleAttributes
{
    class ImportArticleAttributes
    {
        private Artikelattribute  boA;
        private ArtikelattributeEntity boAE;
        public bool Import(string FullQualifiedFileName)
        {
            try
            {


                ArticleAttributesRootObject oA = FullQualifiedFileName.CreateFromJsonFile<ArticleAttributesRootObject>();

                boA = new Artikelattribute();
                for (int i = 0; i < oA.articleAttributes.Count; i++)
                {
                    boAE = boA.GetArtikelAttributByBezeichnung(oA.articleAttributes[i].ToString());
                    if(boAE==null)
                    {
                      boAE=   boA.NewEntity();

                    }
                    if(boAE!=null)
                    {
                        boAE.AttributName = oA.articleAttributes[i].ToString();
                        boA.SaveEntity(boAE);
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
