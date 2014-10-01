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

        public MandantEntity GetMandantByNr(string nr)
        {
            IQueryable<MandantEntity> query = from Mandant in this.ObjectContext.MandantEntities
                                              where Mandant.MandantNr.Trim() == nr
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

      
    }
}