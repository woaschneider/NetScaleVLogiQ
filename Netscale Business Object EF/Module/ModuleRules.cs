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
    /// Summary description for ModuleRules.
    /// </summary>
    public partial class ModuleRules : ABusinessRule
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ModuleRules(ImmBusinessRuleHost hostObject) : base(hostObject)
        {
            this.EntityFramework = true;
            this.EntityCentric = true;
            this.HookConstructor();
        }

        /// <summary>
        /// Checks business rules against the specified entity list
        /// </summary>
        /// <typeparam name="EntityType">Entity Type</typeparam>
        /// <param name="entityList">Entity List</param>
        /// <returns>Logical true if rules passed, otherwise false</returns>
        public override bool CheckRulesHook<EntityType>(mmBindingList<EntityType> entityList)
        {
            // Call any generated rules
            if (entityList != null)
            {
                foreach (EntityType CurrentEntity in entityList)
                {
                    this.CurrentEntity = CurrentEntity as mmBusinessEntity;

                    if (((Module) this.HostObject).IsEntityChanged(this.CurrentEntity))
                    {
                        ModuleEntity Entity = CurrentEntity as ModuleEntity;

                        // Call validation methods
                        this.CheckExtendedRulesHook<EntityType>(CurrentEntity);
                    }
                }
            }
            // Change this return value to indicate result of rule checking
            return this.ErrorProviderBrokenRuleCount == 0;
        }
    }
}