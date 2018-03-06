using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    [SerializeField, Tooltip("Target for the camera to follow")] Transform target;

    [Header("Settings")]

    [SerializeField, Tooltip("Lock the camera y movement.")]
    bool lockY = false;
    [SerializeField, Tooltip("Lock the camera x movement.")]
    bool lockX = false;
    [SerializeField, Tooltip("Distance in x and y axis the player can move before the camera follows")]
    Vector2 XAndYIdelMargin = new Vector2(2,2);
    [SerializeField, Tooltip("")]
    Vector2 XAndYOffset = new Vector2(0,-2);
    [SerializeField, Range(0, 10), Tooltip("How smoothly the camera moves toward the target in the x axis (lower is smoother).")]
    float lerpSpeedX = 1.5f;
    [SerializeField, Range(0, 10), Tooltip("How smoothly the camera moves toward the target in the Y axis (lower is smoother).")]
    float lerpSpeedY = 1.5f;
    [SerializeField, Range(0, 1.5f), Tooltip("How smoothly the camera moves toward the mouse in the x axis (lower is smoother).")]
    float lerpToMouseSpeedX = 1.5f;
    [SerializeField, Range(0, 1.5f), Tooltip("How smoothly the camera moves toward the mouse in the Y axis (lower is smoother).")]
    float lerpToMouseSpeedY = 1.5f;
    [SerializeField, Tooltip("The maximum x and y coordinates the camera can have.")]
    Vector2 maxXAndY = new Vector2(100,100);
    [SerializeField, Tooltip("The minimum x and y coordinates the camera can have.")]
    Vector2 minXAndY = new Vector2(-100,-100);

    Vector3 targetPos;
    Vector3 pos;
    Camera cameraComponent;

    private void Start()
    {
        cameraComponent = GetComponent<Camera>();

        transform.position = new Vector3(transform.position.x + XAndYOffset.x, transform.position.y + XAndYOffset.y, transform.position.z);
    }

    private void LateUpdate()
    {
        FollowTarget();
    }

    private void FollowTarget()
    {
        targetPos = target.position;
        pos = new Vector3(transform.position.x - XAndYOffset.x, transform.position.y - XAndYOffset.y, transform.position.z);

        Vector3 mousePos = cameraComponent.ScreenToWorldPoint(Input.mousePosition);

        // If the player has moved beyond the x margin.
        if (!lockX && Mathf.Abs((pos - targetPos).x) > XAndYIdelMargin.x)
        {
            // Lerp between the camera's current x position and the target's current x position.
            if (targetPos.x < pos.x)
                pos.x = Mathf.Lerp(pos.x, targetPos.x + XAndYIdelMargin.x, lerpSpeedX * Time.deltaTime);
            else
                pos.x = Mathf.Lerp(pos.x, targetPos.x - XAndYIdelMargin.x, lerpSpeedX * Time.deltaTime);
        }
        else if (Input.GetButton("Mouse look"))
        {
            pos.x = Mathf.Lerp(pos.x, mousePos.x, lerpToMouseSpeedX * Time.deltaTime);
            pos.x = Mathf.Clamp(pos.x, targetPos.x - XAndYIdelMargin.x, targetPos.x + XAndYIdelMargin.x);
        }

        // If the player has moved beyond the y margin.
        if (!lockY && Mathf.Abs((pos - targetPos).y) > XAndYIdelMargin.y)
        {
            // Lerp between the camera's current y position and the targets's current y position.
            if (targetPos.y < pos.y)
                pos.y = Mathf.Lerp(pos.y, targetPos.y + XAndYIdelMargin.y, lerpSpeedY * Time.deltaTime);
            else
                pos.y = Mathf.Lerp(pos.y, targetPos.y - XAndYIdelMargin.y, lerpSpeedY * Time.deltaTime);
        }
        else if (Input.GetButton("Mouse look"))
        {
            pos.y = Mathf.Lerp(pos.y, mousePos.y, lerpToMouseSpeedY * Time.deltaTime);
            pos.y = Mathf.Clamp(pos.y, targetPos.y - XAndYIdelMargin.y, targetPos.y + XAndYIdelMargin.y);
        }

        // Clamp the camera positon between the max and min size.
        pos.x = Mathf.Clamp(pos.x, minXAndY.x, maxXAndY.x);
        pos.y = Mathf.Clamp(pos.y, minXAndY.y, maxXAndY.y);

        // Set the new camera position.
        transform.position = new Vector3(pos.x + XAndYOffset.x, pos.y + XAndYOffset.y, transform.position.z);
    }

    public void SetPosition(Transform targetTransform)
    {
        transform.position = new Vector3(targetTransform.position.x + XAndYOffset.x, targetTransform.position.y + XAndYOffset.y, transform.position.z);
    }

    public void SetPosition(Vector3 targetPosition)
    {
        transform.position = new Vector3(targetPosition.x + XAndYOffset.x, targetPosition.y + XAndYOffset.y, transform.position.z);
    }

    // Draw target idel area.
    void OnDrawGizmos()
    {
        Vector3 gizmoPos = new Vector3(transform.position.x - XAndYOffset.x, transform.position.y - XAndYOffset.y, transform.position.z);

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(gizmoPos, new Vector3(XAndYIdelMargin.x * 2, XAndYIdelMargin.y * 2, 0));
    }

    /// <summary>
    /// Target for the camera to follow.
    /// </summary>
    public Transform Target
    {
        get
        {
            return target;
        }

        set
        {
            target = value;
        }
    }
}