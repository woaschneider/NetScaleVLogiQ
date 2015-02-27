using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.EntityClient;
using System.Linq;
using System.Security.Cryptography;
using OakLeaf.MM.Main.Business;
using OakLeaf.MM.Main.Collections;
using OakLeaf.MM.Main.Data;

namespace HWB.NETSCALE.BOEF
{
    /// <summary>
    /// Summary description for Serv.
    /// </summary>
    public partial class Serv
    {
        /// <summary>
        /// Hook method automatically executed from the mmBusinessObject constructor
        /// </summary>
        protected override void HookConstructor()
        {
            // Place code here to be executed when the business object instantiates
        }

        public ServEntity GetByPk(int pk)
        {
            IQueryable<ServEntity> query = from s in ObjectContext.ServEntities
                where s.PK == pk
                select s;
            return GetEntity(query);

        }

        public ServEntity GetById_Fk(int id , int fk)
        {
            IQueryable<ServEntity> query = from s in ObjectContext.ServEntities
                                           where s.id== id && s.FK == fk
                                           select s;
            return GetEntity(query);

        }

        public mmBindingList<ServEntity> GetAllByFk(int fk)
        {
            IQueryable<ServEntity> query = from s in ObjectContext.ServEntities
                where s.FK == fk
                select s;
            return GetEntityList(query);
        }

        public mmBindingList<ServEntity> GetAllByProduktId(int? id)
        {
            int? ProductPk = new Produkte().GetById(id).PK;
            IQueryable<ServEntity> query = from s in ObjectContext.ServEntities
                                           where  s.FK == ProductPk
                                           select s;
            return GetEntityList(query);
        }
    }
}
