using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using OakLeaf.MM.Main;
using OakLeaf.MM.Main.Business;
using OakLeaf.MM.Main.Collections;
using OakLeaf.MM.Main.Patterns;
using NUnit.Framework;

namespace HWB.NETSCALE.BOEF
{
    /// <summary>
    /// AAppTest
    /// </summary>
    public class AAppTest
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public AAppTest()
        {
            mmAppBase.Factory = new AAppTestFactory();
            mmAppBase.IsRunning = true;
        }
    }

    /// <summary>
    /// AAppTestFactory
    /// </summary>
    public class AAppTestFactory : mmFactory
    {
    }
}