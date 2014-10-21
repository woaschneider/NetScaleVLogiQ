using System;
using HWB.NETSCALE.BOEF;
using NetScalePolosIO;
using Xceed.Wpf.Toolkit;

namespace HWB.NETSCALE.POLOSIO.AuftragsImport
{
    public class ImportAuftraege
    {
        private Orderitem boO = new Orderitem();
        private OrderitemEntity boOE;

        private HWB.NETSCALE.BOEF.OrderItemservice boOIS;
        private HWB.NETSCALE.BOEF.OrderItemserviceEntity boOISE;

        private HWB.NETSCALE.BOEF.OrderArticleAttribute boOAA;
        private HWB.NETSCALE.BOEF.OrderArticleAttributeEntity boOAAE;


        public bool Import(string FullQualifiedFileName)
        {
            var oOI = FullQualifiedFileName.CreateFromJsonFile<RootObject>();


            foreach (OrderEntity obj in oOI.orderEntities)
            {
                if (obj.id != null)
                {
                    boOE = boO.GetById(obj.id);
                }
                if (boOE == null)
                {
                    boOE = boO.NewEntity();
                }
                if (boOE != null)
                {
                    boOE.id = obj.id;
                    boOE.locationId = obj.locationId;
                    boOE.number = obj.number;
                    boOE.date = PolosUtitlities.ConvertPolosDateTime2DateTime(obj.date);
                    boOE.orderstate = obj.orderState;
                    boOE.reference = obj.reference;

                    // Adressen

                    #region Customer and Invoice

                    // Eigentich brauchen wir nur den BusinessIdentfier 
                    // Später vielleicht zusätzlich (redundant) nehmen wir noch PK und ID

                    //customer
                    boOE.customerBusinessIdentifier = obj.customer.businessIdentifier;
                    //invoice
                    boOE.invoiceReceicerBusinessIdentifier = obj.invoiceReceiver.businessIdentifier;

                    #endregion

                    boO.SaveEntity(boOE);
                    boOE = boO.GetById(boOE.id); // Damit ich jetzt den OK habe


                    // OrderItemservice

                    #region orderItemService

                    //foreach (OrderItemService obj2 in oOI.orderEntities.)
                    //{)

                    for (int i = 0; i < obj.orderItems[0].orderItemServices.Count; i++)
                    {
                        boOIS = new OrderItemservice();
                        boOISE = boOIS.GetById(boOE.PK, obj.orderItems[0].orderItemServices[i].identifier);
                        if (boOISE == null)
                        {
                            boOISE = boOIS.NewEntity();
                        }
                        boOISE.identifier = obj.orderItems[0].orderItemServices[i].identifier;
                        boOISE.remark = obj.orderItems[0].orderItemServices[i].remark;
                        boOISE.sequence = obj.orderItems[0].orderItemServices[i].sequence;
                        boOISE.product = obj.orderItems[0].product.id;

                        boOISE.productdescription = obj.orderItems[0].product.description; // Nicht in der Schnittstelle
                   
                        boOISE.remark = obj.orderItems[0].orderItemServices[i].remark;

                        boOISE.deliveryType = obj.orderItems[0].orderItemServices[i].deliveryType;

                        boOISE.articleId = obj.orderItems[0].orderItemServices[i].articleInstance.article.id;
                        boOISE.ownerId = obj.orderItems[0].orderItemServices[i].articleInstance.article.locationId;

                        boOISE.supplierOrConsigneeId = obj.orderItems[0].orderItemServices[i].supplierOrConsignee.id;
                        boOISE.supplierOrConsigneeBusinessIdentifier =
                            obj.orderItems[0].orderItemServices[i].supplierOrConsignee.businessIdentifier;
                        boOISE.supplierOrConsigneeName = obj.orderItems[0].orderItemServices[i].supplierOrConsignee.name;
                        boOISE.supplierOrConsigneeSubName2 =
                            obj.orderItems[0].orderItemServices[i].supplierOrConsignee.subName;
                        boOISE.supplierOrConsigneeCity =
                            obj.orderItems[0].orderItemServices[i].supplierOrConsignee.address.city;
                        boOISE.supplierOrConsigneeStreet =
                            obj.orderItems[0].orderItemServices[i].supplierOrConsignee.address.street;
                        boOISE.supplierOrConsigneeZipCode =
                            obj.orderItems[0].orderItemServices[i].supplierOrConsignee.address.zipCode;
                    //    boOISE.supplierOrConsigneeCountry =
                      //      obj.orderItems[0].orderItemServices[i].supplierOrConsignee.address.country;

                        boOISE.PKOrderItem = boOE.PK;
                        boOIS.SaveEntity(boOISE);
                    }

                    //}

                    #endregion

                    // Save the stuff
                }
            }

            return true;
        }
    }
}