using System;
using HWB.NETSCALE.BOEF;
using HWB.NETSCALE.POLOSIO.ArticleImport;
using Xceed.Wpf.Toolkit;

namespace HWB.NETSCALE.POLOSIO
{
    internal class ImportArticle
    {
        private Artikel boA;
        private ArtikelEntity boAE;

        public bool Import(string FullQualifiedFileName)
        {
            try
            {
                var oA = FullQualifiedFileName.CreateFromJsonFile<ArticleRootObject>();

                boA = new Artikel();

                foreach (ArticleInformation obj in oA.articleInformation)
                {
                    if (obj.article.id != null)
                    {
                        boAE = boA.GetById(obj.article.id);
                        if (boAE == null)
                        {
                            boAE = boA.NewEntity();
                        }
                        boAE.id = obj.article.id;
                        boAE.number = obj.article.number;
                        boAE.ownerId = obj.article.ownerId.ToString();
                        boAE.kindOfGoodDescription = obj.article.kindOfGoodDescription;
                        boAE.locationId = obj.article.ownerId.ToString();


                        boAE.baseUnitId = obj.article.baseUnit.id;
                        boAE.baseUnitShortDescription = obj.article.baseUnit.shortDescription;
                        boAE.baseUnitDescription = obj.article.baseUnit.description;

                        if (obj.article.conversionUnit != null)
                        {
                            boAE.conversionUnitId = obj.article.conversionUnit.id;
                            boAE.conversionUnitDescription = obj.article.conversionUnit.description;
                            boAE.conversionUnitShortDescription = obj.article.conversionUnit.shortDescription;
                            boA.SaveEntity(boAE);
                        }
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