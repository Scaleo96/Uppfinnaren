using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializeGame : MonoBehaviour {

	// Use this for initialization
	void Start () {

        // Default language to Swedish if that's the system langauge
        if (Application.systemLanguage == SystemLanguage.Swedish && GlobalStatics.isFirstRun)
        {
            GlobalStatics.Language = ELanguage.Swedish;
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
