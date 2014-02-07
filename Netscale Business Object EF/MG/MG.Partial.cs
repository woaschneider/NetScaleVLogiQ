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
    /// Summary description for MG.
    /// </summary>
    public partial class MG
    {
        /// <summary>
        /// Hook method automatically executed from the mmBusinessObject constructor
        /// </summary>
        protected override void HookConstructor()
        {
            // Place code here to be executed when the business object instantiates
        }

        public MGEntity GetMGById(int PK)
        {
            IQueryable<MGEntity> query = from MG in this.ObjectContext.MGEntities
                                         where MG.PK == PK
                                         select MG;
            return this.GetEntity(query);
        }

        public MGEntity GetMGByNr(string Nr)
        {
            IQueryable<MGEntity> query = from MG in this.ObjectContext.MGEntities
                                         where MG.SortenNr == Nr
                                         select MG;
            return this.GetEntity(query);
        }

        public mmBindingList<MGEntity> GetAllMG()
        {
            IQueryable<MGEntity> query = from a in ObjectContext.MGEntities
                                         orderby a.Sortenbezeichnung1
                                         select a;
            return GetEntityList(query);
        }

        public mmBindingList<MGEntity> GetAllMGByMatchCode(string MatchCode)
        {
            IQueryable<MGEntity> query = from a in ObjectContext.MGEntities
                                         orderby a.Sortenbezeichnung1
                                         where a.SortenNr.Contains(MatchCode) ||
                                               a.Sortenbezeichnung1.Contains(MatchCode) ||
                                               a.Sortenbezeichnung2.Contains(MatchCode)
                                         select a;
            return GetEntityList(query);
        }

        public void SetAllTouch2ToFalse()
        {
            IQueryable<MGEntity> query = from a in ObjectContext.MGEntities
                                         select a;
            var ii = GetEntityList(query);

            for (int i = 0; i < (ii.Count() - 1); i++)
            {
                ii[i].touch = false;

                // SaveEntity(ii[i]);
            }
            var uRet = this.SaveEntityList(ii);
        }

        public void DeleteAllNotTouch()
        {
            IQueryable<MGEntity> query = from a in ObjectContext.MGEntities
                                         where a.touch == false
                                         select a;
            var ii = GetEntityList(query);

            DeleteEntityList();
        }
    }
}