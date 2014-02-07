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
    /// Summary description for Frachtmittel.
    /// </summary>
    public partial class Frachtmittel : ABusinessObject<FrachtmittelEntity>
    {
        #region Association Properties

        /// <summary>
        /// Business Entity object
        /// </summary>
        public override FrachtmittelEntity Entity
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

        private FrachtmittelEntity _entity;

        /// <summary>
        /// Business Rule object
        /// </summary>
        public virtual FrachtmittelRules Rules
        {
            get { return (FrachtmittelRules) this.BusinessRuleObj; }
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
        public Frachtmittel()
        {
            this.EntityFramework = true;
            this.TableName = "Frachtmittel";
            this.PhysicalDbcObjectName = "dbo.Frachtmittel";
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
            return new FrachtmittelRules(this);
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