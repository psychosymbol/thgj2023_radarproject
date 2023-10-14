using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ScanPattern", menuName = "ScriptableObj/ScanPattern")]

public class ScanPattern : ScriptableObject
{
    public List<PingSettings> pingSettings = new List<PingSettings>();
}

[System.Serializable]
public class PingSettings
{
    public Vector3 pos;
    public float min_radius;
    public float max_radius;

    public float lineWidth;
    public int circleDivision;

    public float duration;
    public float interval;
}
