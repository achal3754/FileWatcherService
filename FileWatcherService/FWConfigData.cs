using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using Microsoft.Win32;
using System.Reflection;

namespace FileWatcherService
{
    [XmlRootAttribute("FileWatchServiceData", Namespace = "", IsNullable = false)]
    public class FWXmlConfigData
    {
        public FWXmlConfigData()
        {

        }
        public FWXmlConfigData(FWXmlConfigData obj)
        {
            this.SourceDir = obj.SourceDir;
            this.DestinationDir = obj.DestinationDir;
            this.MaxDegreeOfParallelism = obj.MaxDegreeOfParallelism;
            this.TypeOfFilesToCopy = obj.TypeOfFilesToCopy;
            this.PollInterval = obj.PollInterval;

        }

        public FWXmlConfigData(string sSourceDir, string sDestinationDir, string sMaxDegreeOfParallelism, string sTypeOfFilesToCopy = "*.*", string sPollInterval = "10")
        {
            this.SourceDir = sSourceDir;
            this.DestinationDir = sDestinationDir;
            this.MaxDegreeOfParallelism = sMaxDegreeOfParallelism;
            this.TypeOfFilesToCopy = sTypeOfFilesToCopy;
            this.PollInterval = sPollInterval;
        }
        [XmlElement("SourceDir")]
        public String SourceDir { get; set; }

        [XmlElement("DestinationDir")]
        public String DestinationDir { get; set; }

        [XmlElement("MaxDegreeOfParallelism")]
        public String MaxDegreeOfParallelism { get; set; }

        [XmlElement("TypeOfFilesToCopy")]
        public String TypeOfFilesToCopy { get; set; }

        [XmlElement("PollInterval")]
        public String PollInterval { get; set; }

        public bool Serialize(string fileName)
        {
            try
            {
                XmlSerializer serialization = new XmlSerializer(typeof(FWXmlConfigData));

                using (TextWriter writer = new StreamWriter(fileName))
                {
                    serialization.Serialize(writer, this);
                    writer.Close();
                }
            }
            catch (Exception e)
            {
                FWLogger.Log.Error(e.Message);
                return false;
            }

            return true;
        }
    }

    public sealed class FWConfigData
    {
        private FWConfigData()
        {

        }
        private static readonly Lazy<FWConfigData> lazy = new Lazy<FWConfigData>(() => new FWConfigData());
        public static FWConfigData Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        public String m_SourceDir;
        public String SourceDir
        {
            get { return m_SourceDir; }
        }

        public String m_DestinationDir;
        public String DestinationDir
        {
            get { return m_DestinationDir; }
        }

        public int m_MaxDegreeOfParallelism;
        public int MaxDegreeOfParallelism
        {
            get { return m_MaxDegreeOfParallelism; }
        }

        public String m_TypeOfFilesToCopy;
        public String TypeOfFilesToCopy
        {
            get { return m_TypeOfFilesToCopy; }
        }

        public int m_iPollInterval;
        public int PollInterval { get; set; }


        public String ConfigFilePath
        {
            get
            {
                string sDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                return Path.Combine(sDir,"FileWatchServiceData.xml"); //current directory                         

            }
        }
        public bool Initialize()
        {
            try
            {
                //Read data from configutility xml
                XmlSerializer serializer = new XmlSerializer(typeof(FWXmlConfigData));
                // A FileStream is needed to read the XML document.  
                using (FileStream fs = new FileStream(ConfigFilePath, FileMode.Open))
                {
                    FWXmlConfigData configXml = (FWXmlConfigData)serializer.Deserialize(fs);
                    if (!Directory.Exists(configXml.SourceDir))
                    {
                        FWLogger.Log.Error(configXml.SourceDir + " doesn't exist.");
                        return false;
                    }
                    m_SourceDir = configXml.SourceDir;

                    if (!Directory.Exists(configXml.DestinationDir))
                    {
                        FWLogger.Log.Error(configXml.DestinationDir + " doesn't exist.");
                        return false;
                    }
                    // Add check for write permission


                    m_DestinationDir = configXml.DestinationDir;
                    // 

                    try
                    {
                        m_MaxDegreeOfParallelism = int.Parse(configXml.MaxDegreeOfParallelism);
                    }
                    catch (FormatException e)
                    {
                        m_MaxDegreeOfParallelism = 1;
                        FWLogger.Log.Info("Invalid value for NoOfFilesForParallelCopy. Defaul to 1. " + e.Message);
                    }

                    if(string.IsNullOrEmpty(configXml.TypeOfFilesToCopy))
                    {
                        m_TypeOfFilesToCopy = "*.*"; //default
                    }
                    else
                    {
                        m_TypeOfFilesToCopy = configXml.TypeOfFilesToCopy;
                    }

                    try
                    {
                        m_iPollInterval = int.Parse(configXml.PollInterval);
                    }
                    catch (FormatException e)
                    {
                        m_iPollInterval = 10; //Default to 10 second
                        FWLogger.Log.Info("Invalid value for PollInterval. Defaul to 10 second : " + e.Message);
                    }

                }

            }
            catch (Exception e)
            {
                FWLogger.Log.Error(e.Message);
                return false;

            }

            return true;
        }

    } 
}
