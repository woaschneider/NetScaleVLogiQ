using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.EntityClient;
using System.Linq;
using HWB.NETSCALE.BOEF.JoinClasses;
using OakLeaf.MM.Main.Business;
using OakLeaf.MM.Main.Collections;
using OakLeaf.MM.Main.Data;

namespace HWB.NETSCALE.BOEF
{
    /// <summary>
    /// Summary description for AP.
    /// </summary>
    public partial class AP
    {
        /// <summary>
        /// Hook method automatically executed from the mmBusinessObject constructor
        /// </summary>
        protected override void HookConstructor()
        {
            // Place code here to be executed when the business object instantiates
        }

        public APEntity GetAPById(int? PK)
        {
            IQueryable<APEntity> query = from AP in this.ObjectContext.APEntities
                                         where AP.PK == PK
                                         select AP;
            return this.GetEntity(query);
        }

        public APEntity GetAPByNr(string Nr)
        {
            IQueryable<APEntity> query = from AP in this.ObjectContext.APEntities
                                         where AP.Nr.Trim() == Nr
                                         select AP;
            return this.GetEntity(query);
        }

        public APEntity GetAPByNr(string Nr, string rollenkz)
        {
            IQueryable<APEntity> query;
            switch (rollenkz)
            {
                case "AU":
                    query = from AP in this.ObjectContext.APEntities
                            where AP.Nr == Nr && AP.Rolle_AU == true
                            select AP;
                    return this.GetEntity(query);
                case "LI":
                    query = from AP in this.ObjectContext.APEntities
                            where AP.Nr == Nr && AP.Rolle_LI == true
                            select AP;
                    return this.GetEntity(query);
                case "SP":
                    query = from AP in this.ObjectContext.APEntities
                            where AP.Nr == Nr && AP.Rolle_SP == true
                            select AP;
                    return this.GetEntity(query);
                case "FU":
                    query = from AP in this.ObjectContext.APEntities
                            where AP.Nr == Nr && AP.Rolle_FU == true
                            select AP;
                    return this.GetEntity(query);

                default:
                    query = from AP in this.ObjectContext.APEntities
                            where AP.Nr == Nr
                            select AP;
                    return this.GetEntity(query);
            }
        }

        public mmBindingList<APEntity> GetAllAP()
        {
            IQueryable<APEntity> query = from a in ObjectContext.APEntities
                                         orderby a.Name1
                                         select a;
            return GetEntityList(query);
        }

        public mmBindingList<APEntity> GetAllAPByMatchCode(string MatchCode)
        {
            IQueryable<APEntity> query = from a in ObjectContext.APEntities
                                         orderby a.Name1
                                         where a.Nr.Contains(MatchCode) ||
                                               a.Firma.Contains(MatchCode) ||
                                               a.Name1.Contains(MatchCode) ||
                                               a.Anschrift.Contains(MatchCode) ||
                                               a.Plz.Contains(MatchCode) ||
                                               a.Ort.Contains(MatchCode)
                                         select a;
            return GetEntityList(query);
        }

        public mmBindingList<APEntity> GetAllAPByMatchCode(string MatchCode, string rollenkz)
        {
            IQueryable<APEntity> query;

            switch (rollenkz)
            {
                case "AU":
                    query = from a in ObjectContext.APEntities
                            orderby a.Name1
                            where (a.Nr.Contains(MatchCode) ||
                                   a.Firma.Contains(MatchCode) ||
                                   a.Name1.Contains(MatchCode) ||
                                   a.Anschrift.Contains(MatchCode) ||
                                   a.Plz.Contains(MatchCode) ||
                                   a.Ort.Contains(MatchCode)) & a.Rolle_AU == true
                            select a;
                    return GetEntityList(query);

                case "LI":
                    query = from a in ObjectContext.APEntities
                            orderby a.Name1
                            where (a.Nr.Contains(MatchCode) ||
                                   a.Firma.Contains(MatchCode) ||
                                   a.Name1.Contains(MatchCode) ||
                                   a.Anschrift.Contains(MatchCode) ||
                                   a.Plz.Contains(MatchCode) ||
                                   a.Ort.Contains(MatchCode)) & a.Rolle_LI == true
                            select a;
                    return GetEntityList(query);

                case "SP":
                    query = from a in ObjectContext.APEntities
                            orderby a.Name1
                            where (a.Nr.Contains(MatchCode) ||
                                   a.Firma.Contains(MatchCode) ||
                                   a.Name1.Contains(MatchCode) ||
                                   a.Anschrift.Contains(MatchCode) ||
                                   a.Plz.Contains(MatchCode) ||
                                   a.Ort.Contains(MatchCode)) & a.Rolle_SP == true
                            select a;
                    return GetEntityList(query);

                case "FU":
                    query = from a in ObjectContext.APEntities
                            orderby a.Name1
                            where (a.Nr.Contains(MatchCode) ||
                                   a.Firma.Contains(MatchCode) ||
                                   a.Name1.Contains(MatchCode) ||
                                   a.Anschrift.Contains(MatchCode) ||
                                   a.Plz.Contains(MatchCode) ||
                                   a.Ort.Contains(MatchCode)) & a.Rolle_FU == true
                            select a;
                    return GetEntityList(query);

                default: // Alles 
                    query = from a in ObjectContext.APEntities
                            orderby a.Name1
                            where a.Nr.Contains(MatchCode) ||
                                  a.Firma.Contains(MatchCode) ||
                                  a.Name1.Contains(MatchCode) ||
                                  a.Anschrift.Contains(MatchCode) ||
                                  a.Plz.Contains(MatchCode) ||
                                  a.Ort.Contains(MatchCode)
                            select a;
                    return GetEntityList(query);
            }
        }

       


       


        public void SetAllTouch2False()
        {
            IQueryable<APEntity> query = from a in ObjectContext.APEntities
                                         orderby a.Name1
                                         select a;
            var ii = GetEntityList(query);

            for (int i = 0; i < (ii.Count() - 1); i++)
            { if(ii[i].Rolle_FU == false &  ii[i].Rolle_SP == false)
                ii[i].touch = false;

                // SaveEntity(ii[i]);
            }
            var uRet = this.SaveEntityList(ii);
        }

        // Speditionen werden nicht gelöscht, da diese händisch angelegt werden und nicht aus dem Import kommen
        // Das gleich gilt für Fuhrunternehmer
        public void DeleteAllNotTouch()
        {
            IQueryable<APEntity> query = from a in ObjectContext.APEntities
                                         orderby a.Name1
                                         where a.touch == false
                                         select a;
            var ii = GetEntityList(query);

            DeleteEntityList();
        }
    }
}