using System;
using Serilog;
using Serilog.Events;
using ILogger = Exquisite.Shared.Contracts.ILogger;

namespace Exquisite.Shared.Components;
//方法一：静态类
/*public  class Logger 
{
    #region Fields

    private  Serilog.Core.Logger? _logger;

    #endregion

    #region Function

     Logger()
    {
        InitializeLogger();
    }
   

    public  void InitializeLogger()
    {
        string LogFilePath(string LogEvent)
        {
            return $@"{AppContext.BaseDirectory}Log\{DateTime.Now.ToString("yyyy_MM_dd")}\log{LogEvent}.log";
        }

        var SerilogOutputTemplate =
            "Date：{Timestamp:yyyy-MM-dd HH:mm:ss.fff}{NewLine}LogLevel：{Level}{NewLine}Message：{Message}{NewLine}{Exception}" +
            new string('-', 50) + "{NewLine}{NewLine}";
        var SerilogOutputTemplate2 =
            "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}";


        if (_logger == null)
            // _logger = new LoggerConfiguration()
            //.WriteTo.File(logFilePath, 
            //outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}", 
            // rollingInterval: RollingInterval.Day, retainedFileCountLimit: 30)
            // .WriteTo.Logger(lc => lc
            //     .Filter.ByIncludingOnly(p => p.Level == LogEventLevel.Information).WriteTo.File(logFilePath1,  outputTemplate: SerilogOutputTemplate))
            // .WriteTo.Logger(lc => lc
            //     .Filter.ByIncludingOnly(p => p.Level == LogEventLevel.Error).WriteTo.File(logFilePath2,  outputTemplate: SerilogOutputTemplate))
            // .CreateLogger();
            _logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.Debug() // 所有Sink的最小记录级别
                .WriteTo.Logger(lg =>
                    lg.Filter.ByIncludingOnly(p => p.Level == LogEventLevel.Debug).WriteTo.File(LogFilePath("Debug"),
                        rollingInterval: RollingInterval.Day, outputTemplate: SerilogOutputTemplate,
                        retainedFileCountLimit: 30))
                .WriteTo.Logger(lg =>
                    lg.Filter.ByIncludingOnly(p => p.Level == LogEventLevel.Information).WriteTo.File(
                        LogFilePath("Information"), rollingInterval: RollingInterval.Day,
                        outputTemplate: SerilogOutputTemplate, retainedFileCountLimit: 30))
                .WriteTo.Logger(lg =>
                    lg.Filter.ByIncludingOnly(p => p.Level == LogEventLevel.Warning).WriteTo.File(
                        LogFilePath("Warning"), rollingInterval: RollingInterval.Day,
                        outputTemplate: SerilogOutputTemplate, retainedFileCountLimit: 30))
                .WriteTo.Logger(lg =>
                    lg.Filter.ByIncludingOnly(p => p.Level == LogEventLevel.Error).WriteTo.File(LogFilePath("Error"),
                        rollingInterval: RollingInterval.Day, outputTemplate: SerilogOutputTemplate,
                        retainedFileCountLimit: 30))
                .WriteTo.Logger(lg =>
                    lg.Filter.ByIncludingOnly(p => p.Level == LogEventLevel.Fatal).WriteTo.File(LogFilePath("Fatal"),
                        rollingInterval: RollingInterval.Day, outputTemplate: SerilogOutputTemplate,
                        retainedFileCountLimit: 30))
                .CreateLogger();
    }

    #endregion

    #region Properties

    //public  Logger Instance
    //{
    //    get
    //    {
    //        if (_instance == null) _instance = new Logger();

    //        return _instance;
    //    }
    //}

    //private  Logger? _instance;

    #endregion

    #region Methods

    public  void Error(Exception exception, string messageTemplate)
    {
        //InitializeLogger();
        _logger?.Error(exception, messageTemplate);
        //_logger?.Dispose();
    }

    public  void Error(Exception exception, string messageTemplate, params object[] args)
    {
        //InitializeLogger();
        _logger?.Error(exception, messageTemplate, args);
        //_logger?.Dispose();
    }

    public  void Error(string messageTemplate, params object[] args)
    {
        //InitializeLogger();
        _logger?.Error(messageTemplate, args);
        //_logger?.Dispose();
    }

    public  void Information(string message)
    {
        //InitializeLogger();
        _logger?.Information(message);
        //_logger?.Dispose();
    }

    public  void Information(string messageTemplate, params object[] args)
    {
        //InitializeLogger();
        _logger?.Information(messageTemplate, args);
        //_logger?.Dispose();
    }

    public  void Debug(string message)
    {
        //InitializeLogger();
        _logger?.Debug(message);
        //_logger?.Dispose();
    }

    public  void Debug(string messageTemplate, params object[] args)
    {
        //InitializeLogger();
        _logger?.Debug(messageTemplate, args);
        //_logger?.Dispose();
    }

    public  void Warning(string message)
    {
        //InitializeLogger();
        _logger?.Warning(message);
        //_logger?.Dispose();
    }

    public  void Warning(string messageTemplate, params object[] args)
    {
        //InitializeLogger();
        _logger?.Warning(messageTemplate, args);
        //_logger?.Dispose();
    }

    #endregion
}*/


public class Logger : ILogger
{
    #region Fields

    private static Serilog.Core.Logger? _logger;

    #endregion

    #region Function

    public Logger()
    {
        InitializeLogger();
    }


    public void InitializeLogger()
    {
        string LogFilePath(string LogEvent)
        {
            return $@"{AppContext.BaseDirectory}Log\{DateTime.Now.ToString("yyyy_MM_dd")}\log{LogEvent}.log";
        }

        var SerilogOutputTemplate =
            "Date：{Timestamp:yyyy-MM-dd HH:mm:ss.fff}{NewLine}LogLevel：{Level}{NewLine}Message：{Message}{NewLine}{Exception}" +
            new string('-', 50) + "{NewLine}{NewLine}";
        var SerilogOutputTemplate2 =
            "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}";


        if (_logger == null)
            // _logger = new LoggerConfiguration()
            //.WriteTo.File(logFilePath, 
            //outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}", 
            // rollingInterval: RollingInterval.Day, retainedFileCountLimit: 30)
            // .WriteTo.Logger(lc => lc
            //     .Filter.ByIncludingOnly(p => p.Level == LogEventLevel.Information).WriteTo.File(logFilePath1,  outputTemplate: SerilogOutputTemplate))
            // .WriteTo.Logger(lc => lc
            //     .Filter.ByIncludingOnly(p => p.Level == LogEventLevel.Error).WriteTo.File(logFilePath2,  outputTemplate: SerilogOutputTemplate))
            // .CreateLogger();
            _logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.Debug() // 所有Sink的最小记录级别
                .WriteTo.Logger(lg =>
                    lg.Filter.ByIncludingOnly(p => p.Level == LogEventLevel.Debug).WriteTo.File(LogFilePath("Debug"),
                        rollingInterval: RollingInterval.Day, outputTemplate: SerilogOutputTemplate,
                        retainedFileCountLimit: 30))
                .WriteTo.Logger(lg =>
                    lg.Filter.ByIncludingOnly(p => p.Level == LogEventLevel.Information).WriteTo.File(
                        LogFilePath("Information"), rollingInterval: RollingInterval.Day,
                        outputTemplate: SerilogOutputTemplate, retainedFileCountLimit: 30))
                .WriteTo.Logger(lg =>
                    lg.Filter.ByIncludingOnly(p => p.Level == LogEventLevel.Warning).WriteTo.File(
                        LogFilePath("Warning"), rollingInterval: RollingInterval.Day,
                        outputTemplate: SerilogOutputTemplate, retainedFileCountLimit: 30))
                .WriteTo.Logger(lg =>
                    lg.Filter.ByIncludingOnly(p => p.Level == LogEventLevel.Error).WriteTo.File(LogFilePath("Error"),
                        rollingInterval: RollingInterval.Day, outputTemplate: SerilogOutputTemplate,
                        retainedFileCountLimit: 30))
                .WriteTo.Logger(lg =>
                    lg.Filter.ByIncludingOnly(p => p.Level == LogEventLevel.Fatal).WriteTo.File(LogFilePath("Fatal"),
                        rollingInterval: RollingInterval.Day, outputTemplate: SerilogOutputTemplate,
                        retainedFileCountLimit: 30))
                .CreateLogger();
    }

    #endregion

    #region Properties

    public static Logger Instance
    {
        get
        {
            if (_instance == null) _instance = new Logger();

            return _instance;
        }
    }

    private static Logger? _instance;

    #endregion

    #region Methods

    public void Error(Exception exception, string messageTemplate)
    {
        //InitializeLogger();
        _logger?.Error(exception, messageTemplate);
        //_logger?.Dispose();
    }

    public void Error(Exception exception, string messageTemplate, params object[] args)
    {
        //InitializeLogger();
        _logger?.Error(exception, messageTemplate, args);
        //_logger?.Dispose();
    }

    public void Error(string messageTemplate, params object[] args)
    {
        //InitializeLogger();
        _logger?.Error(messageTemplate, args);
        //_logger?.Dispose();
    }

    public void Information(string message)
    {
        //InitializeLogger();
        _logger?.Information(message);
        //_logger?.Dispose();
    }

    public void Information(string messageTemplate, params object[] args)
    {
        //InitializeLogger();
        _logger?.Information(messageTemplate, args);
        //_logger?.Dispose();
    }

    public void Debug(string message)
    {
        //InitializeLogger();
        _logger?.Debug(message);
        //_logger?.Dispose();
    }

    public void Debug(string messageTemplate, params object[] args)
    {
        //InitializeLogger();
        _logger?.Debug(messageTemplate, args);
        //_logger?.Dispose();
    }

    public void Warning(string message)
    {
        //InitializeLogger();
        _logger?.Warning(message);
        //_logger?.Dispose();
    }

    public void Warning(string messageTemplate, params object[] args)
    {
        //InitializeLogger();
        _logger?.Warning(messageTemplate, args);
        //_logger?.Dispose();
    }

    #endregion
}