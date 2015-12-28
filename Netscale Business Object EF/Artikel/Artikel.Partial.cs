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
	/// Summary description for Artikel.
	/// </summary>
    public partial class Artikel
    {
        /// <summary>
        /// Hook method automatically executed from the mmBusinessObject constructor
        /// </summary>
        protected override void HookConstructor()
        {
            // Place code here to be executed when the business object instantiates
        }

        public ArtikelEntity GetById(string id)
        {
            IQueryable<ArtikelEntity> query = from a in this.ObjectContext.ArtikelEntities
                                              where a.id == id
                                              select a;
            return GetEntity(query);
        }
        public ArtikelEntity GetByPk(int pk)
        {
            IQueryable<ArtikelEntity> query = from a in this.ObjectContext.ArtikelEntities
                                              where a.PK == pk
                                              select a;
            return GetEntity(query);
        }
       
        public mmBindingList<ArtikelEntity> GetAll()
        {

        IQueryable<ArtikelEntity>    query = from a in ObjectContext.ArtikelEntities
                    orderby a.kindOfGoodDescription
                    select a;
        return GetEntityList(query);
        }

        public mmBindingList<ArtikelEntity> GetByMatchCode(string mc)
        {

            IQueryable<ArtikelEntity> query = from a in ObjectContext.ArtikelEntities
                                              orderby a.kindOfGoodDescription
                                              where a.number.Contains(mc)||
                                              a.kindOfGoodDescription.Contains(mc)||
                                              a.description.Contains(mc)

                                              select a;
            return GetEntityList(query);
        }
        public mmBindingList<ArtikelEntity> GetByMatchCode(string mc, string ownerId)
        {

            IQueryable<ArtikelEntity> query = from a in ObjectContext.ArtikelEntities
                                              orderby a.kindOfGoodDescription
                                              where a.ownerId==ownerId && (a.number.Contains(mc) ||
                                              a.kindOfGoodDescription.Contains(mc) ||
                                              a.description.Contains(mc))

                                              select a;
            return GetEntityList(query);
        }

      
    }
}
