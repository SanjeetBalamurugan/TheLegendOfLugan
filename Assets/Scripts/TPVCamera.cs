using UnityEngine;
using Cinemachine;

public class TPVCamera : MonoBehaviour
{
    public CinemachineFreeLook freeLookCamera;
    public Transform player;
    public float mouseSensitivity = 3f;
    public float smoothing = 5f;
    public float upperClamp = 70f;
    public float lowerClamp = -30f;

    private float xRotation = 0f;
    private float yRotation = 0f;

    private Vector2 currentMouseLook;
    private Vector2 currentMouseDelta;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        currentMouseDelta = new Vector2(mouseX, mouseY);
        currentMouseLook = Vector2.Lerp(currentMouseLook, currentMouseLook + currentMouseDelta, Time.deltaTime * smoothing);

        yRotation += currentMouseLook.x;
        xRotation -= currentMouseLook.y;

        xRotation = Mathf.Clamp(xRotation, lowerClamp, upperClamp);

        freeLookCamera.m_XAxis.Value = yRotation;
        freeLookCamera.m_YAxis.Value = xRotation;

        HandleCameraCollision();
    }

    private void HandleCameraCollision()
    {
        Vector3 cameraPosition = freeLookCamera.transform.position;
        Vector3 direction = cameraPosition - player.position;
        RaycastHit hit;

        if (Physics.Raycast(player.position, direction, out hit, direction.magnitude))
        {
            freeLookCamera.transform.position = hit.point;
        }
        else
        {
            freeLookCamera.transform.position = cameraPosition;
        }
    }
}
