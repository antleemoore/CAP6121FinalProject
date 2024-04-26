using System.Collections.Generic;
using System.Linq;
using Interfaces;
using Unity.VisualScripting;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour, IEnemy
{
    // Flag to check against during updates to adjust behavior i.e. do not attack or move while held
    protected bool Selected = false;

    private IGrabber _grabbingObject;

    [field:SerializeField]
    public int Health { get; set; }
    
    private GameObject player; // Reference to the player object
    private Transform parentSpawnLocationTransform; // Point where the enemy will spawn
    private Transform parentTargetLocationTransform; // Point where the enemy will move
    public Transform fruitTransform;
    public float changeDirectionInterval = 2f; // Interval to change direction
    private bool hasReachedTarget = false; // Flag to track if the enemy has reached the target point
    private float timer;
    public Transform targetPoint;
    private Rigidbody rb;
    protected float MoveSpeed = 0.5f; // Speed at which the enemy moves
    protected float KnockbackDistance = 2f;
    protected float AttackInterval = 5f;
    private float attackTimer = 0f;
    protected float FruitDamage = 0.05f;

    public List<Transform> waypoints = new List<Transform>();
    
    public float _knockoutTimer = 0;

    public bool gameOver;
    public bool moldy;
    public bool parried;
    public bool hasAttacked;
    public Vector3 directionToTarget;
    private bool _eventAdded;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        IPlayer p = player.GetComponent<IPlayer>();
        if (p is not null && p.GameOverEvent is not null)
        {
            p.GameOverEvent.AddListener(GameOver);
            _eventAdded = true;
        }
        else _eventAdded = false;
        
        rb = GetComponent<Rigidbody>();
        //SpawnEnemy();
        // Initialize the timer
        timer = changeDirectionInterval;
        
        //set initial target
        SetTargetPoint();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (EnemyManager.GameActive != true) return;
        
        if (!_eventAdded && player is not null && !player.IsDestroyed())
        {
            player.GetComponent<IPlayer>().GameOverEvent.AddListener(GameOver);
            _eventAdded = true;
        }
        
        if (gameOver) return;
        
        if (Health <= 0)
        {
            Destroy(gameObject);
            return;
        }

        // Do nothing if knocked out/moldy
        if (_knockoutTimer > 0)
        {
            _knockoutTimer -= Time.deltaTime;
            if (_knockoutTimer <= 0) moldy = false;
            return;
        }

        if (hasAttacked)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= AttackInterval)
            {
                attackTimer = 0f;
                hasAttacked = false;
                if (parried) parried = false;
            }
        }
        
        if (Selected) return;

        if (hasReachedTarget)
        {
            // Update the timer
            timer -= Time.deltaTime;

            if (PlayerDetected) //likely want to move this logic to the collision w player
            {
                //knock back from player and continue moving towards the player to attack again
                hasAttacked = true;

                rb.AddForce(-(directionToTarget * KnockbackDistance));
                hasReachedTarget = false;
            }
            // If it's time to change direction
            else if (timer <= 0f)
            {
                SetTargetPoint();
                // Reset the timer
                timer = changeDirectionInterval;
                
                MoveEnemy();
            }

        }
        else
        {
            MoveEnemy();
        }
    }

    protected void OnDestroy()
    {
        _grabbingObject?.Clear();
        if (!player.IsDestroyed()) player.GetComponent<IPlayer>().GameOverEvent.RemoveListener(GameOver);
    }

    public virtual bool Select(IGrabber parent)
    {
        if (Selected) return false;

        transform.SetParent(parent.GetTransform(), true);

        _grabbingObject = parent;
        
        return Selected = true;
    }

    private void GameOver()
    {
        gameOver = true;
    }

    private void MoveEnemy()
    {
        if (hasAttacked) return; //wait for attackDelay before approaching to attack again
        
        if (PlayerDetected) SetTargetPoint();
      
        // Move towards the target point - not sure how to use physics and if that solves collisions issues
        transform.position = Vector3.MoveTowards(transform.position, targetPoint.position,
            MoveSpeed * Time.deltaTime);
        fruitTransform.LookAt(targetPoint.transform.position);
        
        // Check if the enemy has reached the target point
        if (!hasReachedTarget && transform.position == targetPoint.position)
        {
            hasReachedTarget = true;
        }
    }

    private void SetTargetPoint()
    {
        if (PlayerDetected) targetPoint = player.transform;
        else
        {
            int newWaypointIdx = 0;
            if (waypoints.Contains(targetPoint))
            {
                newWaypointIdx = Random.Range(0, waypoints.Count - 2);
                if (newWaypointIdx >= waypoints.IndexOf(targetPoint)) newWaypointIdx++;
            }
            else newWaypointIdx = Random.Range(0, waypoints.Count - 1);

            targetPoint = waypoints[newWaypointIdx];
        }

        directionToTarget = targetPoint.position - transform.position;
        directionToTarget.Normalize();

        hasReachedTarget = false;
    }
    
    public virtual bool Throw()
    {
        if (!Selected) return false;

        Selected = false;
        transform.SetParent(null);
        _grabbingObject = null;
        hasReachedTarget = true;
        
        return !Selected;
    }

    protected virtual int TakeDamage(int dmg)
    {
        Health -= dmg;
        return Health;
    }

    public virtual int Slash(int dmg)
    {
        return TakeDamage(dmg);
    }

    public virtual int Stab(int dmg)
    {
        return TakeDamage(dmg);
    }
    
    public virtual int ShurikenHit(int dmg)
    {
        return TakeDamage(dmg);
    }

    public virtual void Parry()
    {
        rb.AddForce(-(directionToTarget * KnockbackDistance));
        hasReachedTarget = false;

        parried = true;
        hasAttacked = true;
    }

    public virtual void Mold()
    {
        if (moldy) return;
        _knockoutTimer = 5.0f;
        moldy = true;
    }

    public bool PlayerDetected { get; set; }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (Selected) return;
        
        if (other.GetComponent<IPlayer>() is { } p)
        {
            if (parried)
            {
                parried = false;
                return;
            }
            
            if (hasAttacked) return;
            
            p.TakeDamage(FruitDamage);
            hasAttacked = true;
        }
    }
}
