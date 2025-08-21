using UnityEngine;
using Cinemachine;

public class TPVCamera : MonoBehaviour
{
    public CinemachineFreeLook freeLookCamera;
    public float mouseSensitivity = 3f;
    public float smoothing = 5f;
    public float verticalClampMin = 0.1f;
    public float verticalClampMax = 0.9f;

    private Vector2 mouseDelta;
    private Vector2 smoothMouse;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity;

        mouseDelta = Vector2.Lerp(mouseDelta, new Vector2(mouseX, mouseY), Time.deltaTime * smoothing);

        freeLookCamera.m_XAxis.Value += -mouseDelta.x;
        freeLookCamera.m_YAxis.Value -= mouseDelta.y * 0.01f;

        freeLookCamera.m_YAxis.Value = -Mathf.Clamp(freeLookCamera.m_YAxis.Value, verticalClampMin, verticalClampMax);
    }
}
