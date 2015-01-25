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
	/// Summary description for ExportLog.
	/// </summary>
	public partial class ExportLog
	{
		/// <summary>
		/// Hook method automatically executed from the mmBusinessObject constructor
		/// </summary>
		protected override void HookConstructor()
		{
			// Place code here to be executed when the business object instantiates
		}

	    public ExportLogEntity GetExportLogByPk(int pk)
	    {
	        IQueryable<ExportLogEntity> query = from e in ObjectContext.ExportLogEntities
	            where e.PK == pk
	            select e;

	        return GetEntity(query);
	    }

	    public mmBindingList<ExportLogEntity> GetAll()
	    {
	        IQueryable<ExportLogEntity> query = from e in ObjectContext.ExportLogEntities
                                                orderby (e.PK) descending
                                                select e;
	        return GetEntityList(query);
	    }

        public mmBindingList<ExportLogEntity> GetAll(DateTime? dt)
        {
            IQueryable<ExportLogEntity> query = from e in ObjectContext.ExportLogEntities
                                                where e.dt == dt 
                                                orderby (e.PK) descending
                                                select e;
            return GetEntityList(query);
        }
 

	}
}
