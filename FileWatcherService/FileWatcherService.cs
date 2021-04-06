using System;
using System.ServiceProcess;
using Microsoft.Win32;
using System.IO;
using System.Timers;
using System.Globalization;


namespace FileWatcherService
{
    public partial class FileWatcher : ServiceBase
    {
        private System.Timers.Timer m_Timer;
        private FileCopySerivce fileCopy = new FileCopySerivce();
        public FileWatcher()
        {
            InitializeComponent();
        }

        FWMonitor monitorService = new FWMonitor();
        protected override void OnStart(string[] args)
        {
            FWLogger.Log.Debug("OnStart");

            // Ensure configuation file is read before proceeding
            if (!FWConfigData.Instance.Initialize())
            {
                FWLogger.Log.Error("Failed to read config file");
                return;
            }

            // Create a thread

            double elapseTime = FWConfigData.Instance.m_iPollInterval * 1000.0; //seconds
            m_Timer = new System.Timers.Timer(elapseTime);
            m_Timer.Elapsed += OnTimedEvent;
            m_Timer.AutoReset = true;
            m_Timer.Enabled = true;

            //monitorService.Start();
        }

        protected override void OnStop()
        {
            m_Timer.Stop();
            m_Timer.Dispose();
            FWLogger.Log.Debug("OnStop");
            //monitorService.Stop();
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            fileCopy.CheckAndCopy();
        }
    }
}
