using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum CursorState { Default, Hover, CLick }

public class CursorController : MonoBehaviour
{
    [SerializeField]
    CursorLockMode cursorLockMode;

    [SerializeField]
    Vector2 cursorOffset;

    [SerializeField]
    Texture2D defaultCursor;

    [SerializeField]
    Texture2D hoverCursor;

    [SerializeField]
    Texture2D clickCursor;

    Texture2D currentCursorTexture;

    bool isHovering;
    bool isHoveringUI;

    private void Start()
    {
        Cursor.lockState = cursorLockMode;
    }

    public void Update()
    {
        isHoveringUI = FindObjectOfType<EventSystem>().IsPointerOverGameObject();
    }

    private void LateUpdate()
    {
        if (Input.GetButton("Interact"))
        {
            SetCursor(CursorState.CLick);
        }
        else if (isHovering || isHoveringUI)
        {
            SetCursor(CursorState.Hover);
        }
        else
        {
            SetCursor(CursorState.Default);
        }

        isHovering = false;
    }

    public void SetCursor(CursorState cursorState)
    {
        switch (cursorState)
        {
            case CursorState.Default:
                Cursor.SetCursor(defaultCursor, cursorOffset, CursorMode.ForceSoftware);
                currentCursorTexture = defaultCursor;
                break;
            case CursorState.Hover:
                Cursor.SetCursor(hoverCursor, cursorOffset, CursorMode.ForceSoftware);
                currentCursorTexture = hoverCursor;
                break;
            case CursorState.CLick:
                Cursor.SetCursor(clickCursor, cursorOffset, CursorMode.ForceSoftware);
                currentCursorTexture = clickCursor;
                break;
        }
    }

    public void HoverCursor()
    {
        isHovering = true;
    }

    private void OnValidate()
    {
        Cursor.SetCursor(currentCursorTexture, cursorOffset, CursorMode.ForceSoftware);
    }
}
