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
    public SpriteRenderer sr;

    public Color color;

    public bool spawnFinish = false;

    float alpha = 1;
    public float fadingRate = 1;

    public bool pingable = true;
    public bool distractable = true;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.color = color;
    }

    public virtual void Start()
    {
        player = GameManager.instance;
        sizeInit();
        onSpawn();
    }

    public virtual void Update()
    {

        var dt = Time.deltaTime;

        if (!spawnFinish)
        {

        }
        else
        {
            behavior();
        }

        //

        alpha -= fadingRate * dt;
        alpha = Mathf.Clamp01(alpha);

        UpdateAlpha();

    }

    public void Reveal()
    {
        alpha = 1;
    }

    void UpdateAlpha()
    {
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alpha);
    }

    public virtual void sizeInit()
    {

        var size = 1f;

        switch (fishSize)
        {
            case FishSize.XS:
                size = .1f;
                break;
            case FishSize.S:
                size = .15f;
                break;
            case FishSize.M:
                size = .25f;
                break;
            case FishSize.L:
                size = .4f;
                break;
            case FishSize.XL:
                size = .5f;
                break;
            case FishSize.XXL:
                size = .75f;
                break;
            default:
                break;
        }
        transform.localScale = Vector3.one * size;
    }

    public virtual void behavior()
    {

    }

    public virtual void onSpawn()
    {
        Debug.Log("testSpawning from mother class");
        spawnFinish = true;
    }

    public virtual void OnPing()
    {
        Reveal();
    }

    public virtual void OnDistract()
    {

    }
}
