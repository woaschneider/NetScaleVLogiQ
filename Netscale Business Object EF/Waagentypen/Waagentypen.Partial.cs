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
    /// Summary description for Waagentypen.
    /// </summary>
    public partial class Waagentypen
    {
        /// <summary>
        /// Hook method automatically executed from the mmBusinessObject constructor
        /// </summary>
        protected override void HookConstructor()
        {
            // Place code here to be executed when the business object instantiates
        }

        public mmBindingList<WaagentypenEntity> GetAllWT()
        {
            IQueryable<WaagentypenEntity> query = from a in ObjectContext.WaagentypenEntities
                                                  where a.implementiert == true
                                                  orderby a.WaagenID
                                                  select a;
            return GetEntityList(query);
        }

        public WaagentypenEntity GetWTByPK(int PK)
        {
            IQueryable<WaagentypenEntity> query = from a in this.ObjectContext.WaagentypenEntities
                                                  where a.PK == PK
                                                  select a;
            return this.GetEntity(query);
        }
    }
}