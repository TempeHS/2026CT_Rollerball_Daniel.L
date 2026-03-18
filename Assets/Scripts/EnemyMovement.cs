using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
public Transform player;
private NavMeshAgent navMeshAgent;
private float normalSpeed;
private float speedUpUntil;

void Start()
{
navMeshAgent = GetComponent<NavMeshAgent>();
normalSpeed = navMeshAgent.speed;
}

void Update()
{
if (Time.time >= speedUpUntil && navMeshAgent.speed != normalSpeed)
{
navMeshAgent.speed = normalSpeed;
}

if (player != null)
{
navMeshAgent.SetDestination(player.position);
}
}

public void SpeedUp(float duration)
{
navMeshAgent.speed = normalSpeed * 1.5f;
speedUpUntil = Time.time + duration;
}
}