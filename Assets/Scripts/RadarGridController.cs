using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarGridController : MonoBehaviour
{

    public List<Transform> gridGroups = new List<Transform>();
    public float gridHeight = 9;
    public float gridMovingSpeed = 1;

    public float totalDescendUnit = 0;

    public List<Transform> stations = new List<Transform>();

    public static RadarGridController instance;

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

    // Start is called before the first frame update
    void Start()
    {
        //totalDescendUnit = GameManager.instance.currentDepth * GameManager.instance.depthToUnit;
    }

    // Update is called once per frame
    void Update()
    {

        var lowestY = 0f;

        for (int i = 0; i < gridGroups.Count; i++)
        {
            var gridGroup = gridGroups[i];
            if(gridGroup.position.y < lowestY)
            {
                lowestY = gridGroup.position.y;
            }
        }

        //gridMovingSpeed = Mathf.InverseLerp(0, GameManager.instance.descendSpeed, GameManager.instance.currentSpeed);
        gridMovingSpeed = GameManager.instance.currentSpeed * GameManager.instance.depthToUnit;

        var speed = gridMovingSpeed;
        var dt = Time.deltaTime;

        totalDescendUnit += speed * dt;

        for (int i = 0; i < gridGroups.Count; i++)
        {
            var gridGroup = gridGroups[i];

            var newY = gridGroup.position.y + speed * dt;

            if (newY > gridHeight)
            {
                newY = lowestY - gridHeight;
            }

            gridGroup.position = new Vector3(
                gridGroup.position.x,
                newY,
                gridGroup.position.z
                );

        }

        for (int i = 0; i < stations.Count; i++)
        {
            var station = stations[i];

            var newY = station.position.y + speed * dt;

            station.position = new Vector3(
                station.position.x,
                newY,
                station.position.z
                );

        }

    }
}
