using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Collider2D))]
public class Entity : MonoBehaviour
{
    [Header("> Entity")]

    [Tooltip("The name of the entity.")]
    [SerializeField]
    string entityName;

    [Tooltip("The minumum distance a character has to be from the entity to interact with it.")]
    [SerializeField]
    float interactDistance = 1f;

    [SerializeField]
    protected CanUseCondition canUseCondition;

    [SerializeField]
    ValueEvent interactionEvents;
    [Tooltip("If the Character is not allowed to use the entity this event will be called on interaction")]
    [SerializeField]
    ValueEvent cantUseEvents;

    [SerializeField]
    bool canOnlyBeUsedOnce;
    bool usedOnce;
    [SerializeField]
    ValueEvent alreadyUsedEvent;

    bool canBeInteractedWith;

    /// <summary>
    /// Behaviour when entity is interacted with.
    /// </summary>
    /// <param name="character">The entity that is interacting with this entity.</param>
    public void Interact(EntityValues values)
    {
        if (canUseCondition.CanInteract(values.character) == true && !usedOnce)
        {
            Logger.Log("Player clicked [" + EntityName + "] with [" + GameController.instance.GetCurrentCharacter().EntityName + "] successfully.");
            OnInteract(values);
            if (canOnlyBeUsedOnce)
            {
                usedOnce = true;
            }
        }
        else if (canUseCondition.CanInteract(values.character) == false)
        {
            Logger.Log("Player clicked [" + EntityName + "] with [" + GameController.instance.GetCurrentCharacter().EntityName + "] unsuccessfully.");
            OnCantUse(values);
        }
       
        else if (canUseCondition.CanInteract(values.character) == true && usedOnce && canOnlyBeUsedOnce)
        {
            values.trigger = EntityValues.TriggerType.AlreadyUsed;
            alreadyUsedEvent.Invoke(values);
        }
    }

    // Call function to toggle entities can use variable
    public void SetCanUseAmina()
    {
        if (canUseCondition.aminaUse)
        {
            canUseCondition.aminaUse = false;
        }

        else if (!canUseCondition.aminaUse)
        {
            canUseCondition.aminaUse = true;
        }
    }

    public void SetCanUseIda()
    {
        if (canUseCondition.idaUse)
        {
            canUseCondition.idaUse = false;
        }

        else if (!canUseCondition.idaUse)
        {
            canUseCondition.idaUse = true;
        }
    }

    public void SetCanUseJonathan()
    {
        if (canUseCondition.jonathanUse)
        {
            canUseCondition.jonathanUse = false;
        }

        else if (!canUseCondition.jonathanUse)
        {
            canUseCondition.jonathanUse = true;
        }
    }

    /// <summary>
    /// Behaviour when entity is interacted with.
    /// </summary>
    /// <param name="character">The entity that is interacting with this entity.</param>
    protected virtual void OnInteract(EntityValues values)
    {
        interactionEvents.Invoke(values);
    }

    protected virtual void OnCantUse(EntityValues values)
    {
        cantUseEvents.Invoke(values);
    }

    public string EntityName
    {
        get
        {
            return entityName;
        }
    }

    public float InteractDistance
    {
        get
        {
            return interactDistance;
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
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        UnityEditor.Handles.color = Color.blue;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.forward, interactDistance);
    }
#endif
}

[System.Serializable]
public class CanUseCondition
{
    public bool aminaUse = true;
    public bool idaUse = true;
    public bool jonathanUse = true;

    public bool CanInteract(Character character)
    {
        bool canPickup = false;

        switch (character.EntityName)
        {
            case "Amina":
                if (aminaUse)
                {
                    canPickup = true;
                }
                break;
            case "Jonathan":
                if (jonathanUse)
                {
                    canPickup = true;
                }
                break;
            case "Ida":
                if (idaUse)
                {
                    canPickup = true;
                }
                break;
            default:
                break;
        }

        return canPickup;
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(CanUseCondition))]
public class CanUseConditionDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty aminaUse = property.FindPropertyRelative("aminaUse");
        SerializedProperty idaUse = property.FindPropertyRelative("idaUse");
        SerializedProperty jonathanUse = property.FindPropertyRelative("jonathanUse");

        float height = 15;
        if (property.isExpanded)
        {
            height += EditorGUI.GetPropertyHeight(aminaUse) + EditorGUI.GetPropertyHeight(idaUse) + EditorGUI.GetPropertyHeight(jonathanUse) + 15;
        }

        return height;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty aminaUse = property.FindPropertyRelative("aminaUse");
        SerializedProperty idaUse = property.FindPropertyRelative("idaUse");
        SerializedProperty jonathanUse = property.FindPropertyRelative("jonathanUse");

        float width = EditorGUIUtility.currentViewWidth;

        position.height = 16;
        property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, "Can be used by:");
        if (property.isExpanded)
        {
            Rect aminaRect = new Rect(position.x, position.y + 20, width, 20);
            Rect idaRect = new Rect(position.x, position.y + 40, width, 20);
            Rect jonathanRect = new Rect(position.x, position.y + 60, width, 20);

            EditorGUI.PropertyField(aminaRect, aminaUse, new GUIContent("Amina"));
            EditorGUI.PropertyField(idaRect, idaUse, new GUIContent("Ida"));
            EditorGUI.PropertyField(jonathanRect, jonathanUse, new GUIContent("Jonathan"));
        }
    }
}
#endif

[System.Serializable]
public class ValueEvent : UnityEvent<EntityValues> { }

public struct EntityValues
{
    public enum TriggerType { PuzzleSolved, Inspect, PositionTrigger, PickupItem, EnterDoor, UseItem, FailedUse, ContainerEmpty, AlreadyUsed }

    public TriggerType trigger;
    public Entity entity;
    public Character character;
    public Collider2D collider2d;
    public Item item;
}
