using Godot;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Diamondmine.scripts;

public class Logger
{
    private static Logger _logger;

    private readonly string logsDir = "res://logs/";

    public enum LogTypes
    {
        all,
        debug,
        buttons,
        error,
        exception
    }

    private readonly Dictionary<LogTypes, string> Map = new()
    {
        {LogTypes.all, "AllLog.txt"},
        {LogTypes.debug, "DebugLogs.txt"},
        {LogTypes.buttons, "ButtonsLogs.txt"},
        {LogTypes.error, "ErrorLogs.txt"},
        {LogTypes.exception, "ExceptionLogs.txt"}

    };
    private Logger()
    {
        
    }

    public static Logger GetLogger()
    {
        _logger ??= new Logger();

        return _logger;
    }

    public void Log(LogTypes type, string message)
    {
        //duplicate all logs to file with all logs
        if (type != LogTypes.all)
        {
            Log(LogTypes.all, message);
        }
        //////
         
        
        var filename = logsDir+Map[type];
        FileAccess.ModeFlags openMode;
        if (FileAccess.FileExists(filename))
        {
            openMode = FileAccess.ModeFlags.ReadWrite;
        }
        else
        {
            openMode = FileAccess.ModeFlags.Write;
        }
        var logFile = FileAccess.Open(filename, openMode);

        logFile.SeekEnd();
        logFile.StoreLine(Time.GetUnixTimeFromSystem() + " " + message);
        logFile.Close();
    }


}