using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.EntityClient;
using System.Linq;

using OakLeaf.MM.Main.Business;
using OakLeaf.MM.Main.Collections;
using OakLeaf.MM.Main.Data;

namespace HWB.NETSCALE.BOEF
{
	/// <summary>
	/// Summary description for Abruf.
	/// </summary>
	public partial class Abruf
	{
		/// <summary>
		/// Hook method automatically executed from the mmBusinessObject constructor
		/// </summary>
		protected override void HookConstructor()
		{
			// Place code here to be executed when the business object instantiates
		}
        public AbrufEntity GetAbrufByNr(string nr)
        {
            IQueryable<AbrufEntity> query = from a in ObjectContext.AbrufEntities
                                            where a.AbrufNr == nr
                                            select a;
            return GetEntity(query);
        }
        public AbrufEntity GetAbrufById(int? pk)
        {
            IQueryable<AbrufEntity> query = from a in ObjectContext.AbrufEntities
                                            where a.PK == pk
                                            select a;
            return GetEntity(query);
        }
        public string GetNextFreeAbrufNr()
        {
            bool loopReady = false;
            int ii = 0;
            do
            {
                ii = ii + 1;
                AbrufEntity boAe = GetAbrufByNr(ii.ToString());
                if (boAe == null)
                {
                    loopReady = true;
                }
            } while (loopReady == false);
            return ii.ToString();
        }
        public mmBindingList<AbrufEntity> GetAllAbruf()
        {
            IQueryable<AbrufEntity> query = from a in ObjectContext.AbrufEntities
                                            orderby a.AbrufNr
                                            select a;
            return GetEntityList(query);
        }

        // TODO_ Erweitern nach Kundenwunsch
        public AbrufEntity CompareAbrufData(WaegeEntity oWe)
        {
            IQueryable<AbrufEntity> query = from a in ObjectContext.AbrufEntities
                orderby a.AbrufNr
                where a.customerBusinessIdentifier == oWe.customerBusinessIdentifier &&
                      a.invoiceReceicerBusinessIdentifier == oWe.invoiceReceicerBusinessIdentifier &&
                      a.ownerBusinessIdentifier == oWe.ownerBusinessIdentifier &&
                      a.supplierOrConsigneeBusinessIdentifier == oWe.supplierOrConsigneeBusinessIdentifier &&
                      a.articleNumber == oWe.articleNumber && a.identifier == oWe.identifier                                     
                                            select a;
            AbrufEntity oAE = GetEntity(query);

            return oAE;
        }
        public bool HasAbrufChanged(WaegeEntity boWE)
        {
            // Vergleiche
            bool uRet = false;
            AbrufEntity oAE = GetAbrufByNr(boWE.AbrufNr);
            if (oAE != null)
            {
                //if (oAE.NrKU != boWE.NrKU)
                //{
                //    uRet = true;
                //}

                //if (oAE.FirmaKU != boWE.FirmaKU)
                //{
                //    uRet = true;
                //}

                //if (oAE.SortenNr != boWE.SortenNr)
                //{
                //    uRet = true;
                //}
                //if (oAE.Sortenbezeichnung1 != boWE.Sortenbezeichnung1)
                //{
                //    uRet = true;
                //}
                //if (oAE.WiegeartKz != boWE.WiegeartKz)
                //{
                //    uRet = true;
                //}
                //if (oAE.wefirma != boWE.wefirma)
                //{
                //    uRet = true;
                //}
                //if (oAE.wename1 != boWE.wename1)
                //{
                //    uRet = true;
                //}
                return uRet;
            }
            else
            {
                return false;
            }
        }
        public AbrufEntity CreateAbruf(WaegeEntity oWE, decimal ist, decimal soll, decimal rest)
        {

            Abruf boA = new Abruf();
            AbrufEntity boAE = boA.NewEntity();



            CopyEntityPropertyValues(oWE, boAE);
            if (oWE.AbrufNr == null)
                boAE.AbrufNr = GetNextFreeAbrufNr();
           
            boAE.abrufFest = true;
            boAE.abrufDatum = DateTime.Today;
            boA.SaveEntity(boAE);
            return boAE;
        }

        public AbrufEntity CreateAbrufautomatically(WaegeEntity oWe)
        {
            AbrufEntity boAe = NewEntity();
            CopyEntityPropertyValues(oWe, boAe);
       
            boAe.AbrufNr = GetNextFreeAbrufNr();
            boAe.abrufDatum = DateTime.Today;
      
            SaveEntity(boAe);
            return boAe;
        }
        public WaegeEntity CopyAbrufToWaege(int? pk, WaegeEntity oWe)
        {
            AbrufEntity boAe = GetAbrufById(pk);
            if (boAe != null)
            {
                CopyEntityPropertyValues(boAe, oWe);
               // oWe.Abrufid = boAe.PK;
                oWe.AbrufNr = boAe.AbrufNr;
              //  oWe.PK_Mandant = boAe.PK_Mandant;
            }

            return oWe;
        }
        public static void CopyEntityPropertyValues(object source, object destination)
        {
            var destProperties = destination.GetType().GetProperties();

            foreach (var sourceProperty in source.GetType().GetProperties())
            {
                foreach (var destProperty in destProperties)
                {
                    if (destProperty.Name == sourceProperty.Name &&
                        destProperty.Name.ToString() != "PK" &&
                        destProperty.Name.ToString() != "EntityFramework" &&
                        destProperty.Name.ToString() != "HasValues" &&
                        destProperty.Name.ToString() != "HasErrors" &&
                        destProperty.Name.ToString() != "Error" &&
                        destProperty.Name.ToString() != "Item" &&
                        destProperty.Name.ToString() != "EntityState" &&
                        destProperty.Name.ToString() != "EntityKey" &&
                        destProperty.PropertyType.IsAssignableFrom(sourceProperty.PropertyType))
                    {
                        destProperty.SetValue(destination, sourceProperty.GetValue(
                            source, new object[] { }), new object[] { });

                        break;
                    }
                }
            }
        }
        public void DeleteOldAbrufe()
        {
            Einstellungen _boE = new Einstellungen();
            EinstellungenEntity _boEE = _boE.GetEinstellungen();
            double DeleteAfterNDay = 0;
            if (_boEE.AbrufeNachNTagenloeschen != null)
                DeleteAfterNDay = (double)_boEE.AbrufeNachNTagenloeschen;

            if (DeleteAfterNDay != 0)
            {
                DateTime today = DateTime.Today;
                DateTime nDaysEarlier = today.AddDays(-DeleteAfterNDay + 1);


                IQueryable<AbrufEntity> query = from a in ObjectContext.AbrufEntities
                                                where a.abrufDatum < nDaysEarlier || a.abrufFest != true

                                                select a;
                var ii = GetEntityList(query);

                DeleteEntityList();
            }
        }

	   
	}
}
