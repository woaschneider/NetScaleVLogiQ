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

	}
}
