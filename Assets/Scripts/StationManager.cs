using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationManager : MonoBehaviour
{
    public static StationManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }
    }

    public List<StationController> stationControllers = new List<StationController>();

    // Start is called before the first frame update
    void Start()
    {
        RadarGridController.instance.stations.Clear();
        foreach (StationController station in stationControllers)
        {
            station.SetupStation();
            RadarGridController.instance.stations.Add(station.transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public StationController GetNextDockingable()
    {
        var currentDepth = GameManager.instance.currentDepth;
        for (int i = 0; i < stationControllers.Count; i++)
        {
            var station = stationControllers[i];
            if (station.dockingEnable && station.depth > currentDepth)
            {
                return station;
            }
        }
        return null;
    }

}
