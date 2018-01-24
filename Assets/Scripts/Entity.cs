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
    UnityEvent interactionEvents;

    /// <summary>
    /// Behaviour when entity is interacted with.
    /// </summary>
    /// <param name="character">The entity that is interacting with this entity.</param>
    public virtual void Interact(Character character)
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
