using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scrolling : MonoBehaviour
{
    [SerializeField]
    CameraFollow targetCamera;
    [SerializeField]
    Transform referencePoint;
    [SerializeField]
    [Range (0, 1)]
    float speedMultiplier = 0.1f;

    Vector2 startPos;

    // Use this for initialization
    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 distance = referencePoint.transform.position - targetCamera.transform.position;
        Vector2 direction = distance.normalized;

        if (direction.magnitude != 0)
        {
            transform.position = startPos - new Vector2(distance.x / direction.magnitude * speedMultiplier, 0);
            //transform.position = startPos + new Vector2(distance.x / direction.magnitude * speed, distance.y / direction.magnitude * speed);
        }
    }
}
