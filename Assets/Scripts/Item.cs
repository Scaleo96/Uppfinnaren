using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Item : Entity
{
    [HideInInspector] public bool aminaUse;
    [HideInInspector] public bool idaUse;
    [HideInInspector] public bool jonathanUse;

    [SerializeField]
    Sprite inventorySprite;

    public Sprite InventorySprite
    {
        get
        {
            return inventorySprite;
        }

        set
        {
            inventorySprite = value;
        }
    }

    public override void Interact(Character character)
    {
        if (CanPickup(character))
        {
            base.Interact(character);

            character.AddItemToInventory(this);
            RemoveFromWorld();
        }
    }

    private void RemoveFromWorld()
    {
        gameObject.SetActive(false);
    }

    private bool CanPickup(Character character)
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

[CustomEditor(typeof(Item))]
public class ItemEditor : Editor
{
    bool isFolded;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        Item item = (Item)target;

        isFolded = EditorGUILayout.Foldout(isFolded, "Can be used by:");
        if (isFolded)
        {
            item.aminaUse = EditorGUILayout.Toggle("Amina", item.aminaUse);
            item.jonathanUse = EditorGUILayout.Toggle("Jonathan", item.jonathanUse);
            item.idaUse = EditorGUILayout.Toggle("Ida", item.idaUse);
        }
    }
}