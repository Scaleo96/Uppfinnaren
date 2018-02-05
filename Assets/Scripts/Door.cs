using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using UnityEngine.UI;

public class Door : Entity
{

    [Header("> Door")]

    [HideInInspector]
    public bool aminaUse = true;
    [HideInInspector] public bool idaUse = true;
    [HideInInspector] public bool jonathanUse = true;

    [SerializeField]
    ValueEvent EnterEvents;

    [Tooltip("The other door this one leads to.")]
    [SerializeField]
    Door exitDoor;

    [Tooltip("Is the Door locked?")]
    [SerializeField]
    bool doorLocked;

    [Tooltip("The minumum distance a character has to be from the door to open it.")]
    [SerializeField]
    float minRadiusDistance = 1;

    [SerializeField]
    float transitionTime = 1;

    protected override void OnInteract(Character character, EntityValues entityValues, Item item = null)
    {
        float distance = Vector2.Distance(character.transform.position, transform.position);
        if (distance <= minRadiusDistance && doorLocked == false)
        {
            StartCoroutine(EnterDoor(character));

            EntityValues values;
            values.entity = this;
            values.collider2d = null;
            values.character = character;
            values.trigger = EntityValues.TriggerType.EnterDoor;
            EnterEvents.Invoke(values);
        }
        else
        {
            EntityValues values;
            values.entity = this;
            values.collider2d = null;
            values.trigger = EntityValues.TriggerType.Inspect;
            values.character = character;
            base.Interact(character, values);
        }
    }

    private IEnumerator EnterDoor(Character character)
    {
        StartCoroutine(Fade.FadeIn(transitionTime));
        yield return new WaitForSeconds(transitionTime);
        float characterHeight = character.GetComponent<CapsuleCollider2D>().size.y;
        character.transform.position = new Vector2(exitDoor.transform.position.x, exitDoor.transform.position.y - characterHeight);
    }

    private void OnDrawGizmos()
    {
        UnityEditor.Handles.color = Color.green;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.forward, minRadiusDistance);
    }
}
