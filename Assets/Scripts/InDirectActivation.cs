using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public struct RequiredActivation
{
    public bool useTags;

    [ConditionalHide("useTags", true)]
    public string tagName;
    public Entity entity;
    [HideInInspector]
    public bool activated;
}

public class InDirectActivation : MonoBehaviour
{
    [ConditionalHide("giveItem", true)]
    [SerializeField]
    Vector2 throwForceInterval;

    [ConditionalHide("giveItem", true)]
    [SerializeField]
    Item item;

    [SerializeField]
    bool giveItem;

    [SerializeField]
    RequiredActivation[] requiredActivations;

    [SerializeField]
    UnityEvent completeEvent;

    bool allComplete;

    public void CheckActivations(Entity entity)
    {
        for (int i = 0; i < requiredActivations.Length; i++)
        {
            if (entity == requiredActivations[i].entity)
            {
                requiredActivations[i].activated = true;
            }
        }
        CompleteCheck();
    }

    public bool CompleteCheck()
    {
        Debug.Log("Checking");
        for (int i = 0; i < requiredActivations.Length; i++)
        {
            Debug.Log(requiredActivations[i].activated);
            if (requiredActivations[i].activated == false)
            {
                return false;
            }
        }

        completeEvent.Invoke();

        if (giveItem)
        {
            DropItem(item);
        }

        return true;
    }

    public void DropItem(Item item)
    {
        item.gameObject.SetActive(true);
        item.gameObject.transform.position = (Vector2)transform.position + (Vector2.up * GetComponent<Collider2D>().bounds.extents.y);

        Vector2 throwDir = Vector2.up + (Vector2.right * Random.Range(-1f, 1f));
        item.gameObject.GetComponent<Rigidbody2D>().AddForce(throwDir * Random.Range(throwForceInterval.x, throwForceInterval.y), ForceMode2D.Impulse);
    }
}
