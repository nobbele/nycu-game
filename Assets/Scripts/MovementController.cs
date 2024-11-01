using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovementController : MonoBehaviour
{
    private new Rigidbody rigidbody;

    [SerializeField] private Animator animator;
    [SerializeField] private Transform rotationTracker;
    [SerializeField] private float rotationSpeed = 400f;
    [SerializeField] private float speed = 3;
    
    public Quaternion CameraForwardRotation => Quaternion.Euler(0, rotationTracker.eulerAngles.y, 0);
    public Vector3 CameraForward => CameraForwardRotation * Vector3.forward;
    public Vector3 CameraRight => CameraForwardRotation * Vector3.right;

    public bool DisabledMovement;
    
    //Rotation For Animation
    private Vector3 characterRotation = new (0, 0, 0);
    private Vector3 idleRotation = new (0, 15, 0);
    private Vector3 faceFrontRotation = new (0, 30, 0);
    private Vector3 faceFrontLeftRotation = new (0, -20, 0);
    private Vector3 faceFrontRightRotation = new (0, 80, 0);

    private Quaternion frontRot => Quaternion.Euler(CameraForwardRotation.eulerAngles + faceFrontRotation);
    private Quaternion frontLeftRot => Quaternion.Euler(CameraForwardRotation.eulerAngles + faceFrontLeftRotation);
    private Quaternion frontRightRot => Quaternion.Euler(CameraForwardRotation.eulerAngles + faceFrontRightRotation);
    private Quaternion idleRot => Quaternion.Euler(characterRotation + idleRotation);
    
    
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
            Cursor.lockState = CursorLockMode.None;
            return;
        }

        // Handle cursor locking
        if (Input.GetMouseButtonDown(0))
            Cursor.lockState = CursorLockMode.Locked;
        if (Input.GetKeyDown(KeyCode.Escape))
            Cursor.lockState = CursorLockMode.None;
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

    public IEnumerator AttackAnimation()
    {
        if (animator.GetBool("IsDoingAnimation")) yield break;
        
        //Enabling the animation
        animator.applyRootMotion = true;
        animator.SetBool("IsDoingAnimation",true);
        animator.SetBool("IsSlashing", true);
        animator.SetBool("IsMoving", false);
        
        //Setting Rotation
        characterRotation = CameraForwardRotation.eulerAngles;
        Quaternion attackRot = idleRot;
        while (rigidbody.rotation != attackRot)
        {
            rigidbody.rotation = Quaternion.RotateTowards(rigidbody.rotation, attackRot, rotationSpeed * Time.deltaTime);
            yield return null;
        }
        
        //Wait until the animation is over
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Slash") || animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
        {
            yield return null;
        }
        
        //Disabling the animation
        animator.applyRootMotion = false;
        animator.SetBool("IsSlashing", false);
        animator.SetBool("IsDoingAnimation",false);
    }

    void FixedUpdate()
    {
        if (DisabledMovement) return;
        if (animator.GetBool("IsDoingAnimation")) return;
        
        Vector3 movement = Vector3.zero;
        
        
        Quaternion toRot = frontRot;

        if (Input.GetKey(keybinds["Left"]))
            movement += -CameraRight;
        if (Input.GetKey(keybinds["Right"]))
            movement += CameraRight;
        if (Input.GetKey(keybinds["Forward"]))
            movement += CameraForward;
        if (Input.GetKey(keybinds["Backward"]))
            movement += -CameraForward;

        movement.Normalize();
        movement *= speed * Time.deltaTime;
        rigidbody.position += movement;

        //Is moving forward or backward
        if (Input.GetKey(keybinds["Forward"]) || Input.GetKey(keybinds["Backward"]))
        {
            //Is moving forward
            if (Input.GetKey(keybinds["Forward"]))
            {
                if (Input.GetKey(keybinds["Left"]))
                    toRot = frontLeftRot;
                if (Input.GetKey(keybinds["Right"]))
                    toRot = frontRightRot;
            }
            //Is moving backward
            if (Input.GetKey(keybinds["Backward"]))
            {
                if (Input.GetKey(keybinds["Left"]))
                    toRot = frontRightRot;
                if (Input.GetKey(keybinds["Right"]))
                    toRot = frontLeftRot;
            }
            //If both left and right key are pressed
            if (Input.GetKey(keybinds["Left"]) && Input.GetKey(keybinds["Right"]))
                toRot = frontRot;
        }
        
        animator.SetBool("IsMoving", false);
        if (movement.sqrMagnitude > 0)
        {
            animator.SetBool("IsMoving", true);
            rigidbody.rotation = Quaternion.RotateTowards(rigidbody.rotation, toRot, rotationSpeed * Time.deltaTime);
            characterRotation = CameraForwardRotation.eulerAngles;
        }
        else
            rigidbody.rotation = idleRot;
        
        animator.SetBool("IsRunningForward", Input.GetKey(keybinds["Forward"]) && animator.GetBool("IsMoving"));
        animator.SetBool("IsRunningBackward", Input.GetKey(keybinds["Backward"]) && animator.GetBool("IsMoving"));
        animator.SetBool("IsStrafingLeft", IsStrafing("Left") && animator.GetBool("IsMoving"));
        animator.SetBool("IsStrafingRight", IsStrafing("Right") && animator.GetBool("IsMoving"));
        
        bool IsStrafing(string s)
        {
            return !Input.GetKey(keybinds["Forward"]) && !Input.GetKey(keybinds["Backward"]) && Input.GetKey(keybinds[s]);
        }
    }
}
