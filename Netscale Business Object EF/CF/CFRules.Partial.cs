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
    /// Summary description for CFRules.
    /// </summary>
    public partial class CFRules
    {
        /// <summary>
        /// Checks business rules against the specified entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public override void CheckExtendedRulesHook<EntityType>(EntityType entity)
        {
            CFEntity currentEntity = entity as CFEntity;
            if (currentEntity.EntityState == EntityState.Added)
            // Call Validation methods
            {
                IsKfzUnique(currentEntity.Kfz1);
            }
            ValidateKfz1(currentEntity.Kfz1);
            
                
        }

        public string ValidateKfz1(string kfz1)
        {
            string Msg = null;
            if (mmType.IsEmpty(kfz1, true) || kfz1 == "")
            {
                this.EntityPropertyDisplayName = "Kfz-Kennzeichen";

                Msg = this.RequiredFieldMessagePrefix +
                      this.EntityPropertyDisplayName +
                      this.RequiredFieldMessageSuffix;

                AddErrorProviderBrokenRule("Kfz1", Msg);
            }

            return Msg;
        }

        private string IsKfzUnique(string kfz1)
        {
            string Msg = null;
            CF boCF = new CF();
            var xCheck = boCF.GetCFByKennzeichen(kfz1);
            if(xCheck!=null)
            {
                this.EntityPropertyDisplayName = "Kfz-Kennzeichen";

                Msg = "Das Kennzeichen ist bereits vorhanden";

                AddErrorProviderBrokenRule("Kfz1", Msg);
            }
            return Msg;
        }


    }
}