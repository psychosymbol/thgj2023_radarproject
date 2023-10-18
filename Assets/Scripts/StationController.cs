using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationController : MonoBehaviour
{
    public float depth;

    public bool dockingEnable = true;

    [ContextMenu("SetupStation")]
    public void SetupStation()
    {
        var unitY = depth * GameManager.instance.depthToUnit;

        transform.position = new Vector3(
            0,
            -(unitY - RadarGridController.instance.totalDescendUnit),
            -2
            );
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
