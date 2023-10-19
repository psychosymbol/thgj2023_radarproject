using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class MonsterManager : MonoBehaviour
{
    public static MonsterManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(this.gameObject); // game only has 1 scene. no need to dontDestroyOnload
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
                    creature.OnDistract();
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

    }

    // Update is called once per frame
    void Update()
    {

    }
}
