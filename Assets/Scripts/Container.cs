using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Container : Entity
{
    [SerializeField]
    Item[] requiredItems;

    [SerializeField]
    List<Item> containedItems;

    [SerializeField]
    ValueEvent fullEvent;

    int requiredItemsCount;

    protected override void OnInteract(EntityValues values)
    {

        base.OnInteract(values);

        // Checks if the item that the player is holding is one of the required items. Then adds it.
        foreach (Item requiredItem in requiredItems)
        {
            if (values.item == requiredItem)
            {
                containedItems.Add(values.item);
                values.character.RemoveItemFromInventory(values.item);
                requiredItemsCount++;

                if (requiredItemsCount >= requiredItems.Length)
                {
                    values.trigger = EntityValues.TriggerType.PuzzleSolved;
                    fullEvent.Invoke(values);
                }
            }
        }
    }
}
