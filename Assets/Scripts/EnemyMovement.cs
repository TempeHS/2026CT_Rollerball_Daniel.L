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

    private bool hasStartedChasing = false;
    private GameObject spawnedExtraEnemy;
    private bool spawnPending;

    private float baseNavSpeed;
    private float baseFlyingSpeed;
    private float slowUntilTime;
    private float currentSlowMultiplier = 1f;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        playerRb = player != null ? player.GetComponent<Rigidbody>() : null;

        // Store baseline speeds
        if (navMeshAgent != null)
            baseNavSpeed = navMeshAgent.speed;
        baseFlyingSpeed = flyingEnemySpeed;

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

        UpdateSlowState();

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

    public void ApplySlow(float multiplier, float duration)
    {
        currentSlowMultiplier = Mathf.Clamp(multiplier, 0.1f, 1f);
        slowUntilTime = Time.time + duration;
    }

    private void UpdateSlowState()
    {
        float activeMultiplier = Time.time < slowUntilTime ? currentSlowMultiplier : 1f;

        if (navMeshAgent != null)
            navMeshAgent.speed = baseNavSpeed * activeMultiplier;

        flyingEnemySpeed = baseFlyingSpeed * activeMultiplier;
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
            extraEnemy.isFlyingEnemyInstance = true;
        }

        spawnPending = false;
    }
}