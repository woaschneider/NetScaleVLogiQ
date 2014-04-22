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
    /// Summary description for Einstellungen.
    /// </summary>
    public partial class Einstellungen
    {
        /// <summary>
        /// Hook method automatically executed from the mmBusinessObject constructor
        /// </summary>
        protected override void HookConstructor()
        {
            // Place code here to be executed when the business object instantiates
        }

        // TODO: Setzt im Moment voraus das es nur einen DS in Tabelle Einstellungen gibt. Das ist nicht schön!
        public string NewLsNrGlobal()
        {
            string LSNRGlobal  = "";

            IQueryable<EinstellungenEntity> query = from E in this.ObjectContext.EinstellungenEntities
                                                    select E;
            EinstellungenEntity oEE = this.GetEntity(query);
            LSNRGlobal  = oEE.LSNRGlobal.ToString();
            oEE.LSNRGlobal = oEE.LSNRGlobal + 1;
            SaveEntity(oEE);
            return LSNRGlobal ;
        }

        public int? NewAp_Id()
        {
            int? Ret = 0;
            IQueryable<EinstellungenEntity> query = from E in this.ObjectContext.EinstellungenEntities
                                                    select E;
            EinstellungenEntity oEE = this.GetEntity(query);
            Ret = oEE.AP_Id_counter;
            oEE.AP_Id_counter = oEE.AP_Id_counter + 1;
            SaveEntity(oEE);


            return Ret;
        }
        public int? NewMg_Id()
        {
            int? Ret = 0;
            IQueryable<EinstellungenEntity> query = from E in this.ObjectContext.EinstellungenEntities
                                                    select E;
            EinstellungenEntity oEE = this.GetEntity(query);
            Ret = oEE.MG_Id_counter;
            oEE.MG_Id_counter = oEE.MG_Id_counter + 1;
            SaveEntity(oEE);


            return Ret;
        }

        public int GetMaxGewicht()
        {
            IQueryable<EinstellungenEntity> query = from E in this.ObjectContext.EinstellungenEntities
                                                    select E;
            EinstellungenEntity oEE = this.GetEntity(query);

            if (oEE != null)
            {
                if (oEE.MaxGewicht == null)
                    return 0;
                else
                {
                    return (int) oEE.MaxGewicht;
                }
            }
            else
            {
                return 0;
            }
        }

        public bool GetMaxGewichtValidieren()
        {
            IQueryable<EinstellungenEntity> query = from E in this.ObjectContext.EinstellungenEntities
                                                    select E;
            EinstellungenEntity oEE = this.GetEntity(query);

            if (oEE != null)
            {
                if (oEE.MaxGewichtValidieren == null)
                    return false;
                else
                {
                    return (bool) oEE.MaxGewichtValidieren;
                }
            }
            else
            {
                return false;
            }
        }

        public EinstellungenEntity GetEinstellungen()
        {
            IQueryable<EinstellungenEntity> query = from E in this.ObjectContext.EinstellungenEntities
                                                    select E;
            return this.GetEntity(query);
        }
    }
}