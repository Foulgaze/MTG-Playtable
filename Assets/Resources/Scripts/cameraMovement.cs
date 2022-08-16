using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraMovement : MonoBehaviour
{
    public float moveSpeed = 15f;
    public float horiFloor = 2;
    public float scrollSpeed = 10f;
    public bool isInverted = true;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) 
        {
            float invert = isInverted? -1 : 1;
            transform.position += moveSpeed * new Vector3(Input.GetAxisRaw("Horizontal")*invert, 0, Input.GetAxisRaw("Vertical")*invert) * Time.deltaTime;
        }
        if(Input.mouseScrollDelta.y != 0)
        {
            Vector3 newPosition = transform.position + transform.forward * Input.mouseScrollDelta.y;
            if(newPosition.y > horiFloor)
            {
                transform.position += transform.forward * Input.mouseScrollDelta.y * Time.deltaTime * scrollSpeed;
            }
            
        }
    }
}
