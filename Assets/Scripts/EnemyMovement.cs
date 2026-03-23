using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    public Transform player;

    private NavMeshAgent navMeshAgent;
    private Rigidbody playerRb;
    private float normalSpeed;
    private float speedUpUntil;

    [SerializeField] private float playerMoveThreshold = 0.05f;
    private bool hasStartedChasing = false;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        normalSpeed = navMeshAgent.speed;

        if (player != null)
        {
            playerRb = player.GetComponent<Rigidbody>();
        }

        navMeshAgent.isStopped = true;
    }

    void Update()
    {
        if (player == null || playerRb == null)
        {
            return;
        }

        if (Time.time >= speedUpUntil && navMeshAgent.speed != normalSpeed)
        {
            navMeshAgent.speed = normalSpeed;
        }

        if (!hasStartedChasing)
        {
            bool playerMoved = playerRb.linearVelocity.sqrMagnitude >
                               (playerMoveThreshold * playerMoveThreshold);

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

    public void SpeedUp(float duration)
    {
        navMeshAgent.speed = normalSpeed * 1.5f;
        speedUpUntil = Time.time + duration;
    }
}