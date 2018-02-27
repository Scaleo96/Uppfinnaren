using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SimpleSpriteAnimation : MonoBehaviour
{
    [SerializeField]
    private float rotationSpeed;

    private Vector3 rotationVector = Vector3.zero;

    private void Update()
    {
        rotationVector.z = rotationSpeed * Time.unscaledDeltaTime;
        transform.Rotate(rotationVector);
    }

    private void Start()
    {
        //RotateObject();
        
    }

    IEnumerator RotateObject()
    {
        while (false)
        {
            rotationVector.z = rotationSpeed * Time.time;
            transform.Rotate(rotationVector);
        }

        yield return null;
    }

}