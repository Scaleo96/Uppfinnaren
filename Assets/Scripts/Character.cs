using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Character : Entity
{
    [SerializeField]
    ValueEvent positionEvents;

    [Header("> Character")]

    [SerializeField]
    bool isActive = false;

    [SerializeField]
    float walkSpeed = 1f;

    [SerializeField]
    float throwForce = 20f;

    [SerializeField]
    int inventorySize;

    [SerializeField]
    List<Item> items;

    [SerializeField]
    BigItem bigItem;

    [SerializeField]
    SpriteRenderer bigItemSR;

    [SerializeField]
    bool handsFree;

    [SerializeField]
    float tolerance = 0.3f;

    [SerializeField]
    float xClimbOffset = 0.75f;
    [SerializeField]
    float yClimbOffset = 1;

    [SerializeField]
    bool canClimb;

    [SerializeField]
    float climbDistance = 0.5f;

    bool isInClimbArea;

    Rigidbody2D rb2D;
    Animator animator;

    bool isFliped = false;
    Vector3 posPreClimb;

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
    }

    private void FixedUpdate()
    {
        if (isActive)
        {
            Move();

            if (Input.GetKey(KeyCode.W))
            {
                Climb();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EntityValues values;
        values.entity = null;
        values.collider2d = collision;
        values.character = this;
        values.item = null;
        values.trigger = EntityValues.TriggerType.PositionTrigger;
        positionEvents.Invoke(values);
    }

    private void Move()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");

        rb2D.velocity = new Vector2(moveHorizontal * walkSpeed, rb2D.velocity.y);

        animator.SetBool("isWalking", false);
        animator.speed = 1;

        if (moveHorizontal > tolerance || moveHorizontal < -tolerance)
        {
            animator.SetBool("isWalking", true);
            animator.speed = Mathf.Abs(moveHorizontal);
        }

        if (moveHorizontal != 0)
        {
            GetComponentInChildren<SpriteRenderer>().flipX = moveHorizontal > 0;

            if (moveHorizontal > 0 && !isFliped)
                FlipBigItem();
            else if (moveHorizontal < 0 && isFliped)
                FlipBigItem();
        }
    }

    private void FlipBigItem()
    {
        isFliped = !isFliped;

        bigItemSR.transform.localPosition = new Vector3
        (
            bigItemSR.transform.localPosition.x * -1,
            bigItemSR.transform.localPosition.y,
            bigItemSR.transform.localPosition.z
        );
    }

    public bool AddItemToInventory(Item item)
    {
        if (items.Count < inventorySize)
        {
            items.Add(item);
            GameController.instance.SetInventoryItems(items);
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool AddBigItem(BigItem item)
    {
        if (bigItem == null)
        {
            bigItem = item;
            bigItemSR.sprite = item.HoldSprite;

            if (!isFliped)
                bigItemSR.transform.Translate(item.Posoffset);
            else
                bigItemSR.transform.Translate(new Vector3(-item.Posoffset.x, item.Posoffset.y, item.Posoffset.z));

            handsFree = false;

            return true;
        }
        else
        {
            return false;
        }
    }

    public bool RemoveItemFromInventory(Item item)
    {
        return items.Remove(item);
    }

    public bool IsInventoryFull()
    {
        if (items.Count >= inventorySize)
        {
            return true;
        }

        return false;
    }

    public bool HasBigItem()
    {
        if (bigItem != null)
        {
            return true;
        }

        return false;
    }

    public void DropBigItem()
    {
        bigItem.gameObject.SetActive(true);
        bigItem.gameObject.transform.position = bigItemSR.transform.position;
        bigItem.gameObject.transform.rotation = new Quaternion(0, 0, 0, 0);

        Vector2 throwDir = GameController.instance.CameraComponent.ScreenToWorldPoint(Input.mousePosition) - bigItemSR.transform.position;
        bigItem.gameObject.GetComponent<Rigidbody2D>().AddForce(throwDir * throwForce, ForceMode2D.Impulse);

        if (!isFliped)
            bigItemSR.transform.Translate(new Vector3(-bigItem.Posoffset.x, -bigItem.Posoffset.y, -bigItem.Posoffset.z));
        else
            bigItemSR.transform.Translate(new Vector3(bigItem.Posoffset.x, -bigItem.Posoffset.y, -bigItem.Posoffset.z));

        bigItemSR.sprite = null;
        bigItem = null;
        StartCoroutine(HandsFreedom());
    }

    IEnumerator HandsFreedom()
    {
        yield return new WaitForEndOfFrame();
        handsFree = true;
    }

    public bool DropItem(Item item)
    {
        item.gameObject.SetActive(true);
        item.gameObject.transform.position = transform.position;

        Vector2 throwDir = GameController.instance.CameraComponent.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        item.gameObject.GetComponent<Rigidbody2D>().AddForce(throwDir * throwForce, ForceMode2D.Impulse);

        return items.Remove(item);
    }

    public void Climb()
    {
        if (rb2D.velocity != Vector2.zero)
            return;

        Vector2 direction = isFliped ? Vector2.right : Vector2.left;

        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, direction, climbDistance);

        foreach(RaycastHit2D hit in hits)
        {
            if(hit.collider.tag == "Climbable")
            {
                posPreClimb = transform.position;
                animator.SetTrigger("climb");
                isActive = false;
            }
        }
    }

    public void TeleportToTarget()
    {
        float x = isFliped ? xClimbOffset : -xClimbOffset;

        transform.position = new Vector3(posPreClimb.x + x, posPreClimb.y + yClimbOffset, posPreClimb.z);
        isActive = true;
    }

    public int InventorySize
    {
        get
        {
            return inventorySize;
        }
    }

    public void SetActive(bool value)
    {
        isActive = value;

        if (isActive)
        {
            animator.SetTrigger("toActiveIdle");
        }
        else
        {
            animator.SetTrigger("toInactiveIdle");
        }

        rb2D.velocity = new Vector2(0, rb2D.velocity.y);
    }

    public bool IsActive
    {
        get
        {
            return isActive;
        }
    }

    public bool HandsFree
    {
        get
        {
            return handsFree;
        }
    }

    public bool IsInClimbArea
    {
        get
        {
            return isInClimbArea;
        }

        set
        {
            isInClimbArea = value;
        }
    }

    public Item GetItemFromInventory(int index)
    {
        return items[index];
    }

    public int GetItemCount()
    {
        return items.Count;
    }
}