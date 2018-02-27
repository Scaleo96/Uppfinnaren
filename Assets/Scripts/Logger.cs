using System;
using System.IO;
using UnityEngine;

static public class Logger
{
    private const string DEBUG_CONSOLE_PREFIX = "<b><color=orange>Logger™</color></b>: ";
    private const string FILE_ENDING = ".txt";

    static public bool echoToConsole = true;
    static public string logFile = "log";
    static public bool timeStampLog = true;
    static public bool timeStampLogFile = true;

    static private DateTime gameStartTime;
    static private bool hasGameStart;
    static private bool initialized;
    static private StreamWriter OutputStream;

    static public void Initialize(string logFilePath)
    {
        if (initialized)
        {
            LogToConsole("Logger already initialized.");
            return;
        }
        else
        {
            logFile = logFilePath;
            TimeStampLogFileName();
            OpenLogFile();
            initialized = true;

            LogToConsole("Logger™ initialized - Logging to '" + logFile + "'");

            AppendOpeningMessageToLog();
        }
    }

    private static void AppendOpeningMessageToLog()
    {
        // Append message to start of log
        Write("New log initialized at " + gameStartTime.ToString(), false, true);
        Write("==============================================", false, true);
        WriteWhiteSpaceToLog();
    }

    private static void AppendClosingMessageToLog()
    {
        WriteWhiteSpaceToLog();
        Write("==============================================", false, true);
        Write("Ending log at " + DateTime.Now.ToString(), false, true);

        // Add a bit of extra room at the end of the log
        int endingLineSpaces = 3;
        for (int i = 0; i < endingLineSpaces; i++)
        {
            WriteWhiteSpaceToLog();
        }
    }

    private static void WriteWhiteSpaceToLog()
    {
        Write("", false, true);
    }

    public static void Terminate()
    {
        // Close the file
        CloseFile();
        initialized = false;
    }

    private static void CloseFile()
    {
        if (OutputStream != null)
        {
            AppendClosingMessageToLog();
            OutputStream.Close();
            OutputStream = null;
        }
    }

    /// <summary>
    /// Log the specified message to a file.
    /// </summary>
    /// <param name="message">Message.</param>
    public static void Log(string message)
    {
        if (initialized)
        {
            Write(message);
        }
        else
        {
            LogToConsole("</i>Logger not initialized yet. The following message will not be logged:</i> " + message, true);
        }
    }

    private static void LogToConsole(string message, bool isWarning = false)
    {
        if (!isWarning)
        {
            UnityEngine.Debug.Log(DEBUG_CONSOLE_PREFIX + message);
        }
        else
        {
            UnityEngine.Debug.LogWarning(DEBUG_CONSOLE_PREFIX + message);
        }
    }

    /// <summary>
    /// Open the log file to append the new log to it.
    /// </summary>
    private static void OpenLogFile()
    {
        OutputStream = new StreamWriter(logFile, true);
    }

    /// <summary>
    /// Adds a time stamp to the beginning of the log file name if timeStampLogFile is set to true
    /// </summary>
    static private void TimeStampLogFileName()
    {
        if (!timeStampLogFile)
        {
            return;
        }
        else
        {
            // Set time stamp to use for the log for later use
            if (!hasGameStart)
            {
                gameStartTime = DateTime.Now;
                hasGameStart = true;
            }
            string dateString = gameStartTime.ToString("yyyy-MM-dd");
            string clockString = gameStartTime.ToString("hh-mm-ss");
            System.IO.Directory.CreateDirectory("Logs/" + dateString);
            logFile = "Logs/" + dateString + "/" + logFile + " - " + clockString + FILE_ENDING;
        }
    }

    /// <summary>
    /// Write the specified message with added time stamp.
    /// </summary>
    /// <param name="message">Message to log.</param>
    static private void Write(string message)
    {
        Write(message, timeStampLog);
    }

    /// <summary>
    /// Write the specified message.
    /// </summary>
    /// <param name="message">Message to log.</param>
    /// <param name="AddTimeStamp">If set to <c>true</c> add time stamp.</param>
    static private void Write(string message, bool AddTimeStamp, bool suppressConsoleMessage = false)
    {
        // Show message in editor as well?
        if (echoToConsole && !suppressConsoleMessage)
        {
            LogToConsole(message);
        }

        // Add time stamp
        if (AddTimeStamp)
        {
            message = "[" + Time.unscaledTime + "] " + message;
        }

        // Write to file
        if (OutputStream != null)
        {
            OutputStream.WriteLine(message);
            OutputStream.Flush();
        }
    }
}