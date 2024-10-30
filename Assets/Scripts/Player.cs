using System;
using UnityEngine;

[Serializable]
public class PlayerAttributes
{
    public int Damage = 0;
    public int Speed = 0;
    public int Stamina = 0;
}
public class Player : MonoBehaviour, IDamageHandler
{

    public int Experience;
    public int Level;

    public int MaxHealth = 100;
    public int Health = 100;

    public float InvincibilityTime = 1f;

    private float invincibilityTimer = 0;
    public bool IsInvincible => invincibilityTimer > 0;

    public int AttributePoints;

    public PlayerAttributes Attributes;

    public int XpRequired => 20 * Level + 100;

    [SerializeField] private Transform attackRangeHint;
    [SerializeField] private HUD HUD;

    private MovementController movementController;

    void Start()
    {
        movementController = GetComponent<MovementController>();
    }

    void Update()
    {
        if (Experience >= XpRequired)
            LevelUp();

        if (Health <= 0)
            GameOver();

        if (Health > MaxHealth)
            Health = MaxHealth;

        if (invincibilityTimer > 0)
        {
            invincibilityTimer -= Time.deltaTime;
            invincibilityTimer = Mathf.Max(0, invincibilityTimer);
        }

        HUD.XpSlider.value = Experience / XpRequired;
        HUD.HealthSlider.value = (float)Health / MaxHealth;

        // Input Handlers
        if (Input.GetMouseButtonDown(0))
            Attack();

        // Debug Input
        if (Input.GetKeyDown(KeyCode.U))
            Experience += 10;
    }

    void Attack()
    {
        StartCoroutine(movementController.co_FaceCameraForward());
        if (!movementController.IsFacingCameraForward()) return;

        var rayVector = attackRangeHint.position - transform.position;
        if (Physics.Raycast(
            origin: transform.position,
            direction: rayVector.normalized,
            hitInfo: out RaycastHit hit,
            maxDistance: rayVector.magnitude))
        {
            var gameObject = hit.collider.gameObject;
            if (gameObject.TryGetComponent(out IDamageHandler damageHandler))
            {
                var damage = 1 + Attributes.Damage;
                damageHandler.OnDamage(this.gameObject, damage);
            }
        }
    }

    void LevelUp()
    {
        Experience = 0;
        Level += 1;
        AttributePoints += 1;
        print($"Level Up: {Level - 1} -> {Level}");
        print($"+1 Attribute Point");
        // TODO Update UI
    }

    void GameOver()
    {
        print("Game over");
        // TODO
    }

    public void OnDamage(GameObject source, int damage)
    {
        if (source == this) return;
        if (IsInvincible) return;

        Health -= damage;
        invincibilityTimer = InvincibilityTime;
        // TODO Damage effect
    }
}
