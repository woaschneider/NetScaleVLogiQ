using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.EntityClient;
using System.Linq;
using HWB.NETSCALE.BOEF.JoinClasses;
using OakLeaf.MM.Main.Business;
using OakLeaf.MM.Main.Collections;
using OakLeaf.MM.Main.Data;

namespace HWB.NETSCALE.BOEF
{
    /// <summary>
    /// Summary description for KK.
    /// </summary>
    public partial class KK
    {
        /// <summary>
        /// Hook method automatically executed from the mmBusinessObject constructor
        /// </summary>
        protected override void HookConstructor()
        {
            // Place code here to be executed when the business object instantiates
        }


        public KKEntity GetKKByAuftragsNr(string auftragsnr)
        {
            IQueryable<KKEntity> query = from KK in this.ObjectContext.KKEntities
                                         where KK.kontraktnr == auftragsnr
                                         select KK;
            return this.GetEntity(query);
        }

        public KKEntity GetKKByPK(int? pk)
        {
            IQueryable<KKEntity> query = from KK in this.ObjectContext.KKEntities
                                         where KK.pk  == pk
                                         select KK;
            return this.GetEntity(query);
        }
      

        // Dieses BO ist nicht direkt aus den Tabellen abgeleitet. Siehe Join-Klassen -> Auftragsliste:BusinessObject
        // Eigentlich ist das eine recht elegante Möglichkeit. 
        public mmBindingList<AuftragsListe> GetAuftragsListe(string mc, string baustelle)
        {
           
            var query = from a in ObjectContext.APEntities
                        from k in ObjectContext.KKEntities
                        from km in ObjectContext.KKEntities
                        // <- Join auf KM
                        orderby a.Firma
                        //<- Diesen Teil brauchen wir vielleicht später mal. Join auf KM  
                        where
                            a.Firma.Contains(mc) && a.Rolle_AU == true && a.PK == k.APFK &&
                            k.kontraktnr == km.kontraktnr && k.wefirma.Contains(baustelle)
                        select new AuftragsListe
                                   {
                                       appk = a.PK,
                                       nr = a.Nr,
                                       Firma = a.Firma,
                                       Name1 = a.Name1,
                                       Plz = a.Plz,
                                       Ort = a.Ort,
                                       Anschrift= a.Anschrift,
                                       
                                       kkpk = k.pk,
                                       mandant = k.mandant,
                                       werksnr = k.werksnr,
                                       
                                       auftragsart = k.auftragsart,
                                       kontraktart = k.kontraktart,
                                       KontraktNr = k.kontraktnr,
                                       wefirma = k.wefirma,
                                       wename1 = k.wename1
                                   };

           var x = query.ToString();
            return GetEntityList(query);
        
        }

        public mmBindingList<Auftragsdetailliste> GetAuftragsDetailliste(int kkpk)
        {
            var query = from a in ObjectContext.APEntities
                        from k in ObjectContext.KKEntities
                        from km in ObjectContext.KMEntities
                        from m in ObjectContext.MGEntities




                        where k.pk == kkpk && a.PK == k.APFK && km.kkpk == kkpk && m.PK == km.mgpk
                        select new Auftragsdetailliste
                                   {
                                       appk = a.PK,
                                       nrKU = a.Nr,
                                       FirmaKU = a.Firma,
                                       Name1KU = a.Name1,
                                       PlzKU = a.Plz,
                                       OrtKU = a.Ort,
                                       AnschriftKU = a.Anschrift,

                                       kkpk = k.pk,
                                       mandant = k.mandant,
                                       werksnr = k.werksnr,

                                       auftragsart = k.auftragsart,
                                       kontraktart = k.kontraktart,
                                       KontraktNr = k.kontraktnr,
                                       wefirma = k.wefirma,
                                       wename1 = k.wename1,

                                       kmpk = km.pk,
                                       posnr = km.posnr,
                                       Sortennr = m.SortenNr,
                                       Sortenbezeichnung1 = m.Sortenbezeichnung1,
                                       Sortenbezeichnung2 = m.Sortenbezeichnung2

                                                    };

            var x = query.ToString();
            return GetEntityList(query);
          
        }
        public Auftragsdetail GetAuftragDetail(int kmpk)
      {
          var query = from a in ObjectContext.APEntities
                      from k in ObjectContext.KKEntities
                      from km in ObjectContext.KMEntities
                      from m in ObjectContext.MGEntities




                      where km.pk == kmpk && a.PK == k.APFK && km.kkpk == k.pk && m.PK == km.mgpk
                      select new Auftragsdetail
                      {
                          appk = a.PK,
                          nrKU = a.Nr,
                          FirmaKU = a.Firma,
                          Name1KU = a.Name1,
                          PlzKU = a.Plz,
                          OrtKU = a.Ort,
                          AnschriftKU = a.Anschrift,

                          kkpk = k.pk,
                          mandant = k.mandant,
                          werksnr = k.werksnr,

                          auftragsart = k.auftragsart,
                          kontraktart = k.kontraktart,
                          KontraktNr = k.kontraktnr,
                          wefirma = k.wefirma,
                          wename1 = k.wename1,

                          kmpk = km.pk,
                          posnr = km.posnr,
                          Sortennr = m.SortenNr,
                          Sortenbezeichnung1 = m.Sortenbezeichnung1,
                          Sortenbezeichnung2 = m.Sortenbezeichnung2

                      };

          var x = query.ToString();
          return GetEntity(query);
          
      }

        public void SetAllTouch2False()
        {
            IQueryable<KKEntity> query = from a in ObjectContext.KKEntities
                                         orderby a.kontraktnr 
                                         select a;
            var ii = GetEntityList(query);

            for (int i = 0; i < (ii.Count() - 1); i++)
            {
                ii[i].touch = false;

               
            }
            var uRet = this.SaveEntityList(ii);
        }

        public void DeleteAllNotTouch()
        {
            IQueryable<KKEntity> query = from a in ObjectContext.KKEntities
                                          where a.touch == false
                                         select a;
            var ii = GetEntityList(query);

            DeleteEntityList();
        }


    }
}