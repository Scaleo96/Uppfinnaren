using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Material { None, Grass, Woods, Stone, Forest }

public class MaterialScript : MonoBehaviour
{
    [SerializeField]
    Material material;

    public Material Material
    {
        get
        {
            return material;
        }
    }
}