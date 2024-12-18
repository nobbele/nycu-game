using UnityEngine;

public class EnemyHealthDisplay : MonoBehaviour
{
    [SerializeField] private BarScaler healthBarScaler;
    private float currentHealth;

    public float HealthPercent {
        get => currentHealth;
        set {
            currentHealth = value;
            if (healthBarScaler != null)
            {
                healthBarScaler.SetValue(value * 100f);
            }
        }
    }

    private void Start()
    {
        if (healthBarScaler == null)
        {
            Debug.LogWarning("Health Bar Scaler not assigned in Enemy Health Display!");
        }
    }
}
