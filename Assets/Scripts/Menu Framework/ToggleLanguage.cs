using UnityEngine;
using UnityEngine.UI;

public class ToggleLanguage : MonoBehaviour
{
    private void OnEnable()
    {
        bool isEnglish;

        // Find out if language is set to English or not
        switch (GlobalStatics.Language)
        {
            case ELanguage.English:
                isEnglish = true;
                break;

            case ELanguage.Swedish:
                isEnglish = false;
                break;

            default:
                isEnglish = false;
                break;
        }

        // Set toggle as such
        GetComponent<Toggle>().isOn = isEnglish;
    }

    /// <summary>
    /// Switch between English and Swedish
    /// </summary>
    /// <param name="isEnglish"></param>
    public void SetLanguageToEnglissh(bool isEnglish)
    {
        ELanguage wantedLanguage;

        if (isEnglish)
        {
            wantedLanguage = ELanguage.English;
        }
        else
        {
            wantedLanguage = ELanguage.Swedish;
        }

        GlobalStatics.Language = wantedLanguage;
    }
}