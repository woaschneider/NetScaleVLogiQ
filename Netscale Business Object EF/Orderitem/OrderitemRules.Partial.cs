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
	/// Summary description for OrderitemRules.
	/// </summary>
	public partial class OrderitemRules
	{
		/// <summary>
		/// Checks business rules against the specified entity
		/// </summary>
		/// <param name="entity">Entity</param>
		public override void CheckExtendedRulesHook<EntityType>(EntityType entity)
		{
			OrderitemEntity currentEntity = entity as OrderitemEntity;
			
			// Call Validation methods

		}
	}
}
