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
    /// Summary description for WaagentypenRules.
    /// </summary>
    public partial class WaagentypenRules
    {
        /// <summary>
        /// Checks business rules against the specified entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public override void CheckExtendedRulesHook<EntityType>(EntityType entity)
        {
            WaagentypenEntity currentEntity = entity as WaagentypenEntity;

            // Call Validation methods
        }
    }
}