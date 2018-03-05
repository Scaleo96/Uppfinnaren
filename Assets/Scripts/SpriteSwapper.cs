using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteSwapper : MonoBehaviour
{
    [Tooltip("The sprite that the SpriteSwapper will swap to.")]
    [SerializeField]
    Sprite alternativeSprite;
    Sprite defaultSprite;

    [SerializeField]
    bool switchToDefaultEachFrame = true;

    SpriteRenderer spriteRenderer;

    bool isAlternative;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        defaultSprite = spriteRenderer.sprite;
    }

    private void LateUpdate()
    {
        if (switchToDefaultEachFrame)
        {
            Debug.Log(isAlternative);
            if (isAlternative)
            {
                spriteRenderer.sprite = alternativeSprite;
            }
            else if (spriteRenderer.sprite != defaultSprite)
            {
                spriteRenderer.sprite = defaultSprite;
            }

            isAlternative = false;
        }      
    }

    /// <summary>
    /// Swaps between the specified alternative sprite and the default sprite.
    /// </summary>
    public void SwapSprite()
    {
        isAlternative = spriteRenderer.sprite == alternativeSprite;
        spriteRenderer.sprite = isAlternative ? defaultSprite : alternativeSprite;
    }

    /// <summary>
    /// Resets the sprite of the object to the sprite it started with.
    /// </summary>
    public void ResetSprite()
    {
        if (isAlternative)
        {
            spriteRenderer.sprite = defaultSprite;
            isAlternative = false;
        }
    }

    /// <summary>
    /// Sets the sprite of the object to the specified alternative sprite.
    /// </summary>
    public void SetAlternativeSprite()
    {
        if (isAlternative == false)
        {
            spriteRenderer.sprite = alternativeSprite;
            isAlternative = true;
        }
    }

    public void SetAlternativeSpriteThisFrame()
    {
        isAlternative = true;
    }

    /// <summary>
    /// If the sprite of the object is the specified alternative sprite.
    /// </summary>
    public bool IsAlternative
    {
        get
        {
            return isAlternative;
        }
    }
}
