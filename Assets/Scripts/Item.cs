using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : Entity
{
    [SerializeField]
    Sprite inventorySprite;

    public Sprite InventorySprite
    {
        get
        {
            return inventorySprite;
        }

        set
        {
            inventorySprite = value;
        }
    }

    protected override void OnInteract(Character character, Item item = null)
    {
        base.OnInteract(character);

        character.AddItemToInventory(this);
        RemoveFromWorld();
    }

    private void RemoveFromWorld()
    {
        gameObject.SetActive(false);
    }
}