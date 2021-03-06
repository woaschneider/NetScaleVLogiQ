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
    /// Summary description for Arbeitsleistungsfilter.
    /// </summary>
    public partial class Arbeitsleistungsfilter
    {
        /// <summary>
        /// Hook method automatically executed from the mmBusinessObject constructor
        /// </summary>
        protected override void HookConstructor()
        {
            // Place code here to be executed when the business object instantiates
        }

        public mmBindingList<ArbeitsleistungsfilterEntity> GetAll()
        {
            IQueryable<ArbeitsleistungsfilterEntity> query = from a in ObjectContext.ArbeitsleistungsfilterEntities
                select a;
            return GetEntityList(query);
        }

        public string GetServiceByProduct(string productId)
        {
            IQueryable<ArbeitsleistungsfilterEntity> query = from a in ObjectContext.ArbeitsleistungsfilterEntities
                where a.ProduktId == productId
                select a;
            ArbeitsleistungsfilterEntity oAe = GetEntity(query);
            if (oAe != null)

            {
                return oAe.ServicesId;
            }
            else
            {
                return "";
            }
        }
    }
}