using UnityEngine;

public class CenterBillboard : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;
    [SerializeField] private Transform targetObject;
    [SerializeField] private float distanceFromCamera = 5f;

    private void LateUpdate()
    {
        if (!targetCamera || !targetObject) return;

        targetObject.position = targetCamera.transform.position + targetCamera.transform.forward * distanceFromCamera;
        targetObject.LookAt(targetCamera.transform);
        targetObject.Rotate(0, 180f, 0); // flip to face properly
    }
}
