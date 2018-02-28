using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public struct RequiredItem
{
    [ConditionalHide("useTags", true)]
    public string tagName;
    public Item item;
    public bool useTags;
}

[System.Serializable]
public class Container : Entity
{
    [SerializeField]
    bool input = true;

    [SerializeField]
    Vector2 throwForceInterval;

    [SerializeField]
    RequiredItem[] requiredItems;

    [SerializeField]
    List<Item> containedItems;

    [SerializeField]
    ValueEvent fullEvent;

    int requiredItemsCount;

    protected override void OnInteract(EntityValues values)
    {
        if (input && values.item != null)
        {
            int count = 0;

            // Checks if the item that the player is holding is one of the required items. Then adds it.
            foreach (RequiredItem requiredItem in requiredItems)
            {
                if (requiredItem.useTags ? (values.item.tag == requiredItem.tagName) : (values.item == requiredItem.item))
                {
                    containedItems.Add(values.item);
                    values.character.RemoveItemFromInventory(values.item);
                    GameController.instance.DeselectItem(GameController.instance.SelectedInventorySlot);
                    GameController.instance.UpdateInventory(GameController.instance.CurrentCharacterID);
                    requiredItemsCount++;

                    if (requiredItemsCount >= requiredItems.Length)
                    {
                        values.trigger = EntityValues.TriggerType.PuzzleSolved;
                        fullEvent.Invoke(values);
                    }
                }
                else
                {
                    count++;
                }
            }
            if (count >= requiredItems.Length)
            {
                values.trigger = EntityValues.TriggerType.FailedUse;
            }
        }
        else if (!input)
        {
            if (containedItems.Count > 0)
            {
                DropItem(containedItems[containedItems.Count - 1]);
            }
            else if (containedItems.Count == 0)
            {
                values.trigger = EntityValues.TriggerType.ContainerEmpty;
            }
        }

        base.OnInteract(values);
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
