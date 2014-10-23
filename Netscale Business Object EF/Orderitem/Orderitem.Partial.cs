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
	/// Summary description for Orderitem.
	/// </summary>
	public partial class Orderitem
	{
		/// <summary>
		/// Hook method automatically executed from the mmBusinessObject constructor
		/// </summary>
		protected override void HookConstructor()
		{
			// Place code here to be executed when the business object instantiates
		}

        public OrderitemEntity GetById(string id)
        {
            IQueryable<OrderitemEntity> query = from o in ObjectContext.OrderitemEntities
                                                where o.id == id
                                                select o;
            return GetEntity(query);
        }
        public mmBindingList< OrderitemEntity> GetByBusinessIdentifier(string ident)
        {
            IQueryable<OrderitemEntity> query = from o in ObjectContext.OrderitemEntities
                                                where o.customerBusinessIdentifier.Contains(ident)
                                                select o;
            return GetEntityList(query);
        }

        public mmBindingList<OrderitemEntity> GetAll()
        {
            IQueryable<OrderitemEntity> query = from o in ObjectContext.OrderitemEntities
                                              
                                                select o;

            var uRet  = GetEntityList(query);
            return uRet;
        }

        public mmBindingList<OrderitemEntity> GetByCustomerIdentifier(string mc)
        {
            IQueryable<OrderitemEntity> query = from o in ObjectContext.OrderitemEntities
                                                       where o.customerBusinessIdentifier.Contains(mc)
                                                       select o;
            return GetEntityList(query);
        }
	}
}
