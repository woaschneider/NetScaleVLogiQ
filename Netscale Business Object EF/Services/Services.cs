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
	/// Summary description for Services.
	/// </summary>
	public partial class Services : ABusinessObject<ServicesEntity>
	{
		
		#region Association Properties

		/// <summary>
		/// Business Entity object
		/// </summary>
		public override ServicesEntity Entity
		{
			get
			{
				if (this._entity == null)
				{
					this._entity = this.CreateEntityObject();
				}
				return this._entity;
			}
			set
			{
				this._entity = value;
			}
		}
		private ServicesEntity _entity;

		/// <summary>
		/// Business Rule object
		/// </summary>
		public virtual ServicesRules Rules
		{
			get { return (ServicesRules)this.BusinessRuleObj; }
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
		public Services()
		{
			this.EntityFramework = true;
			this.TableName = "Services";
			this.PhysicalDbcObjectName = "dbo.Services";
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
			return new ServicesRules(this);
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
		

	}
}
