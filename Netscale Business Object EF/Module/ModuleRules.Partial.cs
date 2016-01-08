
namespace HWB.NETSCALE.BOEF
{
    /// <summary>
    /// Summary description for ModuleRules.
    /// </summary>
    public partial class ModuleRules
    {
        /// <summary>
        /// Checks business rules against the specified entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public override void CheckExtendedRulesHook<EntityType>(EntityType entity)
        {
            ModuleEntity currentEntity = entity as ModuleEntity;

            // Call Validation methods
        }
    }
}