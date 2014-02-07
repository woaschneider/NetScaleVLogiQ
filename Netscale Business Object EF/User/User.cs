using System.Data.EntityClient;
using System.Data.Objects;
using OakLeaf.MM.Main.Business;

namespace HWB.NETSCALE.BOEF.User
{
    /// <summary>
    /// Summary description for User.
    /// </summary>
    public partial class User : ABusinessObject<UserEntity>
    {
        #region Association Properties

        /// Business Rule object
        /// </summary>
        public virtual UserRules Rules
        {
            get { return (UserRules) this.BusinessRuleObj; }
            set { this.BusinessRuleObj = value; }
        }

        /// <summary>
        /// Object Context Property
        /// </summary>
        public EntityDataModelContainer ObjectContext
        {
            get { return (EntityDataModelContainer) this.ObjectContextEF; }
            set { this.ObjectContextEF = value; }
        }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public User()
        {
            this.EntityFramework = true;
            this.TableName = "Users";
            this.PhysicalDbcObjectName = "dbo.Users";
            this.PrimaryKey = "UserPK";
            this.HookConstructor();
            this.EntityCentric = true;
        }

        /// <summary>
        /// Factory method that creates a business rule object
        /// </summary>
        /// <returns>Reference to the business rule object</returns>
        protected override mmBusinessRule CreateBusinessRuleObject()
        {
            return new UserRules(this);
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
        protected override void HookSetDefaultValues(UserEntity entity)
        {
            // Store the hard-coded default values via the entity object
            if (entity != null)
            {
                entity.LanguageFK = (1);
            }
        }
    }
}