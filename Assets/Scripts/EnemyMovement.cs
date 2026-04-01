using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    [Header("References")]
    public Transform player;

    [Header("Spawning")]
    public float extraEnemySpawnDelay = 5f;
    public float extraEnemyHeightOffset = 2f;
    public float despawnPlayerYThreshold = 1f;

    [Header("Flying Mode")]
    public bool isFlyingEnemyInstance = false;
    public float flyingEnemySpeed = 6f;
    public float playerMoveThreshold = 0.05f;

    private NavMeshAgent navMeshAgent;
    private Rigidbody rb;
    private Rigidbody playerRb;

    private float normalSpeed;
    private float normalFlyingSpeed;
    private float speedUpUntil;

    private bool hasStartedChasing = false;
    private GameObject spawnedExtraEnemy;
    private bool spawnPending;
    private bool isSecondaryEnemyInstance = false;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();

        if (navMeshAgent != null)
            normalSpeed = navMeshAgent.speed;

        normalFlyingSpeed = flyingEnemySpeed;
        playerRb = player != null ? player.GetComponent<Rigidbody>() : null;

        if (isFlyingEnemyInstance)
        {
            if (navMeshAgent != null)
                navMeshAgent.enabled = false;

            if (rb != null)
            {
                rb.useGravity = false;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
        else if (navMeshAgent != null)
            navMeshAgent.isStopped = true;
    }

    private void Update()
    {
        if (player == null)
            return;

        if (Time.time >= speedUpUntil)
        {
            if (isFlyingEnemyInstance)
                flyingEnemySpeed = normalFlyingSpeed;
            else if (navMeshAgent != null && navMeshAgent.speed != normalSpeed)
                navMeshAgent.speed = normalSpeed;
        }

        if (isFlyingEnemyInstance)
        {
            FlyTowardsPlayer();
            return;
        }

        if (playerRb == null || navMeshAgent == null)
            return;

        if (spawnedExtraEnemy != null && player.position.y < despawnPlayerYThreshold)
        {
            Destroy(spawnedExtraEnemy);
            spawnedExtraEnemy = null;
        }

        if (spawnedExtraEnemy == null && !spawnPending && player.position.y >= despawnPlayerYThreshold)
            StartCoroutine(SpawnExtraEnemyAfterDelay());

        if (!hasStartedChasing)
        {
            float playerMoveSqr = playerMoveThreshold * playerMoveThreshold;
            bool playerMoved = playerRb.linearVelocity.sqrMagnitude > playerMoveSqr;

            if (!playerMoved)
            {
                navMeshAgent.isStopped = true;
                return;
            }

            hasStartedChasing = true;
            navMeshAgent.isStopped = false;
        }

        navMeshAgent.SetDestination(player.position);
    }

    private void FlyTowardsPlayer()
    {
        Vector3 toPlayer = player.position - transform.position;
        if (toPlayer.sqrMagnitude < 0.0001f)
            return;

        Vector3 step = toPlayer.normalized * flyingEnemySpeed * Time.deltaTime;
        if (step.sqrMagnitude > toPlayer.sqrMagnitude)
            step = toPlayer;

        if (rb != null)
            rb.MovePosition(rb.position + step);
        else
            transform.position += step;
    }

    public void SpeedUp(float duration)
    {
        if (isFlyingEnemyInstance)
        {
            flyingEnemySpeed = normalFlyingSpeed * 1.5f;
        }
        else if (navMeshAgent != null)
        {
            navMeshAgent.speed = normalSpeed * 1.5f;
        }

        speedUpUntil = Time.time + duration;
    }

    private IEnumerator SpawnExtraEnemyAfterDelay()
    {
        spawnPending = true;
        yield return new WaitForSeconds(extraEnemySpawnDelay);

        if (this == null || spawnedExtraEnemy != null || player == null || player.position.y < despawnPlayerYThreshold)
        {
            spawnPending = false;
            yield break;
        }

        Vector3 spawnPos = transform.position + Vector3.up * extraEnemyHeightOffset;
        spawnedExtraEnemy = Instantiate(gameObject, spawnPos, transform.rotation);

        EnemyMovement extraEnemy = spawnedExtraEnemy.GetComponent<EnemyMovement>();
        if (extraEnemy != null)
        {
            extraEnemy.player = player;
            extraEnemy.isSecondaryEnemyInstance = true;
            extraEnemy.isFlyingEnemyInstance = true;
        }

        spawnPending = false;
    }
}