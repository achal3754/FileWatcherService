using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FileWatcherService;
using System.Linq;
using System.Threading.Tasks;
using System.Security.AccessControl;

namespace UnitTest
{
    /// <summary>
    /// Summary description for FileCopyTest
    /// </summary>
    [TestClass]
    public class FileCopyTest
    {
        public FileCopyTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

 
        [TestInitialize]
        public void Initialize()
        {
            FWConfigData.Instance.Initialize();
            IOUtils.DeleteAllFilesInDir(FWConfigData.Instance.SourceDir);
            IOUtils.DeleteAllFilesInDir(FWConfigData.Instance.DestinationDir);

        }

        [TestCleanup]
        public void Cleanup()
        {
            
        }

        [TestMethod]
        public void CreateFile()
        {
            IOUtils.CreateFiles(FWConfigData.Instance.SourceDir, 10000);
        }
        [TestMethod]
        public void CopyTest()
        {
            List<string> srcfiles = IOUtils.CreateFiles(FWConfigData.Instance.SourceDir, 10);
            FileCopySerivce copySrv = new FileCopySerivce();
            bool bResult = copySrv.CheckAndCopy();
            Assert.AreEqual(true, bResult);

            List<string> destFiles = new List<string>();
            Array.ForEach((new DirectoryInfo(FWConfigData.Instance.DestinationDir)).GetFiles(FWConfigData.Instance.m_TypeOfFilesToCopy),
                         delegate (FileInfo file) { destFiles.Add(file.Name); });



            bool areEqual = srcfiles.SequenceEqual(destFiles);
            Assert.AreEqual(true, areEqual);
            


        }


    
        [TestMethod]
        public void ParallelCopyTest()
        {
            List<string> srcfiles = IOUtils.CreateFiles(FWConfigData.Instance.SourceDir, 10000);
            FileCopySerivce copySrv = new FileCopySerivce();

            bool bResult1, bResult2 = true;
            Parallel.Invoke(
                           () => bResult1 = copySrv.CheckAndCopy(),
                           () => bResult2 = copySrv.CheckAndCopy());

            Assert.AreEqual(false, bResult2);



        }

        [TestMethod]
        public void SourceDirPermissionTest()
        {
            IOUtils.CreateFiles(FWConfigData.Instance.SourceDir, 10);
            // Remove the access control entry from the directory.
            IOUtils.AddDirectorySecurity(FWConfigData.Instance.SourceDir, FileSystemRights.FullControl, AccessControlType.Deny);
            FileCopySerivce copySrv = new FileCopySerivce();
            Assert.AreEqual(false, copySrv.CheckAndCopy());

            IOUtils.RemoveDirectorySecurity(FWConfigData.Instance.SourceDir, FileSystemRights.FullControl, AccessControlType.Deny);
        }

        [TestMethod]
        public void DestinationDirPermissionTest()
        {
            IOUtils.CreateFiles(FWConfigData.Instance.SourceDir, 10);
            // Remove the access control entry from the directory.
            IOUtils.AddDirectorySecurity(FWConfigData.Instance.DestinationDir, FileSystemRights.FullControl, AccessControlType.Deny);
            FileCopySerivce copySrv = new FileCopySerivce();
            Assert.AreEqual(false, copySrv.CheckAndCopy());

            IOUtils.RemoveDirectorySecurity(FWConfigData.Instance.DestinationDir, FileSystemRights.FullControl, AccessControlType.Deny);
        }


    

    }
}
