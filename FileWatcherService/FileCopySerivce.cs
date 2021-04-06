using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security;
using System.Threading;

using System.Collections.Concurrent;

namespace FileWatcherService
{
    public class FileCopySerivce
    {
        private bool bProgress = false;
        private static readonly object mylock = new object();

        public bool CheckAndCopy()
        {
            lock (mylock)
            {
                if (bProgress)
                {
                    FWLogger.Log.Error("Copy already in progress. Retry next time");
                    return false;
                }
            }            

            try
            {
                // Extra check although not required as Config check this
                if ((!Directory.Exists(FWConfigData.Instance.SourceDir)) || (!Directory.Exists(FWConfigData.Instance.DestinationDir)))
                {
                    FWLogger.Log.Info("Either " + FWConfigData.Instance.SourceDir + " or " + FWConfigData.Instance.DestinationDir + " doesn't exits");
                    return false;

                }

                bool isEmpty = !Directory.EnumerateFiles(FWConfigData.Instance.SourceDir).Any();
                if (isEmpty)
                {
                    FWLogger.Log.Error(FWConfigData.Instance.SourceDir + " directory is empty");
                    return false;

                }

                FWLogger.Log.Debug("Start the copy process");

                bProgress = true;
                // Create list of files to copy
                DirectoryInfo di = new DirectoryInfo(FWConfigData.Instance.SourceDir);
                FileInfo[] files = di.GetFiles(FWConfigData.Instance.m_TypeOfFilesToCopy);

                var exceptions = new ConcurrentQueue<Exception>();

                Parallel.ForEach(
                                files,
                                new ParallelOptions { MaxDegreeOfParallelism = FWConfigData.Instance.MaxDegreeOfParallelism },
                                file => {
                                    try
                                    {
                                        string destFile = FWConfigData.Instance.DestinationDir + file.Name;
                                        //Delete if already exist
                                        if (File.Exists(destFile))
                                        {
                                            File.Delete(destFile);
                                        }

                                        //Now move
                                        FWLogger.Log.Debug(file.FullName + "Thread Id : " + Thread.CurrentThread.ManagedThreadId.ToString());
                                        File.Move(file.FullName, destFile);
                                    }                                    
                                    catch(Exception e)
                                    {
                                        FWLogger.Log.Error(e.Message);
                                        exceptions.Enqueue(e);
                                    }

                                    }
                            );

                   if (exceptions.Count > 0) return false;
            }
            catch (UnauthorizedAccessException e)
            {
                FWLogger.Log.Error(e.Message);
                return false;
            }
            catch (DirectoryNotFoundException e)
            {
                FWLogger.Log.Error(e.Message);
                return false;
            }
            catch (IOException e)
            {
                FWLogger.Log.Error(e.Message);
                return false;
            }

            FWLogger.Log.Debug("Completed the copy process");

            bProgress = false;
           
            return true;
        }

 
    }
}
