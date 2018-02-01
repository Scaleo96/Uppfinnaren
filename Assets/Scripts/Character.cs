using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Character : Entity
{
    [Header("> Character")]

    [SerializeField]
    bool isActive = false;

    [SerializeField]
    float walkSpeed = 1f;

    [SerializeField]
    float throwForce = 20f;

    [SerializeField]
    float itemPickupDistance = 3f;

    [SerializeField]
    int inventorySize;

    [SerializeField]
    List<Item> items;

    Rigidbody2D rb2D;
    Animator animator;

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
        }
    }

    private void Move()
    {
        float moveHorizontal = Input.GetAxisRaw("Horizontal");

        rb2D.velocity = new Vector2(moveHorizontal * walkSpeed, rb2D.velocity.y);
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

    public bool RemoveItemFromInventory(Item item)
    {
        return items.Remove(item);
    }

    public bool DropItem(Item item)
    {
        item.gameObject.SetActive(true);
        item.gameObject.transform.position = transform.position;

        Vector2 throwDir = GameController.instance.CameraComponent.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        item.gameObject.GetComponent<Rigidbody2D>().AddForce(throwDir * throwForce, ForceMode2D.Impulse);

        return items.Remove(item);
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
            animator.SetTrigger("toActiveIdle");
        else
            animator.SetTrigger("toInactiveIdle");

        rb2D.velocity = new Vector2(0, rb2D.velocity.y);
    }

    public bool IsActive
    {
        get
        {
            return isActive;
        }
    }

    public float ItemPickupDistance
    {
        get
        {
            return itemPickupDistance;
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