using System;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private float sensitivity;
    [SerializeField] private Player player;
    private Vector3 distanceOffset;
    public float rotX, rotY;
    private void Start()
    {
        distanceOffset = player.LookHint.position - transform.position;
    }

    private void Update()
    {
        transform.position = player.LookHint.position - distanceOffset;
        
        if (Cursor.lockState != CursorLockMode.Locked)
            return;
        
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        rotY += mouseX;
        rotX -= mouseY;

        static float Mod(float x, float m) => (x%m + m)%m;

        rotX = Math.Clamp(rotX, -90, 90);
        rotY = Mod(rotY, 360);
        transform.eulerAngles = new Vector3(rotX, rotY, 0);
    }
}
