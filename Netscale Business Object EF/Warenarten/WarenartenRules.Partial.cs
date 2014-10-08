using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

using OakLeaf.MM.Main;
using OakLeaf.MM.Main.Business;
using OakLeaf.MM.Main.Collections;

namespace HWB.NETSCALE.BOEF
{
	/// <summary>
	/// Summary description for WarenartenRules.
	/// </summary>
	public partial class WarenartenRules
	{
		/// <summary>
		/// Checks business rules against the specified entity
		/// </summary>
		/// <param name="entity">Entity</param>
		public override void CheckExtendedRulesHook<EntityType>(EntityType entity)
		{
			WarenartenEntity currentEntity = entity as WarenartenEntity;
			
			// Call Validation methods

		}
	}
}
