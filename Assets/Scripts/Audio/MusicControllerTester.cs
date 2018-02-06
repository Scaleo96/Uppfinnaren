using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MusicMixer;

class MusicControllerTester : MonoBehaviour {

    [SerializeField]
    List<MusicTrack> trackCopy;

	// Use this for initialization
	void Start () {
        trackCopy = MusicController.GetAllTracks();
        Debug.Log("<color=cyan><b>MusicTester</b></color> - HAHA TESTING FORMATING: " + this.ToString(), this);
        if (MusicController.PlayTrack(trackCopy[0]))
        {
            Debug.LogWarning("<color=cyan><b>MusicTester</b></color> - I tried playing a track, and it worked!", this);
        }
    }
	
	// Update is called once per frame
	void Update () {
    }
}
