using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
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

    [ConditionalHide("useTags", true)]
    public string tagName;

    [SerializeField]
    bool useTags;

    [SerializeField]
    bool destroyKeyOnUse;

    [SerializeField]
    float transitionTime = 1;

    [SerializeField]
    public bool setExitManually;

    [ConditionalHide("setExitManually", true)]
    public Transform manualExit;


    protected override void OnInteract(EntityValues values)
    {
        if (doorLocked == false)
        {
            StartCoroutine(EnterDoor(values.character));

            values.trigger = EntityValues.TriggerType.EnterDoor;
            base.OnInteract(values);
        }
        else if (doorLocked == true)
        {
            base.OnInteract(values);

            Debug.Log(values.item.EntityName.Length);
            Debug.Log(tagName.Length);

            if (useTags ? (values.item != null && values.item.tag == tagName) : (values.item != null && values.item == key))
            {
                SetLock(false);
                if (destroyKeyOnUse)
                {
                    GameController.instance.GetCurrentCharacter().RemoveItemFromInventory(GameController.instance.SelectedItem);
                    GameController.instance.DeselectItem(GameController.instance.SelectedInventorySlot);
                    GameController.instance.UpdateInventory(GameController.instance.CurrentCharacterID);
                }
            }
        }
    }

    private IEnumerator EnterDoor(Character character)
    {
        StartCoroutine(Fade.FadeIn(transitionTime));
        yield return new WaitForSeconds(transitionTime);
        if (!setExitManually)
        {
            float characterHeight = character.GetComponent<BoxCollider2D>().size.y;
            float doorHeight = exitDoor.GetComponent<BoxCollider2D>().size.y;
            float scaleCharacter = character.transform.localScale.y;
            float doorScale = exitDoor.transform.localScale.y;
            character.transform.position = new Vector2(exitDoor.transform.position.x, exitDoor.transform.position.y - (doorHeight * doorScale) + (characterHeight * scaleCharacter));
        }
        else
        {
            character.transform.position = manualExit.transform.position;
        }

        Camera.main.GetComponent<CameraFollow>().SetPosition(character.transform);
    }

    public void SetLock(bool value)
    {

        doorLocked = value;
    }
}
