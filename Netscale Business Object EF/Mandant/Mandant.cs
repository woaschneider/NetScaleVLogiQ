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
	/// Summary description for Mandant.
	/// </summary>
	public partial class Mandant : ABusinessObject<MandantEntity>
	{
		
		#region Association Properties

	
		/// Business Rule object
		/// </summary>
		public virtual MandantRules Rules
		{
			get { return (MandantRules)this.BusinessRuleObj; }
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
		public Mandant()
		{
			this.EntityFramework = true;
			this.TableName = "Mandant";
			this.PhysicalDbcObjectName = "dbo.Mandant";
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
			return new MandantRules(this);
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
		protected override void HookSetDefaultValues(MandantEntity entity)
		{

			// Store the hard-coded default values via the entity object
			if (entity != null)
			{
				entity.AnzahlLS = (2);
			}
		}

	}
}
