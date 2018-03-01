using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
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

    protected override void OnInteract(EntityValues values)
    {
        values.trigger = EntityValues.TriggerType.PickupItem;
        base.OnInteract(values);

        if (values.character.IsInventoryFull() == false && values.character.HandsFree)
        {
            values.character.AddItemToInventory(this);
            RemoveFromWorld();
        }
    }

    private void RemoveFromWorld()
    {
        gameObject.SetActive(false);
    }
}