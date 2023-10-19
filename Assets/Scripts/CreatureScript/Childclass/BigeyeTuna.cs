using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigeyeTuna : Creature
{
    public override void Start()
    {
        base.Start();
    }
    public override void Update()
    {
        base.Update();
    }
    public override void sizeInit()
    {
        base.sizeInit();
    }
    public override void behavior()
    {
        if(player.descending)
        {

        }
        else
        {

        }
        base.behavior();
    }
    public override void onSpawn()
    {
        Debug.Log("testSpawning from child class");
        base.onSpawn();
    }
    public override void reactionToPing()
    {
        base.reactionToPing();
    }
    public override void reactionToDistract()
    {
        base.reactionToDistract();
    }
}
