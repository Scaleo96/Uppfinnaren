using UnityEngine;

namespace MenUI
{
    [RequireComponent(typeof(MenUI))]
    public class GameOver : MonoBehaviour
    {

        MenUI menUI;

        [SerializeField]
        MenuParameters GameOverMenuParameters;

        bool gameOver = false;

        // Use this for initialization
        void Awake()
        {
            menUI = GetComponent<MenUI>();
        }

        // What happens on Game Over
        void OnGameOver()
        {
            menUI.OpenMenu(GameOverMenuParameters);
            gameOver = true;
        }

        void LateUpdate()
        {
            if (menUI.GetActiveMenu() == null)
            {
                gameOver = false;
            }
        }

        // Check to see if Game Over state is active
        public bool GetGameOver()
        {
            return gameOver;
        }
    }
}