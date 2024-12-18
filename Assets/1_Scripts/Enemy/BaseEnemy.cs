using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public abstract class BaseEnemy<T> : MonoBehaviour, IDamageHandler where T : BaseEnemyData
{
    public T enemyData;
    public int Health { get; protected set; }
    public bool IsDead => Health <= 0;
    public event Action onDeath;
    
    public GameObject meshInstance;
    protected Animator animator;
    protected bool isInitialized;
    
    public EnemyHealthDisplay healthDisplay;

    protected virtual void Start()
    {
        if (enemyData == null)
        {
            Debug.LogError($"Enemy Data is missing on {gameObject.name}!");
            enabled = false;
            return;
        }

        InitializeComponents();
    }

    protected virtual void InitializeComponents()
    {
        InitializeMeshAndAnimator();
        InitializeHealthSystem();
        isInitialized = true;
    }

    protected virtual void InitializeMeshAndAnimator()
    {
        if (meshInstance == null)
        {
            Debug.LogError($"Missing mesh instance on {gameObject.name}");
            return;
        }

        // First try to get animator from the mesh
        animator = meshInstance.GetComponent<Animator>();
        
        // If not found, try children
        if (animator == null)
        {
            animator = meshInstance.GetComponentInChildren<Animator>();
        }
        
        // If still not found, add one
        if (animator == null)
        {
            animator = meshInstance.AddComponent<Animator>();
        }

        // Set controller if available
        if (enemyData?.animatorController != null)
        {
            animator.runtimeAnimatorController = enemyData.animatorController;
        }
        else
        {
            Debug.LogWarning($"No animator controller assigned in enemy data for {gameObject.name}");
        }
    }

    private void InitializeHealthSystem()
    {
        Health = enemyData.health;
        
        if (healthDisplay == null)
        {
            var hdPrefab = AssetDatabase.LoadAssetAtPath<EnemyHealthDisplay>("Assets/2_Prefab/Enemy UI.prefab");
            if (hdPrefab != null)
            {
                healthDisplay = Instantiate(hdPrefab, transform);
                healthDisplay.transform.Translate(Vector3.up);
                healthDisplay.gameObject.SetActive(false);
            }
        }
    }

    protected virtual void Update()
    {
        if (!isInitialized) return;
        UpdateHealthDisplay();
        if (IsDead) OnDead();
    }

    private void UpdateHealthDisplay()
    {
        if (healthDisplay == null) return;

        if (Health < enemyData.health)
        {
            healthDisplay.gameObject.SetActive(true);
            healthDisplay.HealthPercent = Health / (float)enemyData.health;
        }
        else
        {
            healthDisplay.gameObject.SetActive(false);
        }
    }

    protected virtual void OnDead()
    {
        onDeath?.Invoke();
        gameObject.SetActive(false);
    }

    public virtual void OnDamage(GameObject source, int damage)
    {
        if (source == this) return;
        Health -= damage;
    }

    public Animator GetAnimator() => animator;
}
