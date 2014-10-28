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

        public OrderitemEntity GetById(string id)
        {
            IQueryable<OrderitemEntity> query = from o in ObjectContext.OrderitemEntities
                                                where o.id == id
                                                select o;
            return GetEntity(query);
        }

        public mmBindingList<OrderitemEntity> GetByCustomerBusinessIdentifier(string ident)
        {
            IQueryable<OrderitemEntity> query = from o in ObjectContext.OrderitemEntities
                                                where o.customerBusinessIdentifier.Contains(ident)
                                                select o;
            return GetEntityList(query);
        }

        public mmBindingList<OrderitemEntity> GetByInvoiceReceiverBusinessIdentifier(string ident)
        {
            IQueryable<OrderitemEntity> query = from o in ObjectContext.OrderitemEntities
                                                where o.invoiceReceicerBusinessIdentifier.Contains(ident)
                                                select o;
            return GetEntityList(query);
        }

        public mmBindingList<OrderitemEntity> GetByOwnerBusinessIdentifier(string ident)
        {
            IQueryable<OrderitemEntity> query = from o in ObjectContext.OrderitemEntities
                                                where o.invoiceReceicerBusinessIdentifier.Contains(ident)
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

        public mmBindingList<OrderParentAndChild> GetOrderWithAllDetailsByMatchCode(string customerBi,
                                                                                    string invoiceReceiverBi,
                                                                                    string lagermandantBi,
                                                                                    string article)

        {
            var query = from o in ObjectContext.OrderitemEntities
                        from ois in ObjectContext.OrderItemserviceEntities
                        where o.PK == ois.PKOrderItem &
                              o.customerBusinessIdentifier.Contains(customerBi) &
                              o.invoiceReceicerBusinessIdentifier.Contains(invoiceReceiverBi) &
                              ois.supplierOrConsigneeBusinessIdentifier.Contains(lagermandantBi)
                        select new OrderParentAndChild
                                   {
                                       customerBusinessIdentifier = o.customerBusinessIdentifier,
                                       invoiceReceiverBusinessIdentifier = o.invoiceReceicerBusinessIdentifier,
                                       ownerBusinessIdentifier = ois.ownerBusinessIdentifier,
                                       remark = ois.remark,
                                       product = ois.product,
                                       productdescription =  ois.productdescription,
                                       supplierOrConsigneeBusinessIdentifiert = ois.supplierOrConsigneeBusinessIdentifier,
                                       articleId = ois.articleId,
                                       kindOfGoodDescription = ois.kindOfGoodDescription,
                                       deliveryType =  ois.deliveryType,
                                       articleDescription= ois.articleDescription,
                                       plannedDate = ois.plannedDate
                                       



                                       
                                  


                                   };
            var x = query.ToString();
            return GetEntityList(query);
        
        }
    }
}