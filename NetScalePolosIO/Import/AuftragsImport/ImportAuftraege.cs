using System;
using HWB.NETSCALE.BOEF;
using NetScalePolosIO;
using Xceed.Wpf.Toolkit;

namespace HWB.NETSCALE.POLOSIO.AuftragsImport
{
    public class ImportAuftraege
    {
        private Adressen boA;
        private AdressenEntity boAE;

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
                    boOE.customerId = obj.customer.id;
                    boA = new Adressen();
                    if (boOE.customerId != null)
                    {
                        boAE = boA.GetById(obj.customer.id);
                        if (boAE != null)
                        {
                            boOE.customerName = boAE.name;
                            boOE.customerSubName2 = boAE.subName2;
                            boOE.customerOwningLocationId = boAE.owningLocationId;
                            boOE.customerZipCode = boAE.zipCode;
                            boOE.customerCity = boAE.city;
                            boOE.customerStreet = boAE.street;
                            boOE.customerIdCountry = boAE.idCountry;
                            boOE.customerIsocodeCountry = boAE.isocodeCountry;

                        }
                    }
                    //invoice
                    boOE.invoiceReceicerBusinessIdentifier = obj.invoiceReceiver.businessIdentifier;
                    boOE.invoiceReceiverId = obj.invoiceReceiver.id;
                    if (boOE.invoiceReceiverId != null)
                    {
                        boAE = boA.GetById(obj.customer.id);
                        if (boAE != null)
                        {
                            boOE.invoiceReceiverName = boAE.name;
                            boOE.InvoiceReceiverSubName2 = boAE.subName2;
                            boOE.invoiceReceiverOwningLocationId = boAE.owningLocationId;
                            boOE.InvoiceReceiverZipCode= boAE.zipCode;
                            boOE.InvoiceReceiverCity = boAE.city;
                            boOE.invoiceReceiverStreet = boAE.street;
                            boOE.invoiceReceiverIdCountry = boAE.idCountry;
                            boOE.invoiceReceiverIsocodeCountry = boAE.isocodeCountry;

                        }
                    }
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
                        boOISE = boOIS.GetByIdAndPKOrderItem(boOE.PK, obj.orderItems[0].orderItemServices[i].identifier);
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
                        boOISE.articleDescription =
                            obj.orderItems[0].orderItemServices[i].articleInstance.article.number;// Ganz schräg Inhalt zB Testbleche
                        boOISE.ownerId = obj.orderItems[0].orderItemServices[i].articleInstance.article.ownerId;
                        boOISE.kindOfGoodId =
                            obj.orderItems[0].orderItemServices[i].articleInstance.article.kindOfGoodId;
                        boOISE.kindOfGoodDescription =
                            obj.orderItems[0].orderItemServices[i].articleInstance.article.kindOfGoodDescription;
                        boOISE.plannedDate = PolosUtitlities.ConvertPolosDateTime2DateTime( obj.orderItems[0].plannedDate);
                        
                        
                        boOISE.ownerBusinessIdentifier =
                            boA.GetById(obj.orderItems[0].orderItemServices[i].articleInstance.article.ownerId).
                                businessIdentifier;
                        

                        #region Supplier Consignee
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
                        boOISE.supplierOrConsigneedIdCountry =
                            obj.orderItems[0].orderItemServices[i].supplierOrConsignee.address.country.id;

                        boOISE.supplierOrConsigneeIsocodeCountry =
                           obj.orderItems[0].orderItemServices[i].supplierOrConsignee.address.country.isoCode;
                        #endregion


                        #region Clearance
                        // boOISE.clearanceQuantity   = Nicht vorhanden
                        boOISE.clearanceReferenz = obj.orderItems[0].orderItemServices[i].clearance.reference;
                        boOISE.clearanceValidFrom = PolosUtitlities.ConvertPolosDateTime2DateTime( obj.orderItems[0].orderItemServices[i].clearance.validFrom);
                        boOISE.clearanceValidTo = PolosUtitlities.ConvertPolosDateTime2DateTime(obj.orderItems[0].orderItemServices[i].clearance.validTo);
                        boOISE.clearanceUnitId = obj.orderItems[0].orderItemServices[i].clearance.unit.id;
                        boOISE.clearanceUnitShortDescription =
                            obj.orderItems[0].orderItemServices[i].clearance.unit.shortDescription;
                        boOISE.clearanceDescription = obj.orderItems[0].orderItemServices[i].clearance.unit.description;
                        #endregion


                        #region Attribute

                        boOISE.SerialNumber = obj.orderItems[0].orderItemServices[i].articleInstance.attributes.SERIAL_NUMBER;
                        boOISE.batch =  obj.orderItems[0].orderItemServices[i].articleInstance.attributes.BATCH;
                        boOISE.orign = obj.orderItems[0].orderItemServices[i].articleInstance.attributes.ORIGIN;
                        boOISE.grade = obj.orderItems[0].orderItemServices[i].articleInstance.attributes.GRADE;
                        boOISE.originalNumber =
                            obj.orderItems[0].orderItemServices[i].articleInstance.attributes.ORIGINAL_NUMBER;
                        boOISE.length = obj.orderItems[0].orderItemServices[i].articleInstance.attributes.LENGTH;
                        boOISE.width = obj.orderItems[0].orderItemServices[i].articleInstance.attributes.WIDTH;
                        boOISE.height = obj.orderItems[0].orderItemServices[i].articleInstance.attributes.HEIGHT;

                        boOISE.storageAreaReference =
                            obj.orderItems[0].orderItemServices[i].articleInstance.attributes.STORAGE_AREA_REFERENCE;
                        boOISE.diameter = obj.orderItems[0].orderItemServices[i].articleInstance.attributes.DIAMETER;
                        boOISE.orignalMarking =
                            obj.orderItems[0].orderItemServices[i].articleInstance.attributes.ORIGINAL_MARKING;
                        boOISE.storageAreaReferenceNumber =
                            obj.orderItems[0].orderItemServices[i].articleInstance.attributes.
                                STORAGE_AREA_REFERENCE_NUMBER;
                        boOISE.dimension = obj.orderItems[0].orderItemServices[i].articleInstance.attributes.DIMENSION;



                       

                        #endregion

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