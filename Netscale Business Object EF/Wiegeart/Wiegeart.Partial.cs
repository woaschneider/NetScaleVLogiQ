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
    /// Summary description for Wiegeart.
    /// </summary>
    public partial class Wiegeart
    {
        /// <summary>
        /// Hook method automatically executed from the mmBusinessObject constructor
        /// </summary>
        protected override void HookConstructor()
        {
            // Place code here to be executed when the business object instantiates
        }

        public WiegeartEntity GetWiegeartByKz(string kz)
        {
            IQueryable<WiegeartEntity> query = from b in this.ObjectContext.WiegeartEntities
                                               where b.Kennung == kz
                                               select b;
            return this.GetEntity(query);
        }

        public WiegeartEntity GetDefaultWiegeart()
        {
            IQueryable<WiegeartEntity> query = from b in this.ObjectContext.WiegeartEntities
                                               where b.DefaultW == true
                                               select b;
            return this.GetEntity(query);
        }

        public mmBindingList<WiegeartEntity> GetAllWiegeart()
        {
            IQueryable<WiegeartEntity> query = from b in this.ObjectContext.WiegeartEntities
                                               orderby b.Kennung
                                               select b;


            ;
            return this.GetEntityList(query);
        }

        public void SetDefaultWiegeart(string kz)
        {
            // Prüfen: Gibt es diese Wiegeart?
            var CheckAktuell = GetWiegeartByKz(kz);
            if (CheckAktuell != null)
            {
                // Dann setze das alte Default kz zurück
                var CheckAlt = GetDefaultWiegeart();
                if (CheckAlt != null)
                {
                    CheckAlt.DefaultW = false;
                    this.SaveEntity(CheckAlt);
                }
                CheckAktuell.DefaultW = true;
                this.SaveEntity(CheckAktuell);
            }
        }
    }
}