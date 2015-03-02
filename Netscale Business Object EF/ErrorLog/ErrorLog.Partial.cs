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
	/// Summary description for ErrorLog.
	/// </summary>
	public partial class ErrorLog
	{
		/// <summary>
		/// Hook method automatically executed from the mmBusinessObject constructor
		/// </summary>
		protected override void HookConstructor()
		{
			// Place code here to be executed when the business object instantiates
		}

	    public mmBindingList<ErrorLogEntity> GetAll()
	    {
	        IQueryable<ErrorLogEntity> query = from err in ObjectContext.ErrorLogEntities
	            orderby (err.PK ) descending 
	            select err;
	        return GetEntityList(query);
	    }
	}
}
