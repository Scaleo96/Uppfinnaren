using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

struct InventorySlot
{
    public Item item;
    public GameObject slot;
    public bool isSelected;
}

public class GameController : MonoBehaviour
{
    public static GameController instance = null;

    [SerializeField]
    Camera cameraComponent;

    [Header("Character Settings")]
    [SerializeField]
    Character[] characters;
    Character currentCharacter;
    int currentCharID;

    [Header("Mouse Setting")]
    [SerializeField]
    Entity hoverEntity;

    [SerializeField]
    Entity selectedEntity;

    [Header("UI Settings")]
    [SerializeField]
    GameObject hoverTextObject;
    Text hoverText;

    [SerializeField]
    bool textFollowMouse = true;

    [SerializeField]
    GameObject inventoryObject;
    InventorySlot[][] inventorySlots;

    [SerializeField]
    GameObject inventorySlotPrefab;


    /// <summary>
    /// The entity that the mouse is hovering over.
    /// </summary>
    public Entity HoverEntity
    {
        get
        {
            return hoverEntity;
        }

        set
        {
            hoverEntity = value;
        }
    }

    /// <summary>
    /// The entity that is selected by the player (i.e has been clicked on).
    /// </summary>
    public Entity SelectedEntity
    {
        get
        {
            return selectedEntity;
        }

        set
        {
            selectedEntity = value;
        }
    }


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        hoverText = hoverTextObject.GetComponentInChildren<Text>();

        inventorySlots = new InventorySlot[characters.Length][];

        // Set inventory slot sizes.
        for (int i = 0; i < characters.Length; i++)
        {
            inventorySlots[i] = new InventorySlot[characters[i].InventorySize];
        }

        // Instantiate inventory slots.
        ChangeCharacter(0);

        // Get main camera if camera isn't specified.
        if (!cameraComponent)
        {
            cameraComponent = Camera.main;
        }

    }

    private void Update()
    {
        RaycastSelect();

        if (Input.GetButtonDown("Change Character"))
        {
            if (currentCharID < 2)
            {
                ChangeCharacter(currentCharID + 1);
            }
            else
            {
                ChangeCharacter(0);
            }
        }
    }

    /// <summary>
    /// Checks what entity the mouse is hovering over, and if mouse is clicked: selects this entity.
    /// </summary>
    private void RaycastSelect()
    {
        hoverTextObject.transform.position = Input.mousePosition;

        RaycastHit2D hit = Physics2D.Raycast(cameraComponent.ScreenToWorldPoint(Input.mousePosition), Vector3.forward, 20);

        Entity entity;
        if (hit)
        {          
            if (entity = hit.transform.GetComponent<Entity>())
            {
                if (!textFollowMouse)
                {
                    hoverTextObject.transform.position = entity.transform.position;

                    hoverTextObject.transform.position = new Vector2(
                        hoverTextObject.transform.position.x,
                        hoverTextObject.transform.position.y + entity.GetComponent<Collider2D>().bounds.extents.y
                    );

                    hoverTextObject.transform.position = cameraComponent.WorldToScreenPoint(hoverTextObject.transform.position);
                }

                hoverEntity = entity;

                if (Input.GetButtonDown("Interact"))
                {
                    selectedEntity = entity;
                    entity.Interact(currentCharacter);
                }

                hoverText.text = entity.EntityName;
            }
            else
            {
                hoverText.text = "";
            }
        }
        else
        {
            hoverText.text = "";
        }
    }

    /// <summary>
    /// Change the currently controlled character.
    /// </summary>
    /// <param name="charID">The array index of the character.</param>
    private void ChangeCharacter(int charID)
    {
        if (currentCharacter)
        {
            currentCharacter.SetActive(false);
        }

        SetInventoryUISize(charID);

        currentCharID = charID;
        currentCharacter = characters[currentCharID];

        currentCharacter.SetActive(true);

        // TODO: Do stuff with camera
    }

    /// <summary>
    /// Resets and sets the the UI inventory slots to the given count.
    /// </summary>
    private void SetInventoryUISize(int newCharID)
    {
        foreach (InventorySlot inventorySlot in inventorySlots[currentCharID])
        {
            Destroy(inventorySlot.slot);
        }

        for (int i = 0; i < characters[newCharID].InventorySize; i++)
        {
            inventorySlots[newCharID][i].slot = Instantiate(inventorySlotPrefab, inventoryObject.transform, false);
        }
    }

}
