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
    /// Summary description for Bonitaet.
    /// </summary>
    public partial class Bonitaet
    {
        /// <summary>
        /// Hook method automatically executed from the mmBusinessObject constructor
        /// </summary>
        protected override void HookConstructor()
        {
            // Place code here to be executed when the business object instantiates
        }

        public BonitaetEntity GetBonitaetByKz(string kz)
        {
            IQueryable<BonitaetEntity> query = from b in this.ObjectContext.BonitaetEntities
                                               where b.Kennung == kz
                                               select b;
            return this.GetEntity(query);
        }

        public mmBindingList<BonitaetEntity> GetAllBonitaeten()
        {
            IQueryable<BonitaetEntity> query = from b in this.ObjectContext.BonitaetEntities
                                               orderby b.Kennung
                                               select b;


            ;
            return this.GetEntityList(query);
        }
    }
}