using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimCameraController : MonoBehaviour
{
    public float maxAngleX = 10;
    public float maxAngleY = 5;



    // Transform of the camera to shake. Grabs the gameObject's transform
    // if null.
    public Transform camTransform;

    // How long the object should shake for.
    public float shakeDuration = 0f;

    // Amplitude of the shake. A larger value shakes the camera harder.
    public float shakeAmount = 0.7f;
    public float decreaseFactor = 1.0f;

    Vector3 originalPos;

    void Awake()
    {
        if (camTransform == null)
        {
            camTransform = GetComponent(typeof(Transform)) as Transform;
        }
    }

    void OnEnable()
    {
        originalPos = camTransform.localPosition;
    }


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


        if (shakeDuration > 0)
        {
            camTransform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;

            shakeDuration -= Time.deltaTime * decreaseFactor;
        }
        else
        {
            shakeDuration = 0f;
            camTransform.localPosition = originalPos;
        }

    }


    [ContextMenu("TestShake")]
    public void TestShake()
    {
        shakeDuration = 0.5f;
    }
}
