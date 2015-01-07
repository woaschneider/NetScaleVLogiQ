using System;
using System.Collections.Generic;
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

        public OrderitemEntity GetByPk(int? pk)
        {
            IQueryable<OrderitemEntity> query = from o in ObjectContext.OrderitemEntities
                                                where o.PK == pk
                                                select o;
            return GetEntity(query);
        }

        public OrderitemEntity GetById(string id)
        {
            IQueryable<OrderitemEntity> query = from o in ObjectContext.OrderitemEntities
                                                where o.id == id
                                                select o;
            return GetEntity(query);
        }

      

      


        public mmBindingList<OrderitemEntity> GetByAU_RE_KR_MatchCode(string customerBi,
                                                                                 string invoiceReceiverBi, string kundenreferenz,
                                                                               string artikelbeschreibung, string freistellung  )
        {
            IQueryable<OrderitemEntity> query = from o in ObjectContext.OrderitemEntities
                                                from ois in ObjectContext.OrderItemserviceEntities
                                                where o.PK== ois.PKOrderItem &&
                                                o.customerBusinessIdentifier.Contains(customerBi) &&
                                                o.invoiceReceicerBusinessIdentifier.Contains(invoiceReceiverBi)&&
                                                o.reference.Contains(kundenreferenz) &&
                                                ois.articleDescription.Contains(artikelbeschreibung)&&
                                                ois.clearanceReferenz.Contains(freistellung)

                                                orderby o.customerBusinessIdentifier
                                                select o;
            return GetEntityList(query);
        }

        public mmBindingList<OrderitemEntity> GetAll()
        {
            IQueryable<OrderitemEntity> query = from o in ObjectContext.OrderitemEntities
                                                select o;

            var uRet = GetEntityList(query);
            return uRet;
        }


    }
}