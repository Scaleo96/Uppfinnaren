using UnityEditor;
using UnityEngine;

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
    }

    private void CheckSceneForEntities()
    {
        Entity[] allEntities = GameObject.FindObjectsOfType<Entity>();

        foreach (Entity entity in allEntities)
        {
            bool hasSpriteSwapper = entity.gameObject.GetComponent<SpriteSwapper>();
            if (!hasSpriteSwapper)
            {
                Debug.LogWarning(entity.EntityName + " does not have a sprite swapper.", entity.gameObject);
            }
            else
            {
                CheckForAltSprite(entity);
            }
        }
    }
}