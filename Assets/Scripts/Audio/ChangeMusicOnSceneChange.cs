using MusicMixer;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class ChangeMusicOnSceneChange : MonoBehaviour
{
    private UnityAction myAction;

    [SerializeField]
    private int startComposition = 0;

    [SerializeField]
    private int compositionToChangeTo = 1;

    private void Start()
    {
        MusicController.ActivateMusicComposition(MusicController.Compositions[startComposition]);
        SceneManager.activeSceneChanged += SceneChangeMusic;
    }

    private void SceneChangeMusic(Scene first, Scene second)
    {
        MusicController.ActivateMusicComposition(MusicController.Compositions[compositionToChangeTo]);
    }
}