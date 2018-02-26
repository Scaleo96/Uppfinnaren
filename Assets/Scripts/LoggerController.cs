using UnityEngine;

public class LoggerController : MonoBehaviour
{
    [SerializeField]
    private string logFile = "log.txt";

    [SerializeField]
    private bool timeStampLogFile = true;

    [SerializeField]
    private bool timeStampLog = true;

    [SerializeField]
    private bool echoToConsole = true;

    private void Awake()
    {
        Logger.timeStampLogFile = timeStampLogFile;
        Logger.timeStampLog = timeStampLog;
        Logger.echoToConsole = echoToConsole;
        Logger.Initialize(logFile);

        DontDestroyOnLoad(this);
    }

    private void OnValidate()
    {
        Logger.echoToConsole = echoToConsole;
    }

    private void OnDestroy()
    {
        Logger.Terminate();
    }

    private void OnApplicationQuit()
    {
        Logger.Terminate();
    }
}