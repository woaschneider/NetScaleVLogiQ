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
	/// Summary description for Artikelattribute.
	/// </summary>
	public partial class Artikelattribute
	{
		/// <summary>
		/// Hook method automatically executed from the mmBusinessObject constructor
		/// </summary>
		protected override void HookConstructor()
		{
			// Place code here to be executed when the business object instantiates
		}

      public  mmBindingList<ArtikelattributeEntity> GetAll()
      {
          IQueryable<ArtikelattributeEntity> query = from a in ObjectContext.ArtikelattributeEntities
                                                     select a;
          return GetEntityList(query);
      }

        public ArtikelattributeEntity GetArtikelAttributByBezeichnung (string ab)
        {
            IQueryable<ArtikelattributeEntity> query = from a in ObjectContext.ArtikelattributeEntities
                                                       where a.AttributName == ab
                                                       select a;
            return GetEntity(query);
        }

        public ArtikelattributeEntity GetByMatchCode(string mc)
        {
            IQueryable<ArtikelattributeEntity> query = from a in ObjectContext.ArtikelattributeEntities
                                                       where a.AttributName.Contains(mc)
                                                       select a;
            return GetEntity(query);
        }
	}
}
