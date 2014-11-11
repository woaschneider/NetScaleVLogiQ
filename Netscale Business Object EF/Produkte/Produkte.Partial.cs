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
	/// Summary description for Produkte.
	/// </summary>
	public partial class Produkte
	{
		/// <summary>
		/// Hook method automatically executed from the mmBusinessObject constructor
		/// </summary>
		protected override void HookConstructor()
		{
			// Place code here to be executed when the business object instantiates
		}

       public  ProdukteEntity GetById(int id)
       {
           IQueryable<ProdukteEntity> query = from p in ObjectContext.ProdukteEntities
                                              where p.id == id
                                              select p;

           return GetEntity(query);
       }
       public ProdukteEntity GetByPk(int pk)
       {
           IQueryable<ProdukteEntity> query = from p in ObjectContext.ProdukteEntities
                                              where p.PK == pk
                                              select p;

           return GetEntity(query);
       }
        public mmBindingList<ProdukteEntity> GetByMatchCode(string mc)
        {
            IQueryable<ProdukteEntity> query = from p in ObjectContext.ProdukteEntities
                                               where p.description.Contains(mc)
                                               select p;
            return GetEntityList(query);
        }
	}
}
