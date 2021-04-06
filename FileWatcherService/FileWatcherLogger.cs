using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using log4net.Config;

namespace FileWatcherService
{
    static class FWLogger
    {
        static FWLogger()
        {
            var assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            XmlConfigurator.Configure(new FileInfo(Path.Combine(assemblyFolder, "log4net.config")));
        }

        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static log4net.ILog Log { get { return _log; } }
    }
}
