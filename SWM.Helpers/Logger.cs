using log4net;
using log4net.Config;
using System;
using System.IO;
using System.Reflection;
using System.Xml;

namespace SWM.Helpers
{
    public class Logger<T> where T : class
    {

        private const string LOG_CONFIG_FILE = @"log4net.config";

        private readonly ILog _log;

        public Logger()
        {
            _log = LogManager.GetLogger(typeof(T));
            SetLog4NetConfiguration();
        }

        public void Log(object message)
        {
            _log.Info($"{Environment.NewLine}{message}{Environment.NewLine}");
        }

        private void SetLog4NetConfiguration()
        {
            XmlDocument log4netConfig = new XmlDocument();
            log4netConfig.Load(File.OpenRead(LOG_CONFIG_FILE));

            var repo = LogManager.CreateRepository(
                Assembly.GetEntryAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));
            var log4net = log4netConfig["configuration"].SelectSingleNode("log4net") as XmlElement;
            XmlConfigurator.Configure(repo, log4net);
        }
    }
}
