using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MenUI
{
    [RequireComponent(typeof(Canvas))]
    public class MenUI : MonoBehaviour
    {
        [Tooltip("Have the menu be persistant and never unload during scene changes")]
        [SerializeField]
        private bool neverUnloadMenu = true;

        private static MenUI instance = null;           // Make sure there is only one instance at all times

        [Tooltip("Default scene to load when starting the game")]
        [SerializeField]
        private int startSceneBuildIndex = 1;

        [Tooltip("Scene containing the main menu")]
        [SerializeField]
        private int mainMenuSceneBuildIndex = 0;

        [SerializeField]
        private MenuParameters mainMenuParameters;

        private Stack<MenuParameters> menuStack = new Stack<MenuParameters>();

        [Tooltip("Show debug messages in console?")]
        [SerializeField]
        private bool verbose;

        private void Awake()
        {
            // Make sure there are no other running instances of the MenUI
            if (instance)
            {
                if (Debug.isDebugBuild && verbose) Debug.LogWarning("Other instance of MenUI already running. Terminating.");
                Destroy(gameObject);
            }
            else
            {
                if (Debug.isDebugBuild && verbose) Debug.Log("Initializing MenUI", this);
                instance = this;
            }
        }

        // Use this for initialization
        private void Start()
        {
            if (Debug.isDebugBuild && verbose) Debug.Log("MenUI starting up", this);

            InitializeEventSystem();
            PersistantMenu(neverUnloadMenu);
            OpenMainMenuInMainScene();
        }

        private void OpenMainMenuInMainScene()
        {
            // Set reference to MainMenuPanel by finding a child object by that name if it's blank
            if (mainMenuParameters.menuGameObject == null)
                mainMenuParameters.menuGameObject = transform.Find("MainMenuPanel").gameObject;

            // Display Main Menu
            if (SceneManager.GetActiveScene().buildIndex == mainMenuSceneBuildIndex)
            {
                OpenMenu(mainMenuParameters);
            }
        }

        /// <summary>
        /// Checks if an EventSystem exists and adds one otherwise
        /// </summary>
        private void InitializeEventSystem()
        {
            if (!EventSystem.current)
            {
                // Instantiate new EventSystem
                GameObject eventSystem = Instantiate(new GameObject());
                eventSystem.name = "EventSystem";
                eventSystem.AddComponent<EventSystem>();
                eventSystem.AddComponent<StandaloneInputModule>();
            }
        }

        /// <summary>
        /// Makes MenUI and the event system persistant by flagging them as DontDestroyOnLoad
        /// </summary>
        /// <param name="makePersistant"></param>
        private void PersistantMenu(bool makePersistant)
        {
            // Make sure the menu stays persistant if checked
            if (makePersistant)
            {
                DontDestroyOnLoad(gameObject);

                // Make the event system is also persistent
                DontDestroyOnLoad(EventSystem.current.gameObject);
            }
        }

        public void StartButtonPressed()
        {
            if (Debug.isDebugBuild && verbose) Debug.Log("Loading scene #" + startSceneBuildIndex, this);
            CloseMenu();
            StartCoroutine(LoadStartScene());
        }

        private IEnumerator LoadStartScene()
        {
            // The Application loads the Scene in the background at the same time as the current Scene.
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(startSceneBuildIndex, LoadSceneMode.Additive);

            //Wait until the last operation fully loads to return anything
            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            // Unload main scene
            SceneManager.UnloadSceneAsync(mainMenuSceneBuildIndex);
        }

        /// <summary>
        /// Unloads current scene, transitions to Main Menu scene and displays the Main Menu again
        /// </summary>
        public void ReturnToMainMenu()
        {
            // Load starting screen
            SceneManager.LoadScene(mainMenuSceneBuildIndex);
            OpenMenu(mainMenuParameters);
        }

        /// <summary>
        /// Reload the current scene
        /// </summary>
        public void ReloadLevel()
        {
            /// TODO: Add loading screen
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void QuitButtonPressed()
        {
            QuitGame.Quit();
        }

        public GameObject GetActiveMenu()
        {
            if (menuStack.Count != 0)
            {
                return menuStack.Peek().menuGameObject;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns true if a menu is being shown
        /// </summary>
        /// <returns></returns>
        public bool GetIsShowingMenu()
        {
            if (menuStack.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if the Main Menu is active and present
        /// </summary>
        /// <returns></returns>
        public bool GetIsInMainMenu()
        {
            return (bool)(GetActiveMenu() == mainMenuParameters.menuGameObject);
        }

        /// <summary>
        /// Open a new menu and add it to the menu stack
        /// </summary>
        /// <param name="menuToOpen"></param>
        public void OpenMenu(MenuParameters menuToOpen)
        {
            // If menuStack isn't empty, add top item as parent
            if (menuStack.Count != 0)
            {
                menuToOpen.SetParent(GetActiveMenu());
                // Check MenuParameters if parent needs to be hidden
                if (menuToOpen.isParentHidden)
                {
                    // Hide the parent from MenuParameters
                    DisableMenu(menuToOpen, true);
                }
            }

            // Add menu to stack
            menuStack.Push(menuToOpen);

            // Activate/Show menu
            EnableMenu(menuToOpen);

            // Check if sub menu is required by looking for component SubMenuHolder
            if (menuToOpen.menuGameObject.GetComponent<SubMenuHolder>())
            {
                // Find default sub menu in SubMenuHolder and OpenMenu()
                OpenMenu(menuToOpen.menuGameObject.GetComponent<SubMenuHolder>().defaultSubMenu);
            }
            else if (menuToOpen.firstSelected)
            {
                // Select the specified Selectable
                menuToOpen.firstSelected.Select();
            }
        }

        /// <summary>
        /// Close menu and remove it from the menu stack
        /// </summary>
        public void CloseMenu()
        {
            // Make sure menuParameterStack isn't empty
            if (menuStack.Count != 0)
            {
                // Remove menu from stack
                MenuParameters closingMenu = menuStack.Pop();

                // Close/hide menu
                DisableMenu(closingMenu);

                // If menuParameterStack isn't empty
                if (menuStack.Count != 0)
                {
                    // Check if current active menu is SubMenuHolder
                    if (menuStack.Peek().menuGameObject.GetComponent<SubMenuHolder>())
                    {
                        CloseMenu();
                        return;
                    }

                    MenuParameters topMenu = menuStack.Peek();

                    // Enable top-most menu
                    EnableMenu(topMenu);

                    // Select a Selectable
                    topMenu.firstSelected.Select();

                    // Check if parent is hidden, if not...
                    if (!topMenu.isParentHidden)
                    {
                        // Make an array of the remaining menus (includes the topmost)
                        MenuParameters[] remainingMenus = menuStack.ToArray();

                        // Iterate through remaining menus (skipping the topmost) to enable them and see if their parents need enabling
                        for (int i = remainingMenus.Length - 1; i < 0; i--)
                        {
                            EnableMenu(remainingMenus[i]);

                            // Break loop if parent is hidden...
                            if (remainingMenus[i].isParentHidden)
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Disable menu on top of menu stack and open another
        /// </summary>
        /// <param name="menuToSwitchTo"></param>
        public void SwitchMenu(MenuParameters menuToSwitchTo)
        {
            // Make sure menuParameterStack isn't empty
            if (menuStack.Count != 0)
            {
                // Close current menu - Remove from stack and disable
                DisableMenu(menuStack.Pop());
            }

            // Open the new menu
            OpenMenu(menuToSwitchTo);
        }

        /// <summary>
        /// Enables the menu. Note that this isn't the same as opening it, merely enabling the GameObject
        /// </summary>
        /// <param name="menuToEnable"></param>
        private void EnableMenu(MenuParameters menuToEnable)
        {
            if (menuToEnable.menuGameObject)
            {
                menuToEnable.menuGameObject.SetActive(true);
            }
            else
            {
                if (Debug.isDebugBuild && verbose) Debug.LogWarning("No gameobject set in menu parameters, unable to enable", this);
            }
        }

        /// <summary>
        /// Disables the menu. Note that this isn't the same as closing it, merely disabling the GameObject
        /// </summary>
        /// <param name="menuToDisable"></param>
        private void DisableMenu(MenuParameters menuToDisable)
        {
            menuToDisable.menuGameObject.SetActive(false);
        }

        /// <summary>
        /// Disables the menu. Note that this isn't the same as closing it, merely disabling the GameObject
        /// </summary>
        /// <param name="menuToDisable"></param>
        /// <param name="DisableParentInstead">Should the parent menu be disabled instead?</param>
        private void DisableMenu(MenuParameters menuToDisable, bool disableParentInstead)
        {
            if (disableParentInstead)
            {
                menuToDisable.GetParent().SetActive(false);
            }
            else
            {
                DisableMenu(menuToDisable);
            }
        }
    }

    /// <summary>
    /// Holds information about how to display menus
    /// </summary>
    [Serializable]
    public class MenuParameters
    {
        public GameObject menuGameObject;           // GameObject reference
        public Selectable firstSelected;            // First object that is selected by default
        private GameObject mParent;                 // Parent gameobjext reference
        public bool isParentHidden = false;         // Is the parent hidden

        public void SetParent(GameObject parent)
        {
            mParent = parent;
        }

        public GameObject GetParent()
        {
            return mParent;
        }
    }
}