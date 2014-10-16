using System;
using HWB.NETSCALE.BOEF;
using Xceed.Wpf.Toolkit;

namespace HWB.NETSCALE.POLOSIO.ArticleAttributes
{
    internal class ImportArticleAttributes
    {
        private Artikelattribute boA;
        private ArtikelattributeEntity boAE;

        public bool Import(string FullQualifiedFileName)
        {
            try
            {
                var oA = FullQualifiedFileName.CreateFromJsonFile<ArticleAttributesRootObject>();

                boA = new Artikelattribute();
                for (int i = 0; i < oA.articleAttributes.Count; i++)
                {
                    boAE = boA.GetArtikelAttributByBezeichnung(oA.articleAttributes[i]);
                    if (boAE == null)
                    {
                        boAE = boA.NewEntity();
                    }
                    if (boAE != null)
                    {
                        boAE.AttributName = oA.articleAttributes[i];
                        boA.SaveEntity(boAE);
                    }
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