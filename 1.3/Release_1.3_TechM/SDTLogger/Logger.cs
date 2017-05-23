using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using System.IO;

namespace SDTLogger
{
    public static class Logger
    {
        private static log4net.ILog Log { get; set; }

        static Logger()
        {
            Log = log4net.LogManager.GetLogger(typeof(Logger));

            // Gets directory path of the calling application
            // RelativeSearchPath is null if the executing assembly i.e. calling assembly is a
            // stand alone exe file (Console, WinForm, etc). 
            // RelativeSearchPath is not null if the calling assembly is a web hosted application i.e. a web site
            var log4NetConfigDirectory = AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory;
            var log4NetConfigFilePath = Path.Combine(log4NetConfigDirectory, "log4net.config");
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo(log4NetConfigFilePath));
        }

        public static void Fatal(object msg)
        {
            Log.Fatal(msg);
        }

        public static void Fatal(object msg, Exception ex)
        {
            Log.Fatal(msg, ex);
        }

        public static void Fatal(Exception ex)
        {
            Log.Fatal(ex.Message, ex);
        }

        public static void Error(object msg)
        {
            Log.Error(msg);
        }

        public static void Error(object msg, Exception ex)
        {
            Log.Error(msg, ex);
        }

        public static void Error(Exception ex)
        {
            Log.Error(ex.Message, ex);
        }

        public static void Warn(object msg)
        {
            Log.Warn(msg);
        }

        public static void Warn(object msg, Exception ex)
        {
            Log.Warn(msg, ex);
        }

        public static void Warn(Exception ex)
        {
            Log.Warn(ex.Message, ex);
        }

        public static void Info(object msg)
        {
            Log.Info(msg);
        }
    }
}
