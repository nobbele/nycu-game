using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovementController : MonoBehaviour
{
    private new Rigidbody rigidbody;

    [SerializeField] private Transform rotationTracker;
    [SerializeField] private float rotationSpeed = 400f;
    [SerializeField] private float speed = 3;
    public Quaternion CameraForwardRotation => Quaternion.Euler(0, rotationTracker.eulerAngles.y, 0);
    public Vector3 CameraForward => CameraForwardRotation * Vector3.forward;
    public Vector3 CameraRight => CameraForwardRotation * Vector3.right;

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
        // // Handle cursor locking
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

    void FixedUpdate()
    {
        Vector3 movement = Vector3.zero;

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

        if (movement.sqrMagnitude > 0)
        {
            rigidbody.rotation = Quaternion.RotateTowards(rigidbody.rotation, CameraForwardRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
