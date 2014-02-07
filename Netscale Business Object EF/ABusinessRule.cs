using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using OakLeaf.MM.Main;
using OakLeaf.MM.Main.Business;
using OakLeaf.MM.Main.Collections;
using OakLeaf.MM.Main.Security;
using OakLeaf.MM.Main.Managers;

namespace HWB.NETSCALE.BOEF
{
    /// <summary>
    /// Application-level Business Rule class
    /// </summary>
    public class ABusinessRule : mmBusinessRule
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="hostObject">Host object</param>
        public ABusinessRule(ImmBusinessRuleHost hostObject)
            : base(hostObject)
        {
        }
    }
}