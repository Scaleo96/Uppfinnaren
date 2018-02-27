using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanUseController : MonoBehaviour
{

    [Tooltip("A container contains an unlimited amount of conditions and a dialogue.")]
    public CanUseContainer[] canUseContainer;

    public void IsAllComplete(EntityValues entityValues)
    {
        for (int i = 0; i < canUseContainer.Length;)
        {
            // Everything breaks if you remove this
            bool failSafe = true;

            for (int j = 0; j < canUseContainer[i].conditions.Length; j++)
            {
                if (canUseContainer[i].activated.Count < canUseContainer[i].conditions.Length)
                {
                    canUseContainer[i].activated.Add(false);
                }

                ContainerEmpty(entityValues, canUseContainer[i], canUseContainer[i].conditions[j], j);
                FailedUse(entityValues, canUseContainer[i], canUseContainer[i].conditions[j], j);
                Inspect(entityValues, canUseContainer[i], canUseContainer[i].conditions[j], j);
                PositionTrigger(entityValues, canUseContainer[i], canUseContainer[i].conditions[j], j);
                PickupItem(entityValues,canUseContainer[i], canUseContainer[i].conditions[j], j);
                EnterDoor(entityValues, canUseContainer[i], canUseContainer[i].conditions[j], j);
                PuzzleSolved(entityValues, canUseContainer[i], canUseContainer[i].conditions[j], j);
                UseItem(entityValues, canUseContainer[i], canUseContainer[i].conditions[j], j);

                Debug.Log(canUseContainer[i].activated.Contains(false));
            }

            if (canUseContainer[i].activated.Contains(false) || canUseContainer[i].isDone == true)
            {
                Debug.Log("Set to false");
                failSafe = false;
            }

            if (canUseContainer[i].isDone == false && failSafe)
            {
                if (canUseContainer[i].changeCanUse == CanUseContainer.ChangeCanUse.Amina)
                {
                    canUseContainer[i].entity.SetCanUseAmina();
                    canUseContainer[i].activated.RemoveAll(ReturnTrue);
                }

                if (canUseContainer[i].changeCanUse == CanUseContainer.ChangeCanUse.Ida)
                {
                    canUseContainer[i].entity.SetCanUseIda();
                }

                if (canUseContainer[i].changeCanUse == CanUseContainer.ChangeCanUse.Jonathan)
                {
                    canUseContainer[i].entity.SetCanUseJonathan();
                }

                canUseContainer[i].isDone = true;
            }
            else
            {
                if (i < canUseContainer.Length)
                {
                    i++;
                }
            }
        }
    }

    private bool ReturnTrue(bool a)
    {
        return true;
    }

    private void ContainerEmpty(EntityValues entityValues, CanUseContainer canUseContainer, Conditions condition, int i)
    {
        if (condition.trigger == Conditions.TriggerType.ContainerEmpty && entityValues.trigger == EntityValues.TriggerType.ContainerEmpty)
        {
            if (entityValues.entity == condition.entity && entityValues.character == condition.character)
            {
                canUseContainer.activated[i] = true;
            }
        }
    }

    private void FailedUse(EntityValues entityValues, CanUseContainer canUseContainer, Conditions condition, int i)
    {
        if (condition.trigger == Conditions.TriggerType.FailedUse && entityValues.trigger == EntityValues.TriggerType.FailedUse)
        {
            if (entityValues.entity == condition.entity && entityValues.character == condition.character)
            {
                canUseContainer.activated[i] = true;
            }
        }
    }

    private void Inspect(EntityValues entityValues, CanUseContainer canUseContainer, Conditions condition, int i)
    {
        if (condition.trigger == Conditions.TriggerType.Inspect && entityValues.trigger == EntityValues.TriggerType.Inspect)
        {
            if (entityValues.entity == condition.entity && entityValues.character == condition.character)
            {
                canUseContainer.activated[i] = true;
            }
        }
    }

    private void PositionTrigger(EntityValues entityValues, CanUseContainer canUseContainer, Conditions condition, int i)
    {
        if (condition.trigger == Conditions.TriggerType.PositionTrigger && entityValues.trigger == EntityValues.TriggerType.PositionTrigger)
        {
            if (entityValues.collider2d == condition.collisionTrigger && entityValues.character == condition.character)
            {
                canUseContainer.activated[i] = true;
            }
        }
    }

    private void PickupItem(EntityValues entityValues, CanUseContainer canUseContainer, Conditions condition, int i)
    {
        if (condition.trigger == Conditions.TriggerType.PickupItem && entityValues.trigger == EntityValues.TriggerType.PickupItem)
        {
            if (entityValues.entity == condition.entity && entityValues.character == condition.character)
            {
                canUseContainer.activated[i] = true;
            }
        }
    }

    private void EnterDoor(EntityValues entityValues, CanUseContainer canUseContainer, Conditions condition, int i)
    {
        if (condition.trigger == Conditions.TriggerType.EnterDoor && entityValues.trigger == EntityValues.TriggerType.EnterDoor)
        {
            if (entityValues.entity == condition.entity && entityValues.character == condition.character)
            {
                canUseContainer.activated[i] = true;
            }
        }
    }

    private void PuzzleSolved(EntityValues entityValues, CanUseContainer canUseContainer, Conditions condition, int i)
    {
        if (condition.trigger == Conditions.TriggerType.PuzzleSolved && entityValues.trigger == EntityValues.TriggerType.PuzzleSolved)
        {
            if (entityValues.entity == condition.entity && entityValues.character == condition.character)
            {
                canUseContainer.activated[i] = true;
            }
        }
    }

    private void UseItem(EntityValues entityValues, CanUseContainer canUseContainer, Conditions condition, int i)
    {
        if (condition.trigger == Conditions.TriggerType.UseItem && entityValues.trigger == EntityValues.TriggerType.UseItem)
        {
            if (entityValues.entity == condition.entity && entityValues.character == condition.character && entityValues.item == condition.item)
            {
                canUseContainer.activated[i] = true;
            }
        }
    }
}

[System.Serializable]
public class CanUseContainer
{
    [HideInInspector]
    public List<bool> activated = new List<bool>();

    [Tooltip("What conditions are required to trigger the Change.")]
    public Conditions[] conditions;

    [HideInInspector]
    public bool isDone;

    public enum ChangeCanUse {Amina, Ida, Jonathan };
    public ChangeCanUse changeCanUse;

    public Entity entity;
}