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
	/// Summary description for Frachtmittel.
	/// </summary>
	public partial class Frachtmittel
	{
		/// <summary>
		/// Hook method automatically executed from the mmBusinessObject constructor
		/// </summary>
		protected override void HookConstructor()
		{
			// Place code here to be executed when the business object instantiates
		}

        public FrachtmittelEntity GetFrachtmittelByKz(string kz)
        {
            IQueryable<FrachtmittelEntity> query = from b in this.ObjectContext.FrachtmittelEntities
                                                   where b.Kennung == kz
                                                   select b;
            return this.GetEntity(query);
        }

        public FrachtmittelEntity GetFrachtmittelByPK(int pk)
        {
            IQueryable<FrachtmittelEntity> query = from b in this.ObjectContext.FrachtmittelEntities
                                                   where b.PK == pk
                                                   select b;
            return this.GetEntity(query);
        }

        public mmBindingList<FrachtmittelEntity> GetAll()
        {
            IQueryable<FrachtmittelEntity> query = from b in this.ObjectContext.FrachtmittelEntities
                                                   orderby b.Kennung
                                                   select b;


            ;
            return this.GetEntityList(query);
        }
	}
}
