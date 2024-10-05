using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    public float Speed;

    private new Rigidbody rigidbody;

    void Start() {
        rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        var joy = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        var moveDir = joy.x * transform.right + joy.y * transform.forward;
        var movement = Speed * Time.deltaTime * moveDir;
        rigidbody.MovePosition(rigidbody.position + movement);
    }
}
