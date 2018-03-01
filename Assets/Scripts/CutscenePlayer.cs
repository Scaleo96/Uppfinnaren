using System;
using MusicMixer;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class CutscenePlayer : MonoBehaviour
{
    private const string SKIP_AXIS_NAME = "Cancel";

    [SerializeField]
    private bool turnOffMusicDuringCutscene = true;

    [SerializeField]
    private VideoClip englishClip;

    [SerializeField]
    private VideoClip swedishClip;

    private VideoPlayer videoPlayer;

    [Header("When cutscene finishes")]
    [Tooltip("What scene should be loaded once the cutscene has finished?")]
    [SerializeField]
    private int sceneBuildIndex;

    // Use this for initialization
    private void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();

        SelectClipBasedOnLanguage();

        TurnOffMusic();

        videoPlayer.loopPointReached += CutsceneFinished;
    }

    private void TurnOffMusic()
    {
        if (turnOffMusicDuringCutscene)
        {
            MusicController.DeactivateAllCompositions();
        }
    }

    private void SelectClipBasedOnLanguage()
    {
        switch (GlobalStatics.Language)
        {
            case ELanguage.English:
                videoPlayer.clip = englishClip;
                break;

            case ELanguage.Swedish:
                videoPlayer.clip = swedishClip;
                break;

            default:
                break;
        }
    }

    private void CutsceneFinished(VideoPlayer videoPlayer)
    {
        LoadNewScene();
    }

    private void LoadNewScene()
    {
        SceneManager.LoadScene(sceneBuildIndex);
    }

    private void Update()
    {
        if (Input.GetAxisRaw(SKIP_AXIS_NAME) > 0)
        {
            SkipCutscenet();
        }
    }

    private void SkipCutscenet()
    {
        videoPlayer.loopPointReached -= CutsceneFinished;
        LoadNewScene();
    }
}