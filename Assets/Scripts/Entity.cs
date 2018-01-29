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

    [SerializeField]
    UnityEvent interactionEvents;
    
    /// <summary>
    /// Behaviour when entity is interacted with.
    /// </summary>
    /// <param name="character">The entity that is interacting with this entity.</param>
    public virtual void Interact(Character character, Item item = null)
    {
        if (canUseCondition.CanPickup(character))
        {
            interactionEvents.Invoke();
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

    public UnityEvent InteractionEvents
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
