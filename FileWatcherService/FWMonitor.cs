using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Timers;

namespace FileWatcherService
{
    class FWMonitor
    {
        private FileSystemWatcher watcher = null;
        private List<string> m_NewFileList = new List<string>();

        public List<string> NewFileList { get { return m_NewFileList; } }

 
        public bool Start()
        {
          
            DirectoryInfo dir = new DirectoryInfo(FWConfigData.Instance.SourceDir);

            watcher = new FileSystemWatcher(FWConfigData.Instance.SourceDir, FWConfigData.Instance.TypeOfFilesToCopy);
            //Watch for changes in LastAccess and LastWrite times, and
            //the renaming of files or directories.
            watcher.NotifyFilter = NotifyFilters.LastAccess
                     | NotifyFilters.FileName;

            // Add event handlers.
            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.Created += new FileSystemEventHandler(OnChanged);
            watcher.Deleted += new FileSystemEventHandler(OnChanged);
            watcher.Renamed += new RenamedEventHandler(OnRenamed);

            // Begin watching.
            watcher.EnableRaisingEvents = true;
            return true;
        }

        public void Stop()
        {
            if (null != watcher)
            {
                watcher.EnableRaisingEvents = false;
                watcher.Dispose();

            }

            FWLogger.Log.Info("Monitoring Stopped");
        }

        //This method is called when a file is created, changed, or deleted.
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            //Show that a file has been created, changed, or deleted.
            WatcherChangeTypes wct = e.ChangeType;
            if (WatcherChangeTypes.Changed == e.ChangeType)
            {
                FWLogger.Log.InfoFormat("File {0} {1}", e.FullPath, wct.ToString());
                m_NewFileList.Add(e.FullPath);
            }
        }

        private void OnRenamed(object source, FileSystemEventArgs e)
        {
            FWLogger.Log.Debug("OnChanged");
            //Show that a file has been created, changed, or deleted.
            WatcherChangeTypes wct = e.ChangeType;
            FWLogger.Log.InfoFormat("File {0} {1}", e.FullPath, wct.ToString());
        }


    }
}
