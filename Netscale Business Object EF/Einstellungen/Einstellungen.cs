using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.EntityClient;
using System.Data.Objects;
using System.Linq;

using OakLeaf.MM.Main.Business;
using OakLeaf.MM.Main.Collections;
using OakLeaf.MM.Main.Data;

namespace HWB.NETSCALE.BOEF
{
	/// <summary>
	/// Summary description for Einstellungen.
	/// </summary>
	public partial class Einstellungen : ABusinessObject<EinstellungenEntity>
	{
		
		#region Association Properties

		/// Business Rule object
		/// </summary>
		public virtual EinstellungenRules Rules
		{
			get { return (EinstellungenRules)this.BusinessRuleObj; }
			set { this.BusinessRuleObj = value; }
		}

		/// <summary>
		/// Object Context Property
		/// </summary>
		public EntityDataModelContainer ObjectContext
		{
			get
			{
				return (EntityDataModelContainer)this.ObjectContextEF;
			}
			set
			{
				this.ObjectContextEF = value;
			}
		}
		#endregion

		/// <summary>
		/// Constructor
		/// </summary>
		public Einstellungen()
		{
			this.EntityFramework = true;
			this.TableName = "Einstellungen";
			this.PhysicalDbcObjectName = "dbo.Einstellungen";
			this.PrimaryKey = "PK";
			this.HookConstructor();
			this.EntityCentric = true;
		}

		/// <summary>
		/// Factory method that creates a business rule object
		/// </summary>
		/// <returns>Reference to the business rule object</returns>
		protected override mmBusinessRule CreateBusinessRuleObject()
		{
			return new EinstellungenRules(this);
		}

		/// <summary>
		/// Object Context Factory method
		/// </summary>
		/// <returns>Object context</returns>
		protected override ObjectContext CreateObjectContext()
		{
			return new EntityDataModelContainer();
		}
		

		/// <summary>
		/// Object Context Factory method
		/// </summary>
		/// <param name="conn">Entity Connection</param>
		/// <returns>Object context</returns>
		public override ObjectContext CreateObjectContext(EntityConnection conn)
		{
			return new EntityDataModelContainer(conn);
		}
		

		/// <summary>
		/// Set default values on the new entity
		/// </summary>
		/// <param name="entity">New Entity</param>
		protected override void HookSetDefaultValues(EinstellungenEntity entity)
		{

			// Store the hard-coded default values via the entity object
			if (entity != null)
			{
				entity.AP_Id_counter = (0);
				entity.MG_Id_counter = (0);
				entity.MischerCounterId = (0);
			}
		}

	}
}
