using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Conditions : MonoBehaviour
{
    public enum TriggerType { PuzzleSolved, Inspect, PositionTrigger, PickupItem, EnterDoor, UseItem, FailedUse, AlreadyUsed }

    [Header("> Condition")]

    [Tooltip("The particular entity important to the condition.")]
    public Entity entity;
    [Tooltip("The particular Character important to the condition.")]
    public Character character;
    [Tooltip("What type of condition is this.")]
    public TriggerType trigger;
    [HideInInspector]
    public bool activated = false;
    [Tooltip("If this is a position condition; select the trigger.")]
    public Collider2D collisionTrigger;
    [Tooltip("What item is used?")]
    public Item item;
}