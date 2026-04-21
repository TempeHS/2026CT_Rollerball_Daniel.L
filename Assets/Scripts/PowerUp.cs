using UnityEngine;

public enum PowerUpType { PlayerSpeedBuff }

public class PowerUp : MonoBehaviour
{
    [Header("Settings")]
    public float duration = 6f;
    public bool randomizeType = true;
    public PowerUpType fixedType = PowerUpType.PlayerSpeedBuff;

    [Header("Visuals")]
    public Renderer meshRenderer;
    public Color speedColor = Color.cyan;

    private PowerUpType resolvedType;

    private void Start()
    {
        resolvedType = PowerUpType.PlayerSpeedBuff;
        ApplyVisual();
    }

    private void ApplyVisual()
    {
        if (meshRenderer == null)
            return;

        meshRenderer.material.color = speedColor;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null)
            return;

        player.ApplySpeedBoost(duration);

        Destroy(gameObject);
    }
}