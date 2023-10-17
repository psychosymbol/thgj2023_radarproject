using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarGrid : MonoBehaviour
{
    public GameObject line_prefab;

    public Transform lineGroup;

    public float width;
    public float height;

    public float gridSpacing;

    public float z = -0.1f;


    public float lineWidth = 0.1f;
    public Color lineColor;

    List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();

    [ContextMenu("CreateGrid")]
    public void CreateGrid()
    {

        // clear
        foreach (Transform child in lineGroup)
        {
            DestroyImmediate(child.gameObject);
        }

        spriteRenderers.Clear();


        var pos = transform.position;
        // vertical
        var v_count = Mathf.CeilToInt(width / gridSpacing);
        for (int i = 0; i < v_count; i++)
        {
            var clone = Instantiate(line_prefab, lineGroup);
            var transform = clone.transform;
            var sr = clone.GetComponent<SpriteRenderer>();

            transform.position = new Vector3(pos.x - width / 2 + (gridSpacing * i), 0, z);
            transform.localScale = new Vector3(lineWidth / 2, height, z);
            sr.color = lineColor;

            spriteRenderers.Add(sr);
        }

        // horizontal
        var h_count = Mathf.CeilToInt(height / gridSpacing);
        for (int i = 0; i < h_count; i++)
        {
            var clone = Instantiate(line_prefab, lineGroup);
            var transform = clone.transform;
            var sr = clone.GetComponent<SpriteRenderer>();

            transform.position = new Vector3(0, pos.y - height / 2 + (gridSpacing * i), z);
            transform.localScale = new Vector3(width, lineWidth / 2, z);
            sr.color = lineColor;

            spriteRenderers.Add(sr);
        }
    }


    [ContextMenu("SetColor")]
    public void SetColor()
    {
        foreach (SpriteRenderer sr in spriteRenderers)
        {
            sr.color = lineColor;
        }
    }

}
