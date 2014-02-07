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
    /// Summary description for KbFnn.
    /// </summary>
    public partial class KbFnn : ABusinessObject<KbFnnEntity>
    {
        #region Association Properties

        /// Business Rule object
        /// </summary>
        public virtual KbFnnRules Rules
        {
            get { return (KbFnnRules) this.BusinessRuleObj; }
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
        public KbFnn()
        {
            this.EntityFramework = true;
            this.TableName = "KbFnn";
            this.PhysicalDbcObjectName = "dbo.KbFnn";
            this.PrimaryKey = "pk";
            this.HookConstructor();
            this.EntityCentric = true;
        }

        /// <summary>
        /// Factory method that creates a business rule object
        /// </summary>
        /// <returns>Reference to the business rule object</returns>
        protected override mmBusinessRule CreateBusinessRuleObject()
        {
            return new KbFnnRules(this);
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