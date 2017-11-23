using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Media.Animation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using OakLeaf.MM.Main;
using OakLeaf.MM.Main.Business;
using OakLeaf.MM.Main.Collections;

namespace HWB.NETSCALE.BOEF
{
    /// <summary>
    /// Summary description for WaegeRules.
    /// </summary>
    public partial class WaegeRules
    {
        /// <summary>
        /// Checks business rules against the specified entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public override void CheckExtendedRulesHook<EntityType>(EntityType entity)
        {
            WaegeEntity currentEntity = entity as WaegeEntity;
            //ClearAll();
            //ClearAllRules();
            //AutoClearRules = true;
            IsKfzFilled(currentEntity);
            IsFfFilled(currentEntity);
            IsSupplierFilled(currentEntity);


            IsNettoEmpty(currentEntity);

            // Call Validation methods
            IsProductFilled(currentEntity);
            IsExistedProductFilled(currentEntity);


            IsCustomerFilled(currentEntity);

            IsLagerPlatzFilled(currentEntity);
            IsConversionUnitAmountFilled(currentEntity);

            IsArticlenumberFilled(currentEntity);

            IsAttributeFilled(currentEntity);
        }

        private string IsConversionUnitAmountFilled(WaegeEntity currentEntity)
        {
            string Msg = null;
            if (currentEntity.Waegung == 2)
            {
                if (currentEntity.conversionUnitShortDescription != null)

                {
                    if (mmType.IsEmpty(currentEntity.conversionUnitAmount))
                    {
                        this.EntityPropertyDisplayName = "Mengenangabe";
                        RequiredFieldMessageSuffix = " darf nicht null oder leer sein!";
                        Msg = this.RequiredFieldMessagePrefix +
                              this.EntityPropertyDisplayName + " " +
                              this.RequiredFieldMessageSuffix;

                        AddErrorProviderBrokenRule("conversionUnitAmount", Msg);
                    }
                }
            }
            return Msg;
        }

        private string IsNettoEmpty(WaegeEntity currentEntity)
        {
            string Msg = null;
            if (currentEntity.Waegung == 2)
            {
                if (mmType.IsEmpty(currentEntity.Nettogewicht))
                {
                    this.EntityPropertyDisplayName = "Netto";
                    RequiredFieldMessageSuffix = " darf nicht null oder leer sein!";
                    Msg = this.RequiredFieldMessagePrefix +
                          this.EntityPropertyDisplayName + " " +
                          this.RequiredFieldMessageSuffix;

                    AddErrorProviderBrokenRule("Nettogewicht", Msg);
                }
            }
            return Msg;
        }

        private string IsExistedProductFilled(WaegeEntity currentEntity)
        {
            Produkte boP = new Produkte();
            ProdukteEntity boPe = boP.GetById(currentEntity.productid);
            string Msg = null;
            if (mmType.IsEmpty(currentEntity.productid))
            {
                this.EntityPropertyDisplayName = "Produkt";
                RequiredFieldMessageSuffix = " ist ein Pflichtfeld oder das eingetragene Produkt ist unbekannt!";
                Msg = this.RequiredFieldMessagePrefix +
                      this.EntityPropertyDisplayName + " " +
                      this.RequiredFieldMessageSuffix;

                AddErrorProviderBrokenRule("productdescription", Msg);
            }
            return Msg;
        }

        /// <summary>
        /// Neu Entweder FF-BI oder FF-Freitext
        /// </summary>
        /// <param name="currentEntity"></param>
        /// <returns></returns>
        private string IsFfFilled(WaegeEntity currentEntity)
        {
            string Msg = null;
            if (mmType.IsEmpty(currentEntity.ffName) && mmType.IsEmpty(currentEntity.freightCarrierFreeText))
            {
                this.EntityPropertyDisplayName = "Frachtführer-Name oder Frachtführer-Freitext";
                RequiredFieldMessageSuffix = " ist ein Pflichtfeld";
                Msg = this.RequiredFieldMessagePrefix +
                      this.EntityPropertyDisplayName + " " +
                      this.RequiredFieldMessageSuffix;

                AddErrorProviderBrokenRule("ffBusinessIdentifier", Msg);
                AddErrorProviderBrokenRule("freightCarrierFreeText", "");
            }
            else
            {
                ClearRule("ffBusinessIdentifier");
                ClearRule("freightCarrierFreeTextr");
               
                
            }

            return Msg;
        }

        private string IsSupplierFilled(WaegeEntity currentEntity)
        {
            string Msg = "";
            if (mmType.IsEmpty(currentEntity.receiverBusinessIdentifier) &&
                mmType.IsEmpty(currentEntity.recipientFreeText))
            {
                if (mmType.IsEmpty(currentEntity.supplierBusinessIdentifier) &&
                    mmType.IsEmpty(currentEntity.supplierFreeText))
                {
                    this.EntityPropertyDisplayName = "Wenn Empfänger-Name und Empfänger-Freitext leer sind";
                    RequiredFieldMessageSuffix = " dann ist Lieferant oder Lieferant-Freitext  ein Pflichtfeld!";
                    Msg = this.RequiredFieldMessagePrefix +
                          this.EntityPropertyDisplayName + " " +
                          this.RequiredFieldMessageSuffix;

                    AddErrorProviderBrokenRule("supplierBusinessIdentifier", Msg);
                    AddErrorProviderBrokenRule("recipientFreeText","");
                    AddErrorProviderBrokenRule("receiverBusinessIdentifier", "");
                    AddErrorProviderBrokenRule("supplierFreeText", "");
                
                }
                else
                {
                    ClearAllRules();
                }
            }
            //receiverBusinessIdentifier
            //   

            return Msg;
        }


        private string IsAttributeFilled(WaegeEntity currentEntity)
        {
            //// Welche Pflicht-Attribute hat der Artikel

            string Msg = null;
            var artikelattribute = new Attribut().GetPflichtAttributeByArtikelPk(currentEntity.ArtikelPk);


            foreach (var artikel in artikelattribute)
            {
                bool found = false;
                try
                {
                    //    string js = currentEntity.attributes_as_json.Replace("{", "").Replace("}", "");
                    string js = currentEntity.attributes_as_json;
                    JObject obj = JObject.Parse(js);

                    foreach (var pair in obj)
                    {
                        var propName = pair.Key;
                        var propValue = pair.Value.ToString();
                        if (artikel.AttributName == pair.Key)
                        {
                            found = true;
                        }
                    }
                    if (found == false)
                    {
                        this.EntityPropertyDisplayName = "Attribut: " + artikel.AttributName;
                        RequiredFieldMessageSuffix = " ist ein Pflichtfeld";
                        Msg = this.RequiredFieldMessagePrefix +
                              this.EntityPropertyDisplayName + " " +
                              this.RequiredFieldMessageSuffix;

                        AddErrorProviderBrokenRule("articleNumber", Msg);
                    }
                }
                catch (Exception e)
                {
                    this.EntityPropertyDisplayName = "Attribut: " + artikel.AttributName;
                    RequiredFieldMessageSuffix = " ist ein Pflichtfeld";
                    Msg = this.RequiredFieldMessagePrefix +
                          this.EntityPropertyDisplayName + " " +
                          this.RequiredFieldMessageSuffix;

                    AddErrorProviderBrokenRule("articleNumber", Msg);
                }
            }
            //


            return Msg;
        }


        private void IsOwner(WaegeEntity currentEntity)
        {
            throw new NotImplementedException();
        }

        private void IsInvoiceReceiver(WaegeEntity currentEntity)
        {
            throw new NotImplementedException();
        }

        public string IsProductFilled(WaegeEntity _we)
        {
            string Msg = null;
            if (mmType.IsEmpty(_we.productdescription))
            {
                this.EntityPropertyDisplayName = "Produkt";
                RequiredFieldMessageSuffix = " ist ein Pflichtfeld";
                Msg = this.RequiredFieldMessagePrefix +
                      this.EntityPropertyDisplayName + " " +
                      this.RequiredFieldMessageSuffix;

                AddErrorProviderBrokenRule("productdescription", Msg);
            }
            return Msg;
        }

        public string IsKfzFilled(WaegeEntity _we)
        {
            string Msg = null;
            if (mmType.IsEmpty(_we.Fahrzeug))
            {
                this.EntityPropertyDisplayName = "Kfz-Kennzeichen";
                RequiredFieldMessageSuffix = " ist ein Pflichtfeld";
                Msg = this.RequiredFieldMessagePrefix +
                      this.EntityPropertyDisplayName + " " +
                      this.RequiredFieldMessageSuffix;

                AddErrorProviderBrokenRule("Fahrzeug", Msg);
            }
            return Msg;
        }

        public string IsCustomerFilled(WaegeEntity _we)
        {
            string Msg = null;
            if (mmType.IsEmpty(_we.customerBusinessIdentifier))
            {
                this.EntityPropertyDisplayName = "Auftraggeber";
                RequiredFieldMessageSuffix = " ist ein Pflichtfeld";
                Msg = this.RequiredFieldMessagePrefix +
                      this.EntityPropertyDisplayName + " " +
                      this.RequiredFieldMessageSuffix;

                AddErrorProviderBrokenRule("customerBusinessIdentifier", Msg);
            }
            return Msg;
        }

        public string IsArticlenumberFilled(WaegeEntity _we)
        {
            string Msg = null;
            if (mmType.IsEmpty(_we.articleNumber))
            {
                this.EntityPropertyDisplayName = "Artikel-Nr.";
                RequiredFieldMessageSuffix = " ist ein Pflichtfeld";
                Msg = this.RequiredFieldMessagePrefix +
                      this.EntityPropertyDisplayName + " " +
                      this.RequiredFieldMessageSuffix;

                AddErrorProviderBrokenRule("articleNumber", Msg);
            }
            return Msg;
        }

        public string IsWarenArtFilled(WaegeEntity _we)
        {
            string Msg = null;
            if (mmType.IsEmpty(_we.kindOfGoodDescription))
            {
                this.EntityPropertyDisplayName = "Warenart";
                RequiredFieldMessageSuffix = " ist ein Pflichtfeld";
                Msg = this.RequiredFieldMessagePrefix +
                      this.EntityPropertyDisplayName + " " +
                      this.RequiredFieldMessageSuffix;

                AddErrorProviderBrokenRule("kindOfGoodDescription", Msg);
            }
            return Msg;
        }

        public string IsLagerPlatzFilled(WaegeEntity _we)
        {
            string Msg = null;
            if (mmType.IsEmpty(_we.IstQuellLagerPlatz))
            {
                this.EntityPropertyDisplayName = "Ist-Quell-Lagerplatz";
                RequiredFieldMessageSuffix = " ist ein Pflichtfeld";
                Msg = this.RequiredFieldMessagePrefix +
                      this.EntityPropertyDisplayName + " " +
                      this.RequiredFieldMessageSuffix;

                AddErrorProviderBrokenRule("IstQuellLagerPlatz", Msg);
            }
            return Msg;
        }

        //public string IsOrderChanged(WaegeEntity _we)
        //{
        //    if (!string.IsNullOrEmpty(_we.identifier))
        //    {
        //       // Temporär WaegeEntity
        //        Waege w = new Waege();
        //        WaegeEntity we = new WaegeEntity();
        //        w.Auftrag2Waege(_we.identifier, we);
        //    }
        //}
    }
}