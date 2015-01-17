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
    /// Summary description for Fahrzeuge.
    /// </summary>
    public partial class Fahrzeuge
    {
        /// <summary>
        /// Hook method automatically executed from the mmBusinessObject constructor
        /// </summary>
        protected override void HookConstructor()
        {
            // Place code here to be executed when the business object instantiates
        }

        public override void StateChangeHandler(mmBaseBusinessObject bizObj, mmBusinessStateChangeEventArgs e)
        {
            if (this.State == mmBusinessState.PreSaving)
            {
                PreSaveHook();
            }

            if (this.State == mmBusinessState.Added)
            {
                OnNew();
            }
        }


        private void PreSaveHook()
        {
            this.Entity.Kennzeichen1Raw = ConvertKfzToKfzRaw(this.Entity.Kennzeichen1);
        }

        private void OnNew()
        {
          

        }




        public FahrzeugeEntity GetByPk(int pk)
        {
            IQueryable<FahrzeugeEntity> query = from f in ObjectContext.FahrzeugeEntities
                                                where f.PK == pk
                                                select f;
            return GetEntity(query);
        }

        public FahrzeugeEntity GetByExactKennzeichen(string kennzeichen)
        {
            IQueryable<FahrzeugeEntity> query = from f in ObjectContext.FahrzeugeEntities
                                                where f.Kennzeichen1 == kennzeichen
                                                select f;
            return GetEntity(query);
        }

        public mmBindingList<FahrzeugeEntity> GetByKennzeichen(string mc)
        {
            IQueryable<FahrzeugeEntity> query = from f in ObjectContext.FahrzeugeEntities
                                                where f.Kennzeichen1.Contains(mc)
                                                select f;
            return GetEntityList(query);
        }

        private string ConvertKfzToKfzRaw(string kfz)
        {
            string a = kfz.Replace("-", "");
            return a.Replace(" ", "");
        }
    }

}