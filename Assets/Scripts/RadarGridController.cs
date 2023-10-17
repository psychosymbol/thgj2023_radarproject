using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarGridController : MonoBehaviour
{

    public List<Transform> gridGroups = new List<Transform>();
    public float gridHeight = 9;

    public float testSpeed = 1;

    // Start is called before the first frame update
    void Start()
    {
        
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

        var speed = testSpeed;
        var dt = Time.deltaTime;

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

    }
}
