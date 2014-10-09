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
	/// Summary description for Artikel.
	/// </summary>
	public partial class Artikel
	{
		/// <summary>
		/// Hook method automatically executed from the mmBusinessObject constructor
		/// </summary>
		protected override void HookConstructor()
		{
			// Place code here to be executed when the business object instantiates
		}

        public ArtikelEntity GetById(int id)
        {  IQueryable<ArtikelEntity> query = from a in this.ObjectContext.ArtikelEntities
	                                              where a.id == id
	                                              select a;
            return GetEntity(query);
        }
}
}
