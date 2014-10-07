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
	/// Summary description for Address.
	/// </summary>
	public partial class Address
	{
		/// <summary>
		/// Hook method automatically executed from the mmBusinessObject constructor
		/// </summary>
		protected override void HookConstructor()
		{
			// Place code here to be executed when the business object instantiates
		}

        public AddressEntity GetAPById(int? id)
        {
            IQueryable<AddressEntity> query = from AP in this.ObjectContext.AddressEntities
                                         where AP.id == id
                                         select AP;
            return this.GetEntity(query);
        }
	}
}
