using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class MovementController : MonoBehaviour
{
    private new Rigidbody rigidbody;

    [SerializeField] private Animator animator;
    [SerializeField] private Transform rotationTracker;
    [SerializeField] private float rotationSpeed = 400f;
    [SerializeField] private float forwardSpeed = 5;
    [SerializeField] private float backwardSpeed = 3;
    [SerializeField] private float strafeSpeed = 4;
    
    public Quaternion CameraForwardRotation => Quaternion.Euler(0, rotationTracker.eulerAngles.y, 0);
    public Vector3 CameraForward => CameraForwardRotation * Vector3.forward;
    public Vector3 CameraRight => CameraForwardRotation * Vector3.right;

    public UnityEvent attackAnimationSlash = new();
    private float comboResetTime = 0.4F;
    private float comboTimer = 0.4f;

    public bool DisabledMovement;
    
    //Rotation For Animation
    private Vector3 characterRotation = new (0, 0, 0);
    private Vector3 idleRotation = new (0, 15, 0);
    private Vector3 runForwardRotation = new (0, 40, 0);
    private Vector3 runBackwardRotation = new(0, 30, 0);
    private Vector3 faceFrontLeftRotation = new (0, -20, 0);
    private Vector3 faceFrontRightRotation = new (0, 80, 0);

    private Quaternion forwardRot => Quaternion.Euler(CameraForwardRotation.eulerAngles + runForwardRotation);
    private Quaternion backwardRot => Quaternion.Euler(CameraForwardRotation.eulerAngles + runBackwardRotation);
    private Quaternion frontLeftRot => Quaternion.Euler(CameraForwardRotation.eulerAngles + faceFrontLeftRotation);
    private Quaternion frontRightRot => Quaternion.Euler(CameraForwardRotation.eulerAngles + faceFrontRightRotation);
    private Quaternion idleRot => Quaternion.Euler(characterRotation + idleRotation);

    private float SpeedMultiplier = 1;

    private Vector3 prevChildPos;
    
    private Dictionary<string, KeyCode> keybinds = new Dictionary<string, KeyCode>
    {
        { "Forward", KeyCode.W },
        { "Left", KeyCode.A },
        { "Backward", KeyCode.S },
        { "Right", KeyCode.D }
    };

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        
        var updatedKeybinds = new Dictionary<string, KeyCode>();

        foreach (var key in keybinds.Keys)
        {
            string savedKey = PlayerPrefs.GetString($"Keybind_{key}", keybinds[key].ToString());
            updatedKeybinds[key] = (KeyCode)Enum.Parse(typeof(KeyCode), savedKey);
        }

        keybinds = updatedKeybinds;
    }

    void Update()
    {
        if (DisabledMovement) {
            return;
        }

        // Handle cursor locking
        if (Input.GetMouseButtonDown(0))
            Cursor.lockState = CursorLockMode.Locked;
        
        // Get the child’s root motion delta
        Vector3 motionDelta = animator.transform.position - transform.position;
        motionDelta.y = 0;
        prevChildPos = animator.transform.position;
        // Apply the child’s root motion to the parent
        transform.position += motionDelta;
        animator.transform.position = prevChildPos;

        if (animator.GetInteger("ComboIndex") >= 1)
        {
            comboTimer -= Time.deltaTime;

            if (comboTimer <= 0.0f)
            {
                animator.SetInteger("ComboIndex", 0);
                comboTimer = comboResetTime;
            }
        }
    }

    public bool IsFacingCameraForward() => IsFacing(transform.position + CameraForward);
    public bool IsFacing(Vector3 target)
    {
        var quat = Quaternion.LookRotation((target - transform.position).normalized);
        quat.eulerAngles = new Vector3(0, quat.eulerAngles.y, 0);

        // +- 10 degrees of leniency
        return Mathf.Abs(rigidbody.rotation.eulerAngles.y - quat.eulerAngles.y) < 10f;
    }

    private bool faceTowardsRunning = false;
    public IEnumerator co_FaceCameraForward() => co_FaceTowardsTarget(transform.position + CameraForward);
    public IEnumerator co_FaceTowardsTarget(Vector3 target)
    {
        if (faceTowardsRunning) yield break;
        faceTowardsRunning = true;

        var quat = Quaternion.LookRotation((target - transform.position).normalized);
        quat.eulerAngles = new Vector3(0, quat.eulerAngles.y, 0);

        while (rigidbody.rotation.eulerAngles.y != quat.eulerAngles.y)
        {
            rigidbody.rotation = Quaternion.RotateTowards(rigidbody.rotation, quat, rotationSpeed * Time.deltaTime);
            yield return null;
        }

        faceTowardsRunning = false;
    }

    public IEnumerator SlashComboAnimation()
    {
        if (animator.GetBool("IsDoingAnimation")) yield break;
        
        //Enabling the animation
        animator.applyRootMotion = true;
        animator.SetBool("IsDoingAnimation",true);
        animator.SetBool("IsMoving", false);
        animator.SetBool("IsSlashing", true);
        
        //Setting Rotation
        characterRotation = CameraForwardRotation.eulerAngles;
        Quaternion attackRot = idleRot;
        while (rigidbody.rotation != attackRot)
        {
            rigidbody.rotation = Quaternion.RotateTowards(rigidbody.rotation, attackRot, rotationSpeed * Time.deltaTime);
            yield return null;
        }

        const float slashActionTime = 0.5f;
        bool triggeredSlash = false;
        AnimatorStateInfo stateInfo;
        //Wait until the animation is over
        while (true)
        {
            comboTimer = comboResetTime;
            
            stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        
            if (IsSlashState() && stateInfo.normalizedTime >= slashActionTime && !triggeredSlash)
            {
                triggeredSlash = true;
                attackAnimationSlash.Invoke();
            }

            if (stateInfo.normalizedTime >= 1.0f)  // Check if the animation is complete
                break;
            
            yield return null;
        }
        
        //Disabling the animation
        animator.applyRootMotion = false;
        animator.SetBool("IsDoingAnimation",false);
        animator.SetBool("IsSlashing", false);
        
        animator.SetInteger("ComboIndex", animator.GetInteger("ComboIndex") + 1);
        
        if (animator.GetInteger("ComboIndex") >= 3)
            animator.SetInteger("ComboIndex", 0);
        
        comboTimer = comboResetTime;

        bool IsSlashState()
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("Slash") || stateInfo.IsName("Slash 2") || stateInfo.IsName("Slash 3"))
                return true;
            return false;
        }
    }

    void FixedUpdate()
    {
        if (DisabledMovement) return;
        if (animator.GetBool("IsDoingAnimation")) return;
        
        Vector3 movement = Vector3.zero;
        Quaternion toRot = idleRot;
        float speed = strafeSpeed;
        
        //Determining direction of movement and rotation
        if (Input.GetKey(keybinds["Forward"]) || Input.GetKey(keybinds["Backward"]))
        {
            if (Input.GetKey(keybinds["Forward"]))
            {
                movement += CameraForward;
                speed = forwardSpeed;
                toRot = forwardRot;
                if (Input.GetKey(keybinds["Left"]))
                    toRot = frontLeftRot;
                if (Input.GetKey(keybinds["Right"]))
                    toRot = frontRightRot;
            }
            if (Input.GetKey(keybinds["Backward"]))
            {
                movement += -CameraForward;
                speed = backwardSpeed;
                toRot = backwardRot;
                if (Input.GetKey(keybinds["Left"]))
                    toRot = frontRightRot;
                if (Input.GetKey(keybinds["Right"]))
                    toRot = frontLeftRot;
            }
        }
        
        if (Input.GetKey(keybinds["Left"]))
            movement += -CameraRight;
        if (Input.GetKey(keybinds["Right"]))
            movement += CameraRight;

        movement.Normalize();
        movement *= speed * SpeedMultiplier * Time.deltaTime;
        rigidbody.position += movement;
        
        //Rotating and enabling animation
        animator.SetBool("IsMoving", false);
        if (movement.sqrMagnitude > 0)
        {
            animator.SetBool("IsMoving", true);
            animator.applyRootMotion = false;
            rigidbody.rotation = Quaternion.RotateTowards(rigidbody.rotation, toRot, rotationSpeed * Time.deltaTime);
            characterRotation = CameraForwardRotation.eulerAngles;
        }
        else
        {
            animator.applyRootMotion = true;
            rigidbody.rotation = idleRot;
        }
            
        
        animator.SetBool("IsRunningForward", Input.GetKey(keybinds["Forward"]) && animator.GetBool("IsMoving"));
        animator.SetBool("IsRunningBackward", Input.GetKey(keybinds["Backward"]) && animator.GetBool("IsMoving"));
        animator.SetBool("IsStrafingLeft", IsStrafing("Left") && animator.GetBool("IsMoving"));
        animator.SetBool("IsStrafingRight", IsStrafing("Right") && animator.GetBool("IsMoving"));
        
        bool IsStrafing(string s)
        {
            return !Input.GetKey(keybinds["Forward"]) && !Input.GetKey(keybinds["Backward"]) && Input.GetKey(keybinds[s]);
        }
    }

    public void MultiplySpeedMultiplier(float multiplier)
    {
        SpeedMultiplier *= multiplier;
        animator.SetFloat("SpeedMultiplier", SpeedMultiplier);
    }
    
    public void DivideSpeedMultiplier(float multiplier)
    {
        SpeedMultiplier /= multiplier;
        animator.SetFloat("SpeedMultiplier", SpeedMultiplier);
    }
}
