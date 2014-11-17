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

        public AdressenEntity GetByPK(int pk)
        {
            IQueryable<AdressenEntity> query = from a in this.ObjectContext.AdressenEntities
                                               where a.PK == pk
                                               select a;
            return GetEntity(query);
        }
        public AdressenEntity GetByBusinenessIdentifier(string mc)
        {
            IQueryable<AdressenEntity> query = from a in this.ObjectContext.AdressenEntities
                                               where a.businessIdentifier == mc.Trim()
                                               select a;
            return GetEntity(query);
        }
        public AdressenEntity GetById(int? id)
        {
            IQueryable<AdressenEntity> query = from a in this.ObjectContext.AdressenEntities
                                               where a.id == id
                                               select a;
            return GetEntity(query);
        }
        public mmBindingList<AdressenEntity> GetAll()
        {
            IQueryable<AdressenEntity> query = from a in this.ObjectContext.AdressenEntities
                                               orderby a.businessIdentifier
                                               select a;
            return GetEntityList(query);
        }
        public mmBindingList<AdressenEntity> GetByMatchCode( string mc)
        {
            IQueryable<AdressenEntity> query = from a in this.ObjectContext.AdressenEntities
                                               orderby a.businessIdentifier
                                               where 
                                                     a.businessIdentifier.Contains(mc)||
                                                     a.name.Contains(mc)||
                                                     a.subName2.Contains(mc)||
                                                     a.zipCode.Contains(mc)||
                                                     a.city.Contains(mc)||
                                                     a.street.Contains(mc)
                                               select a;
            return GetEntityList(query);
        }

        // CLIENT, BILLING_RECEIVER, STORAGE_CLIENT, STORAGE_CLIENT,SUPPLIER, RECEIVER, CARRIER, SHIPOWNER, TRAIN_OPERATOR
           public mmBindingList<AdressenEntity> GetByMatchCodeAndRole( string mc,string Role)
           {
               IQueryable<AdressenEntity> query;
            switch(Role)
            {
                    // Client
                case "AU":

                    query = from a in this.ObjectContext.AdressenEntities
                            orderby a.businessIdentifier
                            where a.roleClient==true &
                                  a.businessIdentifier.Contains(mc) ||
                                  a.name.Contains(mc) ||
                                  a.subName2.Contains(mc) ||
                                  a.zipCode.Contains(mc) ||
                                  a.city.Contains(mc) ||
                                  a.street.Contains(mc)
                            select a;

                    break;



                    // INVOICE Receiver
                case "BILLING_RECEIVER":
                    query = from a in this.ObjectContext.AdressenEntities
                            orderby a.businessIdentifier
                            where a.roleInvoiceReceiver == true &
                                  a.businessIdentifier.Contains(mc) ||
                                  a.name.Contains(mc) ||
                                  a.subName2.Contains(mc) ||
                                  a.zipCode.Contains(mc) ||
                                  a.city.Contains(mc) ||
                                  a.street.Contains(mc)
                            select a;

                    break;


                    // Storage_Client
                case   "STORAGE_CLIENT":
                    query = from a in this.ObjectContext.AdressenEntities
                            orderby a.businessIdentifier
                            where a.roleStorageClient == true &
                                  a.businessIdentifier.Contains(mc) ||
                                  a.name.Contains(mc) ||
                                  a.subName2.Contains(mc) ||
                                  a.zipCode.Contains(mc) ||
                                  a.city.Contains(mc) ||
                                  a.street.Contains(mc)
                            select a;
                    break;


                    // Supplier
                case    "SUPPLIER":
                    break;


                    //  Receiver
                case  "RECEIVER":
                    break;


                    // Carrier
                case   "CARRIER":
                    query = from a in this.ObjectContext.AdressenEntities
                            orderby a.businessIdentifier
                            where a.roleCarrier == true &
                                  a.businessIdentifier.Contains(mc) ||
                                  a.name.Contains(mc) ||
                                  a.subName2.Contains(mc) ||
                                  a.zipCode.Contains(mc) ||
                                  a.city.Contains(mc) ||
                                  a.street.Contains(mc)
                            select a;
                    break;


                    // Shipowner
                case  "SHIPOWNER":
                    break;

                    // TRAIN_OPERATOR
                case    "TRAIN_OPERATOR":
                    break;

            }
        
            return GetEntityList(query);
        }
        



        public mmBindingList<AdressenEntity> GetbusinessIdentifierByMatchCode(string mc)
        {
            IQueryable<AdressenEntity> query = from a in this.ObjectContext.AdressenEntities
                                               orderby a.businessIdentifier
                                               where
                                                     a.businessIdentifier.Contains(mc) 
                                               select a;
            return GetEntityList(query);
        }

    

	}
}
