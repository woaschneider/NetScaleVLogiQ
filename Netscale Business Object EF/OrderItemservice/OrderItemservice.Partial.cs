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
	/// Summary description for OrderItemservice.
	/// </summary>
	public partial class OrderItemservice
	{
		/// <summary>
		/// Hook method automatically executed from the mmBusinessObject constructor
		/// </summary>
		protected override void HookConstructor()
		{
			// Place code here to be executed when the business object instantiates
		}

        public OrderItemserviceEntity GetById(int pkorderitem, string id)
        {
            IQueryable<OrderItemserviceEntity> query = from o in ObjectContext.OrderItemserviceEntities
                                                       where o.identifier == id & o.PKOrderItem== pkorderitem
                                                       select o;
            return GetEntity(query);
        }
	}
}
