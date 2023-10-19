    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationManager : MonoBehaviour
{
    public static StationManager instance;
    public GameObject stationPrefab;
    public Transform stationPrefabHolder;
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
            station.SetupStation(station.name);
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

    public void spawnNextStation()
    {
        StationController newStation = Instantiate(stationPrefab, stationPrefabHolder).GetComponent<StationController>();
        newStation.name = "Station " + stationControllers.Count;
        newStation.depth = stationControllers[stationControllers.Count - 1].depth + 1000;
        newStation.SetupStation(newStation.name, true);
        RadarGridController.instance.stations.Add(newStation.transform);
        stationControllers.Add(newStation);
    }
}
