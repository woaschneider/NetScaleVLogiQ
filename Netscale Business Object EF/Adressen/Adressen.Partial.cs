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

        public AdressenEntity GetById(int id)
        {
            IQueryable<AdressenEntity> query = from a in this.ObjectContext.AdressenEntities
                                               where a.id == id
                                               select a;
            return GetEntity(query);
        }
        public  mmBindingList<AdressenEntity> GetAll()
        {
            IQueryable<AdressenEntity> query = from a in this.ObjectContext.AdressenEntities
                                               orderby a.businessIdentifier
                                               select a;
            return GetEntityList(query);
        }

	}
}
