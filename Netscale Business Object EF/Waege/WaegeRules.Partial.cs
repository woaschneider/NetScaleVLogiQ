using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using HWB.NETSCALE.GLOBAL;
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

            IsKfz1(currentEntity.Kfz1);
            IsWiegeartEmpty(currentEntity.WiegeartKz);
            if (goApp.VALDIERUNG_ERST == true)
            {
                if (currentEntity.Waegung == 2)
                {
                    if (goApp.FIRMAKU_VAL == true)
                    {
                        IsFirmaKu(currentEntity.FirmaKU);
                    }
                    IsSortenBezeichnung(currentEntity.Sortenbezeichnung1);
                    IsSortenNr(currentEntity.SortenNr);

                    if(currentEntity.WiegeartKz!="K")
                    IsSortenBeschreibungOk(currentEntity);

                    if(currentEntity.BonitaetKz=="7")
                    {
                        IsAdresseKUOkBoni7(currentEntity);
                    }
                    else
                    {
                        IsAdresseKUOk(currentEntity);
                    }

                    
                    IsKontraktNr_and_NrKu(currentEntity);
                    IsRecycling(currentEntity);
                    IsNotRecycling(currentEntity);
                }
            }
            else
            {
                if (goApp.FIRMAKU_VAL == true)
                {
                    IsFirmaKu(currentEntity.FirmaKU);
                }
                IsSortenBezeichnung(currentEntity.Sortenbezeichnung1);
                IsSortenNr(currentEntity.SortenNr);
                IsSortenBeschreibungOk(currentEntity);
       
                if (currentEntity.BonitaetKz == "7")
                {
                    IsAdresseKUOkBoni7(currentEntity);
                }
                else
                {
                    IsAdresseKUOk(currentEntity);
                }
            }
           IsRecycling(currentEntity);
            IsNotRecycling(currentEntity);
            IsUeberladen(currentEntity);
        }

        /// <summary>
        /// Validates the Kfzkennzeichen
        /// </summary>
        /// <summary>
        public string IsKfz1(string kfz1)
        {
            string Msg = null;
            if (mmType.IsEmpty(kfz1))
            {
                this.EntityPropertyDisplayName = "Kfz-Kennzeichen";

                Msg = this.RequiredFieldMessagePrefix +
                      this.EntityPropertyDisplayName + " " +
                      this.RequiredFieldMessageSuffix;

                AddErrorProviderBrokenRule("Kfz1", Msg);
            }
            return Msg;
        }
        public string IsWiegeartEmpty(string Wiegeart)
        {
            string Msg = null;
            if (mmType.IsEmpty(Wiegeart))
            {
                this.EntityPropertyDisplayName = " Wiegeart ";

                Msg = this.RequiredFieldMessagePrefix +
                      this.EntityPropertyDisplayName + " " +
                      this.RequiredFieldMessageSuffix;

                AddErrorProviderBrokenRule("WiegeartKz", Msg);
            }
            return Msg;
        }
        public string IsFirmaKu(string firmaku)
        {
            string Msg = null;
            if (mmType.IsEmpty(firmaku))
            {
                this.EntityPropertyDisplayName = "Firma";

                Msg = RequiredFieldMessagePrefix +
                      EntityPropertyDisplayName + " " +
                      RequiredFieldMessageSuffix;

                AddErrorProviderBrokenRule("FirmaKU", Msg);
            }
            return Msg;
        }

        public string IsSortenBezeichnung(string sortenbezeichnung1)
        {
            string Msg = null;
            if (mmType.IsEmpty(sortenbezeichnung1))
            {
                this.EntityPropertyDisplayName = "(Sorten) Bezeichnung 1";

                Msg = RequiredFieldMessagePrefix +
                      EntityPropertyDisplayName + " " +
                      RequiredFieldMessageSuffix;

                AddErrorProviderBrokenRule("Sortenbezeichnung1", Msg);
            }
            return Msg;
        }

        public string IsSortenNr(string sortennr)
        {
            string Msg = null;
            if (mmType.IsEmpty(sortennr))
            {
                this.EntityPropertyDisplayName = "(Sorten / Material) Nummer";

                Msg = RequiredFieldMessagePrefix +
                      EntityPropertyDisplayName + " " +
                      RequiredFieldMessageSuffix;

                AddErrorProviderBrokenRule("SortenNr", Msg);
            }
            return Msg;
        }

        // Vergleiche Sortennummer mit Sortenbeschreibung 1
        public string IsSortenBeschreibungOk(WaegeEntity oWE)
        {
            string Msg = null;
            if (oWE.SortenNr != null)
            {
                MG oM = new MG();
                MGEntity oME = oM.GetMGByNr(oWE.SortenNr);
                if (oME != null)
                {
                    if (oWE.SortenNr == oME.SortenNr & oWE.Sortenbezeichnung1 == oME.Sortenbezeichnung1)
                    {
                        // OK
                    }

                    else
                    {
                        this.EntityPropertyDisplayName = "Sorten-Nr ";
                        Msg =
                            this.EntityPropertyDisplayName +
                            " passt nicht mit der Sortenbezeichnung überein!";
                        AddErrorProviderBrokenRule("SortenNr", Msg);
                    }
                }
                else
                {
                    this.EntityPropertyDisplayName = "Sorten-Nr ";
                    Msg =
                        this.EntityPropertyDisplayName +
                        " ist unbekannt!";
                    AddErrorProviderBrokenRule("SortenNr", Msg);
                }
            }


            return Msg;
        }

        public string IsAdresseKUOk(WaegeEntity oWE)
        {
            string Msg = null;
            if (oWE.NrKU != null)
            {
                AP oAP = new AP();
                APEntity oAPE = oAP.GetAPByNr(oWE.NrKU);
                if (oAPE != null)
                {
                    if (oWE.NrKU == oAPE.Nr & oWE.FirmaKU == oAPE.Firma & oWE.Name1KU == oAPE.Name1 &
                        oWE.AnschriftKU == oAPE.Anschrift & oWE.PlzKU == oAPE.Plz & oWE.OrtKU == oAPE.Ort)
                    {
                        // Ok
                    }
                    else
                    {
                        this.EntityPropertyDisplayName = "Kunden-Nr";
                        Msg =
                            this.EntityPropertyDisplayName +
                            " und Kunden-Name, bzw Adresse passen nicht zusammen!";
                        AddErrorProviderBrokenRule("FirmaKU", Msg); 
                    }
                }
                else
                {
                    this.EntityPropertyDisplayName = "Kunden-Nr";
                    Msg =
                        this.EntityPropertyDisplayName +
                        " ist unbekannt!";
                    AddErrorProviderBrokenRule("NrKU", Msg);
                }
            }

            return Msg;
        }
        public string IsAdresseKUOkBoni7(WaegeEntity oWE)
        {
            string Msg = null;
            if (oWE.NrKU != null)
            {
                AP oAP = new AP();
                APEntity oAPE = oAP.GetAPByNr(oWE.NrKU);
                if (oAPE != null)
                {
                    if (oWE.NrKU == oAPE.Nr & oWE.FirmaKU == oAPE.Firma)
                    {
                        // Ok
                    }
                    else
                    {
                        this.EntityPropertyDisplayName = "Kunden-Nr";
                        Msg =
                            this.EntityPropertyDisplayName +
                            " und Kunden-Name passen nicht zusammen!";
                        AddErrorProviderBrokenRule("FirmaKU", Msg);
                    }
                }
                else
                {
                    this.EntityPropertyDisplayName = "Kunden-Nr";
                    Msg =
                        this.EntityPropertyDisplayName +
                        " ist unbekannt!";
                    AddErrorProviderBrokenRule("NrKU", Msg);
                }
            }

            return Msg;
        }

        // Passt KontraktNr zur Kundennummer
         public string IsKontraktNr_and_NrKu(WaegeEntity oWE)
         {
             string Msg = null;
             if(oWE!=null)
             {
                 KK oKK = new KK();
                 KKEntity oKKE = oKK.GetKKByAuftragsNr(oWE.kontraktnr);
                 if(oKKE != null)
                 {
                     AP oAP = new AP();
                     APEntity oAPE = oAP.GetAPByNr(oWE.NrKU);
                     if(oKKE.APFK == oAPE.PK )
                     {
                         // Ok
                     }
                     else
                     {
                         this.EntityPropertyDisplayName = "Auftrags-Nr";
                         Msg =
                             this.EntityPropertyDisplayName +
                             " und Kunden-Nr passen nicht zusammen!";
                         AddErrorProviderBrokenRule("kontraktnr", Msg);
                         AddErrorProviderBrokenRule("nrKU", Msg); 
                     }
                 }
                 else
                 {
                     this.EntityPropertyDisplayName = "Auftrags-Nr";
                     Msg =
                         this.EntityPropertyDisplayName +
                         " ist unbekannt!";
                     AddErrorProviderBrokenRule("kontraktnr", Msg);  
                 }
             }

             return Msg;
         }


        public string IsKu_andAuftragOk()
        {
            string Msg = null;
            return Msg;
        }


        public string IsUeberladen(WaegeEntity oWE)
        {
            string Msg = "";
            var boE = new Einstellungen();
            EinstellungenEntity boEE = boE.GetEinstellungen();

            if (boEE.MaxGewicht != null && boEE.MaxGewichtValidieren != null)
            {
                if (oWE.Waegung == 2 && (oWE.ZweitGewicht > oWE.ErstGewicht) && boEE.MaxGewichtValidieren == true &&
                    oWE.ZweitGewicht > boEE.MaxGewicht)
                {
                    EntityPropertyDisplayName = "Zweitgewicht";
                    Msg = "Das Fahrzeug ist überladen! ";
                    AddErrorProviderBrokenRule("ZweitGewicht", Msg);
                }
            }

            return Msg;
        }

        // Schiewe Valdierung
        public string IsRecycling(WaegeEntity oWE)
        {
            string Msg = null;
            if (oWE.SortenNr.Substring(0,1)=="7" & oWE.WiegeartKz!="R")
            {
                this.EntityPropertyDisplayName = "Wiegeart: ";

                Msg = EntityPropertyDisplayName + "Material muss mit Wiegeart R gewogen werden ";
                  

                AddErrorProviderBrokenRule("Wiegeart", Msg);
            }
            return Msg;

        }
        // Schiewe Valdierung
        public string IsNotRecycling(WaegeEntity oWE)
        {
            string Msg = null;
            if (oWE.SortenNr.Substring(0, 1) != "7" & oWE.WiegeartKz == "R")
            {
                this.EntityPropertyDisplayName = "Wiegeart: ";

                Msg = EntityPropertyDisplayName + "Material darf nicht mit Wiegeart R gewogen werden ";


                AddErrorProviderBrokenRule("Wiegeart", Msg);
            }
            return Msg;

        }

        // TODO: Prüfen ob der Mandant, Material und Auftrag zueinander passen. Folgende Fehlbedienung des Users
        // muss ausgeschlossen werden. Auftrag A wird mit Mandant A ausgewählt und anschließen wird der Mandant auf B geändert
        // Die brutale Lösung wäre bei jedem Mandantenwechsel die Wägemaske zu clearen
        // Zunächst werde ich das aber nur über eine Validierung abfangen. Das muss die Praxis zeigen.
        public String ValidateMandant()
        {
            return "";
        }
    }
}