using System;
using System.IO;

namespace Exquisite.Shared.Components;

public static class Environments
{
    private static string _appDataPath;
    private static string _logFilePath;

    public static string ConnectionString = @"DataSource=" + GetCurrentProjectPath + @"\DataBase\ExquisiteDB.sqlite";

    public static string AppDataPath
    {
        get
        {
            if (string.IsNullOrEmpty(_appDataPath))
                _appDataPath = Environment.ExpandEnvironmentVariables($"{Environment.CurrentDirectory}");
            if (!Directory.Exists(_appDataPath)) Directory.CreateDirectory(_appDataPath);
            return _appDataPath;
        }
    }

    public static string LogFilePath
    {
        get
        {
            if (string.IsNullOrEmpty(_logFilePath)) _logFilePath = Path.Combine(AppDataPath, Fields.LogFileName);
            return _logFilePath;
        }
    }

    public static string GetCurrentProjectPath => Environment.CurrentDirectory.Replace(@"\bin\Debug", @"\bin\Debug");
}