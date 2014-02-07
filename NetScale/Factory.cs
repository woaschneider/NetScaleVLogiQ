using System;
using System.Collections.Generic;
using System.Text;
using OakLeaf.MM.Main.WPF;
using OakLeaf.MM.Main.Business;

namespace HWB.NETSCALE.FRONTEND.WPF
{
    /// <summary>
    /// Application-Level factory
    /// </summary>
    public class Factory : mmFactoryWPF
    {
        /// <summary>
        /// Factory method that creates the Broken Rules window
        /// </summary>
        /// <param name="businessRules">Business Rules template</param>
        /// <returns></returns>
        public override mmBrokenRulesBaseWindow CreateBrokenRulesWindow(mmBusinessRule businessRules)
        {
            return new BrokenRulesWindow(businessRules);
        }
    }
}