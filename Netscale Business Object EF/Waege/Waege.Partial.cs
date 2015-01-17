using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.EntityClient;
using System.Linq;
using System.Windows.Xps;
using combit.ListLabel20.DataProviders;
using OakLeaf.MM.Main.Business;
using OakLeaf.MM.Main.Collections;
using OakLeaf.MM.Main.Data;
using HWB.NETSCALE.BOEF;
using HWB.NETSCALE.GLOBAL;

namespace HWB.NETSCALE.BOEF
{
    /// <summary>
    /// Summary description for Waege.
    /// </summary>
    public partial class Waege
    {
        /// <summary>
        /// Hook method automatically executed from the mmBusinessObject constructor
        /// </summary>
        protected override void HookConstructor()
        {
            // Place code here to be executed when the business object instantiates
            // OK - Ich bin mir hier nicht sicher ob das einfacher geht: So baue ich mir im Moment meinen PresaveHook
            StateChange += new mmBusinessStateChangeDelegate(this.StateChangeHandler);
        }

        public override void StateChangeHandler(mmBaseBusinessObject bizObj, mmBusinessStateChangeEventArgs e)
        {
            if (this.State == mmBusinessState.PreSaving)
            {
                PreSaveHook(this.Entity);
            }

            if (this.State == mmBusinessState.Added)
            {
                OnNew(Entity);
            }

            if (this.State == mmBusinessState.Saved)
            {
            }
        }

        private void OnNew(WaegeEntity boWE)
        {
            Incoterm boI = new Incoterm();
            Entity.incoterm = boI.GetDefaultIncoterm().Kennung;
        }

        public void PreSaveHook(WaegeEntity boWE)
        {
            // Fahrzeug prüfen und ggf. im Stamm neu anlegen
            Fahrzeuge boF = new Fahrzeuge();
            FahrzeugeEntity boFe = boF.GetByExactKennzeichen(boWE.Fahrzeug);
            if (boFe == null)
            {
                if (goApp.AutoAbruf)
                    CheckAndCreateAbr(boWE);
                //   


                //TODO: Kunden fragen, ob es Sinn macht den FF zu speichern
                boFe = boF.NewEntity();
                boFe.Kennzeichen1 = boWE.Fahrzeug;
                boFe.Kennzeichen1Raw = ConvertKfzToKfzRaw(boWE.Fahrzeug);
                Entity.Kennzeichen1Raw = ConvertKfzToKfzRaw(boWE.Fahrzeug);

                if (goApp.SAVE_ABR2CF)

                {
                    boFe.AbrufNr = boWE.AbrufNr;
                    
                }
                else
                {
                    boFe.AbrufNr = null;
                }
                boF.SaveEntity(boFe);
            }
        }

        private void CheckAndCreateAbr(WaegeEntity boWE)
        {
            var boA = new Abruf();
            if (boWE.AbrufNr == null)
            {
                // Gibt es einen Abruf mit diesen Daten
                var oAe = boA.CompareAbrufData(boWE);
                if (oAe == null)

                {
                    AbrufEntity oAE = boA.CreateAbrufautomatically(boWE);
                    Entity.AbrufNr = oAE.AbrufNr;
                }
                else
                {
                    Entity.AbrufNr = oAe.AbrufNr;
                }
            }
        }

        public mmBindingList<WaegeEntity> GetLsListe(DateTime vonDatum, DateTime bisDatum, string f1, string mc1,
            string f2, string mc2, string f3, string mc3)
        {
            string lieferscheinnr = "";
            string auftraggeber = "";
            string produkt = "";
            string fahrzeug = "";


            if (f1 == "Lieferschein-Nr.")
                lieferscheinnr = mc1;
            if (f1 == "Auftraggeber")
                auftraggeber = mc1;
            if (f1 == "Produkt")
                produkt = mc1;
            if (f1 == "Fahrzeug")
                fahrzeug = mc1;

            if (f2 == "Lieferschein-Nr.")
                lieferscheinnr = mc2;
            if (f2 == "Auftraggeber")
                auftraggeber = mc2;
            if (f2 == "Produkt")
                produkt = mc2;
            if (f2 == "Fahrzeug")
                fahrzeug = mc2;


            if (f3 == "Lieferschein-Nr.")
                lieferscheinnr = mc3;
            if (f3 == "Auftraggeber")
                auftraggeber = mc3;
            if (f3 == "Produkt")
                produkt = mc3;
            if (f3 == "Fahrzeug")
                fahrzeug = mc3;

            IQueryable<WaegeEntity> query = from w in ObjectContext.WaegeEntities
                where w.Waegung == 2
                      && w.LSDatum >= vonDatum
                      && w.LSDatum <= bisDatum
                      && w.LieferscheinNr.Contains(lieferscheinnr)
                      && w.customerBusinessIdentifier.Contains(auftraggeber)
                      && w.productdescription.Contains(produkt)
                      && w.Fahrzeug.Contains(fahrzeug)
                select w;

            return GetEntityList(query);
        }

        public mmBindingList<WaegeEntity> GetYeomanTaabListe(DateTime ExportDatum, string auftraggeber)
        {
            IQueryable<WaegeEntity> query = from w in ObjectContext.WaegeEntities
                where w.Waegung == 2
                      && w.LSDatum == ExportDatum
                      && w.customerBusinessIdentifier.Contains(auftraggeber)
                      && (w.taabExcel == false | w.taabExcel == null)
                orderby (w.LieferscheinNr) descending
                select w;

            return GetEntityList(query);
        }

        public mmBindingList<WaegeEntity> GetYeomanTaabListeRecover(DateTime ExportDatum, string auftraggeber)
        {
            IQueryable<WaegeEntity> query = from w in ObjectContext.WaegeEntities
                where w.Waegung == 2
                      && w.LSDatum == ExportDatum
                      && w.customerBusinessIdentifier.Contains(auftraggeber)
                      && w.Waegung == 2
                select w;

            return GetEntityList(query);
        }


        private static AdressenEntity GetApByPk(int pk)
        {
            var boA = new Adressen();
            return boA.GetByPk(pk);
        }

        public void Product2Waege(int pk)
        {
            Produkte boP = new Produkte();
            ProdukteEntity boPE = boP.GetByPk(pk);
            if (boPE != null)
            {
                Entity.product = boPE.id;
                Entity.productdescription = boPE.description;
            }
            else
            {
                Entity.product = null;
                Entity.productdescription = null;
            }
        }

        public void WarenArt2Waege(int pk)
        {
            Warenarten boW = new Warenarten();
            WarenartenEntity boWE = boW.GetByPk(pk);
            if (boWE != null)
            {
                // TODO:
                // Entity.kindOfGoodId = boWE.id; Typen passt nicht
                Entity.kindOfGoodDescription = boWE.description;
            }
            else
            {
                ClearWarenArtInWaege();
            }
        }

        public void ClearWarenArtInWaege()
        {
        }

        public void Article2Waege(int pk)

        {
            Artikel boA = new Artikel();
            ArtikelEntity boAE = boA.GetByPk(pk);
            if (boAE != null)
            {
                Entity.articleId = boAE.id;
                Entity.articleNumber = boAE.number;
                Entity.articleDescription = boAE.description;
            }
        }

        public void ClearArticleInWaege()
        {
            Entity.articleId = null;
            Entity.articleDescription = null;
        }

        public void FillKfz(int pk, WaegeEntity oWe)
        {
            if (oWe != null & pk != 0)
            {
                Fahrzeuge _boF = new Fahrzeuge();
                FahrzeugeEntity _boFE = _boF.GetByPk(pk);
                if (_boFE != null)
                {
                    Entity.Fahrzeug = _boFE.Kennzeichen1;
                    //   Entity.Frachtführer = _boFE.Frachtfuehrer;
                    Entity.Frachtmittel = _boFE.Bezeichnung;
                }
                else
                {
                    Entity.Fahrzeug = "";
                    // Entity.Frachtführer = "";
                    Entity.Frachtmittel = "";
                }
            }
        }

        public void FillFrachtmittel(int pk)
        {
            Frachtmittel boF = new Frachtmittel();
            FrachtmittelEntity boFE = boF.GetFrachtmittelByPK(pk);
            if (boFE != null)
            {
                Entity.Frachtmittel = boFE.Bezeichnung;
            }
            else
            {
                Entity.Frachtmittel = null;
            }
        }

        #region Adressen 2 Waege

        public void Customer2Waege(int pk)
        {
            AdressenEntity boAE = GetAPByPk(pk);
            if (boAE != null)
            {
                Entity.customerId = boAE.id;
                Entity.customerBusinessIdentifier = boAE.businessIdentifier;
                Entity.customerName = boAE.name;
                Entity.customerSubName2 = boAE.subName2;
                Entity.customerOwningLocationId = boAE.owningLocationId;
                Entity.customerStreet = boAE.street;
                Entity.customerZipCode = boAE.zipCode;
                Entity.customerCity = boAE.city;
                Entity.customerIdCountry = boAE.idCountry;
                Entity.customerIsocodeCountry = boAE.isocodeCountry;
            }
        }

        public void ClearCustomerInWaege()
        {
            Entity.customerBusinessIdentifier = null;
            Entity.customerName = null;
            Entity.customerSubName2 = null;
            Entity.customerOwningLocationId = null;
            Entity.customerStreet = null;
            Entity.customerZipCode = null;
            Entity.customerCity = null;
            Entity.customerIdCountry = null;
            Entity.customerIsocodeCountry = null;
        }

        public void InvoiceReceiver2Waege(int pk)
        {
            AdressenEntity boAe = GetApByPk(pk);
            if (boAe != null)
            {
                Entity.invoiceReceiverId = boAe.id;
                Entity.invoiceReceicerBusinessIdentifier = boAe.businessIdentifier;
                Entity.invoiceReceiverName = boAe.name;
                Entity.invoiceReceiverSubName2 = boAe.subName2;
                Entity.invoiceReceiverOwningLocationId = boAe.owningLocationId;
                Entity.invoiceReceiverStreet = boAe.street;
                Entity.invoiceReceiverZipCode = boAe.zipCode;
                Entity.invoiceReceiverCity = boAe.city;
                Entity.invoiceReceiverIdCountry = boAe.idCountry;
                Entity.invoiceReceiverIsocodeCountry = boAe.isocodeCountry;
            }
        }

        public void ClearinvoiceReceiverInWaege()
        {
            Entity.invoiceReceiverId = null;
            Entity.invoiceReceicerBusinessIdentifier = null;
            Entity.invoiceReceiverName = null;
            Entity.invoiceReceiverSubName2 = null;
            Entity.invoiceReceiverOwningLocationId = null;
            Entity.invoiceReceiverStreet = null;
            Entity.invoiceReceiverZipCode = null;
            Entity.invoiceReceiverCity = null;
            Entity.invoiceReceiverIdCountry = null;
            Entity.invoiceReceiverIsocodeCountry = null;
        }

        public void SupplierOrConsignee2Waege(int pk)
        {
            AdressenEntity boAe = GetApByPk(pk);
            if (boAe != null)
            {
                Entity.supplierOrConsigneeId = boAe.id;
                Entity.supplierOrConsigneeBusinessIdentifier = boAe.businessIdentifier;
                Entity.supplierOrConsigneeName = boAe.name;
                Entity.supplierOrConsigneeSubName2 = boAe.subName2;
                Entity.supplierOrConsigneeOwningLocationId = boAe.owningLocationId;
                Entity.supplierOrConsigneeStreet = boAe.street;
                Entity.supplierOrConsigneeZipCode = boAe.zipCode;
                Entity.supplierOrConsigneeCity = boAe.city;

                Entity.supplierOrConsigneeIdCountry = boAe.idCountry;
                Entity.supplierOrConsigneeIsocodeCountry = boAe.isocodeCountry;
            }
        }

        public void ClearsupplierOrConsigneeInWaege()
        {
            Entity.supplierOrConsigneeId = null;
            Entity.supplierOrConsigneeBusinessIdentifier = null;
            Entity.supplierOrConsigneeName = null;
            Entity.supplierOrConsigneeSubName2 = null;
            Entity.supplierOrConsigneeOwningLocationId = null;
            Entity.supplierOrConsigneeStreet = null;
            Entity.supplierOrConsigneeZipCode = null;
            Entity.supplierOrConsigneeCity = null;
            Entity.supplierOrConsigneeIdCountry = null;
            Entity.supplierOrConsigneeIsocodeCountry = null;
        }

        public void FrachtFuehrer2Waege(int pk)
        {
            AdressenEntity boAE = GetAPByPk(pk);
            if (boAE != null)
            {
                Entity.ffId = boAE.id;
                Entity.ffBusinessIdentifier = boAE.businessIdentifier;
                Entity.ffName = boAE.name;
                Entity.ffSubName2 = boAE.subName2;
                Entity.ffOwningLocationId = boAE.owningLocationId;
                Entity.ffStreet = boAE.street;
                Entity.ffZipCode = boAE.zipCode;
                Entity.ffCity = boAE.city;
                Entity.ffIdCountry = boAE.idCountry;
                Entity.ffIsocodeCountry = boAE.isocodeCountry;
            }
            else
            {
                ClearFrachtFuehrerInWaege();
            }
        }

        public void ClearFrachtFuehrerInWaege()
        {
            Entity.ffId = null;
            Entity.ffBusinessIdentifier = null;
            Entity.ffName = null;
            Entity.ffSubName2 = null;
            Entity.ffOwningLocationId = null;
            Entity.ffStreet = null;
            Entity.ffZipCode = null;
            Entity.ffCity = null;
            Entity.ffIdCountry = null;
            Entity.ffIsocodeCountry = null;
        }

        public void Owner2Waege(int pk)
        {
            AdressenEntity boAE = GetAPByPk(pk);
            if (boAE != null)
            {
                Entity.ownerId = boAE.id;
                Entity.ownerBusinessIdentifier = boAE.businessIdentifier;
                Entity.ownerName = boAE.name;
                Entity.ownerSubName2 = boAE.subName2;
                Entity.ownerOwningLocationId = boAE.owningLocationId;
                Entity.ownerStreet = boAE.street;
                Entity.ownerZipCode = boAE.zipCode;
                Entity.ownerCity = boAE.city;
                Entity.ownerIdCountry = boAE.idCountry;
                Entity.ownerIsocodeCountry = boAE.isocodeCountry;
            }
        }

        public void ClearOwnerInWaege()
        {
            Entity.ownerId = null;
            Entity.ownerBusinessIdentifier = null;
            Entity.ownerName = null;
            Entity.ownerSubName2 = null;
            Entity.ownerOwningLocationId = null;
            Entity.ownerStreet = null;
            Entity.ownerZipCode = null;
            Entity.ownerCity = null;
            Entity.ownerIdCountry = null;
            Entity.ownerIsocodeCountry = null;
        }


        private AdressenEntity GetAPByPk(int pk)
        {
            Adressen boA = new Adressen();

            return boA.GetByPk(pk);
        }

        #endregion

        public void Auftrag2Waege(int fkpk)
        {
            Orderitem _boOI = new Orderitem();
            OrderItemservice _boOIS = new OrderItemservice();
            OrderItemserviceEntity _boOISE = _boOIS.GetByPK(fkpk);

            if (_boOISE == null)
                return;
            if (_boOISE.PKOrderItem == null)
                return;

            OrderitemEntity _boOIE = _boOI.GetByPk(_boOISE.PKOrderItem);

            if (_boOI != null & _boOIE != null)
            {
                // Kopfdaten
                Entity.locationId = _boOIE.locationId; // ???? Kopf oder Detail
                Entity.customerBusinessIdentifier = _boOIE.customerBusinessIdentifier;
                Entity.invoiceReceicerBusinessIdentifier = _boOIE.invoiceReceicerBusinessIdentifier;
                Entity.reference = _boOIE.reference;

                // Detail
                Entity.sequence = _boOISE.sequence;

                Entity.productdescription = _boOISE.productdescription;
                Entity.identifier = _boOISE.identifier;
                Entity.ownerBusinessIdentifier = _boOISE.ownerBusinessIdentifier;
                Entity.remark = _boOISE.remark;
                Entity.supplierOrConsigneeBusinessIdentifier = _boOISE.ownerBusinessIdentifier;
                Entity.deliveryType = _boOISE.deliveryType;
                Entity.kindOfGoodDescription = _boOISE.kindOfGoodDescription;
                Entity.articleId = _boOISE.articleId;
                Entity.articleNumber = _boOISE.number;
                Entity.articleDescription = _boOISE.articleDescription;
                Entity.plannedDate = _boOISE.plannedDate;
                Entity.clearanceReferenz = _boOISE.clearanceReferenz;


                // 
                Entity.orign = _boOISE.orign;
                Entity.grade = _boOISE.grade;
                Entity.batch = _boOISE.batch;
                Entity.storageAreaReference = _boOISE.storageAreaReference;
                Entity.storageAreaReferenceNumber = _boOISE.storageAreaReferenceNumber;
                Entity.originalNumber = _boOISE.originalNumber;
                Entity.orignalMarking = _boOISE.orignalMarking;
                Entity.SerialNumber = _boOISE.SerialNumber;
                Entity.length = _boOISE.length;
                Entity.height = _boOISE.height;
                Entity.width = _boOISE.width;
                Entity.diameter = _boOISE.diameter;
                Entity.dimension = _boOISE.dimension;
            }
        }


        public mmBindingList<WaegeEntity> GetHofListe()
        {
            IQueryable<WaegeEntity> query = from Waege in this.ObjectContext.WaegeEntities
                where Waege.Waegung == 1
                select Waege;
            return this.GetEntityList(query);
        }

        public WaegeEntity GetWaegungByPk(int pk)
        {
            IQueryable<WaegeEntity> query = from Waege in this.ObjectContext.WaegeEntities
                where Waege.PK == pk
                select Waege;
            return this.GetEntity(query);
        }

        public int GetLastWaegung()
        {
            int i = (from w in this.ObjectContext.WaegeEntities
                where w.Waegung == 2
                select w.PK).DefaultIfEmpty().Max();
            return i;
        }

        public ObjectDataProvider GetWaegungOdpbyPk(int pWaegePk)
        {
            IQueryable<WaegeEntity> query = from waege in ObjectContext.WaegeEntities
                where waege.PK == pWaegePk
                select waege;
            var oDp = new ObjectDataProvider(query, 2);
            return oDp;
        }

        public ObjectDataProvider GetLsbyLsnrGlobal(string lsnr)
        {
            IQueryable<WaegeEntity> query = from waege in ObjectContext.WaegeEntities
                where waege.LieferscheinNr == lsnr
                select waege;
            var oDp = new ObjectDataProvider(query, 2);
            return oDp;
        }

        public int IsKfzErstVerwogen(string kf)
        {
            var k = ConvertKfzToKfzRaw(kf);

            var query = from w in ObjectContext.WaegeEntities
                where w.Kennzeichen1Raw == k && w.Waegung == 1
                select w;
            var cWe = GetEntity(query);
            if (cWe == null)
                return 0;
            return cWe.PK;
        }

        private string ConvertKfzToKfzRaw(string kfz)
        {
            string a = kfz.Replace("-", "");
            return a.Replace(" ", "");
        }
    }
}