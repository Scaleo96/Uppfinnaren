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
            if (Input.GetButton("Interact"))
            {
                followMouseToggle = true;
            }
            else
            {
                followMouseToggle = false;
            }
        }
    }

    private void FollowMouse()
    {
        RectTransform pusselScreenParent = transform.parent.GetComponent<RectTransform>();
        //Bounds puzzleScreen = transform.parent.GetComponent<CapsuleCollider2D>().bounds;
       // if (puzzleScreen.Contains(Input.mousePosition))
        //{
            transform.position = Input.mousePosition;
        //}
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (!followMouseToggle)
        {
            for (int i = 0; i < requiredCollision.Length; i++)
            {
                if (collision == requiredCollision[i])
                {
                    continueEnterEvent.Invoke();
                    activated = true;
                    transform.position = collision.transform.position;
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
                    falseEnterEvent.Invoke();
                    transform.position = collision.transform.position;
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
                if (deactivateOnUse)
                {
                    this.gameObject.SetActive(false);
                }
            }
        }
    }
}
