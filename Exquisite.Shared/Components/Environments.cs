using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exquisite.Shared.Components
{
    public static class Environments
    {
        public static string AppDataPath
        {
            get
            {
                if (string.IsNullOrEmpty(_appDataPath))
                {
                    _appDataPath = Environment.ExpandEnvironmentVariables($"{Environment.CurrentDirectory}");
                }
                if (!Directory.Exists(_appDataPath))
                {
                    Directory.CreateDirectory(_appDataPath);
                }
                return _appDataPath;
            }
        }
        private static string _appDataPath;

        public static string LogFilePath
        {
            get
            {
                if (string.IsNullOrEmpty(_logFilePath))
                {
                    _logFilePath = Path.Combine(AppDataPath, Fields.LogFileName);
                }
                return _logFilePath;
            }
        }
        private static string _logFilePath;

        public static string GetCurrentProjectPath
        {
            get
            {
                return Environment.CurrentDirectory.Replace(@"\bin\Debug", @"\bin\Debug");
            }
        }

        public static string ConnectionString = @"DataSource=" + GetCurrentProjectPath + @"\DataBase\ExquisiteDB.sqlite";
    }
}
