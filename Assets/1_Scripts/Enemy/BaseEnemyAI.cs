using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Collections;

public abstract class BaseEnemyAI : MonoBehaviour
{
    protected Transform player;
    protected Transform spawnPoint;
    protected BaseEnemyData enemyData;
    
    protected NavMeshAgent agent;
    protected Animator animator;
    protected Enemy enemy;
    
    protected Dictionary<string, Timer> timers = new();
    protected bool isInitialized = false;
    
    public virtual void Initialize(Transform player, Transform spawnPoint, BaseEnemyData data)
    {
        if (player == null || spawnPoint == null || data == null)
        {
            Debug.LogError($"Failed to initialize {GetType().Name}: Missing required parameters");
            enabled = false;
            return;
        }

        this.player = player;
        this.spawnPoint = spawnPoint;
        this.enemyData = data;
        
        StartCoroutine(InitializeComponents());
    }
    
    private IEnumerator InitializeComponents()
    {
        yield return null;
        
        agent = GetComponent<NavMeshAgent>();
        enemy = GetComponent<Enemy>();
        
        if (enemy != null)
        {
            animator = enemy.meshInstance?.GetComponent<Animator>();
        }
        
        if (agent == null || enemy == null)
        {
            Debug.LogError($"Missing required components on {gameObject.name}");
            enabled = false;
            yield break;
        }

        InitializeTimers();
        isInitialized = true;
    }
    
    protected virtual void InitializeTimers()
    {
        if (enemyData == null) return;
        
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
        
        if (timers.TryGetValue(name, out Timer timer))
        {
            return timer.Update(Time.deltaTime);
        }
        return false;
    }
    
    protected virtual void Update()
    {
        if (!isInitialized || enemyData == null) return;
        UpdateBehavior();
    }
    
    protected virtual void UpdateBehavior() {
        
    }
}
