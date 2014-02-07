using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OakLeaf.MM.Main;
using OakLeaf.MM.Main.Windows.Forms;
using OakLeaf.MM.Main.Patterns;
using OakLeaf.MM.Main.Security;
using OakLeaf.MM.Main.WPF;

namespace HWB.NETSCALE.FRONTEND.WPF
{
    /// <summary>
    /// AppWPF class
    /// </summary>
    public class AppWPF : mmAppWPF
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public AppWPF()
        {
            Factory = (Factory) mmAppBase.Factory;

            // Set the application name property
            ApplicationName = "<MMApplicationName>";

            // Change the default control security level to Full
            mmAppBase.DefaultSecurityAccessLevel = mmSecurityAccessLevel.Full;
            UseMostPrivilegedRoleAccess = false;
        }

        /// <summary>
        /// Create the application-level factory
        /// </summary>
        /// <returns>Factory object</returns>
        public override mmFactory CreateFactory()
        {
            return new Factory();
        }
    }
}