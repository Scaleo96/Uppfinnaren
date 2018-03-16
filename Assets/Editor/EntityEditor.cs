using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(Entity))]
public class EntityEditor : Editor
{
    private Entity entity;

    public override void OnInspectorGUI()
    {
        entity = (Entity)target;

        DrawButtons();

        // Add space
        EditorGUILayout.Space();

        DrawDefaultInspector();
    }

    private bool CheckForAltSprite(Entity entity)
    {
        bool hasAltSpriteAssigned = entity.gameObject.GetComponent<SpriteSwapper>().AlternativeSpriteAssigned;
        if (!hasAltSpriteAssigned)
        {
            Debug.LogWarning(entity.EntityName + " does not have an alternate sprite assigned.", entity.gameObject);
        }
        else
        {
            bool isDefaultSameAsAlternativeSprite;
            isDefaultSameAsAlternativeSprite = entity.GetComponent<SpriteSwapper>().AlternativeSprite == entity.GetComponent<SpriteRenderer>().sprite;

            if (isDefaultSameAsAlternativeSprite)
            {
                Debug.LogWarning(entity.EntityName + " has the default sprite set as alternative sprite.", entity.gameObject);
            }
        }

        return hasAltSpriteAssigned;
    }

    private void DrawButtons()
    {
        if (GUILayout.Button("Add sprite swapper"))
        {
            if (entity.gameObject.GetComponent<SpriteSwapper>())
            {
                Debug.LogError("Entity already has a sprite swapper");
                return;
            }
            entity.gameObject.AddComponent<SpriteSwapper>();
        }

        if (GUILayout.Button("Check whole scene for entities missing highlights"))
        {
            CheckSceneForEntities();
        }

        if (GUILayout.Button("Add sprite swapper to all entities in the scene"))
        {
            AddSpriteSwapperToAllEntities();
        }
    }

    /// <summary>
    /// Only adds to those missing one
    /// </summary>
    private void AddSpriteSwapperToAllEntities()
    {
        Entity[] allEntities = FindAllEntities();

        for (int i = 0; i < allEntities.Length; i++)
        {
            GameObject entityObject = allEntities[i].gameObject;
            bool hasSpriteSwapper = entityObject.GetComponent<SpriteSwapper>();

            // Only add sprite swappers to objects with sprite renderers
            if (entityObject.GetComponent<SpriteRenderer>())
            {
                if (!hasSpriteSwapper)
                {
                    Sprite defaultSprite = entityObject.GetComponent<SpriteRenderer>().sprite;

                    // Add a new sprite swapper using the default sprite as alternative sprite
                    entityObject.AddComponent<SpriteSwapper>().AlternativeSprite = defaultSprite;
                }
            }
        }
    }

    /// <summary>
    /// Only checks those who have a SpriteRenderer
    /// </summary>
    private void CheckSceneForEntities()
    {
        Entity[] allEntities = FindAllEntities();

        for (int i = 0; i < allEntities.Length; i++)
        {
            Entity entity = (Entity)allEntities[i];

            bool hasSpriteSwapper = entity.gameObject.GetComponent<SpriteSwapper>();
            if (entity.GetComponent<SpriteRenderer>())
            {
                if (!hasSpriteSwapper)
                {
                    Debug.LogWarning(entity.EntityName + " does not have a sprite swapper.", entity.gameObject);
                }
                else
                {
                    CheckForAltSprite(entity);
                }
            }
            else
            {
                Debug.LogWarning(entity.EntityName + " has no SpriteRenderer.", entity.gameObject);
            }
        }
    }

    private static Entity[] FindAllEntities()
    {
        return FindObjectsOfType<Entity>(); ;
    }
}