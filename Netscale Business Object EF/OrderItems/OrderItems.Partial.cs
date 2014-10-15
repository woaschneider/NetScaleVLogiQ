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
	/// Summary description for OrderItems.
	/// </summary>
    public partial class OrderItems
    {
        /// <summary>
        /// Hook method automatically executed from the mmBusinessObject constructor
        /// </summary>
        protected override void HookConstructor()
        {
            // Place code here to be executed when the business object instantiates
        }

        public mmBindingList<orderItemEntity> GetAll()
        {
            IQueryable<orderItemEntity> query = from a in ObjectContext.orderItemEntities
                                                orderby a.number
                                                select a;
            return GetEntityList(query);
        }

        public orderItemEntity GetById( string id)
        {
            IQueryable<orderItemEntity> query = from a in ObjectContext.orderItemEntities
                                                where a.id == id
                                                select a;
            return GetEntity(query);
            
        }

        public orderItemEntity GetByPK(int  pk)
        {
            IQueryable<orderItemEntity> query = from a in ObjectContext.orderItemEntities
                                                where a.PK == pk
                                                select a;
            return GetEntity(query);

        }
    }
}
