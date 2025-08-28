using UnityEngine;

public class TPVCamera : MonoBehaviour
{
    public Transform player;
    public float followSpeed = 10f;
    public Vector3 defaultOffset = new Vector3(0f, 1.5f, -4f);
    public Vector3 aimOffset = new Vector3(0f, 1.5f, -2f);
    public float rotationSpeed = 5f;
    public Camera mainCamera;
    public TPVPlayerMove playerMoveScript;
    private Vector3 currentOffset;

    void Start()
    {
        currentOffset = defaultOffset;
        if (mainCamera == null) mainCamera = Camera.main;
        if (playerMoveScript == null) playerMoveScript = player.GetComponent<TPVPlayerMove>();
    }

    void Update()
    {
        bool isAiming = playerMoveScript.GetAimValue();
        currentOffset = isAiming ? aimOffset : defaultOffset;
        HandleCameraPosition();
        HandleCameraRotation();
    }

    private void HandleCameraPosition()
    {
        Vector3 desiredPosition = player.position + currentOffset;
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, desiredPosition, Time.deltaTime * followSpeed);
    }

    private void HandleCameraRotation()
    {
        float horizontalRotation = Input.GetAxis("Mouse X") * rotationSpeed;
        float verticalRotation = -Input.GetAxis("Mouse Y") * rotationSpeed;
        mainCamera.transform.RotateAround(player.position, Vector3.up, horizontalRotation);
        mainCamera.transform.RotateAround(player.position, mainCamera.transform.right, verticalRotation);
    }
}
