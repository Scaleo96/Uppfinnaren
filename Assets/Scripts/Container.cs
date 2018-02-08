using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Container : Entity
{
    [SerializeField]
    bool input = true;

    [SerializeField]
    Vector2 throwForceInterval;

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

        if (input)
        {
            // Checks if the item that the player is holding is one of the required items. Then adds it.
            foreach (Item requiredItem in requiredItems)
            {
                if (valuesitem == requiredItem)
                {
                    containedItems.Add(valuesitem);
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
        else
        {
            if (containedItems.Count > 0)
            {
                DropItem(containedItems[containedItems.Count - 1]);
            }
        }
    }

    public bool DropItem(Item item)
    {
        item.gameObject.SetActive(true);
        item.gameObject.transform.position = (Vector2)transform.position + (Vector2.up * GetComponent<Collider2D>().bounds.extents.y);

        Vector2 throwDir = Vector2.up + (Vector2.right * Random.Range(-1f, 1f));
        item.gameObject.GetComponent<Rigidbody2D>().AddForce(throwDir * Random.Range(throwForceInterval.x, throwForceInterval.y), ForceMode2D.Impulse);

        return containedItems.Remove(item);
    }
}
