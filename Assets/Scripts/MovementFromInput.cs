using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovementFromInput : MonoBehaviour
{
    private new Rigidbody rigidbody;

    [SerializeField] private Transform rotationTracker;
    [SerializeField] private float rotationSpeed = 0.1f;
    [SerializeField] private float speed = 3;
    public Quaternion CameraForwardRotation => Quaternion.Euler(0, rotationTracker.eulerAngles.y, 0);
    public Vector3 CameraForward => CameraForwardRotation * Vector3.forward;
    public Vector3 CameraRight => CameraForwardRotation * Vector3.right;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // // Handle cursor locking
        if (Input.GetMouseButtonDown(0))
            Cursor.lockState = CursorLockMode.Locked;
        if (Input.GetKeyDown(KeyCode.Escape))
            Cursor.lockState = CursorLockMode.None;


    }

    void FixedUpdate()
    {
        var movement = Vector3.zero;
        movement += Input.GetAxis("Horizontal") * CameraRight;
        movement += Input.GetAxis("Vertical") * CameraForward;
        movement.Normalize(); // Fixes diagonal movement speed-up
        movement *= speed * Time.deltaTime;
        rigidbody.position += movement;

        // Slowly rotate the player towards the camera facing angle when moving.
        if (movement.sqrMagnitude > 0)
        {
            rigidbody.rotation = Quaternion.RotateTowards(rigidbody.rotation, CameraForwardRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
