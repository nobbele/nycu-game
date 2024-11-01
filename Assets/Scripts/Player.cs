using System;
using Cinemachine;
using UnityEngine;

[Serializable]
public class PlayerAttributes
{
    public int Damage = 0;
    public int Speed = 0;
    public int Stamina = 0;
    public int Health = 0;
}
public class Player : MonoBehaviour, IDamageHandler
{

    public int Experience;
    public int Level;

    public int MaxHealth => 100 + 20 * Attributes.Health;
    public int Health = 100;

    public float InvincibilityTime = 1f;

    private float invincibilityTimer = 0;
    public bool IsInvincible => invincibilityTimer > 0;

    public int AttributePoints;

    public PlayerAttributes Attributes;

    public int XpRequired => 20 * Level + 100;

    public bool IsDead => Health <= 0;

    [SerializeField] private Transform attackRangeHint;
    [SerializeField] private HUD HUD;
    [SerializeField] private CharacterMenu CharacterMenu;
    //[SerializeField] private CinemachineFreeLook CinemachineFreeLook;

    private MovementController movementController;

    void Start()
    {
        movementController = GetComponent<MovementController>();
        CharacterMenu.gameObject.SetActive(false);

        CharacterMenu.DamageAttributeUI.Title = "Damage";
        CharacterMenu.DamageAttributeUI.Value = Attributes.Damage;
        CharacterMenu.DamageAttributeUI.OnValueChanged.AddListener((int newValue) => {
            Attributes.Damage = newValue;
            AttributePoints -= 1;
        });

        CharacterMenu.HealthAttributeUI.Title = "Health";
        CharacterMenu.HealthAttributeUI.Value = Attributes.Health;
        CharacterMenu.HealthAttributeUI.OnValueChanged.AddListener((int newValue) => {
            Attributes.Health = newValue;
            AttributePoints -= 1;
        });
    }

    void Update()
    {
        if (Experience >= XpRequired)
            LevelUp();

        if (IsDead)
            GameOver();

        if (Health > MaxHealth)
            Health = MaxHealth;

        if (invincibilityTimer > 0)
        {
            invincibilityTimer -= Time.deltaTime;
            invincibilityTimer = Mathf.Max(0, invincibilityTimer);
        }

        HUD.XpSlider.value = (float)Experience / XpRequired;
        HUD.HealthSlider.value = (float)Health / MaxHealth;
        HUD.LevelLabel.text = $"Level: {Level + 1}";

        CharacterMenu.DamageAttributeUI.SetEnabled(AttributePoints > 0);
        CharacterMenu.HealthAttributeUI.SetEnabled(AttributePoints > 0);
        CharacterMenu.AttributePointsLeftText.text = $"Points Left: {AttributePoints}";

        movementController.DisabledMovement = CharacterMenu.gameObject.activeInHierarchy;

        if (Input.GetKeyDown(KeyCode.P))
            CharacterMenu.gameObject.SetActive(!CharacterMenu.gameObject.activeInHierarchy);

        if (!CharacterMenu.gameObject.activeInHierarchy) {
            // Input Handlers
            if (Input.GetMouseButtonDown(0))
                Attack();

            // Debug Input
            if (Input.GetKeyDown(KeyCode.U))
                Experience += 10;

            // if (!CinemachineFreeLook.enabled) {
            //     CinemachineFreeLook.enabled = true;
            // }
            Cursor.lockState = CursorLockMode.Locked;
        } else {
            // if (CinemachineFreeLook.enabled) {
            //     CinemachineFreeLook.enabled = false;
            // }
            Cursor.lockState = CursorLockMode.None;
        }
    }

    void Attack()
    {
        // StartCoroutine(movementController.co_FaceCameraForward());
        // if (!movementController.IsFacingCameraForward()) return;
        StartCoroutine(movementController.AttackAnimation());
        
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
                var damage = 10 + 2 * Attributes.Damage;
                damageHandler.OnDamage(this.gameObject, damage);

                // If our attack caused the enemy to die, gain some xp
                if (damageHandler.IsDead) 
                {
                    Experience += 10;
                }
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
