using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.AccessControl;
using System.Security.Principal;
using System.IO;


namespace UnitTest
{
    public static class IOUtils
    {

        public static List<string> CreateFiles(string sDir, int n)
        {
            List<string> fileNames = new List<string>();
            for (int i = 1; i < n; i++)
            {
                string sFileName = "MyTest" + i + ".txt";
                fileNames.Add(sFileName);
                string sFilePath = sDir + sFileName;
                // Create a file to write to. 
                using (StreamWriter sw = File.CreateText(sFilePath))
                {
                    sw.WriteLine("Hello");
                    sw.WriteLine("Welcome");
                }
            }

            return fileNames;
        }

        public static void DeleteAllFilesInDir(string dirPath)
        {
            try
            {
                //Delete all files in source and destination
                Array.ForEach(Directory.GetFiles(dirPath),
                             delegate (string path) { File.Delete(path); });

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        // Adds an ACL entry on the specified directory for the specified account.
        public static void AddDirectorySecurity(string FileName, FileSystemRights Rights, AccessControlType ControlType)
        {
            // Create a new DirectoryInfo object.
            DirectoryInfo dInfo = new DirectoryInfo(FileName);

            // Get a DirectorySecurity object that represents the
            // current security settings.
            DirectorySecurity dSecurity = dInfo.GetAccessControl();

            // Add the FileSystemAccessRule to the security settings.
            dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null),
                                                            Rights,
                                                            ControlType));

            // Set the new access settings.
            dInfo.SetAccessControl(dSecurity);
        }

        // Removes an ACL entry on the specified directory for the specified account.
        public static void RemoveDirectorySecurity(string FileName, FileSystemRights Rights, AccessControlType ControlType)
        {
            // Create a new DirectoryInfo object.

            try
            {
                DirectoryInfo dInfo = new DirectoryInfo(FileName);

                // Get a DirectorySecurity object that represents the
                // current security settings.
                DirectorySecurity dSecurity = dInfo.GetAccessControl();

                // Add the FileSystemAccessRule to the security settings.
                dSecurity.RemoveAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null),
                                                                Rights,
                                                                ControlType));

                // Set the new access settings.
                dInfo.SetAccessControl(dSecurity);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

    }


}
