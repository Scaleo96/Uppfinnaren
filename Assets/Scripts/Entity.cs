using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;

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

    bool canBeInteractedWith;
    
    /// <summary>
    /// Behaviour when entity is interacted with.
    /// </summary>
    /// <param name="character">The entity that is interacting with this entity.</param>
    public virtual void Interact(Character character, Item item = null)
    {
        if (canUseCondition.CanInteract(character) == true)
        {
            interactionEvents.Invoke();
        }
        else
        {
            // Stop interact from running...
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

[System.Serializable]
public class CanUseCondition
{
    public bool aminaUse;
    public bool idaUse;
    public bool jonathanUse;

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
