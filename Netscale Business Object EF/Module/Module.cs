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
    /// Summary description for Module.
    /// </summary>
    public partial class Module : ABusinessObject<ModuleEntity>
    {
        #region Association Properties

        /// <summary>
        /// Business Entity object
        /// </summary>
        public override ModuleEntity Entity
        {
            get
            {
                if (this._entity == null)
                {
                    this._entity = this.CreateEntityObject();
                }
                return this._entity;
            }
            set { this._entity = value; }
        }

        private ModuleEntity _entity;

        /// <summary>
        /// Business Rule object
        /// </summary>
        public virtual ModuleRules Rules
        {
            get { return (ModuleRules) this.BusinessRuleObj; }
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
        public Module()
        {
            this.EntityFramework = true;
            this.TableName = "Module";
            this.PhysicalDbcObjectName = "dbo.Module";
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
            return new ModuleRules(this);
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