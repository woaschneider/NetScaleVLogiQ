
using HWB.NETSCALE.BOEF;

using HWB.NETSCALE.POLOSIO;
using NetScalePolosIO;
using OakLeaf.MM.Main;
using OakLeaf.MM.Main.Patterns;using NUnit.Framework;


namespace HWB.NETSCALE.FRONTEND.WPF
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