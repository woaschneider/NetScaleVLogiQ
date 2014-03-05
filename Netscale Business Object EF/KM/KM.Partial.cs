using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.EntityClient;
using System.Linq;
using HWB.NETSCALE.GLOBAL;
using OakLeaf.MM.Main.Business;
using OakLeaf.MM.Main.Collections;
using OakLeaf.MM.Main.Data;

namespace HWB.NETSCALE.BOEF
{
    /// <summary>
    /// Summary description for KM.
    /// </summary>
    public partial class KM
    {
        /// <summary>
        /// Hook method automatically executed from the mmBusinessObject constructor
        /// </summary>
        protected override void HookConstructor()
        {
            // Place code here to be executed when the business object instantiates
        }

        public KMEntity GetKMByAuftragsNr(string mandant, string auftragsnr, string posnr)
        {
            IQueryable<KMEntity> query = from KM in this.ObjectContext.KMEntities
                                         where
                                             KM.Kontraktnr == auftragsnr && KM.Mandant == mandant &&
                                             KM.Kontraktnr == auftragsnr
                                         select KM;
            return this.GetEntity(query);
        }

        public KMEntity GetKMByPK(int kmpk)
        {
            IQueryable<KMEntity> query = from KM in this.ObjectContext.KMEntities
                                         where KM.pk == kmpk
                                         select KM;
            return this.GetEntity(query);
        }

        public mmSaveDataResult AddPos(int mgpk, int kkpk)
        {
            KMEntity oKME = this.NewEntity();
            if (oKME != null)
            {
                Mandant oM = new Mandant();
                oKME.Mandant = oM.GetMandantByNr(goApp.Mandant).MandantNr;
                KK oKK = new KK();
                KKEntity oKKE = oKK.GetKKByPK(kkpk);
                oKME.Kontraktnr = oKKE.kontraktnr;
                oKME.kkpk = oKKE.pk;

                MG oMg = new MG();
                MGEntity oMGE = oMg.GetMGById(mgpk);
                if (oMGE != null)
                {
                    oKME.mgpk = oMGE.PK;
                    oKME.SortenNr = oMGE.SortenNr;
                }
            }
            return this.SaveEntity(oKME);
        }

        public void SetAllTouch2False()
        {
            IQueryable<KMEntity> query = from a in ObjectContext.KMEntities
                                         select a;
            var ii = GetEntityList(query);

            for (int i = 0; i < (ii.Count() - 1); i++)
            {
                ii[i].touch = false;

                // SaveEntity(ii[i]);
            }
            var uRet = this.SaveEntityList(ii);
        }

        public void DeleteAllNotTouch()
        {
            IQueryable<KMEntity> query = from a in ObjectContext.KMEntities
                                         where a.touch == false
                                         select a;
            var ii = GetEntityList(query);

            DeleteEntityList();
        }
    }
}