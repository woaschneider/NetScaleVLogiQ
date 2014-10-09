using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HWB.NETSCALE.BOEF;
using HWB.NETSCALE.FRONTEND.WPF.Import.ArticleImport;
using Microsoft.Windows.Controls;

namespace HWB.NETSCALE.FRONTEND.WPF.Import
{
    class ImportArticle
    {
        private Artikel boA;
        private ArtikelEntity boAE;
        public bool Import(string FullQualifiedFileName)
        {
            try
            {


                ArticleRootObject oA = FullQualifiedFileName.CreateFromJsonFile<ArticleRootObject>();

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
