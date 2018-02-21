using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MusicMixer;

public class MusicControllerTester : MonoBehaviour {

    //[SerializeField]
    //int trackIndex;
    //[SerializeField]
    //float testTimer = 5f;
    //[SerializeField]
    //bool didIt;

    [Range(0,1)]
    public float targetVolume = 0f;
    public float fadeDuration = 0f;

    //public MusicComposition testingMusicComposition;

    //[SerializeField]
    //List<MusicTrack> trackCopy;

    // Use this for initialization
    void Start () {
      //  testingMusicComposition = gameObject.AddComponent<MusicComposition>();

        //testingMusicComposition.allTracks = MusicController.GetAllTracks();
        // Play first track
        //MusicController.FadeTrack(MusicController.GetAllTracks()[0], 1f, 15);

        //trackCopy = MusicController.GetAllTracks();
        //Debug.Log("<color=cyan><b>MusicTester</b></color> - HAHA TESTING FORMATING: " + this.ToString(), this);
        //if (MusicController.PlayTrack(trackCopy[0]))
        //{
        //    Debug.LogWarning("<color=cyan><b>MusicTester</b></color> - I tried playing a track, and it worked!", this);
        //}



    }
    private void Update()
    {
        MusicComposition testCompo = MusicController.Compositions[0];
        string testCompName = MusicController.ActivateMusicComposition(testCompo);
        Debug.Log("Activating composition: " + testCompName);
    }

    //// Update is called once per frame
    //void Update () {
    //       if (Time.time > testTimer && !didIt)
    //       {
    //           MusicController.FadeTrack(MusicController.GetAllTracks()[trackIndex], targetVolume, fadeDuration);
    //           //trackCopy[0].StartFade(targetVolume, fadeDuration);
    //           didIt = true;
    //       }
    //   }
}
