using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float speed = 40f;
    public float borderThickness = 25f;
    private Vector3 pos;

    void Update()
    {
        pos = transform.position;
        handleKeyMovement();
        //handleMouseMovement();
        transform.position = pos;
    }

    private void handleMouseMovement()
    {
        if (Input.mousePosition.y >= Screen.height - borderThickness)
        {
            moveUp();
        }
        else if (Input.mousePosition.y <= borderThickness)
        {
            moveDown();
        }
        else if (Input.mousePosition.x >= Screen.width - borderThickness)
        {
            moveRight();
        }
        else if (Input.mousePosition.x <= borderThickness)
        {
            moveLeft();
        }
    }

    private void handleKeyMovement()
    {
        if (Input.GetKey(KeyCode.UpArrow)) 
        {
            moveUp();
        } 
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            moveDown();
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            moveRight();
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            moveLeft();
        }
    }

    private void moveUp()
    {
        pos.y += speed * Time.deltaTime;
    }

    private void moveDown()
    {
        pos.y -= speed * Time.deltaTime;
    }

    private void moveRight()
    {
        pos.x += speed * Time.deltaTime;
    }

    private void moveLeft()
    {
        pos.x -= speed * Time.deltaTime;
    }
}
