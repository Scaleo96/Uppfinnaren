using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class Entity : MonoBehaviour
{
    [Header("> Entity")]

    [Tooltip("The name of the entity.")]
    [SerializeField]
    string entityName;

    [SerializeField]
    protected CanUseCondition canUseCondition;

    [Header("> Events")]

    [SerializeField]
    ValueEvent interactionEvents;

    /// <summary>
    /// Behaviour when entity is interacted with.
    /// </summary>
    /// <param name="character">The entity that is interacting with this entity.</param>
    public virtual void Interact(Character character, Item item = null)
    {
        if (canUseCondition.CanPickup(character))
        {
            EntityValues values;
            values.entity = this;
            values.collider2d = null;
            values.character = character;
            values.trigger = EntityValues.TriggerType.Inspect;
            interactionEvents.Invoke(values);
        }
        else
        {
            return;
        }
    }

    public string EntityName
    {
        get
        {
            return entityName;
        }
    }

    public ValueEvent InteractionEvents
    {
        get
        {
            return interactionEvents;
        }

        set
        {
            interactionEvents = value;
        }
    }
}

[System.Serializable]
public class ValueEvent : UnityEvent<EntityValues> { }

public struct EntityValues
{
    public enum TriggerType { PuzzleSloved, Inspect, PositionTrigger, PickupItem, EnterDoor }

    public TriggerType trigger;
    public Entity entity;
    public Character character;
    public Collider2D collider2d;
}