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

    protected override void OnInteract(Character character, EntityValues entityValues, Item item = null)
    {
        EntityValues values;
        values.trigger = EntityValues.TriggerType.PickupItem;
        values.entity = this;
        values.character = character;
        values.collider2d = null;

        base.OnInteract(character, values);

        if (character.IsInventoryFull() == false)
        {
            character.AddItemToInventory(this);
            RemoveFromWorld();
        }
    }

    private void RemoveFromWorld()
    {
        gameObject.SetActive(false);
    }
}