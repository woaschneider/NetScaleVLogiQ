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
	/// Summary description for WaegeRules.
	/// </summary>
	public partial class WaegeRules
	{
		/// <summary>
		/// Checks business rules against the specified entity
		/// </summary>
		/// <param name="entity">Entity</param>
		public override void CheckExtendedRulesHook<EntityType>(EntityType entity)
		{
			WaegeEntity currentEntity = entity as WaegeEntity;
			

            
			// Call Validation methods
            IsProductFilled(currentEntity);
		    IsKfzFilled(currentEntity);
		    IsCustomerFilled(currentEntity);
		    IsArticlenumberFilled(currentEntity);

		}

        public string IsProductFilled(WaegeEntity _we)
        {
            string Msg = null;
            if (mmType.IsEmpty(_we.productdescription))
            {
                this.EntityPropertyDisplayName = "Produkt";
                RequiredFieldMessageSuffix = " ist ein Pflchtfeld";
                Msg = this.RequiredFieldMessagePrefix +
                      this.EntityPropertyDisplayName + " " +
                      this.RequiredFieldMessageSuffix;

                AddErrorProviderBrokenRule("productdescription", Msg);
            }
            return Msg;
        }
        public string IsKfzFilled(WaegeEntity _we)
        {
            string Msg = null;
            if (mmType.IsEmpty(_we.Fahrzeug))
            { 
                this.EntityPropertyDisplayName = "Kfz-Kennzeichen";
                RequiredFieldMessageSuffix = " ist ein Pflchtfeld";
                Msg = this.RequiredFieldMessagePrefix +
                      this.EntityPropertyDisplayName + " " +
                      this.RequiredFieldMessageSuffix;

                AddErrorProviderBrokenRule("Fahrzeug", Msg);
            }
            return Msg;
        }
        public string IsCustomerFilled(WaegeEntity _we)
        {
            string Msg = null;
            if (mmType.IsEmpty(_we.customerBusinessIdentifier))
            {
                this.EntityPropertyDisplayName = "Auftraggeber";
                RequiredFieldMessageSuffix = " ist ein Pflchtfeld";
                Msg = this.RequiredFieldMessagePrefix +
                      this.EntityPropertyDisplayName + " " +
                      this.RequiredFieldMessageSuffix;

                AddErrorProviderBrokenRule("customerBusinessIdentifier", Msg);
            }
            return Msg;
        }
        public string IsArticlenumberFilled(WaegeEntity _we)
        {
            string Msg = null;
            if (mmType.IsEmpty(_we.articleNumber))
            {
                this.EntityPropertyDisplayName = "Artikel-Nr.";
                RequiredFieldMessageSuffix = " ist ein Pflchtfeld";
                Msg = this.RequiredFieldMessagePrefix +
                      this.EntityPropertyDisplayName + " " +
                      this.RequiredFieldMessageSuffix;

                AddErrorProviderBrokenRule("articleNumber", Msg);
            }
            return Msg;
        }

        //public string IsOrderChanged(WaegeEntity _we)
        //{
        //    if (!string.IsNullOrEmpty(_we.identifier))
        //    {
        //       // Temporär WaegeEntity
        //        Waege w = new Waege();
        //        WaegeEntity we = new WaegeEntity();
        //        w.Auftrag2Waege(_we.identifier, we);
        //    }
        //}
	}
}
