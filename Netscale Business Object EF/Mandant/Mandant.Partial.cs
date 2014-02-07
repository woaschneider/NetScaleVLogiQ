using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.EntityClient;
using System.Linq;
using HWB.NETSCALE.GLOBAL;
using OakLeaf.MM.Main.Business;
using OakLeaf.MM.Main.Collections;
using OakLeaf.MM.Main.Data;

namespace HWB.NETSCALE.BOEF
{
    /// <summary>
    /// Summary description for Mandant.
    /// </summary>
    public partial class Mandant
    {
        /// <summary>
        /// Hook method automatically executed from the mmBusinessObject constructor
        /// </summary>
        protected override void HookConstructor()
        {
            // Place code here to be executed when the business object instantiates
        }

        public mmBindingList<MandantEntity> GetAllMandant()
        {
            IQueryable<MandantEntity> query = from a in ObjectContext.MandantEntities
                                              orderby a.MandantNr
                                              select a;
            return GetEntityList(query);
        }

        public MandantEntity GetDefaultMandant()
        {
            IQueryable<MandantEntity> query = from Mandant in this.ObjectContext.MandantEntities
                                              where Mandant.DefaultMandant == true
                                              select Mandant;
            return this.GetEntity(query);
        }


        // Gibt den PK des Default-Mandanten
        public int GetDefaultMandantPK()
        {
            IQueryable<MandantEntity> query = from Mandant in this.ObjectContext.MandantEntities
                                              where Mandant.DefaultMandant == true
                                              select Mandant;
            var oME = this.GetEntity(query);
            return oME.PK;
        }

        public MandantEntity GetMandantByPK(int? pk)
        {
            IQueryable<MandantEntity> query = from Mandant in this.ObjectContext.MandantEntities
                                              where Mandant.PK == pk
                                              select Mandant;
            return this.GetEntity(query);
        }

        public bool IsLsdruck(int pk)
        {
            IQueryable<MandantEntity> query = from Mandant in this.ObjectContext.MandantEntities
                                              where Mandant.PK == pk
                                              select Mandant;
            var Result = this.GetEntity(query);
            if (Result.LSDruck == true)
                return true;
            else
            {
                return false;
            }
        }

        public string GetLsNr(WaegeEntity oWE)
        {  
            string lsnr = "";
            MandantEntity oME = GetMandantByPK(goApp.Mandant_PK);
            if(oME!=null)
            {    //
                if (oWE.BonitaetKz=="7")
                {
                    int? x = oME.lsnrbar;
                    x = x + 1;
                    oME.lsnrbar= x;
                    SaveEntity(oME);
                    lsnr = x.ToString();
                    return lsnr;
                }
                // Jetzt prüfen wir nur die Wiegeart
                if(oWE.WiegeartKz== "A")
                {
                    int? x = oME.lsnrausgang;
                    x = x + 1;
                    oME.lsnrausgang = x;
                    SaveEntity(oME);
                    lsnr = x.ToString();
                }

                if (oWE.WiegeartKz == "E")
                {
                    int? x = oME.lsnreingang;
                    x = x + 1;
                    oME.lsnreingang  = x;
                    SaveEntity(oME);
                    lsnr = x.ToString();
                }

                if (oWE.WiegeartKz == "R")
                {
                    int? x = oME.lsnrrecycling;
                    x = x + 1;
                    oME.lsnrrecycling  = x;
                    SaveEntity(oME);
                    lsnr = x.ToString();
                    return lsnr;
                }

                if (oWE.WiegeartKz == "P")
                {
                    int? x = oME.lsnrprobrecycling;
                    x = x + 1;
                    oME.lsnrprobrecycling  = x;
                    SaveEntity(oME);
                    lsnr = x.ToString();
                }

                if (oWE.WiegeartKz == "K")
                {
                    int? x = oME.lsnrkontrol;
                    x = x + 1;
                    oME.lsnrkontrol = x;
                    SaveEntity(oME);
                    lsnr = x.ToString();
                }
            }

            return lsnr;
        }
    }
}