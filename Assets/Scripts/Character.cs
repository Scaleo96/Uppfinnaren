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
    float runSpeed = 2f;

    [SerializeField]
    float throwForce = 20f;

    [SerializeField]
    float itemPickupDistance = 3f;

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EntityValues values;
        values.entity = this;
        values.collider2d = collision;
        values.character = null;
        values.trigger = EntityValues.TriggerType.PositionTrigger;
        positionEvents.Invoke(values);
    }

    private void Move()
    {
        float moveHorizontal = Input.GetAxisRaw("Horizontal");

        if (moveHorizontal != 0)
        {
            GetComponentInChildren<SpriteRenderer>().flipX = moveHorizontal > 0;
        }

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
