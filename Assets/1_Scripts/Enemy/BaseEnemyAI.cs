using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public interface IEnemyAI 
{
    void Initialize(Transform player, Transform spawnPoint, BaseEnemyData data);
}

public abstract class BaseEnemyAI<T> : MonoBehaviour, IEnemyAI where T : BaseEnemyData
{
    protected Transform player;
    protected Transform spawnPoint;
    protected T enemyData;
    
    protected NavMeshAgent agent;
    protected Animator animator;
    
    protected Dictionary<string, Timer> timers = new();
    protected bool isInitialized;
    
    public virtual void Initialize(Transform player, Transform spawnPoint, BaseEnemyData data)
    {
        if (!ValidateInitializationParameters(player, spawnPoint, data)) return;

        this.player = player;
        this.spawnPoint = spawnPoint;
        this.enemyData = data as T;

        if (TryGetComponent<NavMeshAgent>(out agent))
        {
            // Try to get Enemy component and animator
            var baseEnemy = GetComponent<Enemy>();
            if (baseEnemy != null)
            {
                animator = baseEnemy.GetAnimator();
                if (animator == null)
                {
                    Debug.LogError($"No animator found on {gameObject.name}");
                    enabled = false;
                    return;
                }
                
                InitializeTimers();
                isInitialized = true;
                return;
            }

            // If not Enemy, try Boss
            var bossEnemy = GetComponent<Boss>();
            if (bossEnemy != null)
            {
                animator = bossEnemy.GetAnimator();
                if (animator == null)
                {
                    Debug.LogError($"No animator found on {gameObject.name}");
                    enabled = false;
                    return;
                }
                
                InitializeTimers();
                isInitialized = true;
                return;
            }
        }

        Debug.LogError($"Missing required components on {gameObject.name}");
        enabled = false;

        InitializeTimers();
        isInitialized = true;
    }

    private bool ValidateInitializationParameters(Transform player, Transform spawnPoint, BaseEnemyData data)
    {
        if (player == null || spawnPoint == null || data == null || data is not T)
        {
            Debug.LogError($"Failed to initialize {GetType().Name}: Invalid parameters");
            enabled = false;
            return false;
        }
        return true;
    }
    
    protected virtual void InitializeTimers()
    {
        AddTimer("move", enemyData.moveInterval);
        AddTimer("attack", enemyData.attackInterval);
    }
    
    protected void AddTimer(string name, float interval)
    {
        timers[name] = new Timer(interval);
    }
    
    protected bool CheckTimer(string name)
    {
        if (!isInitialized) return false;
        return timers.TryGetValue(name, out Timer timer) && timer.Update(Time.deltaTime);
    }
    
    protected virtual void Update()
    {
        if (!isInitialized || animator == null)
        {
            enabled = false;
            return;
        }
        UpdateBehavior();
    }
    
    protected void ValidateComponents()
    {
        if (animator == null || !animator)
        {
            Debug.LogError($"Lost animator reference on {gameObject.name}");
            enabled = false;
        }
    }
    
    protected abstract void UpdateBehavior();
}
