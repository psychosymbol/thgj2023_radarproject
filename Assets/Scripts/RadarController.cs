using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class RadarController : MonoBehaviour
{
    public static RadarController instance;


    [Header("Settings")]

    public float lineWidth = 0.1f;
    public Color lineColor = Color.white;
    public int circleDivision = 50;

    public float pingDuration = 0.5f;
    public float pingInterval = 0.1f;

    public float pingDelay = 0.5f;

    public float min_radius = 1;
    public float max_radius = 5;

    public List<ScanPattern> scanPatterns = new List<ScanPattern>();

    [Header("Prefabs")]
    public GameObject line_prefab;
    public GameObject sonarping_prefab;

    [Header("RadarUI")]

    public List<LineRenderer> lineRenderers = new List<LineRenderer>();

    public LineRenderer LT;
    public LineRenderer RT;
    public LineRenderer LB;
    public LineRenderer RB;
    public LineRenderer MT;
    public LineRenderer MB;

    [Header("RadarUI Settings")]
    public float ui_maxwidth;
    public float ui_maxheight;
    public float ui_innerwidth;
    public float ui_innerheight;

    [Header("Obj ref")]
    public Transform sonar_group;
    public List<Transform> testObjects = new List<Transform>();

    bool hideFlag = false;

    public int hideFrame = 4;
    public int hideFrameCount = 0;


    public ScanPattern ping_pattern;
    public ScanPattern distract_pattern;

    float currentTime = 0f;
    int currentPattern = 0;

    bool isPinging = false;
    bool stopPingFlag = false;

    void ShowUI()
    {
        for (int i = 0; i < lineRenderers.Count; i++)
        {
            lineRenderers[i].startColor = new Color(lineRenderers[i].startColor.r, lineRenderers[i].startColor.g, lineRenderers[i].startColor.b, 1);
            lineRenderers[i].endColor = new Color(lineRenderers[i].startColor.r, lineRenderers[i].startColor.g, lineRenderers[i].startColor.b, 1);
        }
    }

    void HideUI()
    {
        for (int i = 0; i < lineRenderers.Count; i++)
        {
            lineRenderers[i].startColor = new Color(lineRenderers[i].startColor.r, lineRenderers[i].startColor.g, lineRenderers[i].startColor.b, 0);
            lineRenderers[i].endColor = new Color(lineRenderers[i].startColor.r, lineRenderers[i].startColor.g, lineRenderers[i].startColor.b, 0);
        }
    }

    public void flashHide()
    {
        hideFrameCount = 0;
        hideFlag = true;
        HideUI();
    }

    public void TestInsideCircle(Vector3 pos, float radius)
    {

        for (int i = 0; i < testObjects.Count; i++)
        {
            Transform t = testObjects[i];

            //(x-center_x)^2 + (y - center_y)^2 < radius^2

            if (Mathf.Pow(t.position.x - pos.x, 2) + Mathf.Pow(t.position.y - pos.y, 2) < Mathf.Pow(radius, 2))
            {
                t.GetComponent<SpriteRenderer>().color = Color.red;
            }

        }

    }

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

    [ContextMenu("SetUpRadarUI")]
    public void SetUpRadarUI()
    {
        var zValue = -1;

        var halfwidth = ui_maxwidth / 2;
        var halfheight = ui_maxheight / 2;
        var halfinnerwidth = ui_innerwidth / 2;
        var halfinnerheight = ui_innerheight / 2;

        // LT
        LT.SetPosition(0, new Vector3(halfwidth, halfheight, zValue));
        LT.SetPosition(1, new Vector3(halfinnerwidth, halfinnerheight, zValue));
        // RT
        RT.SetPosition(0, new Vector3(-halfwidth, halfheight, zValue));
        RT.SetPosition(1, new Vector3(-halfinnerwidth, halfinnerheight, zValue));

        // LB
        LB.SetPosition(0, new Vector3(halfwidth, -halfheight, zValue));
        LB.SetPosition(1, new Vector3(halfinnerwidth, -halfinnerheight, zValue));
        // RB
        RB.SetPosition(0, new Vector3(-halfwidth, -halfheight, zValue));
        RB.SetPosition(1, new Vector3(-halfinnerwidth, -halfinnerheight, zValue));

        // MT
        MT.SetPosition(0, new Vector3(0, halfheight, zValue));
        MT.SetPosition(1, new Vector3(0, halfinnerheight, zValue));

        // MB
        MB.SetPosition(0, new Vector3(0, -halfheight, zValue));
        MB.SetPosition(1, new Vector3(0, -halfinnerheight, zValue));
    }


    [ContextMenu("TestCreateCircle")]
    public void TestCreateCircle(int index = -1)
    {
        if (index == -1)
        {
            index = Random.Range(0, 6);
        }

        var patterns = scanPatterns[index];

        Ping(patterns, Color.green);

    }

    public void Ping(ScanPattern patterns, Color color)
    {
        for (int i = 0; i < patterns.pingSettings.Count; i++)
        {
            var pattern = patterns.pingSettings[i];

            var clone = Instantiate(sonarping_prefab, sonar_group);

            var sonarping = clone.GetComponent<SonarPing>();

            //sonarping.SetUp(new Vector3(0, 0, -1), min_radius, max_radius, lineWidth, circleDivision, pingDuration, pingInterval);

            var newPos = new Vector3(pattern.pos.x, pattern.pos.y, -0.5f);

            sonarping.SetUp(
                //pattern.pos,
                newPos,
                pattern.min_radius,
                pattern.max_radius,
                pattern.lineWidth,
                pattern.circleDivision,
                pattern.duration,
                pattern.interval,
                color
                );
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // ping spawn control
        if (isPinging)
        {
            currentTime += Time.deltaTime;

            if (currentTime > (pingDuration + pingDelay))
            {
                if (stopPingFlag)
                {
                    isPinging = false;
                }
                else
                {
                    switch (currentPattern)
                    {
                        case 0:
                            currentPattern = 1;
                            break;
                        case 1:
                            currentPattern = 0;
                            break;
                    }
                    SpawnNextPing();
                }
                currentTime -= (pingDuration + pingDelay);
            }

        }


        if (hideFlag)
        {

            hideFrameCount++;

            if (hideFrameCount > hideFrame)
            {

                ShowUI();
                hideFlag = false;

            }
        }
    }
    public void TogglePingStatus()
    {
        if (!isPinging)
        {
            //ping_pattern = scanPatterns[0];
            //distract_pattern = scanPatterns[1];

            StartPing();
        }
        else
        {
            StopPing();
        }
    }

    public void StartPing()
    {
        isPinging = true;
        SpawnNextPing();
        currentTime = 0;
        stopPingFlag = false;
    }
    public void StopPing()
    {
        stopPingFlag = true;
    }

    void SpawnNextPing()
    {
        switch (currentPattern)
        {
            case 0:
                Ping(ping_pattern, Color.green);
                break;
            case 1:
                Ping(distract_pattern, Color.red);
                break;
        }
    }

    public void SetScanPattern(int diceFace, bool isRotate)
    {
        if (!isRotate)
        {
            ping_pattern = scanPatterns[diceFace - 1];
        }
        else
        {
            switch (diceFace)
            {
                case 2:
                    ping_pattern = scanPatterns[(int)PATTERN.D2_R];
                    break;
                case 3:
                    ping_pattern = scanPatterns[(int)PATTERN.D3_R];
                    break;
                case 6:
                    ping_pattern = scanPatterns[(int)PATTERN.D6_R];
                    break;
                default:
                    ping_pattern = scanPatterns[diceFace - 1];
                    break;
            }
        }
    }

    public void SetDistractPattern(int diceFace, bool isRotate)
    {
        if (!isRotate)
        {
            distract_pattern = scanPatterns[diceFace - 1];
        }
        else
        {
            switch (diceFace)
            {
                case 2:
                    distract_pattern = scanPatterns[(int)PATTERN.D2_R];
                    break;
                case 3:
                    distract_pattern = scanPatterns[(int)PATTERN.D3_R];
                    break;
                case 6:
                    distract_pattern = scanPatterns[(int)PATTERN.D6_R];
                    break;
                default:
                    distract_pattern = scanPatterns[diceFace - 1];
                    break;
            }
        }
    }

    public enum PATTERN
    {
        D1 = 0, D2, D3, D4, D5, D6, D2_R, D3_R, D6_R
    }

}
