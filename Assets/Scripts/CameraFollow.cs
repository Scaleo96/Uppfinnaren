using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    [SerializeField, Tooltip("Target for the camera to follow")] Transform target;

    [Header("Settings")]

    [SerializeField, Tooltip("Lock the camera y movement.")]
    bool lockY;
    [SerializeField, Tooltip("Lock the camera x movement.")]
    bool lockX;
    [SerializeField, Tooltip("Distance in the x axis the player can move before the camera follows.")]
    float xIdelMargin;
    [SerializeField, Tooltip("Distance in the y axis the player can move before the camera follows.")]
    float yIdelMargin;
    [SerializeField, Range(0, 1.5f), Tooltip("How smoothly the camera catches up with it's target movement in the x axis (lower is smoother).")]
    float lerpSpeedX;
    [SerializeField, Range(0, 1.5f), Tooltip("How smoothly the camera catches up with it's target movement in the Y axis (lower is smoother).")]
    float lerpSpeedY;
    [SerializeField, Tooltip("The maximum x and y coordinates the camera can have.")]
    Vector2 maxXAndY;
    [SerializeField, Tooltip("The minimum x and y coordinates the camera can have.")]
    Vector2 minXAndY;

    Vector3 targetPos;
    Vector3 newPos;
    Camera cameraComponent;

    private void Start()
    {
        cameraComponent = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        FollowTarget();
    }

    private void FollowTarget()
    {
        targetPos = target.position;
        newPos = transform.position;

        Vector3 mousePos = cameraComponent.ScreenToWorldPoint(Input.mousePosition);

        // If the player has moved beyond the x margin.
        if (!lockX && Mathf.Abs((transform.position - target.position).x) > xIdelMargin)
        {
            //Lerp between the camera's current x position and the target's current x position.
            newPos.x = Mathf.Lerp(newPos.x, targetPos.x, lerpSpeedX * Time.deltaTime);
        }
        else
        {
            newPos.x = Mathf.Lerp(newPos.x, mousePos.x, lerpSpeedX * Time.deltaTime);
            newPos.x = Mathf.Clamp(newPos.x, targetPos.x - xIdelMargin, targetPos.x + xIdelMargin);
        }

        // If the player has moved beyond the y margin.
        if (!lockY && Mathf.Abs((transform.position - target.position).y) > yIdelMargin)
        {
            //Lerp between the camera's current y position and the targets's current y position.
            newPos.y = Mathf.Lerp(newPos.y, targetPos.y, lerpSpeedY * Time.deltaTime);
        }
        else
        {
            newPos.y = Mathf.Lerp(newPos.y, mousePos.y, lerpSpeedY * Time.deltaTime);
            newPos.y = Mathf.Clamp(newPos.y, targetPos.y - yIdelMargin, targetPos.y + yIdelMargin);
        }

        // Clamp the camera positon between the max and min size.
        newPos.x = Mathf.Clamp(newPos.x, minXAndY.x, maxXAndY.x);
        newPos.y = Mathf.Clamp(newPos.y, minXAndY.y, maxXAndY.y);

        // Set the new camera position.
        transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);
    }

    // Draw target idel area.
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(xIdelMargin * 2, yIdelMargin * 2, 0));
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