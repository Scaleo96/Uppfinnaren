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
    float runSpeed = 2f;

    [SerializeField]
    int inventorySize;

    [SerializeField]
    List<Item> items;

    Rigidbody2D rb2D;

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
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

        float speed = walkSpeed;
        if (Input.GetButton("Run"))
        {
            speed = runSpeed;
        }

        rb2D.velocity = new Vector2(moveHorizontal * speed, rb2D.velocity.y);
    }

    public bool AddItemToInventory(Item item)
    {
        if (items.Count < inventorySize)
        {
            items.Add(item);
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

        rb2D.velocity = new Vector2(0, rb2D.velocity.y);
    }

    public bool IsActive
    {
        get
        {
            return isActive;
        }
    }
}
