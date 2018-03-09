using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
public class UIMovable : MonoBehaviour
{
    [SerializeField]
    Collider2D[] falseCollision;

    [SerializeField]
    Collider2D[] requiredCollision;

    bool followMouseToggle;

    [SerializeField]
    UnityEvent continueEnterEvent;

    [SerializeField]
    UnityEvent continueExitEvent;

    [SerializeField]
    UnityEvent falseEnterEvent;

    [SerializeField]
    UnityEvent falseExitEvent;

    [SerializeField]
    bool deactivateOnUse;

    bool eventSent;

    [HideInInspector]
    public bool activated;

    private void Update()
    {
        RaycastSelect();

        if (followMouseToggle == true)
        {
            FollowMouse();
        }
    }

    private void RaycastSelect()
    {
        RaycastHit2D raycastHit2D = Physics2D.Raycast(Input.mousePosition, Vector3.forward, 20);

        if (raycastHit2D && raycastHit2D.transform.GetComponent<UIMovable>() == this)
        {
            if (Input.GetButtonDown("Interact") && GlobalStatics.MovingUI == false)
            {
                GlobalStatics.MovingUI = true;
                followMouseToggle = true;
            }
        }
        if (Input.GetButtonUp("Interact"))
        {
            GlobalStatics.MovingUI = false;
            followMouseToggle = false;
        }
    }

    private void FollowMouse()
    {
        transform.position = Input.mousePosition;
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (!followMouseToggle)
        {
            for (int i = 0; i < requiredCollision.Length; i++)
            {
                if (collision == requiredCollision[i])
                {
                    if (!eventSent)
                    {
                        continueEnterEvent.Invoke();
                    }
                    eventSent = true;
                    activated = true;
                    transform.position = new Vector3( collision.transform.position.x, collision.transform.position.y, transform.position.z);
                    if (deactivateOnUse)
                    {
                        this.gameObject.SetActive(false);
                    }
                }
            }

            for (int i = 0; i < falseCollision.Length; i++)
            {
                if (collision == falseCollision[i])
                {
                    if (!eventSent)
                    {
                        falseEnterEvent.Invoke();
                    }
                    eventSent = true;
                    transform.position = new Vector3(collision.transform.position.x, collision.transform.position.y, transform.position.z);
                    if (deactivateOnUse)
                    {
                        this.gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        for (int i = 0; i < requiredCollision.Length; i++)
        {
            if (collision == requiredCollision[i])
            {
                continueExitEvent.Invoke();
                eventSent = false;
                activated = false;
                if (deactivateOnUse)
                {
                    this.gameObject.SetActive(false);
                }
            }
        }

        for (int i = 0; i < falseCollision.Length; i++)
        {
            if (collision == falseCollision[i])
            {
                falseExitEvent.Invoke();
                eventSent = false;
                if (deactivateOnUse)
                {
                    this.gameObject.SetActive(false);
                }
            }
        }
    }
}
