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
}