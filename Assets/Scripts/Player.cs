using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    public float Speed;

    public float Sensitivity;

    private new Rigidbody rigidbody;
    private new Camera camera;

    private Vector2 currentRotation;
    public float maxYAngle = 90f;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        camera = GetComponentInChildren<Camera>();
    }

    void Update()
    {
        // Handle cursor locking
        if (Input.GetMouseButtonDown(0))
            Cursor.lockState = CursorLockMode.Locked;
        if (Input.GetKeyDown(KeyCode.Escape))
            Cursor.lockState = CursorLockMode.None;

        // We'll handle camera rotation only when the mouse locked for use.
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            currentRotation.x += Input.GetAxis("Mouse X") * Sensitivity;
            currentRotation.y -= Input.GetAxis("Mouse Y") * Sensitivity;
            currentRotation.x = Mathf.Repeat(currentRotation.x, 360);
            currentRotation.y = Mathf.Clamp(currentRotation.y, -maxYAngle, maxYAngle);

            /*
             * For the horizontal rotation we rotate the player itself so they face the right direction.
             * But for vertical rotation we'll only rotate the camera itself,
             * so the player will still face forward and not tilt.
             */
            transform.rotation = Quaternion.Euler(0, currentRotation.x, 0);
            camera.transform.localRotation = Quaternion.Euler(currentRotation.y, 0, 0);
        }
    }

    void FixedUpdate()
    {
        var joy = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        var moveDir = joy.x * transform.right + joy.y * transform.forward;
        var movement = Speed * Time.deltaTime * moveDir;
        rigidbody.MovePosition(rigidbody.position + movement);
    }
}
