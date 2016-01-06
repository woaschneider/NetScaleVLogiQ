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

        public OrderItemserviceEntity GetByIdAndPKOrderItem(int pkorderitem, string id)
        {
            IQueryable<OrderItemserviceEntity> query = from o in ObjectContext.OrderItemserviceEntities
                                                       where o.identifierOItemService == id && o.PKOrderItem== pkorderitem
                                                       select o;
            return GetEntity(query);
        }
        public OrderItemserviceEntity GetByPK(int pk)
        {
            IQueryable<OrderItemserviceEntity> query = from o in ObjectContext.OrderItemserviceEntities
                                                       where o.PK == pk
                                                       
                                                       select o;
            return GetEntity(query);
        }
        public OrderItemserviceEntity GetByIdentitifier(string ident)
        {
            IQueryable<OrderItemserviceEntity> query = from o in ObjectContext.OrderItemserviceEntities
                                                       where o.identifierOItem == ident
                                                       select o;
            return GetEntity(query);
        }

        public mmBindingList<OrderItemserviceEntity> GetByParentPK(int pk)
        {
            IQueryable<OrderItemserviceEntity> query = from o in ObjectContext.OrderItemserviceEntities
                where o.PKOrderItem == pk & o.InvisibleSendedOrderItems == false & o.HasBinUsed==false
              
            select o;
            var uRet = GetEntityList(query);
            uRet.OrderBy(oo => int.Parse(oo.sequence)); // Strings numerisch sortiert
            return uRet ;

        }
         
	}
}
