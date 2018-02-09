using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasTextLocaliser : MonoBehaviour {

    [SerializeField]
    string englishText, swedishText = "";

    Text canvasText;

    private void Start()
    {
        // Get and store the Text component on this GameObject
        canvasText = GetComponent<Text>();
    }

    private void Update()
    {
        switch (GlobalStatics.Language)
        {
            case ELanguage.English:
                canvasText.text = englishText;
                break;
            case ELanguage.Swedish:
                canvasText.text = swedishText;
                break;
            default:
                break;
        }
    }

}
