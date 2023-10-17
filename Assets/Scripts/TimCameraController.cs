using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimCameraController : MonoBehaviour
{
    public float maxAngleX = 10;
    public float maxAngleY = 5;
    private void Update()
    {
        Vector3 middleCenterBasedMouse =
            (Camera.main.ScreenToViewportPoint(Input.mousePosition) //get mouse position base on cursor location on the game' viewport
            + new Vector3(-0.5f, -0.5f, 0)) //set the value to make the center of the screen become zero position
            * 2; //multiplyer to make the value we get round up to 1 ?

        //Debug.Log("try to base on middle screen as center : " + middleCenterBasedMouse);

        gameObject.transform.eulerAngles = new Vector3(
            (-middleCenterBasedMouse.y * maxAngleX),
            (middleCenterBasedMouse.x * maxAngleY),
            gameObject.transform.eulerAngles.z
            );
    }
}
