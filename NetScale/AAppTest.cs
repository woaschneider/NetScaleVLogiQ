
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

        [Test]
        public void ExportTest()
        {
            Waege boW = new Waege();
            int pk = boW.GetLastWaegung();
            WaegeEntity boWe = boW.GetWaegungByPk(pk);
            ImportExportPolos oE = new ImportExportPolos();
            oE.Export(boWe);
        }

          [Test]
        public void ImportTest()
        {
            ImportExportPolos oI = new ImportExportPolos();
            oI.Import();
        }
    }

    /// <summary>
    /// AAppTestFactory
    /// </summary>
    public class AAppTestFactory : mmFactory
    {
    }
}