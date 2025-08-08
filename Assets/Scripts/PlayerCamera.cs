using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform Player;
    float yRot = 0;
    public float mouseSensitivity = 1f;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        yRot -= mouseY;
        yRot = Mathf.Clamp(yRot, -90f, 90f);

        transform.localRotation = Quaternion.Euler(yRot, 0, 0);
        Player.Rotate(Vector3.up * mouseX);
       
    }
}
