using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour
{
    public enum FishSize
    {
        XS=0, //.25f
        S=1, //.5f
        M=2, //1
        L=3, //1.5f
        XL=4, //2
        XXL=5 //2.5f
    }
    public bool noRandom = false;

    public FishSize fishSize;

    public GameManager player;
    public SpriteRenderer sr;

    public Color color;

    public bool spawnFinish = false;

    float alpha = 1;
    public float fadingRate = 1;

    public bool fleeSonar = false;

    public bool pingable = true;
    public bool distractable = true;

    public float distractTimer = 0;
    public float distractedTime = 2f;
    public bool distracted = false;
    public bool pursuitAble = false;
    public bool pursuiting = false;
    public float pursuitStartDistance = 3f;
    public float attackRange = 1f;

    public BehaviorState currentState;

    public float moveSpeed = .5f;
    public float fleeSpeed = 3f;
    public float attackSpeed = 1f;
    public float pursuitSpeed = 3f;
    public enum BehaviorState
    {
        Wonder,
        Flee,
        Attack,
        Pursuit
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
            UpdateBehavior();
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

    public float currentDistance;
    public Vector3 targetPosition;
    public float preferWonderDistance = .1f;

    void ChangeState(BehaviorState toState)
    {
        ChangeState(toState, Vector3.zero);
    }
    void ChangeState(BehaviorState toState, Vector3 targetPos)
    {
        switch (currentState)
        {
            default:
            case BehaviorState.Wonder:
                break;
            case BehaviorState.Flee:
                break;
            case BehaviorState.Attack:
                originalSpawnPosition = targetPosition;
                break;
            case BehaviorState.Pursuit:;
                break;
        }
        switch (toState)
        {
            default:
            case BehaviorState.Wonder:
                Vector3 randomPosition = Random.insideUnitCircle * wonderRange;
                targetPosition = originalSpawnPosition + randomPosition;
                break;
            case BehaviorState.Flee:
                break;
            case BehaviorState.Attack:
                targetPosition = new Vector3(targetPos.x, targetPos.y, originalSpawnPosition.z);
                break;
            case BehaviorState.Pursuit:
                targetPosition = new Vector3(0, 0, originalSpawnPosition.z);
                break;
        }
        currentState = toState;
    }

    void UpdateBehavior()
    {
        switch (currentState)
        {
            default:
            case BehaviorState.Wonder:
                float playerDistance = Vector2.Distance(Vector2.zero, transform.position);
                currentDistance = Vector2.Distance(targetPosition, transform.position);
                if (currentDistance > preferWonderDistance)
                {
                    transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveSpeed);
                }
                else
                {
                    ChangeState(BehaviorState.Wonder);
                }
                transform.position += new Vector3(0, GameManager.instance.currentSpeed * Time.deltaTime * GameManager.instance.depthToUnit, 0);
                targetPosition += new Vector3(0, GameManager.instance.currentSpeed * Time.deltaTime * GameManager.instance.depthToUnit, 0);
                originalSpawnPosition += new Vector3(0, GameManager.instance.currentSpeed * Time.deltaTime * GameManager.instance.depthToUnit, 0);
                if (!pursuiting && pursuitAble)
                {
                    if (playerDistance < pursuitStartDistance)
                        pursuiting = true;
                }

                if(distracted)
                {
                    if(distractTimer < distractedTime)
                    {
                        distractTimer += Time.deltaTime;
                    }
                    distracted = false;
                }
                else
                {
                    if(pursuitAble && pursuiting)
                    {
                        ChangeState(BehaviorState.Pursuit);
                    }

                    if(playerDistance < attackRange && !fleeSonar)
                    {
                        ChangeState(BehaviorState.Attack);
                    }
                }

                break;
            case BehaviorState.Flee:
                Vector3 direction = transform.position.normalized;

                transform.Translate(direction * fleeSpeed * Time.deltaTime);

                MarkForDestroy(3, false);

                transform.position += new Vector3(0, GameManager.instance.currentSpeed * Time.deltaTime * GameManager.instance.depthToUnit, 0);
                targetPosition += new Vector3(0, GameManager.instance.currentSpeed * Time.deltaTime * GameManager.instance.depthToUnit, 0);
                break;
            case BehaviorState.Attack:
                currentDistance = Vector2.Distance(targetPosition, transform.position);
                if (currentDistance > preferWonderDistance)
                {
                    transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * attackSpeed);
                }
                else
                {
                    ChangeState(BehaviorState.Wonder);
                }
                transform.position += new Vector3(0, GameManager.instance.currentSpeed * Time.deltaTime * GameManager.instance.depthToUnit, 0);
                targetPosition += new Vector3(0, GameManager.instance.currentSpeed * Time.deltaTime * GameManager.instance.depthToUnit, 0);
                originalSpawnPosition += new Vector3(0, GameManager.instance.currentSpeed * Time.deltaTime * GameManager.instance.depthToUnit, 0);
                break;
            case BehaviorState.Pursuit:
                float playerDistance2 = Vector2.Distance(Vector2.zero, transform.position);
                currentDistance = Vector2.Distance(targetPosition, transform.position);
                if (currentDistance > preferWonderDistance)
                {
                    transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * pursuitSpeed);
                }
                else
                {
                    ChangeState(BehaviorState.Wonder);
                }
                transform.position += new Vector3(0, GameManager.instance.currentSpeed * Time.deltaTime * GameManager.instance.depthToUnit, 0);
                originalSpawnPosition += new Vector3(0, GameManager.instance.currentSpeed * Time.deltaTime * GameManager.instance.depthToUnit, 0);

                if (playerDistance2 < attackRange)
                {
                    ChangeState(BehaviorState.Attack);
                }

                break;
        }
    }

    public void MarkForDestroy(float duration = .1f, bool dealDamage = true)
    {
        if (markForDestroy) return;
        Destroy(gameObject, duration);
        if (dealDamage)
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

    public virtual void onSpawn()
    {
        originalSpawnPosition = transform.position;
        randomProperty();
        sizeInit();
        spawnFinish = true;
    }

    public monsterType type = monsterType.FISH;

    public enum monsterType
    {
        FISH = 0,
        AGGRESSIVEFISH = 1,
        MONSTER = 2
    }

    public void randomProperty()
    {
        if (noRandom) return;
        type = (monsterType)Random.Range(0, 3);

        switch (type)
        {
            default:
            case monsterType.FISH:
                fleeSonar = true;
                fleeSpeed = Random.Range(1f, 5f);
                wonderRange = Random.Range(.5f, 1.5f);
                pursuitAble = false;
                fadingRate = Random.Range(.1f, .5f);
                fishSize = (FishSize)Random.Range(0, 6);
                break;
            case monsterType.AGGRESSIVEFISH:
                fleeSonar = false;
                attackSpeed = Random.Range(1f, 2.5f);
                distractedTime = Random.Range(3f, 6f);
                wonderRange = Random.Range(3f, 6f);
                pursuitAble = false;
                attackRange = Random.Range(1f, 3f);
                fadingRate = Random.Range(.5f, 1f);
                fishSize = (FishSize)Random.Range(1, 4);
                break;
            case monsterType.MONSTER:
                fleeSonar = false;
                attackSpeed = Random.Range(2f, 5f);
                distractedTime = Random.Range(2f, 4f);
                wonderRange = Random.Range(5f, 8f);
                pursuitAble = true;
                pursuitSpeed = Random.Range(3f, 5f);
                attackRange = Random.Range(1f, 3f);
                pursuitStartDistance = Random.Range(3f, 5f);
                fadingRate = Random.Range(1f, 1.5f);
                fishSize = (FishSize)Random.Range(3, 6);
                break;
        }
    }

    public virtual void OnPing()
    {
        Reveal();
    }

    public virtual void OnDistract(Vector3 sonarPos)
    {
        Reveal();
        if (!fleeSonar)
        {
            distracted = true;
            ChangeState(BehaviorState.Attack, sonarPos);
            distractTimer = 0;
        }
        else
            ChangeState(BehaviorState.Flee);
    }
}
