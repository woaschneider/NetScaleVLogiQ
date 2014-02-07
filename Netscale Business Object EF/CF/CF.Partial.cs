using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.EntityClient;
using System.Linq;
using OakLeaf.MM.Main.Business;
using OakLeaf.MM.Main.Collections;
using OakLeaf.MM.Main.Data;

namespace HWB.NETSCALE.BOEF
{
    /// <summary>
    /// Summary description for CF.
    /// </summary>
    public partial class CF
    {   
        /// <summary>
        /// Hook method automatically executed from the mmBusinessObject constructor
        /// </summary>
        protected override void HookConstructor()
        {
            // Place code here to be executed when the business object instantiates

            // OK - Ich bin mir hier nicht sicher ob das einfacher geht: So baue ich mir im Moment meinen PresaveHook
            StateChange += new mmBusinessStateChangeDelegate(this.StateChangeHandler);
        }

        public override void StateChangeHandler(mmBaseBusinessObject bizObj, mmBusinessStateChangeEventArgs e)
        {
            if (this.State == mmBusinessState.PreSaving)
            {
                PreSaveHook();
            }

            if (this.State == mmBusinessState.Added)
            {
                OnNew();
            }
        }


        private void PreSaveHook()
        {
            this.Entity.Kfz1Raw = ConvertKfzToKfzRaw(this.Entity.Kfz1);
        }

        private void OnNew()
        {
            // Aus PK wird ein KfzId gemacht
           // var x = ObjectContext.CFEntities.Max(s =>  s.PK  );

            Entity.KfzID = GetNextFreeKfzID();

        }


        //TODO Das ist Mist - Aber solange ich nicht weiﬂ, wie ich mit LINQ TO Entity ein (int) MAX  auf ein Varchar hinbekomme....
        private string GetNextFreeKfzID()
        {
            bool loopReady = false;
            int ii = 0;
            do
            {
                ii = ii + 1;
                CFEntity  boCFE  = GetCFById(ii.ToString());
                if (boCFE == null)
                {
                    loopReady = true;
                }
            } while (loopReady == false);
            return ii.ToString();
        }


        private string ConvertKfzToKfzRaw(string kfz)
        {
            string a = kfz.Replace("-", "");
            return a.Replace(" ", "");
        }


        public CFEntity GetCFById(string id)
        {
            IQueryable<CFEntity> query = from CF in this.ObjectContext.CFEntities
                                         where CF.KfzID == id
                                         select CF;
            return this.GetEntity(query);
        }

        public CFEntity GetCFByPK(int PK)
        {
            IQueryable<CFEntity> query = from CF in this.ObjectContext.CFEntities
                                         where CF.PK == PK
                                         select CF;
            return this.GetEntity(query);
        }

        public CFEntity GetCFByKennzeichen(string Nr)
        {
            string kr = ConvertKfzToKfzRaw(Nr);
            IQueryable<CFEntity> query = from CF in this.ObjectContext.CFEntities
                                         where CF.Kfz1Raw == kr
                                         select CF;
            return this.GetEntity(query);
        }


        public CFEntity GetCFByKfzId(string id)
        {
            IQueryable<CFEntity> query = from CF in this.ObjectContext.CFEntities
                                         where CF.KfzID == id
                                         select CF;
            return this.GetEntity(query);
        }

        public mmBindingList<CFEntity> GetAllCF()
        {
            IQueryable<CFEntity> query = from a in ObjectContext.CFEntities
                                         orderby a.Kfz1
                                         select a;
            return GetEntityList(query);
        }

        public bool IsCfNew(string kfz)
        {
            var cFe = GetCFByKennzeichen(kfz);
            if (cFe == null)
                return true;
            else
            {
                return false;
            }
        }

        public void DeleteOldAbrufe()
        
            
        {
            IQueryable<CFEntity> query = from a in ObjectContext.CFEntities
                                            where a.abrufdate < DateTime.Today | a.abrufdate == null 

                                         select a;
            var List  = GetEntityList(query);
            int nc = List.Count;
            for (int i = 0; i <= nc-1;i++ )
            {
                List[i].abruf_PK = null;
                List[i].Abrufnr = null;
            }
   
        }

        public override void HandleException(Exception e)
        {
            if (e.InnerException.Message.Contains("kfz1unique"))
            {
                // Unique key constraint
                this.ExceptionHandled = true;
                this.Rules.AddBrokenRule("Phone number already assigned to another customer");
                return;
            }

            base.HandleException(e);
        }
    }
}