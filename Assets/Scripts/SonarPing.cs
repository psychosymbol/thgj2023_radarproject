using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class SonarPing : MonoBehaviour
{
    public float min_radius;
    public float max_radius;

    [SerializeField]
    float current_radius;

    public float lineWidth;

    public int division;

    public bool isPinging = false;

    public float ping_duration = 0;
    public float current_duration = 0;


    public float interval = 0.1f;
    public float interval_time = 0;

    LineRenderer lr;

    public int ping_count = 1;
    public int current_ping_count = 0;

    public Color color = Color.white;

    public void SetUp(Vector3 pos, float min_radius, float max_radius, float lineWidth, int division, float pingDuration, float ping_interval, Color color ,int loop = 1)
    {
        this.color = color;

        this.ping_count = loop;
        current_ping_count = 0;

        transform.position = pos;

        interval = ping_interval;

        this.min_radius = min_radius;
        this.max_radius = max_radius;
        current_radius = min_radius;

        this.lineWidth = lineWidth;
        this.division = division;

        this.ping_duration = pingDuration;


        CreateCircle(pos, min_radius, division);
        StartPing();
    }

    public void CreateCircle(Vector3 pos, float radius, int division)
    {
        var lr = GetComponent<LineRenderer>();

        lr.positionCount = division + 1;

        var angleDelta = Mathf.PI * 2 / division;

        for (int i = 0; i < division + 1; i++)
        {
            var x = radius * Mathf.Cos(angleDelta * i);
            var y = radius * Mathf.Sin(angleDelta * i);
            lr.SetPosition(i, new Vector3(x + pos.x, y + pos.y, pos.z));
        }
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;

        lr.startColor = color;
        lr.endColor = color;

    }

    [ContextMenu("StartPing")]
    public void StartPing()
    {
        isPinging = true;
        current_radius = min_radius;
        current_duration = 0;
        interval_time = 0;

        UpdateCircle();
    }

    public void FinishPing()
    {

        Debug.Log("finish ping");

        isPinging = false;
        current_radius = max_radius;
        current_duration = 0;

        UpdateCircle();

        Destroy(this.gameObject, 0.1f);

    }

    void UpdateCircle()
    {
        var pos = transform.position;
        var radius = current_radius;

        var angleDelta = Mathf.PI * 2 / division;

        for (int i = 0; i < division + 1; i++)
        {
            var x = radius * Mathf.Cos(angleDelta * i);
            var y = radius * Mathf.Sin(angleDelta * i);
            lr.SetPosition(i, new Vector3(x + pos.x, y + pos.y, pos.z));
        }
    }

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        var dt = Time.deltaTime;

        if (isPinging)
        {
            current_duration += dt;
            interval_time += dt;

            current_radius = current_duration.Remap(0, ping_duration, min_radius, max_radius);

            if (interval_time >= interval)
            {
                interval_time -= interval;

                RadarController.instance.TestInsideCircle(transform.position, current_radius);

                RadarController.instance.flashHide();

                UpdateCircle();


                if (current_duration >= ping_duration)
                {
                    current_ping_count++;
                    if (current_ping_count < ping_count)
                    {
                        // re loop
                        StartPing();
                    }
                    else
                    {
                        FinishPing();
                    }
                }
            }

        }
    }
}
