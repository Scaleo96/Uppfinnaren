﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

struct InventorySlot
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
    GameObject hoverImageObject;
    Image hoverImage;

    InventorySlot selectedInventorySlot;
    bool isHoldingItem;

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

    }

    private void Update()
    {
        RaycastSelect();

        // TODO: Move into own function
        // Change character on input.
        if (Input.GetButtonDown("Change Character"))
        {
            if (currentCharID < (characters.Length - 1))
            {
                ChangeCharacter(currentCharID + 1);
            }
            else
            {
                ChangeCharacter(0);
            }
        }

        // Deselect item on click.
        hoverImageObject.transform.position = Input.mousePosition;
        if (Input.GetButtonDown("Interact") && isHoldingItem)
        {
            if (hoverEntity == false)
            {
                currentCharacter.RemoveItemFromInventory(selectedInventorySlot.item);
                currentCharacter.DropItem(selectedInventorySlot.item);
            }

            DeselectItem(selectedInventorySlot);
            UpdateInventory(currentCharID);
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
        if (hit && hit.transform.GetComponent<Entity>())
        {
            entity = hit.transform.GetComponent<Entity>();

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

            // Interact
            float distance = (entity.transform.position - currentCharacter.transform.position).magnitude;
            if (Input.GetButtonDown("Interact") && distance <= currentCharacter.ItemPickupDistance)
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
    }

    /// <summary>
    /// Sets the seleted entity as well as calls Interact() on it.
    /// </summary>
    private void InteractWithEntity(Entity entity)
    {
        selectedEntity = entity;
        entity.Interact(currentCharacter, selectedInventorySlot.item);
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

        // TODO: Do stuff with camera
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

    /// <summary>
    /// Resets and sets the the UI inventory slots to match the given character's.
    /// </summary>
    private void UpdateInventory(int charID)
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
            inventorySlot.button.onClick.AddListener(new UnityAction( delegate { SelectItem(inventorySlot); } ));

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
    private void DeselectItem(InventorySlot slot)
    {
        isHoldingItem = false;

        slot.isSelected = true;
        UpdateInventorySlotImage(slot);
        hoverImage.color = new Color(1, 1, 1, 0);
    }

    public Character GetCurrentCharacter()
    {
        return currentCharacter;
    }
}