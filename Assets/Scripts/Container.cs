using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : Entity
{
    [SerializeField]
    Item[] requiredItems;

    [SerializeField]
    List<Item> containedItem;

    public override void Interact(Character character, Item item = null)
    {
        base.Interact(character);

        foreach (Item requiredItem in requiredItems)
        {
            if (item == requiredItem)
            {

            }
        }
    }
}
