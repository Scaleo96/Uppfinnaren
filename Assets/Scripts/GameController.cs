using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public struct InventorySlot
{
    public Item item;
    public GameObject slot;
    public Image image;

    public Button button;
    public bool isSelected;
}

public class GameController : MonoBehaviour
{
    public static GameController instance = null;

    [SerializeField]
    Camera cameraComponent;
    public Camera CameraComponent
    {
        get
        {
            return cameraComponent;
        }
    }

    [Header("Character Settings")]
    [SerializeField]
    Character[] characters;
    Character currentCharacter;
    int currentCharID;

    [SerializeField]
    float cameraChangeIntensity = 2f;

    bool canChangeChar = true;

    [SerializeField]
    float changeCharCameraOffset;

    [Header("Mouse Setting")]
    [SerializeField]
    LayerMask selectableLayers;

    [SerializeField]
    GameObject hoverEntity;

    [SerializeField]
    Entity selectedEntity;

    [Header("UI Settings")]
    [SerializeField]
    GameObject hoverTextObject;
    Text hoverText;

    [SerializeField]
    GameObject hoverImageObject;
    Image hoverImage;

    [SerializeField]
    InventorySlot selectedInventorySlot;
    [SerializeField]
    bool isHoldingItem;

    [SerializeField]
    bool textFollowMouse = true;

    [SerializeField]
    GameObject inventoryObject;
    InventorySlot[][] inventorySlots;

    [SerializeField]
    GameObject inventorySlotPrefab;

    [SerializeField]
    bool isActive;

    [SerializeField]
    LayerMask groundLayer;

    [SerializeField]
    Image characterPortraitObject;

    CursorController cursorController;

    [SerializeField]
    GameObject throwCursor;

    Quaternion quaternion;

    /// <summary>
    /// The entity that the mouse is hovering over.
    /// </summary>
    public GameObject HoverEntity
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
    }

    private void Start()
    {
        quaternion = new Quaternion();

        hoverText = hoverTextObject.GetComponentInChildren<Text>();
        hoverImage = hoverImageObject.GetComponentInChildren<Image>();

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

        cursorController = FindObjectOfType<CursorController>();
    }

    private void Update()
    {
        //if (SelectedInventorySlot.item != null)
        //Debug.Log(selectedInventorySlot.item.ToString());

        // Deselect item on click.
        hoverImageObject.transform.position = Input.mousePosition;
        if (Input.GetButtonDown("Interact") && isHoldingItem && isActive)
        {
            if (hoverEntity == false)
            {
                currentCharacter.RemoveItemFromInventory(selectedInventorySlot.item);
                currentCharacter.DropItem(selectedInventorySlot.item);
                DeselectItem(selectedInventorySlot);
                UpdateInventory(currentCharID);
            }
        }
        else if ((Input.GetButtonDown("Right Click") && isHoldingItem || Input.GetButtonDown("Change Character")) && isHoldingItem)
        {
            DeselectItem(selectedInventorySlot);
            UpdateInventory(currentCharID);
        }
        // Drop big item.
        else if (Input.GetButtonDown("Interact") && currentCharacter.HasBigItem())
            currentCharacter.DropBigItem();

        // Change character on input.
        if (Input.GetButtonDown("Change Character") && canChangeChar)
        {
            if (currentCharID < (characters.Length - 1))
            {
                currentCharacter.gameObject.layer = 10;
                ChangeCharacter(currentCharID + 1);
            }
            else
            {
                ChangeCharacter(0);
            }
        }

        if (currentCharacter.HasBigItem())
        {
            if (throwCursor != null)
            {
                throwCursor.gameObject.SetActive(true);
                throwCursor.transform.position = new Vector3(cameraComponent.ScreenToWorldPoint(Input.mousePosition).x, cameraComponent.ScreenToWorldPoint(Input.mousePosition).y, 0);
                throwCursor.transform.rotation = Quaternion.LookRotation(Vector3.forward ,cameraComponent.ScreenToWorldPoint(Input.mousePosition) - currentCharacter.transform.position);
                throwCursor.transform.rotation = throwCursor.transform.rotation * Quaternion.Euler(0, 0, -90);
                throwCursor.transform.localScale = new Vector3(Vector3.Distance((Vector2)cameraComponent.ScreenToWorldPoint(Input.mousePosition), (Vector2)currentCharacter.transform.position) /2, 0.5f, 1);
            }
        }
        else
        {
            if (throwCursor != null)
            {
                throwCursor.gameObject.SetActive(false);
            }
        }

        RaycastSelect();
        RenderPortrait();
    }

    private void RenderPortrait()
    {
        if (GlobalStatics.PuzzleScreenOn)
        {
            characterPortraitObject.gameObject.SetActive(true);
        }
        else
        {
            characterPortraitObject.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Checks what entity the mouse is hovering over, and if mouse is clicked: selects this entity.
    /// </summary>
    private void RaycastSelect()
    {
        hoverTextObject.transform.position = Input.mousePosition;

        RaycastHit2D hit = Physics2D.Raycast(cameraComponent.ScreenToWorldPoint(Input.mousePosition), Vector3.forward, 20, selectableLayers);
        RaycastHit2D raycastHit2D = Physics2D.Raycast(Input.mousePosition, Vector3.forward, 20);


        Entity entity;

        if (hit && hit.transform.GetComponent<Entity>()) // If the mouse is over an entity  
        {
            entity = hit.transform.GetComponent<Entity>();
            SpriteSwapper spriteSwapper = hit.transform.GetComponentInChildren<SpriteSwapper>();

            if (cursorController != null)
            {
                cursorController.HoverCursor();
            }

            if (spriteSwapper != null)
            {
                spriteSwapper.SetAlternativeSpriteThisFrame();
            }


            if (!textFollowMouse)
            {
                hoverTextObject.transform.position = entity.transform.position;

                hoverTextObject.transform.position = new Vector2(
                    hoverTextObject.transform.position.x,
                    hoverTextObject.transform.position.y + entity.GetComponent<Collider2D>().bounds.extents.y
                );

                hoverTextObject.transform.position = cameraComponent.WorldToScreenPoint(hoverTextObject.transform.position);
            }

            hoverEntity = entity.gameObject;

            // Interact
            float distance = (entity.transform.position - currentCharacter.transform.position).magnitude;
            if (Input.GetButtonDown("Interact") && distance <= entity.InteractDistance)
            {
                InteractWithEntity(entity);
            }

            hoverText.text = entity.EntityName;
        }
        else
        {
            hoverText.text = "";
            hoverEntity = null;
        }

        if (raycastHit2D && raycastHit2D.transform.gameObject.tag == "Inventory")
        {
            hoverEntity = raycastHit2D.transform.gameObject;

            if (Input.GetButtonDown("Interact") && isHoldingItem)
            {
                DeselectItem(selectedInventorySlot);
                UpdateInventory(currentCharID);
            }
        }
    }

    /// <summary>
    /// Sets the seleted entity as well as calls Interact() on it.
    /// </summary>
    private void InteractWithEntity(Entity entity)
    {
        EntityValues values;
        values.entity = entity;
        values.character = currentCharacter;
        values.collider2d = null;
        values.item = selectedInventorySlot.item;

        if (selectedInventorySlot.item != null)
        {
            values.trigger = EntityValues.TriggerType.UseItem;
        }
        else
        {
            values.trigger = EntityValues.TriggerType.Inspect;
        }

        selectedEntity = entity;
        entity.Interact(values);
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

        UpdateInventory(charID);
        currentCharID = charID;
        currentCharacter = characters[currentCharID];

        currentCharacter.SetActive(true);
        currentCharacter.gameObject.layer = 2;
        ChangeCharacterPortrait();
        cameraComponent.GetComponent<CameraFollow>().Target = currentCharacter.transform;

        cameraComponent.GetComponent<CameraFollow>().SetPosition((Vector2)currentCharacter.transform.position + (Random.insideUnitCircle.normalized * cameraChangeIntensity));
        // Change music
        MusicMixer.MusicController.ActivateAccompanyingTrackExclusive(charID);

        // TODO: Do stuff with camera
    }

    private void ChangeCharacterPortrait()
    {
        if (currentCharacter.Characterportrait != null)
        {
            characterPortraitObject.sprite = currentCharacter.Characterportrait;
        }
    }

    /// <summary>
    /// Sets the items of each inventory slot in the currently active character.
    /// </summary>
    public void SetInventoryItems(List<Item> items)
    {
        for (int i = 0; i < items.Count; i++)
        {
            inventorySlots[currentCharID][i].item = items[i];
            UpdateInventory(currentCharID);
            //UpdateInventorySlotImage(inventorySlots[currentCharID][i]);
        }
    }

    public void AddItemToCurrentCharacter(Item item)
    {
        if (currentCharacter.IsInventoryFull() == false)
        {
            currentCharacter.AddItemToInventory(item);
        }
    }

    /// <summary>
    /// Resets and sets the the UI inventory slots to match the given character's.
    /// </summary>
    public void UpdateInventory(int charID)
    {
        foreach (InventorySlot inventorySlot in inventorySlots[currentCharID])
        {
            Destroy(inventorySlot.slot);
        }

        for (int i = 0; i < characters[charID].InventorySize; i++)
        {
            Character character = characters[charID];
            InventorySlot inventorySlot = inventorySlots[charID][i];

            inventorySlot.slot = Instantiate(inventorySlotPrefab, inventoryObject.transform, false);
            inventorySlot.button = inventorySlot.slot.GetComponent<Button>();


            if (i < character.GetItemCount())
            {
                inventorySlot.item = character.GetItemFromInventory(i);
            }
            else
            {
                inventorySlot.item = null;
            }

            UpdateInventorySlotImage(inventorySlot);
            inventorySlot.button.onClick.AddListener(new UnityAction(delegate { SelectItem(inventorySlot); }));

            inventorySlots[charID][i] = inventorySlot;
        }
    }

    /// <summary>
    /// Sets the display image of the given slot to either the specified item sprite or the sprite of the item.
    /// </summary>
    private void UpdateInventorySlotImage(InventorySlot slot)
    {
        slot.image = slot.slot.transform.GetChild(1).GetComponent<Image>();

        if (slot.item && slot.isSelected == false)
        {
            slot.image.color = new Color(1, 1, 1, 1);

            slot.image.sprite = slot.item.InventorySprite ?
                               slot.item.InventorySprite :
                               slot.item.gameObject.GetComponent<SpriteRenderer>().sprite;
        }
        else
        {
            slot.image.color = new Color(1, 1, 1, 0);
        }
    }

    /// <summary>
    /// Selects an inventory slot.
    /// </summary>
    private void SelectItem(InventorySlot slot)
    {
        if (slot.item)
        {
            isHoldingItem = true;
            selectedInventorySlot = slot;

            slot.isSelected = true;
            UpdateInventorySlotImage(slot);

            hoverImage.sprite = slot.item.InventorySprite;

            hoverImage.color = new Color(1, 1, 1, 1);

        }
        else
        {
            hoverImage.color = new Color(1, 1, 1, 0);
        }
    }

    /// <summary>
    /// Deselects and item when you drop it or try to interact with someting.
    /// </summary>
    public void DeselectItem(InventorySlot slot)
    {
        isHoldingItem = false;

        slot.isSelected = false;
        UpdateInventorySlotImage(slot);
        hoverImage.color = new Color(1, 1, 1, 0);

        // FIXME: Maybe do something else here?
        selectedInventorySlot.item = null;
    }

    public Character GetCurrentCharacter()
    {
        return currentCharacter;
    }

    /// <summary>
    /// Sets if the player can move the characters or not.
    /// </summary>
    public void SetActiveMovement(bool value)
    {
        isActive = value;
        canChangeChar = value;
        currentCharacter.SetActive(value);
        currentCharacter.Animator.SetBool("isWalking", false);
    }

    public Item SelectedItem
    {
        get
        {
            return selectedInventorySlot.item;
        }
    }

    public InventorySlot SelectedInventorySlot
    {
        get
        {
            return selectedInventorySlot;
        }
    }

    public int CurrentCharacterID
    {
        get
        {
            return currentCharID;
        }
    }

    public LayerMask GroundLayer
    {
        get
        {
            return groundLayer;
        }
    }
}
