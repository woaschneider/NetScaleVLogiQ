using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.EntityClient;
using System.Linq;

using combit.ListLabel21.DataProviders;
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
                //if (this.Entity.Waegung == 1 && Entity.identifierOItem != null)
                //{
                    OrderItemservice boOES = new OrderItemservice();
                    OrderItemserviceEntity boOEISe = boOES.GetByOrderIdentifierItemService(Entity.identifierOItemService);
                    if (boOEISe != null)
                    {  
                        boOEISe.HasBinUsed = true;
                        boOES.SaveEntity(boOEISe);
                    }
                //}
            }

            if (this.State == mmBusinessState.Deleting)
            {// Wenn eine Erstwägung gelöscht wird, Orderitemservice wieder aktiv machen
                if (this.Entity != null)
                {
                    if (this.Entity.Waegung == 1 && Entity.identifierOItem != null)
                    {
                        OrderItemservice boOES = new OrderItemservice();
                        OrderItemserviceEntity boOEISe = boOES.GetByOrderIdentifierItemService(Entity.identifierOItemService);
                        if (boOEISe != null)
                        {
                            boOEISe.HasBinUsed = false;
                            boOES.SaveEntity(boOEISe);
                        }
                    }
                }
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
            Einstellungen boE = new Einstellungen();
            boWE.locationId = new Einstellungen().GetEinstellungen().RestLocation.ToString(); 


            Incoterm boI = new Incoterm();
            Entity.incoterm = boI.GetDefaultIncoterm().Kennung;
            Entity.taab = false;
        }

        public void PreSaveHook(WaegeEntity boWE)
        {
           
            #region // Fahrzeug prüfen und ggf. im Stamm neu anlegen, Erstgewicht speichern
            Fahrzeuge boF = new Fahrzeuge();
            FahrzeugeEntity boFe = boF.GetByExactKennzeichen(boWE.Fahrzeug);
            if (boFe == null)
            {
                //TODO: Kunden fragen, ob es Sinn macht den FF zu speichern
                boFe = boF.NewEntity();
                boFe.Kennzeichen1 = boWE.Fahrzeug;
                boFe.Kennzeichen1Raw = ConvertKfzToKfzRaw(boWE.Fahrzeug);
             
            }
            boWE.Kennzeichen1Raw = ConvertKfzToKfzRaw(boWE.Fahrzeug);

            //TODO: Wenn die Produkte bekannt sind, fertig machen!
         
            // Dispobereich
            Planningdivision boP = new Planningdivision();
            if (goApp.planningDivisionId != null)
            {
                PlanningdivisionEntity boPe = boP.GetById(goApp.planningDivisionId);
                if (boPe != null)
                {
                    boWE.PlanningdivisionId = boPe.id;
                    boWE.PlanningdivisionDescription = boPe.description;
                }
            }

            #endregion




        }

        //private void CheckAndCreateAbr(WaegeEntity boWE)
        //{
        //    AbrufEntity oAe;
        //    var boA = new Abruf();

        //    if (boWE.AbrufNr == null)
        //    {
        //        // Gibt es einen Abruf mit diesen Daten
        //        oAe = boA.CompareAbrufData(boWE);
        //        if (oAe == null)
        //        {
        //            oAe = boA.CreateAbrufautomatically(boWE);

        //            Entity.AbrufNr = oAe.AbrufNr;
        //            Entity.abruf_PK = oAe.PK;
        //        }
        //        else
        //        {
        //            Entity.AbrufNr = oAe.AbrufNr;
        //            Entity.abruf_PK = oAe.PK;
        //        }
        //    }

        //}

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
                      orderby w.LieferscheinNr descending 
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

          
       

        public void Product2Waege(int pk, WaegeEntity we)
        {
            Entity = we;

            Produkte boP = new Produkte();
            ProdukteEntity boPE = boP.GetByPk(pk);
            if (boPE != null)
            {
                Entity.productid = boPE.id;
                Entity.productdescription = boPE.description;
            }
            else
            {
                Entity.productid = null;
                Entity.productdescription = null;
            }
        }
        public void Product2Waege(string pid, WaegeEntity we)
        { 
            Entity = we;
            try
            {
              
              Produkte boP = new Produkte();


              ProdukteEntity boPE = boP.GetById(pid);
              if (boPE != null)
              {
                  Entity.productid = boPE.id;
                  Entity.productdescription = boPE.description;
              }
              else
              {
                  Entity.productid = null;
                  Entity.productdescription = null;
              }
            }
            catch (Exception)
            {
                Entity.productid = null;
                Entity.productdescription = null; 
               
            }
            

            
        }
        public void WarenArt2Waege(int pk, WaegeEntity we)
        {
            Entity = we;

            Warenarten boW = new Warenarten();
            WarenartenEntity boWE = boW.GetByPk(pk);
            if (boWE != null)
            {
                // TODO:
                // Entity.kindOfGoodId = boWE.id; Typen passt nicht
                Entity.kindOfGoodId =  boWE.id;
                Entity.kindOfGoodDescription = boWE.description;
            }
            else
            {
                ClearWarenArtInWaege(Entity);
            }
        }

        public void ClearWarenArtInWaege( WaegeEntity we)
        {
            Entity = we;
            Entity.kindOfGoodDescription = null;

        }

        public void Article2Waege(int pk ,WaegeEntity we)

        {
            Entity = we;

            Artikel boA = new Artikel();
            ArtikelEntity boAE = boA.GetByPk(pk);
            if (boAE != null)
            {
                Entity.ArtikelPk = boAE.PK;
                Entity.articleId = boAE.id;
                Entity.articleNumber = boAE.number;
                Entity.articleDescription = boAE.description;
              //  Entity.attributes_as_json ="{"+ boAE.attributes_as_json+"}";
            }
        }

        public void ClearArticleInWaege( WaegeEntity we)
        {
            Entity = we;

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
                    oWe.Fahrzeug = _boFE.Kennzeichen1;
                    //   Entity.Frachtführer = _boFE.Frachtfuehrer;
                    oWe.Frachtmittel = _boFE.Bezeichnung;
                }
                else
                {
                    oWe.Fahrzeug = "";
                    // Entity.Frachtführer = "";
                    oWe.Frachtmittel = "";
                }
            }
        }

        public void FillFrachtmittel(int pk,WaegeEntity oWe)
        {
            Frachtmittel boF = new Frachtmittel();
            FrachtmittelEntity boFE = boF.GetFrachtmittelByPK(pk);
            if (boFE != null)
            {
                oWe.Frachtmittel = boFE.Bezeichnung;
            }
            else
            {
                oWe.Frachtmittel = null;
            }
        }

        #region Adressen 2 Waege

        public void Customer2Waege(int? pk, WaegeEntity _boWe)
        {
            AdressenEntity boAE = GetAPByPk(pk);
            if (boAE != null)
            {
                Entity = _boWe;

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
        public void Customer2Waege(string bi, WaegeEntity _boWe)
        {
            AdressenEntity boAE = GetAPByBi(bi);
            if (boAE != null)
            {
                Entity = _boWe;

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
        public void ClearCustomerInWaege(WaegeEntity _boWe)
        {
            Entity = _boWe;
            if (Entity != null)
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
        }


        public void InvoiceReceiver2Waege(int? pk, WaegeEntity we)
        {
            Entity = we;
            AdressenEntity boAe = GetApByPk(pk);
            if (boAe != null)
            {
                if (Entity != null)
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
        }
        public void InvoiceReceiver2Waege(string bi, WaegeEntity we)
        {
            Entity = we;
            AdressenEntity boAe = GetAPByBi(bi);
            if (boAe != null)
            {
                if (Entity != null)
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
        }
        public void ClearinvoiceReceiverInWaege(WaegeEntity we)
        {
            Entity = we;

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

     
        public void Supplier2Waege(string bi, WaegeEntity we)
        {
            Entity = we;
            AdressenEntity boAe = GetAPByBi(bi);
            if (boAe != null)
            {
                Entity.supplierId = boAe.id;
                Entity.supplierBusinessIdentifier = boAe.businessIdentifier;
                Entity.supplierName = boAe.name;
                Entity.supplierSubName2 = boAe.subName2;
                Entity.supplierOwningLocationId = boAe.owningLocationId;
                Entity.supplierStreet = boAe.street;
                Entity.supplierZipCode = boAe.zipCode;
                Entity.supplierCity = boAe.city;

                Entity.supplierIdCountry = boAe.idCountry;
                Entity.supplierIsocodeCountry = boAe.isocodeCountry;
            }
        }
        public void Supplier2Waege(int? pk, WaegeEntity we)
        {
            Entity = we;
            AdressenEntity boAe = GetAPByPk(pk);
            
                if (boAe != null)
            {
                Entity.supplierId = boAe.id;
                Entity.supplierBusinessIdentifier = boAe.businessIdentifier;
                Entity.supplierName = boAe.name;
                Entity.supplierSubName2 = boAe.subName2;
                Entity.supplierOwningLocationId = boAe.owningLocationId;
                Entity.supplierStreet = boAe.street;
                Entity.supplierZipCode = boAe.zipCode;
                Entity.supplierCity = boAe.city;

                Entity.supplierIdCountry = boAe.idCountry;
                Entity.supplierIsocodeCountry = boAe.isocodeCountry;
            }
        }
        public void ClearSupplierInWaege()
        {
            Entity.supplierId = null;
            Entity.supplierBusinessIdentifier = null;
            Entity.supplierName = null;
            Entity.supplierSubName2 = null;
            Entity.supplierOwningLocationId = null;
            Entity.supplierStreet = null;
            Entity.supplierZipCode = null;
            Entity.supplierCity = null;

            Entity.supplierIdCountry = null;
            Entity.supplierIsocodeCountry = null;
        }


        public void Receiver2Waege(string bi, WaegeEntity we)
        {
            Entity = we;
            AdressenEntity boAe = GetAPByBi(bi);
            if (boAe != null)
            {
                Entity.receiverId = boAe.id;
                Entity.receiverBusinessIdentifier = boAe.businessIdentifier;
                Entity.receiverName = boAe.name;
                Entity.receiverSubName2 = boAe.subName2;
                Entity.receiverOwningLocationId = boAe.owningLocationId;
                Entity.receiverStreet = boAe.street;
                Entity.receiverZipCode = boAe.zipCode;
                Entity.receiverCity = boAe.city;

                Entity.receiverIdCountry = boAe.idCountry;
                Entity.receiverIsocodeCountry = boAe.isocodeCountry;
            }
        }
        public void Receiver2Waege(int pk, WaegeEntity we)
        {
            Entity = we;
            AdressenEntity boAe = GetAPByPk(pk);
            if (boAe != null)
            {
                Entity.receiverId = boAe.id;
                Entity.receiverBusinessIdentifier = boAe.businessIdentifier;
                Entity.receiverName = boAe.name;
                Entity.receiverSubName2 = boAe.subName2;
                Entity.receiverOwningLocationId = boAe.owningLocationId;
                Entity.receiverStreet = boAe.street;
                Entity.receiverZipCode = boAe.zipCode;
                Entity.receiverCity = boAe.city;

                Entity.receiverIdCountry = boAe.idCountry;
                Entity.receiverIsocodeCountry = boAe.isocodeCountry;
            }
        }
        public void ClearReceiverInWaege()
        {

            Entity.receiverId = null;
                Entity.receiverBusinessIdentifier = null;
            Entity.receiverName = null;
            Entity.receiverSubName2 = null;
            Entity.receiverOwningLocationId = null;
            Entity.receiverStreet = null;
            Entity.receiverZipCode = null;
            Entity.receiverCity = null;

            Entity.receiverIdCountry = null;

        }

        public void FrachtFuehrer2Waege(int? pk, WaegeEntity we)
        {
            Entity = we;
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
                ClearFrachtFuehrerInWaege(Entity);
            }
        }
        public void FrachtFuehrer2Waege(string bi, WaegeEntity we)
        {
            Entity = we;
            AdressenEntity boAE = GetAPByBi(bi);
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
                ClearFrachtFuehrerInWaege(Entity);
            }
        }
        public void ClearFrachtFuehrerInWaege(WaegeEntity we)
        {
            Entity = we;
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

        public void Owner2Waege(int? pk, WaegeEntity we)
        {

            Entity = we;
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
        public void Owner2Waege(string bi, WaegeEntity we)
        {

            Entity = we;
            AdressenEntity boAE = GetAPByBi(bi);
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
        public void ClearOwnerInWaege(WaegeEntity we)
        {
            Entity = we;
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


        private static AdressenEntity GetApByPk(int? pk)
        {
            var boA = new Adressen();
            return boA.GetByPk(pk);
        }
        private AdressenEntity GetAPByPk(int? pk)
        {
            Adressen boA = new Adressen();

            return boA.GetByPk(pk);
        }
        private AdressenEntity GetAPByBi(string bi)
        {
            Adressen boA = new Adressen();
           return boA.GetByBusinenessIdentifier(bi);
           
        }

        #endregion
        //**************************************************************************
             public void Auftrag2Waege(int fkpk, WaegeEntity boWe)
        {
            Entity = boWe;
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

                Entity.customerId = _boOIE.customerId;
               
                Entity.customerBusinessIdentifier = _boOIE.customerBusinessIdentifier;
                // Customer2Waege(Entity.customerId, Entity);
                Customer2Waege(Entity.customerBusinessIdentifier, Entity);

                //Entity.customerName = _boOIE.customerName;
                //Entity.customerStreet = _boOIE.customerStreet;
                //Entity.customerZipCode = _boOIE.customerZipCode;
                //Entity.customerCity = _boOIE.customerCity;

                Entity.invoiceReceiverId = _boOIE.invoiceReceiverId;
                InvoiceReceiver2Waege(Entity.invoiceReceiverId,Entity);
                Entity.invoiceReceicerBusinessIdentifier = _boOIE.invoiceReceicerBusinessIdentifier;
               // InvoiceReceiver2Waege(Entity.invoiceReceiverId, Entity);
                 InvoiceReceiver2Waege(Entity.invoiceReceicerBusinessIdentifier,Entity);  
                
                
                Entity.reference = _boOIE.reference;
                Entity.number = _boOIE.number; // Auftrasgnummer
                // Detail
                Entity.identifierOItemService = _boOISE.identifierOItemService;
                Entity.sequenceOItem = _boOISE.sequenceOItem;
                Entity.sequenceOItemService = _boOISE.sequenceOItemService;
                Entity.productid = _boOISE.product;
                Entity.productdescription = _boOISE.productdescription;
                Entity.identifierOItem = _boOISE.identifierOItem;
                Entity.ownerBusinessIdentifier = _boOISE.ownerBusinessIdentifier;
                Entity.ownerId = _boOISE.ownerId;
                
                //Owner2Waege(Entity.ownerId,Entity);
                Owner2Waege(Entity.ownerBusinessIdentifier,Entity);
                Entity.remark = _boOISE.remark;

                
                Entity.supplierBusinessIdentifier = _boOISE.supplierBusinessIdentifier;
                Entity.supplierId = _boOISE.supplierId;
              
                // SupplierOrConsignee2Waege(Entity.supplierOrConsigneeId,Entity);
                Supplier2Waege(Entity.supplierBusinessIdentifier, Entity);


                Entity.receiverBusinessIdentifier = _boOISE.receiverBusinessIdentifier;
                Entity.receiverId = _boOISE.receiverId;

                // SupplierOrConsignee2Waege(Entity.supplierOrConsigneeId,Entity);
                Receiver2Waege(Entity.receiverBusinessIdentifier, Entity);

                Entity.deliveryType = _boOISE.deliveryType;
                Entity.kindOfGoodDescription = _boOISE.kindOfGoodDescription;
                Entity.articleId = _boOISE.articleId;
                Entity.articleNumber = _boOISE.number;
                Entity.articleDescription = _boOISE.articleDescription;
                Artikel boa = new Artikel();
                ArtikelEntity boAe = boa.GetByNr(Entity.articleNumber);
                //if (boAe != null)
                //{
                //    Entity.attributes_as_json = boAe.attributes_as_json;
                //}

                Entity.conversionUnitShortDescription = _boOISE.conversionUnitShortDescription;
                Entity.plannedDate = _boOISE.plannedDate;
                Entity.clearanceReferenz = _boOISE.clearanceReferenz;


                // Das brauchen wir eigentlich nicht mehr
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
        //**************************************************************************


        public mmBindingList<WaegeEntity> GetHofListe()
        {
            IQueryable<WaegeEntity> query = from Waege in this.ObjectContext.WaegeEntities
                where Waege.Waegung == 1
                select Waege;
            return this.GetEntityList(query);
        }

        public mmBindingList<WaegeEntity> PendingListToPolos()
        {
            IQueryable<WaegeEntity> query = from Waege in this.ObjectContext.WaegeEntities
                                            where Waege.Waegung == 2 && Waege.taab==false
                                            orderby Waege.PK
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
            if (kfz == null)
                kfz = "";

            string a = kfz.Replace("-", "");
            return a.Replace(" ", "");
        }

        public bool SetOrderItemsServiceInvisible(WaegeEntity we)
        {

            OrderItemservice boOIS = new OrderItemservice();
            OrderItemserviceEntity boOISE = boOIS.GetByIdentitifier(we.identifierOItem);
            if (boOISE != null)
            {
                boOISE.InvisibleSendedOrderItems = true;
                boOIS.SaveEntity(boOISE);
                return true;
            }
            else
            {
                return false;
            }

        }
    }
}