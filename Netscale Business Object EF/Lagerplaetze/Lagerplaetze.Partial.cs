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
	/// Summary description for Lagerplaetze.
	/// </summary>
	public partial class Lagerplaetze
	{
		/// <summary>
		/// Hook method automatically executed from the mmBusinessObject constructor
		/// </summary>
		protected override void HookConstructor()
		{
			// Place code here to be executed when the business object instantiates
		}
        
        public LagerplaetzeEntity GetById(string id)
        {
            IQueryable<LagerplaetzeEntity> query = from l in ObjectContext.LagerplaetzeEntities
                                                   where l.id == id
            select l;
            return GetEntity(query);
        }

	    public mmBindingList<LagerplaetzeEntity> GetAll()
        {
            IQueryable<LagerplaetzeEntity> query = from l in ObjectContext.LagerplaetzeEntities
                                                   orderby l.name
                                                   select l;
            return GetEntityList(query);
        }

        public mmBindingList<LagerplaetzeEntity> GetByMatchCode(string name )
        {
            IQueryable<LagerplaetzeEntity> query = from l in ObjectContext.LagerplaetzeEntities
                                                   where l.name.Contains(name)
                                                   select l;
            return GetEntityList(query);
        }
	}
}
