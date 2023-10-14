using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SubmarineController : MonoBehaviour
{
    //public static SubmarineController instance;

    public float currentDepth = 0;
    public TextMeshProUGUI depthText;

    public float maxSpeed = 20;
    private float currentSpeed = 0;

    public bool Descending = false;

    //private void Awake()
    //{
    //    if (!instance) instance = this;
    //    else if (instance != this) Destroy(gameObject);
    //}

    private void Update()
    {
        currentDepth += currentSpeed * Time.deltaTime;
        depthText.text = currentDepth.ToString("f0"); 

        if (Descending)
            Descended();
        else
            Stop();
    }

    public void toggleDescendingStatus()
    {
        Descending = !Descending;
    }

    public void Descended()
    {
        currentSpeed = Mathf.Lerp(currentSpeed, maxSpeed, Time.deltaTime);
    }
    public void Stop()
    {
        currentSpeed = Mathf.Lerp(currentSpeed, 0, Time.deltaTime);
    }
}
