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
    /// Summary description for Incoterm.
    /// </summary>
    public partial class Incoterm
    {
        /// <summary>
        /// Hook method automatically executed from the mmBusinessObject constructor
        /// </summary>
        protected override void HookConstructor()
        {
            // Place code here to be executed when the business object instantiates
        }

        public IncotermEntity GetIncotermByKz(string kz)
        {
            IQueryable<IncotermEntity> query = from b in this.ObjectContext.IncotermEntities
                                               where b.Kennung == kz
                                               select b;
            return this.GetEntity(query);
        }

        public IncotermEntity GetDefaultIncoterm()
        {
            IQueryable<IncotermEntity> query = from b in this.ObjectContext.IncotermEntities
                                               where b.DefaultIncoterm == true
                                               select b;
            return this.GetEntity(query);
        }

        public mmBindingList<IncotermEntity> GetAllIncoterm()
        {
            IQueryable<IncotermEntity> query = from b in this.ObjectContext.IncotermEntities
                                               orderby b.Kennung
                                               select b;


            return this.GetEntityList(query);
        }

        public void SetDefaultIncoterm(string kz)
        {
            var CheckAktuell = GetIncotermByKz(kz);
            if (CheckAktuell != null)
            {
                // Dann setze das alte Default zurück
                var CheckAlt = GetDefaultIncoterm();
                if (CheckAlt != null)
                {
                    CheckAlt.DefaultIncoterm = false;
                    this.SaveEntity(CheckAlt);
                }
                CheckAktuell.DefaultIncoterm = true;
                this.SaveEntity(CheckAktuell);
            }
        }
    }
}