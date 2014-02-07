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
    /// Summary description for UserRules.
    /// </summary>
    public partial class UserRules
    {
        /// <summary>
        /// Checks business rules against the specified entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public override void CheckExtendedRulesHook<EntityType>(EntityType entity)
        {
            UserEntity currentEntity = entity as UserEntity;

            // Call Validation methods
            ValidateVorname(currentEntity.FirstName);
            ValidateNachName(currentEntity.LastName);
            ValidateBenutzerId(currentEntity.UserID);
            ValidatePasswort1(currentEntity.Password);
            ValidatePasswort2(currentEntity.Password);
        }

        public string ValidateVorname(string vn)
        {
            string Msg = null;
            if (mmType.IsEmpty(vn))
            {
                this.EntityPropertyDisplayName = "Vorname";

                Msg = this.RequiredFieldMessagePrefix +
                      this.EntityPropertyDisplayName +
                      this.RequiredFieldMessageSuffix;

                AddErrorProviderBrokenRule("FirstName", Msg);
            }
            return Msg;
        }

        public string ValidateNachName(string vn)
        {
            string Msg = null;
            if (mmType.IsEmpty(vn))
            {
                this.EntityPropertyDisplayName = "Nachname";

                Msg = this.RequiredFieldMessagePrefix +
                      this.EntityPropertyDisplayName +
                      this.RequiredFieldMessageSuffix;

                AddErrorProviderBrokenRule("LastName", Msg);
            }
            return Msg;
        }

        public string ValidateBenutzerId(string vn)
        {
            string Msg = null;
            if (mmType.IsEmpty(vn))
            {
                this.EntityPropertyDisplayName = "Benutzer ID";

                Msg = this.RequiredFieldMessagePrefix +
                      this.EntityPropertyDisplayName +
                      this.RequiredFieldMessageSuffix;

                AddErrorProviderBrokenRule("UserID", Msg);
            }
            return Msg;
        }

        public string ValidatePasswort1(string vn)
        {
            string Msg = null;
            if (mmType.IsEmpty(vn))
            {
                this.EntityPropertyDisplayName = "Passwort";

                Msg = this.RequiredFieldMessagePrefix +
                      this.EntityPropertyDisplayName +
                      this.RequiredFieldMessageSuffix;

                AddErrorProviderBrokenRule("Password", Msg);
            }
            return Msg;
        }

        public string ValidatePasswort2(string vn)
        {
            string Msg = null;
            if (vn != null)
            {
                if (vn.Length < 6)
                {
                    this.EntityPropertyDisplayName = "Passwort";

                    Msg =
                        this.EntityPropertyDisplayName +
                        " muss mindestens 6 Zeichen lang sein";

                    AddErrorProviderBrokenRule("Password", Msg);
                }
            }
            return Msg;
        }
    }
}