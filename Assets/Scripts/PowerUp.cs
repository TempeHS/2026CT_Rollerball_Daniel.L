using UnityEngine;

public enum PowerUpType
{
    PlayerSpeedBuff,
    PlayerInvincibility,
    EnemySpeedup
}

public class PowerUp : MonoBehaviour
{
    [Header("Settings")]
    public float duration = 6f;
    public bool randomizeType = true;
    public PowerUpType fixedType = PowerUpType.PlayerSpeedBuff;

    [Header("Visuals")]
    public Renderer meshRenderer;
    public Color speedColor = Color.cyan;
    public Color invincibleColor = Color.yellow;
    public Color enemySpeedupColor = Color.red;

    private PowerUpType resolvedType;

    private void Start()
    {
        resolvedType = randomizeType
            ? (PowerUpType)Random.Range(0, 3)
            : fixedType;

        ApplyVisual();
    }

    private void ApplyVisual()
    {
        if (meshRenderer == null) return;

        Color c = speedColor;
        switch (resolvedType)
        {
            case PowerUpType.PlayerInvincibility:
                c = invincibleColor;
                break;
            case PowerUpType.EnemySpeedup:
                c = enemySpeedupColor;
                break;
        }

        meshRenderer.material.color = c;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;

        switch (resolvedType)
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

        Destroy(gameObject);
    }
}