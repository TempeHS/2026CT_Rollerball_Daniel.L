using UnityEngine;

public enum PowerUpType { PlayerSpeedBuff, SlowEnemy }

public class PowerUp : MonoBehaviour
{
    [Header("Settings")]
    public float duration = 5f;
    public bool randomizeType = true;
    public PowerUpType fixedType = PowerUpType.PlayerSpeedBuff;
    [Range(0.1f, 1f)] public float slowMultiplier = 0.5f;

    private PowerUpType resolvedType;

    private void Start()
    {
        if (randomizeType)
        {
            resolvedType = Random.value < 0.75f ? PowerUpType.PlayerSpeedBuff : PowerUpType.SlowEnemy;
        }
        else
        {
            resolvedType = fixedType;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null)
            return;

        switch (resolvedType)
        {
            case PowerUpType.PlayerSpeedBuff:
                player.ApplySpeedBoost(duration);
                break;
            case PowerUpType.SlowEnemy:
                player.ApplySlowEnemyBuff(duration, slowMultiplier);
                break;
        }

        Destroy(gameObject);
    }
}