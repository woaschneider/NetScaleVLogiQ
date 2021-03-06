
using System.Data.EntityClient;
using System.Data.Objects;


using OakLeaf.MM.Main.Business;


namespace HWB.NETSCALE.BOEF
{
	/// <summary>
	/// Summary description for Orderitem.
	/// </summary>
	public partial class Orderitem : ABusinessObject<OrderitemEntity>
	{
		
		#region Association Properties


		/// Business Rule object
		/// </summary>
		public virtual OrderitemRules Rules
		{
			get { return (OrderitemRules)this.BusinessRuleObj; }
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
		public Orderitem()
		{
			this.EntityFramework = true;
			this.TableName = "Orderitem";
			this.PhysicalDbcObjectName = "dbo.Orderitem";
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
			return new OrderitemRules(this);
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
