using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConsoleWindowAR : MonoBehaviour
{

    public struct LogMessage
    {
        public string message;
        public string stackTrace;
        public LogType type;
    }

    private static ConsoleWindowAR _instance;
    private readonly Dictionary<LogType, Color> logTypeColors = new Dictionary<LogType, Color>();
    private GameObject window;
    private GameObject canvas;
    private GameObject debugError;
    private bool show = true;
    private Text textUI;
    private List<LogMessage> Messages = new List<LogMessage>();

    // Use this for initialization
    private void OnEnable()
    {
        Application.logMessageReceived += HanleLog;

        window = GameObject.Find("ConsoleText");
        canvas = GameObject.Find("ConsoleCanvas");
        textUI = window.GetComponent<Text>();
        window = GameObject.Find("ConsoleText");

        Debug.Log("ConsoleEnabled");
    }

    private void HanleLog(string condition, string stackTrace, LogType type)
    {
        Log(new LogMessage()
        {
            message = condition,
            stackTrace = stackTrace,
            type = type
        });
    }


    /// <summary>
    /// Method for showing console using voice
    /// </summary>
    public void ShowKeyword()
    {
        Debug.Log("Console shown");

        SetVisibiliy(true);
    }

    /// <summary>
    /// Method for hiding console using voice
    /// </summary>
    public void HideKeyword()
    {
        Debug.Log("Console hidden");

        SetVisibiliy(false);
    }

    /// <summary>
    /// Method for clearing console
    /// </summary>
    public void ClearConsole()
    {
        textUI.text = "";
        textUI.text += "Console cleared";
    }


    /// <summary>
    /// Method for toggling inApp console
    /// </summary>
    /// <param name="visibility">bool</param>
    private void SetVisibiliy(bool visibility)
    {
        canvas.SetActive(visibility);
        show = visibility;
    }

    private void Log(LogMessage message)
    {

    }

    // Use this for initialization
    void Start()
    {
        _instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (Messages.Count > 0)
        {
            textUI.text += "\n";
            while (Messages.Count != 0)
            {
                textUI.text += Messages[0].message;
                textUI.text += "\n" + "__________STACKTRACE____________" + "\n";
                textUI.text += Messages[0].stackTrace;
                Messages.RemoveAt(0);
            }
        }
    }
}
