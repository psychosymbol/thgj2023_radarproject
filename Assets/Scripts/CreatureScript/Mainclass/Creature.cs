using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour
{
    public enum FishSize
    {
        XS, //.25f
        S, //.5f
        M, //1
        L, //1.5f
        XL, //2
        XXL //2.5f
    }

    public FishSize fishSize;
    public GameManager player;


    public bool spawnFinish = false;

    public virtual void Start()
    {
        player = GameManager.instance;
        sizeInit();
        onSpawn();
    }

    public virtual void Update()
    {
        if (!spawnFinish)
        {

        }
        else
        {
            behavior();
        }
    }

    public virtual void sizeInit()
    {
        switch (fishSize)
        {
            case FishSize.XS:
                transform.localScale = transform.localScale * .25f;
                break;
            case FishSize.S:
                transform.localScale = transform.localScale * .5f;
                break;
            case FishSize.M:
                transform.localScale = transform.localScale * 1f;
                break;
            case FishSize.L:
                transform.localScale = transform.localScale * 1.5f;
                break;
            case FishSize.XL:
                transform.localScale = transform.localScale * 2f;
                break;
            case FishSize.XXL:
                transform.localScale = transform.localScale * 2.5f;
                break;
            default:
                break;
        }
    }

    public virtual void behavior()
    {

    }

    public virtual void onSpawn()
    {
        Debug.Log("testSpawning from mother class");
        spawnFinish = true;
    }

    public virtual void reactionToPing()
    {

    }

    public virtual void reactionToDistract()
    {

    }
}
