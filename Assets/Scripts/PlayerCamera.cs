using System;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private float sensitivity;
    [SerializeField] private Transform player;
    private Vector3 distanceOffset;
    private float rotX, rotY;
    private void Start()
    {
        distanceOffset = player.position - transform.position;
    }

    private void Update()
    {
        transform.position = player.position - distanceOffset;
        
        if (Cursor.lockState != CursorLockMode.Locked)
            return;
        
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        rotY += mouseX;
        rotX -= mouseY;

        rotX = Math.Clamp(rotX, -90, 90);
        transform.eulerAngles = new Vector3(rotX, rotY, 0);
    }
}
