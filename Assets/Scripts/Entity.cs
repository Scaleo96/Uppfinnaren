using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Entity : MonoBehaviour
{
    [Header("> Entity")]

    [Tooltip("The name of the entity.")]
    [SerializeField]
    string entityName;

    [SerializeField]
    UnityEvent interactionEvents;

    /// <summary>
    /// Behaviour when entity is interacted with.
    /// </summary>
    /// <param name="entity">The entity that is interacting with this entity.</param>
    public virtual void Interact(Entity entity)
    {
        interactionEvents.Invoke();
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
