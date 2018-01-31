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

    public override void Interact(Character character, Item item = null)
    {
        base.Interact(character);

        character.AddItemToInventory(this);
        RemoveFromWorld();
    }

    private void RemoveFromWorld()
    {
        gameObject.SetActive(false);
    }
}