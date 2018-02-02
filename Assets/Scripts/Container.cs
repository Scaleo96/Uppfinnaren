﻿using System.Collections;
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
    UnityEvent fullEvent;

    int requiredItemsCount;

    protected override void OnInteract(Character character, Item item = null)
    {
        base.OnInteract(character);

        // Checks if the item that the player is holding is one of the required items. Then adds it.
        foreach (Item requiredItem in requiredItems)
        {
            if (item == requiredItem)
            {
                containedItems.Add(item);
                character.RemoveItemFromInventory(item);
                requiredItemsCount++;

                if (requiredItemsCount >= requiredItems.Length)
                {
                    fullEvent.Invoke();
                }
            }
        }
    }
}