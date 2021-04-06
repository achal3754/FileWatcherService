using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FileWatcherService;

namespace UnitTest
{
    [TestClass]
    public class FWConfigDataTest
    {
        FWXmlConfigData _xml = null;
       [TestInitialize]
        public void Initialize()
        {
            FWConfigData.Instance.Initialize();
            _xml = new FWXmlConfigData(FWConfigData.Instance.SourceDir, FWConfigData.Instance.DestinationDir, FWConfigData.Instance.MaxDegreeOfParallelism.ToString());
        }

        [TestCleanup]
        public void Cleanup()
        {
            _xml.Serialize(FWConfigData.Instance.ConfigFilePath);
        }

      
        [TestMethod]
        public void EmptySourceDirTest()
        {

            FWXmlConfigData xmlNew = new FWXmlConfigData(_xml);

            xmlNew.SourceDir = "";
            xmlNew.Serialize(FWConfigData.Instance.ConfigFilePath);

            //load again
            bool bResult = FWConfigData.Instance.Initialize();

            Assert.AreEqual(false, bResult);

        }

        [TestMethod]
        public void EmptyDestinationDirTest()
        {

            FWXmlConfigData xmlNew = new FWXmlConfigData(_xml);

            xmlNew.DestinationDir = "";
            xmlNew.Serialize(FWConfigData.Instance.ConfigFilePath);

            //load again
            bool bResult = FWConfigData.Instance.Initialize();

            Assert.AreEqual(false, bResult);

        }


        [TestMethod]
        public void CheckDefaultMaxDegreeOfParallelism()
        {

            FWXmlConfigData xmlNew = new FWXmlConfigData(_xml);

            xmlNew.MaxDegreeOfParallelism = "test";
            xmlNew.Serialize(FWConfigData.Instance.ConfigFilePath);

            //load again
            FWConfigData.Instance.Initialize();

            Assert.AreEqual(1, FWConfigData.Instance.MaxDegreeOfParallelism);

        }
    }
}
