using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteSwapper : MonoBehaviour
{
    [Tooltip("The sprite that the SpriteSwapper will swap to.")]
    [SerializeField]
    private Sprite alternativeSprite;

    private Sprite defaultSprite;

    [SerializeField]
    private bool switchToDefaultEachFrame = true;

    private SpriteRenderer spriteRenderer;

    [SerializeField]
    [Tooltip("Add a custom sprite renderer to perform the sprite swap with that isn't part of the base object. Leave blank for default behaviour.")]
    private SpriteRenderer spriteRendererOverride;

    private bool isAlternative;

    private void Awake()
    {
        spriteRenderer = GetSpriteRenderer();
        defaultSprite = spriteRenderer.sprite;
    }

    private SpriteRenderer GetSpriteRenderer()
    {
        if (spriteRendererOverride != null)
        {
            return spriteRendererOverride;
        }
        else if (GetComponent<SpriteRenderer>())
        {
            return GetComponent<SpriteRenderer>();
        }
        else
        {
            return GetComponentInChildren<SpriteRenderer>();
        }
    }

    private void LateUpdate()
    {
        if (switchToDefaultEachFrame)
        {
            Debug.Log(isAlternative);
            if (isAlternative)
            {
                spriteRenderer.sprite = AlternativeSprite;
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
        isAlternative = spriteRenderer.sprite == AlternativeSprite;
        spriteRenderer.sprite = isAlternative ? defaultSprite : AlternativeSprite;
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
            spriteRenderer.sprite = AlternativeSprite;
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

    /// <summary>
    /// Has the sprite swapper been assigned an alternative sprite?
    /// </summary>
    public bool AlternativeSpriteAssigned
    {
        get
        {
            if (AlternativeSprite != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public Sprite AlternativeSprite
    {
        get
        {
            return alternativeSprite;
        }

        set
        {
            alternativeSprite = value;
        }
    }
}