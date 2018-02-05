using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionsManager : MonoBehaviour
{
    [Tooltip("A container contains an unlimited amount of conditions and a dialogue.")]
    public ConditionContainer[] container;

    public ConditionValues CheckConditions(EntityValues entityValues)
    {
        return IsAllComplete(entityValues);
    }

    public ConditionValues IsAllComplete(EntityValues entityValues)
    {
        int i = 0;
        ConditionValues values;
        for (i = 0; i < container.Length;)
        {
            bool test = true;
            for (int j = 0; j < container[i].conditions.Length; j++)
            {
                Inspect(entityValues, container[i].conditions[j]);
                PositionTrigger(entityValues, container[i].conditions[j]);
                PickupItem(entityValues, container[i].conditions[j]);
                EnterDoor(entityValues, container[i].conditions[j]);
                if (container[i].conditions[j].activated == false || container[i].isComplete == true)
                {
                    test = false;
                }
            }
            if (container[i].isComplete == false && test == true)
            {
                container[i].isComplete = true;
                values.meetComplete = true;
                values.containerNumber = i;
                return values;
            }
            else
            {
                if (i < container.Length)
                {
                    i++;
                }
            }
        }
        values.meetComplete = false;
        values.containerNumber = 0;
        return values;
    }

    private void Inspect(EntityValues entityValues, Conditions condition)
    {
        if (condition.trigger == Conditions.TriggerType.Inspect && entityValues.trigger == EntityValues.TriggerType.Inspect)
        {
            if (entityValues.entity == condition.entity && entityValues.character == condition.character)
            {
                condition.activated = true;
            }
        }
    }

    private void PositionTrigger(EntityValues entityValues, Conditions condition)
    {
        if (condition.trigger == Conditions.TriggerType.PositionTrigger && entityValues.trigger == EntityValues.TriggerType.PositionTrigger)
        {
            if (entityValues.collider2d == condition.collisionTrigger && entityValues.entity == condition.entity)
            {
               condition.activated = true;
            }
        }
    }

    private void PickupItem(EntityValues entityValues, Conditions condition)
    {
        if (condition.trigger == Conditions.TriggerType.PickupItem && entityValues.trigger == EntityValues.TriggerType.PickupItem)
        {
            if (entityValues.entity == condition.entity && entityValues.character == condition.character)
            {
                condition.activated = true;
            }
        }
    }

    private void EnterDoor(EntityValues entityValues, Conditions condition)
    {
        if (condition.trigger == Conditions.TriggerType.EnterDoor && entityValues.trigger == EntityValues.TriggerType.EnterDoor)
        {
            if (entityValues.entity == condition.entity && entityValues.character == condition.character)
            {
                condition.activated = true;
            }
        }
    }
}

public struct ConditionValues
{
    public bool meetComplete;
    public int containerNumber;
}

[System.Serializable]
public class ConditionContainer
{
    [Tooltip("What conditions are required to trigger the dialogue.")]
    public Conditions[] conditions;
    [Tooltip("What dialogue is triggered.")]
    public Dialogue dialogue;

    public bool isComplete;
}