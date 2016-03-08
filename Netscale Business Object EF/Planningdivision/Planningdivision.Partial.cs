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
	/// Summary description for Planningdivision.
	/// </summary>
	public partial class Planningdivision
	{
		/// <summary>
		/// Hook method automatically executed from the mmBusinessObject constructor
		/// </summary>
		protected override void HookConstructor()
		{
			// Place code here to be executed when the business object instantiates
		}
        public PlanningdivisionEntity GetById(string id)
        {
            IQueryable<PlanningdivisionEntity> query = from p in ObjectContext.PlanningdivisionEntities
                                                   where p.id == id
                                                   select p;
            return GetEntity(query);
        }

        public PlanningdivisionEntity GetByPk(int pk)
        {
            IQueryable<PlanningdivisionEntity> query = from p in ObjectContext.PlanningdivisionEntities
                                                       where p.PK == pk
                                                       select p;
            return GetEntity(query);
        }

        public mmBindingList<PlanningdivisionEntity> GetAll()
        {

            IQueryable<PlanningdivisionEntity> query = from a in ObjectContext.PlanningdivisionEntities
                                              orderby a.description
                                              select a;
            return GetEntityList(query);
        }
	}
}
