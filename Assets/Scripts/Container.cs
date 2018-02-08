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
    UnityEvent fullEvent;

    int requiredItemsCount;

    protected override void OnInteract(Character character, EntityValues entityValues, Item item = null)
    {
        base.OnInteract(character, entityValues);

        if (input)
        {
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
