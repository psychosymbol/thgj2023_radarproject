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
    public override void onSpawn()
    {
        Debug.Log("testSpawning from child class");
        base.onSpawn();
    }
    public override void OnPing()
    {
        base.OnPing();
    }
    public override void OnDistract(Vector3 sonarPos)
    {
        base.OnDistract(sonarPos);
    }
}
