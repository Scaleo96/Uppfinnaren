using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Fire : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Entity entity;
        if (entity = collision.GetComponent<Entity>())
        {
            if (entity.transform.GetComponentInChildren<Fire>() == false)
            {
                Instantiate(gameObject, entity.transform);
            }
        }
    }
}
