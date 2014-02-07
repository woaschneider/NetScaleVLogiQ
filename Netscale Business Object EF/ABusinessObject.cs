using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using OakLeaf.MM.Main;
using OakLeaf.MM.Main.Business;
using OakLeaf.MM.Main.Collections;
using OakLeaf.MM.Main.Security;
using OakLeaf.MM.Main.Managers;

namespace HWB.NETSCALE.BOEF
{
    /// <summary>
    /// Application-level Business Object class
    /// </summary>
    [Serializable]
    public class ABusinessObject<EntityType> : mmBusinessObjectGeneric<EntityType>
        where EntityType : mmBusinessEntity, new()
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ABusinessObject()
        {
            // Enter the default database key as specified in the app.config file
            this.DatabaseKey = "EntityDataModelContainer";
            this.RetrieveAutoIncrementPK = true;


            // Specify the default command type for data retrieval
            this.DefaultCommandType = CommandType.StoredProcedure;
        }
    }
}