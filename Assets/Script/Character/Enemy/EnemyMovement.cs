using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float moveSpeed = 3f; 
    public float stopDistance = 1.5f; 
    public float avoidanceRadius = 2f; 
    public float detectionRadius = 5f; 
    public float randomMoveRadius = 3f;
    public float randomMoveInterval = 2f; 

    private Transform playerTransform;
    private Vector3 moveDirection;
    private List<EnemyMovement> allEnemies;
    private bool isFlipped = false;
    private float randomMoveTimer;

    [SerializeField] private Player player;

    void Start()
    {
        player = FindObjectOfType<Player>();
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("Player script not found in the scene.");
        }

        allEnemies = new List<EnemyMovement>(FindObjectsOfType<EnemyMovement>());

        randomMoveTimer = randomMoveInterval;
    }

    void Update()
    {
        if (BuffInventory.instance.isGamePause) return;
        
        if (playerTransform != null && IsPlayerInDetectionRadius())
        {
            ChasePlayer();
            AvoidOtherEnemies();
        }
        else
        {
            RandomMove();
        }
        MoveEnemy();
        FlipEnemy();
    }

    bool IsPlayerInDetectionRadius()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        return distanceToPlayer <= detectionRadius;
    }

    void ChasePlayer()
    {
        moveDirection = (playerTransform.position - transform.position).normalized;
    }

    void AvoidOtherEnemies()
    {
        Vector3 avoidanceDirection = Vector3.zero;
        int nearbyEnemiesCount = 0;

        foreach (EnemyMovement enemy in allEnemies)
        {
            if (enemy != this)
            {
                if (enemy == null) return;
                float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
                if (distanceToEnemy < avoidanceRadius)
                {
                    avoidanceDirection += (transform.position - enemy.transform.position).normalized;
                    nearbyEnemiesCount++;
                }
            }
        }

        if (nearbyEnemiesCount > 0)
        {
            avoidanceDirection /= nearbyEnemiesCount;
            moveDirection += avoidanceDirection;
        }
    }

    void RandomMove()
    {
        randomMoveTimer -= Time.deltaTime;
        if (randomMoveTimer <= 0f)
        {
            Vector2 randomDirection = Random.insideUnitCircle * randomMoveRadius;
            moveDirection = new Vector3(randomDirection.x, randomDirection.y, 0).normalized;
            randomMoveTimer = randomMoveInterval;
        }
    }

    void MoveEnemy()
    {
        moveDirection = moveDirection.normalized;

        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    void FlipEnemy()
    {
        Vector3 localScale = transform.localScale;
        if (moveDirection.x < 0 && !isFlipped)
        {
            localScale.x = Mathf.Abs(localScale.x) * -1;
            isFlipped = true;
        }
        else if (moveDirection.x > 0 && isFlipped)
        {
            localScale.x = Mathf.Abs(localScale.x);
            isFlipped = false;
        }
        transform.localScale = localScale;
    }
}
