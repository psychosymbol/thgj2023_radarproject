using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    public static MonsterManager instance;
    public int gameDifficulty = 1;
    public int startSpawnNumber = 4;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
        }
    }

    public List<Creature> creatures = new List<Creature>();



    public void CheckCreatures_Scan(Vector3 pos, float radius)
    {
        for (int i = 0; i < creatures.Count; i++)
        {
            var creature = creatures[i];
            Transform t = creature.transform;
                if (TestInsideCircle(t.position, pos, radius))
                {
                    if (creature.pingable)
                    {
                        creature.pingable = false;
                        creature.OnPing();
                    }
                }
        }
    }
    public void CheckCreatures_Distract(Vector3 pos, float radius)
    {
        for (int i = 0; i < creatures.Count; i++)
        {
            var creature = creatures[i];
            Transform t = creature.transform;
            if (TestInsideCircle(t.position, pos, radius))
            {
                if (creature.distractable)
                {
                    creature.distractable = false;
                    creature.OnDistract(pos);
                }
            }
        }
    }

    public void ResetCreaturePingFlag()
    {
        foreach (var creature in creatures)
        {
            creature.pingable = true;
        }
    }

    public void ResetCreatureDistractFlag()
    {
        foreach (var creature in creatures)
        {
            creature.distractable = true;
        }
    }

    public bool TestInsideCircle(Vector3 pos, Vector3 circleCenter, float radius)
    {
        if (Mathf.Pow(pos.x - circleCenter.x, 2) + Mathf.Pow(pos.y - circleCenter.y, 2) < Mathf.Pow(radius, 2))
        {
            return true;
        }

        return false;
    }

    // Start is called before the first frame update
    void Start()
    {
        spawnMonster(500, 800);
    }

    // Update is called once per frame
    void Update()
    {
        Rect elevaterRect = new Rect(-1, -1, 2, 2);
        foreach (var item in creatures)
        {
            if(elevaterRect.Contains(item.transform.position))
            item.MarkForDestroy();
        }
    }

    public GameObject monsterPrefabs;
    public Transform monsterPrefabHolder;

    public void spawnMonster(float minTargetDepth, float maxTargetDepht)
    {
        foreach (var item in creatures)
        {
            if (!item.noRandom)
                item.MarkForDestroy(.1f, false);
        }
        int monsterCreated = 0;
        while (monsterCreated < startSpawnNumber + gameDifficulty)
        {
            Creature newMonster = Instantiate(monsterPrefabs, monsterPrefabHolder).GetComponent<Creature>();
            float depth = Random.Range(minTargetDepth, maxTargetDepht);
            var unitY = depth * GameManager.instance.depthToUnit;

            newMonster.transform.position = new Vector3(
                0,
                -(unitY - RadarGridController.instance.totalDescendUnit),
                -2
                );

            Vector3 randomCirclePosition = Random.insideUnitCircle * Random.Range(1f, 1.5f);

            newMonster.transform.position += randomCirclePosition;
            creatures.Add(newMonster);
            monsterCreated++;
        }

        
    }
}
