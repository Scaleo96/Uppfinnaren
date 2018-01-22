﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController instance = null;

    [SerializeField]
    Camera camera;

    [Header("Character Settings")]
    [SerializeField]
    Character startCharacter;
    [SerializeField]
    Character currentCharacter;

    [Header("Mouse Setting")]
    [SerializeField]
    Entity hoverEntity;

    [SerializeField]
    Entity selectedEntity;

    [SerializeField]
    GameObject hoverTextObject;
    Text hoverText;

    [SerializeField]
    bool textFollowMouse = true;

    /// <summary>
    /// The entity that the mouse is hovering over.
    /// </summary>
    public Entity HoverEntity
    {
        get
        {
            return hoverEntity;
        }

        set
        {
            hoverEntity = value;
        }
    }

    /// <summary>
    /// The entity that is selected by the player (i.e has been clicked on).
    /// </summary>
    public Entity SelectedEntity
    {
        get
        {
            return selectedEntity;
        }

        set
        {
            selectedEntity = value;
        }
    }


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        hoverText = hoverTextObject.GetComponentInChildren<Text>();
        currentCharacter = startCharacter;

        if (!camera)
        {
            camera = Camera.main;
        }
    }

    private void Update()
    {
        RaycastSelect();
    }

    /// <summary>
    /// Checks what entity the mouse is hovering over, and if mouse is clicked: selects this entity.
    /// </summary>
    private void RaycastSelect()
    {
        hoverTextObject.transform.position = Input.mousePosition;

        RaycastHit2D hit = Physics2D.Raycast(camera.ScreenToWorldPoint(Input.mousePosition), Vector3.forward, 20);

        Entity entity;
        if (hit)
        {          
            if (entity = hit.transform.GetComponent<Entity>())
            {
                if (!textFollowMouse)
                {
                    hoverTextObject.transform.position = entity.transform.position;

                    hoverTextObject.transform.position = new Vector2(
                        hoverTextObject.transform.position.x,
                        hoverTextObject.transform.position.y + entity.GetComponent<Collider2D>().bounds.extents.y
                    );

                    hoverTextObject.transform.position = camera.WorldToScreenPoint(hoverTextObject.transform.position);
                }

                hoverEntity = entity;

                if (Input.GetButtonDown("Fire1"))
                {
                    selectedEntity = entity;
                    entity.Interact(currentCharacter);
                }

                hoverText.text = entity.EntityName;
            }
            else
            {
                hoverText.text = "";
            }
        }
        else
        {
            hoverText.text = "";
        }

    }

}
