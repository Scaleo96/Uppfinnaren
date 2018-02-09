using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using UnityEngine.UI;

public class Door : Entity
{

    [Header("> Door")]

    [Tooltip("The other door this one leads to.")]
    [SerializeField]
    Door exitDoor;

    [Tooltip("Is the Door locked?")]
    [SerializeField]
    bool doorLocked;

    [Tooltip("Item that can activate the lock")]
    [SerializeField]
    Item key;

    [SerializeField]
    float transitionTime = 1;

    protected override void OnInteract(EntityValues values)
    {
        float distance = Vector2.Distance(values.character.transform.position, transform.position);
        if (doorLocked == false)
        {
            StartCoroutine(EnterDoor(values.character));

            values.trigger = EntityValues.TriggerType.EnterDoor;
            base.OnInteract(values);
        }
        else if (doorLocked == true)
        {
            values.trigger = EntityValues.TriggerType.UseItem;
            base.OnInteract(values);

            SetLock(false, values.item);
        }
    }

    private IEnumerator EnterDoor(Character character)
    {
        StartCoroutine(Fade.FadeIn(transitionTime));
        yield return new WaitForSeconds(transitionTime);
        float characterHeight = character.GetComponent<CapsuleCollider2D>().size.y;
        float doorHeight = exitDoor.GetComponent<BoxCollider2D>().size.y;
        character.transform.position = new Vector2(exitDoor.transform.position.x, exitDoor.transform.position.y - doorHeight + characterHeight);
    }

    public void SetLock(bool value, Item item)
    {
        if (item == key)
        {
            doorLocked = value;
        }
    }
}
