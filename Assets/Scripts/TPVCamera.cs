using UnityEngine;
using Cinemachine;

public class TPVCamera : MonoBehaviour
{
    public CinemachineFreeLook freeLookCamera;
    public float mouseSensitivity = 2f;
    private float xRotation = 0f;
    private float yRotation = 0f;
    public PlayerMovement playerMovement;

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        yRotation %= 360f;

        freeLookCamera.m_XAxis.Value = yRotation;
        freeLookCamera.m_YAxis.Value = xRotation;

        if (playerMovement != null && !playerMovement.isGrounded)
        {
            freeLookCamera.m_Lens.FieldOfView = Mathf.Lerp(freeLookCamera.m_Lens.FieldOfView, 60f, Time.deltaTime * 5f);
        }
        else
        {
            freeLookCamera.m_Lens.FieldOfView = Mathf.Lerp(freeLookCamera.m_Lens.FieldOfView, 50f, Time.deltaTime * 5f);
        }
    }
}
