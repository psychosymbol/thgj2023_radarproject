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

    public BehaviorState currentState;

    public enum BehaviorState
    {
        Wonder,
        Flee,
        Attack
    }

    public float wonderRange = 1;
    private Vector3 originalSpawnPosition;

    public bool markForDestroy = false;

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
        ChangeState(BehaviorState.Wonder);
        //MonsterManager.instance.creatures.Add(this);
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
        UpdateBehavior();
    }

    public void Reveal()
    {
        alpha = 1;
    }

    void UpdateAlpha()
    {
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alpha);
    }

    public float currentDistance;
    private Vector3 targetPosition;
    public float preferWonderDistance = .1f;

    void ChangeState(BehaviorState newState)
    {
        switch (newState)
        {
            default:
            case BehaviorState.Wonder:
                break;
            case BehaviorState.Flee:
                break;
            case BehaviorState.Attack:
                break;
        }
        currentState = newState;

        GetTargetPosition();
    }

    void GetTargetPosition()
    {
        switch (currentState)
        {
            default:
            case BehaviorState.Wonder:
                Vector3 randomPosition = Random.insideUnitCircle * wonderRange;
                targetPosition = originalSpawnPosition + randomPosition;
                break;
            case BehaviorState.Flee:

                break;
            case BehaviorState.Attack:
                targetPosition = new Vector3(0, 0, originalSpawnPosition.z);
                break;
        }
    }

    void UpdateBehavior()
    {
        switch (currentState)
        {
            default:
            case BehaviorState.Wonder:
                currentDistance = Vector2.Distance(targetPosition, transform.position);
                if (currentDistance > preferWonderDistance)
                {
                    transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * .5f);
                }
                else
                {
                    ChangeState(BehaviorState.Wonder);
                }
                transform.position += new Vector3(0, GameManager.instance.currentSpeed * Time.deltaTime * GameManager.instance.depthToUnit, 0);
                targetPosition += new Vector3(0, GameManager.instance.currentSpeed * Time.deltaTime * GameManager.instance.depthToUnit, 0);
                break;
            case BehaviorState.Flee:
                break;
            case BehaviorState.Attack:
                break;
        }
    }

    public void MarkForDestroy()
    {
        if (markForDestroy) return;
        Destroy(gameObject, .1f);
        GameManager.instance.Damaged();
        markForDestroy = true;
    }

    private void OnDestroy()
    {
        MonsterManager.instance.creatures.Remove(this);
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
        originalSpawnPosition = transform.position;
        spawnFinish = true;
    }

    public virtual void OnPing()
    {
        Reveal();
    }

    public virtual void OnDistract()
    {
        Reveal();
    }
}
