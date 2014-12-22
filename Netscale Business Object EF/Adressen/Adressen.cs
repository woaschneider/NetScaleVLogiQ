using System.Data.EntityClient;
using System.Data.Objects;
using OakLeaf.MM.Main.Business;

namespace HWB.NETSCALE.BOEF.Adressen
{
	/// <summary>
	/// Summary description for Adressen.
	/// </summary>
	public partial class Adressen : ABusinessObject<AdressenEntity>
	{
		
		#region Association Properties

		/// <summary>
		/// Business Entity object
		/// </summary>
		public override AdressenEntity Entity
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
		private AdressenEntity _entity;

		/// <summary>
		/// Business Rule object
		/// </summary>
		public virtual AdressenRules Rules
		{
			get { return (AdressenRules)this.BusinessRuleObj; }
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
		public Adressen()
		{
			this.EntityFramework = true;
			this.TableName = "Adressen";
			this.PhysicalDbcObjectName = "dbo.Adressen";
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
			return new AdressenRules(this);
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
