using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Conditions : MonoBehaviour
{
    public enum TriggerType { PuzzleSolved, Inspect, PositionTrigger, PickupItem, EnterDoor, UseItem, FailedUse, ContainerEmpty, AlreadyUsed }

    [Header("> Condition")]

    [Tooltip("The particular entity important to the condition.")]
    public Entity entity;
    [ConditionalHide("useEntityName", true)]
    public string EntityName;
    [Tooltip("The particular Character important to the condition.")]
    public Character character;
    [ConditionalHide("useEntityName", true)]
    public string characterName;
    [Tooltip("What type of condition is this.")]
    public TriggerType trigger;
    [HideInInspector]
    public bool activated = false;
    [Tooltip("If this is a position condition; select the trigger.")]
    public Collider2D collisionTrigger;
    [Tooltip("What item is used?")]
    public Item item;
    [ConditionalHide("useEntityName", true)]
    public string itemName;

    public bool useEntityName;
}