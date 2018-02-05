using UnityEngine;

namespace MenUI
{
    public class MenuNavigationButton : MonoBehaviour
    {

        [Tooltip("Options for menu to be opened or switched to")]
        [SerializeField]
        private MenuParameters newMenuParameters;

        public void OpenMenu()
        {
            transform.root.GetComponent<MenUI>().OpenMenu(newMenuParameters);
        }

        public void CloseMenu()
        {
            transform.root.GetComponent<MenUI>().CloseMenu();
        }

        public void SwitchMenu()
        {
            transform.root.GetComponent<MenUI>().SwitchMenu(newMenuParameters);
        }

    }
}