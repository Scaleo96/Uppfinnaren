using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigItem : Entity
{
    [SerializeField]
    ValueEvent positionEvents;

    [SerializeField]
    Collider2D triggerPosition;

    [SerializeField]
    Sprite holdSprite;

    [SerializeField]
    Vector3 posoffset;

    protected override void OnInteract(EntityValues values)
    {
        values.trigger = EntityValues.TriggerType.PickupItem;
        base.OnInteract(values);

        if (values.character.HasBigItem() == false)
        {
            values.character.AddBigItem(this);
            RemoveFromWorld();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == triggerPosition)
        {
            EntityValues values;
            values.entity = this;
            values.collider2d = collision;
            values.character = null;
            values.item = null;
            values.trigger = EntityValues.TriggerType.PositionTrigger;
            positionEvents.Invoke(values);
        }
    }

    private void RemoveFromWorld()
    {
        gameObject.SetActive(false);
    }

    public Sprite HoldSprite
    {
        get
        {
            return holdSprite;
        }

        set
        {
            holdSprite = value;
        }
    }

    public Vector3 Posoffset
    {
        get
        {
            return posoffset;
        }

        set
        {
            posoffset = value;
        }
    }
}