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
    /// Summary description for SvMandant.
    /// </summary>
    public partial class SvMandant
    {
        /// <summary>
        /// Hook method automatically executed from the mmBusinessObject constructor
        /// </summary>
        protected override void HookConstructor()
        {
            // Place code here to be executed when the business object instantiates
        }

        public mmBindingList<SvMandantEntity> GetAllMandant()
        {
            IQueryable<SvMandantEntity> query = from a in ObjectContext.SvMandantEntities
                                                orderby a.MandantNr
                                                select a;
            return GetEntityList(query);
        }

        public SvMandantEntity GetDefaultMandant()
        {
            IQueryable<SvMandantEntity> query = from SvMandant in this.ObjectContext.SvMandantEntities
                                                where SvMandant.DefaultMandant == true
                                                select SvMandant;
            return this.GetEntity(query);
        }

        // Gibt den PK des Default-Mandanten
        public int GetDefaultMandantPK()
        {
            IQueryable<SvMandantEntity> query = from SvMandant in this.ObjectContext.SvMandantEntities
                                                where SvMandant.DefaultMandant == true
                                                select SvMandant;
            var oME = this.GetEntity(query);
            return oME.PK;
        }

        public SvMandantEntity GetMandantByPK(int pk)
        {
            IQueryable<SvMandantEntity> query = from SvMandant in this.ObjectContext.SvMandantEntities
                                                where SvMandant.PK == pk
                                                select SvMandant;
            return this.GetEntity(query);
        }
    }
}