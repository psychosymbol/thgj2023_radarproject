using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StationController : MonoBehaviour
{
    public float depth;
    public TextMeshPro stationName;
    public bool dockingEnable = true;

    [ContextMenu("SetupStation")]
    public void SetupStation(string stationName, bool newStation = false)
    {
        var unitY = depth * GameManager.instance.depthToUnit;

        transform.position = new Vector3(
            0,
            -(unitY - RadarGridController.instance.totalDescendUnit),
            -2
            );
        if (newStation)
            transform.position = new Vector3(0, -100, -2);
        this.stationName.text = stationName;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
