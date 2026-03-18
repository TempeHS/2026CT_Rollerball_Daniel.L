using UnityEngine;

public enum PowerUpType
{
PlayerSpeedBuff,
PlayerInvincibility,
EnemySpeedup
}

public class PowerUp : MonoBehaviour
{
public float duration = 5f;

private void OnTriggerEnter(Collider other)
{
PlayerController player = other.GetComponent<PlayerController>();
if (player == null)
{
return;
}
ApplyRandomEffect(player);
gameObject.SetActive(false);
}

PlayerController player = other.GetComponent<PlayerController>();
if (player == null)
{
return;
}

ApplyRandomEffect(player);
gameObject.SetActive(false);
}

private void ApplyRandomEffect(PlayerController player)
{
PowerUpType randomType = (PowerUpType)Random.Range(0, 3);

switch (randomType)
{
case PowerUpType.PlayerSpeedBuff:
player.ApplySpeedBoost(duration);
break;

case PowerUpType.PlayerInvincibility:
player.ApplyInvincibility(duration);
break;

case PowerUpType.EnemySpeedup:
player.ApplyEnemySpeedup(duration);
break;
}
}
}