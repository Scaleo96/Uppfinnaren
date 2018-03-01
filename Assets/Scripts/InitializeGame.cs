using UnityEngine;

public class InitializeGame : MonoBehaviour
{
    private const string FIRST_RUN_PREFS = "firstRun";

    // Use this for initialization
    private void Awake()
    {
        // Default language to Swedish if that's the system langauge
        if (Application.systemLanguage == SystemLanguage.Swedish && IsFirstRun())
        {
            GlobalStatics.Language = ELanguage.Swedish;

            SaveFirstRun();
        }
    }

    private bool IsFirstRun()
    {
        if (PlayerPrefs.GetInt(FIRST_RUN_PREFS, 1) == 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void SaveFirstRun()
    {
        PlayerPrefs.SetInt(FIRST_RUN_PREFS, 0);
        PlayerPrefs.Save();
    }
}