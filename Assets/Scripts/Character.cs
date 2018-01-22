﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Character : Entity
{
    [Header("> Character")]

    [SerializeField]
    float walkSpeed = 1f;

    [SerializeField]
    float runSpeed = 2f;

    Rigidbody2D rb2D;

    private void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        float moveHorizontal = Input.GetAxisRaw("Horizontal");

        float speed = walkSpeed;
        if (Input.GetButton("Fire3"))
        {
            speed = runSpeed;
        }

        rb2D.velocity = new Vector2(moveHorizontal * speed, rb2D.velocity.y);
    }
}
