using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimCameraController : MonoBehaviour
{
    public float maxAngleX = 10;
    public float maxAngleY = 5;

    public static TimCameraController instance;

    // Transform of the camera to shake. Grabs the gameObject's transform
    // if null.
    public Transform camTransform;


    Vector3 originalPos;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }
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
            //camTransform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;
            camTransform.localPosition = Vector3.Lerp(camTransform.localPosition, originalPos + Random.insideUnitSphere * shakeAmount, Time.deltaTime * lerpFactor);
            shakeDuration -= Time.deltaTime * decreaseFactor;
        }
        else
        {
            shakeDuration = 0f;
            camTransform.localPosition = Vector3.Lerp(camTransform.localPosition, originalPos, Time.deltaTime * lerpFactor);
        }

    }


    // How long the object should shake for.
    public float shakeDuration = 0f;

    // Amplitude of the shake. A larger value shakes the camera harder.
    public float shakeAmount = 0.7f;
    public float decreaseFactor = 1.0f;
    public float lerpFactor = 3f;
    public void Shake(float shakeDuration,float shakeAmount, float decreaseFactor,float lerpFactor = 3)
    {
        this.shakeDuration = shakeDuration;
        this.shakeAmount = shakeAmount;
        this.decreaseFactor = decreaseFactor;
        this.lerpFactor = lerpFactor;
    }
}
