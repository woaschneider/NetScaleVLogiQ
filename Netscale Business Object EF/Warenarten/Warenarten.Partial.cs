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
	/// Summary description for Warenarten.
	/// </summary>
	public partial class Warenarten
	{
		/// <summary>
		/// Hook method automatically executed from the mmBusinessObject constructor
		/// </summary>
		protected override void HookConstructor()
		{
			// Place code here to be executed when the business object instantiates
		}

        public WarenartenEntity GetById(string id)
        {
            IQueryable<WarenartenEntity> query = from a in ObjectContext.WarenartenEntities
                                                 where a.id == id
                                                 select a;
            return GetEntity(query);
        }

        public WarenartenEntity GetByPk(int pk)
        {
            IQueryable<WarenartenEntity> query = from a in ObjectContext.WarenartenEntities
                                                 where a.PK == pk 
                                                 select a;
            return GetEntity(query);
        }
        
       
        public mmBindingList<WarenartenEntity> GetByMatchCode(string mc)
        {
            IQueryable<WarenartenEntity> query = from a in ObjectContext.WarenartenEntities
                                                 orderby a.description
                                                 where a.description.Contains(mc)
                                                 select a;
            return GetEntityList(query);

        }
	}
}
