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
    /// Summary description for KbFnn.
    /// </summary>
    public partial class KbFnn
    {
        /// <summary>
        /// Hook method automatically executed from the mmBusinessObject constructor
        /// </summary>
        protected override void HookConstructor()
        {
            // Place code here to be executed when the business object instantiates
        }

        public mmBindingList<KbFnnEntity> GetAllKbFn()
        {
            IQueryable<KbFnnEntity> query = from a in ObjectContext.KbFnnEntities
                                            orderby a.ActionNr
                                            select a;
            return GetEntityList(query);
        }

        public KbFnnEntity GetKbFnnByKey(string Key)
        {
            IQueryable<KbFnnEntity> query = from KbFnn in this.ObjectContext.KbFnnEntities
                                            where KbFnn.Command == Key
                                            select KbFnn;
            return this.GetEntity(query);
        }

        public KbFnnEntity GetKbFnnByActionNr(string Nr)
        {
            IQueryable<KbFnnEntity> query = from KbFnn in this.ObjectContext.KbFnnEntities
                                            where KbFnn.ActionNr == Nr
                                            select KbFnn;
            return this.GetEntity(query);
        }
    }
}