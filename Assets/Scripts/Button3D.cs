using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
public class Button3D : MonoBehaviour
{
    private void OnMouseDown()
    {
        onClick.Invoke();
    }

    public UnityEvent onClick = new UnityEvent();
}
