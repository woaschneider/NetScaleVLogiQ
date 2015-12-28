using System;
using System.Collections;
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
    /// Summary description for Adressen.
    /// </summary>
    public partial class Adressen
    {
        /// <summary>
        /// Hook method automatically executed from the mmBusinessObject constructor
        /// </summary>
        protected override void HookConstructor()
        {
            // Place code here to be executed when the business object instantiates
        }

        public AdressenEntity GetByPk(int? pk)
        {
            IQueryable<AdressenEntity> query
                = from a in ObjectContext.AdressenEntities
                    where a.PK == pk
                    select a;
            return GetEntity(query);
        }

        public AdressenEntity GetByBusinenessIdentifier(string mc)
        {
            IQueryable<AdressenEntity> query = from a in ObjectContext.AdressenEntities
                where a.businessIdentifier == mc.Trim()
                select a;
            return GetEntity(query);
        }

        // 
        public AdressenEntity GetByBusinenessIdentifier(string mc, string role)
        {   
   

            IQueryable<AdressenEntity> query;
            switch (role)
            {
                case "AU": // Client / Auftraggeber
                    query = from a in ObjectContext.AdressenEntities
                        where a.businessIdentifier.Equals(mc)
                              && a.roleClient == true
                        select a;
                
                    return GetEntity(query);


                // INVOICE Receiver / Rechnungsempfänger
                case "RE":
                    query = from a in ObjectContext.AdressenEntities
                        where a.businessIdentifier == mc.Trim()
                              && a.roleInvoiceReceiver == true
                        select a;
                    return GetEntity(query);


                // Storage_Client / Lagermandant
                case "LM":
                    query = from a in ObjectContext.AdressenEntities
                        where a.businessIdentifier == mc.Trim()
                              && a.roleStorageClient == true
                        select a;
                    return GetEntity(query);


                // Supplier

                case "LI":
                    query = from a in ObjectContext.AdressenEntities
                        where a.businessIdentifier == mc.Trim()
                              && a.rolleSupplier == true
                        select a;
                    return GetEntity(query);

                //  Receiver / Empfänger
                case "EM":
                    query = from a in ObjectContext.AdressenEntities
                        where a.businessIdentifier == mc.Trim()
                              && a.roleReceiver == true
                        select a;
                    return GetEntity(query);

                // Carrier / Frachtführer
                case "FF":
                    query = from a in ObjectContext.AdressenEntities
                        where a.businessIdentifier == mc.Trim()
                              && a.roleCarrier == true
                        select a;
                    return GetEntity(query);

                default:

                    return null;
            }
        }

        public AdressenEntity GetById(string id)
        {
            IQueryable<AdressenEntity> query = from a in ObjectContext.AdressenEntities
                where a.id == id
                select a;
            return GetEntity(query);
        }

        public mmBindingList<AdressenEntity> GetAll()
        {
            IQueryable<AdressenEntity> query = from a in ObjectContext.AdressenEntities
                orderby a.businessIdentifier
                select a;
            return GetEntityList(query);
        }

        public mmBindingList<AdressenEntity> GetByMatchCode(string mc)
        {
            IQueryable<AdressenEntity> query = from a in ObjectContext.AdressenEntities
                orderby a.businessIdentifier
                where
                    a.businessIdentifier.Contains(mc) ||
                    a.name.Contains(mc) ||
                    a.subName2.Contains(mc) ||
                    a.zipCode.Contains(mc) ||
                    a.city.Contains(mc) ||
                    a.street.Contains(mc)
                select a;
            return GetEntityList(query);
        }
        public mmBindingList<AdressenEntity> GetByBusinessIdentifier(string mc)
        {
            IQueryable<AdressenEntity> query = from a in ObjectContext.AdressenEntities
                                               orderby a.businessIdentifier
                                               where
                                                   a.businessIdentifier==mc
                                                
                                               select a;
            return GetEntityList(query);
        }

        // CLIENT, BILLING_RECEIVER, STORAGE_CLIENT, STORAGE_CLIENT,SUPPLIER, RECEIVER, CARRIER, SHIPOWNER, TRAIN_OPERATOR
        public mmBindingList<AdressenEntity> GetByMatchCodeAndRole(string mc, string role)
        {
            IQueryable<AdressenEntity> query;
            switch (role)
            {
                // Client / Auftraggeber
                case "AG":

                    query = from a in ObjectContext.AdressenEntities
                        orderby a.businessIdentifier
                        where a.roleClient == true & (
                            a.businessIdentifier.Contains(mc) ||
                            a.name.Contains(mc) ||
                            a.subName2.Contains(mc) ||
                            a.zipCode.Contains(mc) ||
                            a.city.Contains(mc) ||
                            a.street.Contains(mc))
                        select a;
                    return GetEntityList(query);


                // INVOICE Receiver / Rechnungsempfänger
                case "RE":
                    query = from a in ObjectContext.AdressenEntities
                        orderby a.businessIdentifier
                        where a.roleInvoiceReceiver == true & (
                            a.businessIdentifier.Contains(mc) ||
                            a.name.Contains(mc) ||
                            a.subName2.Contains(mc) ||
                            a.zipCode.Contains(mc) ||
                            a.city.Contains(mc) ||
                            a.street.Contains(mc))
                        select a;
                    return GetEntityList(query);


                // Storage_Client / Lagermandant
                case "LM":
                    query = from a in ObjectContext.AdressenEntities
                        orderby a.businessIdentifier
                        where a.roleStorageClient == true & (
                            a.businessIdentifier.Contains(mc) ||
                            a.name.Contains(mc) ||
                            a.subName2.Contains(mc) ||
                            a.zipCode.Contains(mc) ||
                            a.city.Contains(mc) ||
                            a.street.Contains(mc))
                        select a;
                    return GetEntityList(query);

                // Supplier

                case "LI":
                    query = from a in ObjectContext.AdressenEntities
                        orderby a.businessIdentifier
                        where a.rolleSupplier == true & (
                            a.businessIdentifier.Contains(mc) ||
                            a.name.Contains(mc) ||
                            a.subName2.Contains(mc) ||
                            a.zipCode.Contains(mc) ||
                            a.city.Contains(mc) ||
                            a.street.Contains(mc))
                        select a;
                    return GetEntityList(query);


                //  Receiver / Empfänger
                case "EM":
                    query = from a in ObjectContext.AdressenEntities
                        orderby a.businessIdentifier
                        where a.roleReceiver == true & (
                            a.businessIdentifier.Contains(mc) ||
                            a.name.Contains(mc) ||
                            a.subName2.Contains(mc) ||
                            a.zipCode.Contains(mc) ||
                            a.city.Contains(mc) ||
                            a.street.Contains(mc))
                        select a;
                    return GetEntityList(query);

                // Carrier / Frachtführer
                case "FF":
                    query = from a in ObjectContext.AdressenEntities
                        orderby a.businessIdentifier
                        where a.roleCarrier == true & (
                            a.businessIdentifier.Contains(mc) ||
                            a.name.Contains(mc) ||
                            a.subName2.Contains(mc) ||
                            a.zipCode.Contains(mc) ||
                            a.city.Contains(mc) ||
                            a.street.Contains(mc))
                        select a;
                    return GetEntityList(query);


                // Shipowner / Reeder 
                case "SO":
                    query = from a in ObjectContext.AdressenEntities
                        orderby a.businessIdentifier
                        where a.roleShipOwner == true & (
                            a.businessIdentifier.Contains(mc) ||
                            a.name.Contains(mc) ||
                            a.subName2.Contains(mc) ||
                            a.zipCode.Contains(mc) ||
                            a.city.Contains(mc) ||
                            a.street.Contains(mc))
                        select a;
                    return GetEntityList(query);


                // TRAIN_OPERATOR EVU Traktionär
                case "EV":
                    query = from a in ObjectContext.AdressenEntities
                        orderby a.businessIdentifier
                        where a.roleTrainOperator == true & (
                            a.businessIdentifier.Contains(mc) ||
                            a.name.Contains(mc) ||
                            a.subName2.Contains(mc) ||
                            a.zipCode.Contains(mc) ||
                            a.city.Contains(mc) ||
                            a.street.Contains(mc))
                        select a;
                    return GetEntityList(query);


                default:
                    query = from a in ObjectContext.AdressenEntities
                        orderby a.businessIdentifier
                        where
                            a.businessIdentifier.Contains(mc) ||
                            a.name.Contains(mc) ||
                            a.subName2.Contains(mc) ||
                            a.zipCode.Contains(mc) ||
                            a.city.Contains(mc) ||
                            a.street.Contains(mc)
                        select a;
                    return GetEntityList(query);
            }
        }


        public mmBindingList<AdressenEntity> GetbusinessIdentifierByMatchCode(string mc)
        {
            IQueryable<AdressenEntity> query = from a in ObjectContext.AdressenEntities
                orderby a.businessIdentifier
                where
                    a.businessIdentifier.Contains(mc)
                select a;
            return GetEntityList(query);
        }
    }
}