using UnityEngine;

public class IceFireZone : MonoBehaviour
{
    [Header("Status Effects")]
    public float slowAmount = 0.8f;
    public float effectDuration = 5f;
    public int damagePerSecond = 10;

    [Header("Visual & Audio Effects")]
    public ParticleSystem iceFireEffect;
    public AudioSource dragonBreathSound;

    private void Start()
    {
        if (iceFireEffect != null)
        {
            iceFireEffect.Play();
        }

        if (dragonBreathSound != null)
        {
            dragonBreathSound.loop = true;
            dragonBreathSound.Play();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ApplyStatusEffectToPlayer(other);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ApplyStatusEffectToPlayer(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ApplyStatusEffectToPlayer(other);
        }
    }

    private void ApplyStatusEffectToPlayer(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            PlayerStatusManager statusManager = player.GetComponent<PlayerStatusManager>();
            if (statusManager != null)
            {
                statusManager.AddOrUpdateStatus(StatusType.LingeringDamage, damagePerSecond, effectDuration);
            }
        }
    }

    private void OnDestroy()
    {
        if (dragonBreathSound != null)
        {
            dragonBreathSound.Stop();
        }
    }
}
