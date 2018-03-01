using UnityEngine;
using UnityEngine.UI;

public class CanvasTextLocaliser : MonoBehaviour
{
    [SerializeField]
    private string englishText, swedishText = "";

    private bool isEnglish;

    private Text canvasText;

    private void Awake()
    {
        // Get and store the Text component on this GameObject
        canvasText = GetComponent<Text>();
    }

    private void LateUpdate()
    {
        CheckActiveLanguage();
        SetText();
    }

    /// <summary>
    /// Changes text shown if necessary
    /// </summary>
    private void SetText()
    {
        string activeText = canvasText.text;

        if (isEnglish && activeText != englishText)
        {
            activeText = englishText;
        }
        else if (!isEnglish && activeText != swedishText)
        {
            activeText = swedishText;
        }

        if (canvasText.text != activeText)
        {
            canvasText.text = activeText;
        }
    }

    /// <summary>
    /// Checks the active language
    /// </summary>
    private void CheckActiveLanguage()
    {
        switch (GlobalStatics.Language)
        {
            case ELanguage.English:
                isEnglish = true;
                break;

            case ELanguage.Swedish:
                isEnglish = false;
                break;

            default:
                break;
        }
    }
}