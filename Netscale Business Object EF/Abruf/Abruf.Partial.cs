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
    public partial class  Abruf
    {
        public override void StateChangeHandler(mmBaseBusinessObject bizObj, mmBusinessStateChangeEventArgs e)
        {
            if (this.State == mmBusinessState.PreSaving)
            {
                PreSaveHook(this.Entity);
            }
        }

        public void PreSaveHook(AbrufEntity boWE)
        {
           
        }

    

        /// <summary>
        /// Hook method automatically executed from the mmBusinessObject constructor
        /// </summary>
        protected override void HookConstructor()
        {
            // Place code here to be executed when the business object instantiates
            StateChange += new mmBusinessStateChangeDelegate(this.StateChangeHandler);
        }

        public AbrufEntity  CreateAbruf(WaegeEntity oWE,decimal ist, decimal soll, decimal rest)
        {
            
            Abruf boA = new Abruf();
            AbrufEntity boAE = boA.NewEntity();

           

            CopyEntityPropertyValues(oWE, boAE);
            if(oWE.Abrufnr==null)
            boAE.Abrufnr = GetNextFreeAbrufNr();
            boAE.PK_Mandant = oWE.PK_Mandant;
            boAE.Istmenge = ist;
            boAE.Restmenge = rest;
            boAE.Sollmenge = soll;
            boAE.abruffest = true;
            boAE.abrufdatum = DateTime.Today;
            boA.SaveEntity(boAE);
            return boAE;
        }

        public AbrufEntity  CreateAbrufautomatically(WaegeEntity oWe)
        {
            AbrufEntity boAe = NewEntity();
            CopyEntityPropertyValues(oWe, boAe);
            boAe.PK_Mandant = oWe.PK_Mandant;
            boAe.Abrufnr = GetNextFreeAbrufNr();
            boAe.abrufdatum = DateTime.Today;
            boAe.Istmenge = 0;
            boAe.Restmenge= 0;
            boAe.Sollmenge = 0;
            SaveEntity(boAe);
            return boAe;
        }

        public WaegeEntity CopyAbrufToWaege(int? pk, WaegeEntity oWe)
        {
            AbrufEntity boAe = GetAbrufById(pk);
            if (boAe != null)
            {
                CopyEntityPropertyValues(boAe, oWe);
                oWe.Abrufid = boAe.PK;
                oWe.Abrufnr = boAe.Abrufnr;
                oWe.PK_Mandant = boAe.PK_Mandant;
            }

            return oWe;
        }

        // Es wurde ein Abruf verwendet und nachträglich wurde etwas geändert
        public bool HasAbrufChanged(WaegeEntity boWE)
        {
            // Vergleiche
            bool uRet = false;
            AbrufEntity oAE = GetAbrufByNr(boWE.Abrufnr);
            if (oAE != null)
            {
                if (oAE.NrKU != boWE.NrKU)
                {
                    uRet = true;
                }

                if (oAE.FirmaKU != boWE.FirmaKU)
                {
                    uRet = true;
                }

                if (oAE.SortenNr != boWE.SortenNr)
                {
                    uRet = true;
                }
                if (oAE.Sortenbezeichnung1 != boWE.Sortenbezeichnung1)
                {
                    uRet = true;
                }
                if (oAE.WiegeartKz != boWE.WiegeartKz)
                {
                    uRet= true;
                }
                if (oAE.wefirma != boWE.wefirma)
                {
                    uRet = true;
                }
                if (oAE.wename1 != boWE.wename1)
                {
                    uRet = true;
                }
                return uRet;
            }
            else
            {
                return false;
            }
        }

        public AbrufEntity CompareAbrufData(WaegeEntity oWe)
        {
            IQueryable<AbrufEntity> query = from a in ObjectContext.AbrufEntities
                                            orderby a.Abrufnr
                                            where a.NrKU == oWe.NrKU &&
                                                  a.FirmaKU == oWe.FirmaKU &&
                                                  a.SortenNr == oWe.SortenNr &&
                                                  a.Sortenbezeichnung1 == oWe.Sortenbezeichnung1 &&
                                                  a.WiegeartKz == oWe.WiegeartKz &&
                                                  a.wefirma == oWe.wefirma &&
                                                  a.wename1 == oWe.wename1
                                            select a;
            AbrufEntity oAE = GetEntity(query);

            return oAE;
        }

        public AbrufEntity SaveAbruf(WaegeEntity oWe)
        {
            int? pk = oWe.Abrufid;
            AbrufEntity boAE = GetAbrufById(pk);
            CopyEntityPropertyValues(oWe, boAE);
            boAE.Istmenge = oWe.Istmenge ;
            boAE.Restmenge = oWe.Restmenge ;
            boAE.Sollmenge = oWe.Sollmenge ;
            SaveEntity(boAE);
            return boAE;
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
                                            orderby a.Abrufnr
                                            select a;
            return GetEntityList(query);
        }

        public AbrufEntity GetAbrufById(int? pk)
        {
            IQueryable<AbrufEntity> query = from a in ObjectContext.AbrufEntities
                                            where a.PK == pk
                                            select a;
            return GetEntity(query);
        }

        public AbrufEntity GetAbrufByNr(string nr)
        {
            IQueryable<AbrufEntity> query = from a in ObjectContext.AbrufEntities
                                            where a.Abrufnr == nr
                                            select a;
            return GetEntity(query);
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
                            source, new object[] {}), new object[] {});

                        break;
                    }
                }
            }
        }
        public void DeleteOldAbrufe()
        {
            IQueryable<AbrufEntity> query = from a in ObjectContext.AbrufEntities
                                            where (a.abrufdatum < DateTime.Today | a.abrufdatum == null ) &
                                            a.abruffest!=true

                                         select a;
            var ii = GetEntityList(query);

            DeleteEntityList();
        }

      

    }
}