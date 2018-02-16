using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigItem : Entity
{
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