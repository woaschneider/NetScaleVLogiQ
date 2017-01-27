using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.EntityClient;
using System.Linq;
using Newtonsoft.Json.Converters;
using OakLeaf.MM.Main.Business;
using OakLeaf.MM.Main.Collections;
using OakLeaf.MM.Main.Data;

namespace HWB.NETSCALE.BOEF
{
	/// <summary>
	/// Summary description for Attribut.
	/// </summary>
	public partial class Attribut
	{
		/// <summary>
		/// Hook method automatically executed from the mmBusinessObject constructor
		/// </summary>
		protected override void HookConstructor()
		{
			// Place code here to be executed when the business object instantiates
		}
        public bool IsArtikelAttribut(int pkArtikel, int pkattribut)
        {
            IQueryable<AttributEntity> query = from a in ObjectContext.AttributEntities
                                               where a.ArtikelFK == pkArtikel && a.AttributeFK == pkattribut
                                               select a;
            var dummy = GetEntity(query);

            if (dummy == null)
            { return false; }

            else
            {
                return true;
            }
        }

	    public mmBindingList<AttributEntity> GetAttributeByArtikelPk(int pk)
	    {
	        IQueryable<AttributEntity> query = from a in ObjectContext.AttributEntities
	            where a.ArtikelFK == pk
	            select a;
	        return GetEntityList(query);
	    }
        public mmBindingList<AttributEntity> GetPflichtAttributeByArtikelPk(int? pk)
        {
            IQueryable<AttributEntity> query = from a in ObjectContext.AttributEntities
                                               where a.ArtikelFK == pk && a.Required==true
                                               select a;
            return GetEntityList(query);
        }


        public bool IsAttributRequired(string name)
	    {
	        IQueryable<AttributEntity> query = from a in ObjectContext.AttributEntities
	            where a.AttributName == name && a.Required == true 
	            select a;
	        var x = GetEntity(query);
	        if (x != null)
	        { // Dann ist es ein Pflichtfeld
	            return true;
	        }
	        else // Dann ist es kein Pflichtfeld
	        {
	            return false;
	        }
	    }
	}
}
