using UnityEngine;

public class TPVIKController : MonoBehaviour
{
    [Header("IK Settings")]
    [SerializeField] private bool enableIK = true;
    [SerializeField] private Animator animator;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float raycastDistance = 1.5f;
    [SerializeField] private float footOffsetY = 0.1f;
    [SerializeField] private float ikSmooth = 8f;

    [Header("Pelvis Settings")]
    [SerializeField] private float pelvisOffset = 0f;
    [SerializeField] private float pelvisSmooth = 8f;

    [Header("Slope Alignment")]
    [SerializeField] private bool enableSlopeAlignment = true;
    [SerializeField] private float slopeAlignSmooth = 6f;
    [SerializeField] private float maxSlopeAngle = 45f;

    [Header("Debug Settings")]
    [SerializeField] private bool showDebug = false;
    [SerializeField] private Color rayHitColor = Color.green;
    [SerializeField] private Color rayMissColor = Color.red;
    [SerializeField] private Color footSphereColor = Color.yellow;
    [SerializeField] private Color pelvisSphereColor = Color.cyan;
    [SerializeField] private Color slopeNormalColor = Color.blue;
    [SerializeField] private float footSphereSize = 0.05f;
    [SerializeField] private float pelvisSphereSize = 0.08f;

    private Transform leftFootObj;
    private Transform rightFootObj;

    private Vector3 leftFootIKPos;
    private Vector3 rightFootIKPos;
    private Quaternion leftFootIKRot;
    private Quaternion rightFootIKRot;

    private float lastPelvisY;

    private void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        leftFootObj = animator.GetBoneTransform(HumanBodyBones.LeftFoot);
        rightFootObj = animator.GetBoneTransform(HumanBodyBones.RightFoot);
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (!enableIK || animator == null) return;

        AdjustFootTarget(ref leftFootIKPos, ref leftFootIKRot, leftFootObj);
        AdjustFootTarget(ref rightFootIKPos, ref rightFootIKRot, rightFootObj);

        MoveFeetIK(AvatarIKGoal.LeftFoot, leftFootIKPos, leftFootIKRot);
        MoveFeetIK(AvatarIKGoal.RightFoot, rightFootIKPos, rightFootIKRot);

        AdjustPelvisHeight();

        if (enableSlopeAlignment)
            AlignToSlope();
    }

    private void AdjustFootTarget(ref Vector3 footIKPos, ref Quaternion footIKRot, Transform foot)
    {
        RaycastHit hit;
        Vector3 start = foot.position + Vector3.up * 0.5f;

        if (Physics.Raycast(start, Vector3.down, out hit, raycastDistance, groundLayer))
        {
            footIKPos = hit.point + Vector3.up * footOffsetY;
            footIKRot = Quaternion.LookRotation(
                Vector3.ProjectOnPlane(transform.forward, hit.normal),
                hit.normal
            );

            if (showDebug)
                Debug.DrawLine(start, hit.point, rayHitColor);
        }
        else
        {
            footIKPos = foot.position;
            footIKRot = foot.rotation;

            if (showDebug)
                Debug.DrawLine(start, start + Vector3.down * raycastDistance, rayMissColor);
        }
    }

    private void MoveFeetIK(AvatarIKGoal foot, Vector3 pos, Quaternion rot)
    {
        animator.SetIKPositionWeight(foot, 1f);
        animator.SetIKRotationWeight(foot, 1f);

        Vector3 currentPos = animator.GetIKPosition(foot);
        Quaternion currentRot = animator.GetIKRotation(foot);

        animator.SetIKPosition(foot, Vector3.Lerp(currentPos, pos, Time.deltaTime * ikSmooth));
        animator.SetIKRotation(foot, Quaternion.Slerp(currentRot, rot, Time.deltaTime * ikSmooth));
    }

    private void AdjustPelvisHeight()
    {
        float leftY = leftFootIKPos.y - transform.position.y;
        float rightY = rightFootIKPos.y - transform.position.y;
        float targetY = Mathf.Min(leftY, rightY) + pelvisOffset;

        Vector3 newPelvisPos = animator.bodyPosition;
        newPelvisPos.y = Mathf.Lerp(lastPelvisY, newPelvisPos.y + targetY, Time.deltaTime * pelvisSmooth);

        animator.bodyPosition = newPelvisPos;
        lastPelvisY = newPelvisPos.y;

        if (showDebug)
            Gizmos.DrawSphere(newPelvisPos, pelvisSphereSize);
    }

    private void AlignToSlope()
    {
        RaycastHit hit;
        Vector3 origin = transform.position + Vector3.up * 1f;

        if (Physics.Raycast(origin, Vector3.down, out hit, raycastDistance * 2f, groundLayer))
        {
            Vector3 groundNormal = hit.normal;

            if (Vector3.Angle(Vector3.up, groundNormal) <= maxSlopeAngle)
            {
                Quaternion slopeRotation = Quaternion.FromToRotation(transform.up, groundNormal) * transform.rotation;
                transform.rotation = Quaternion.Slerp(transform.rotation, slopeRotation, Time.deltaTime * slopeAlignSmooth);

                if (showDebug)
                    Debug.DrawRay(hit.point, groundNormal, slopeNormalColor);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!showDebug) return;

        Gizmos.color = footSphereColor;
        Gizmos.DrawSphere(leftFootIKPos, footSphereSize);
        Gizmos.DrawSphere(rightFootIKPos, footSphereSize);

        Gizmos.color = pelvisSphereColor;
        Gizmos.DrawSphere(animator != null ? animator.bodyPosition : transform.position, pelvisSphereSize);
    }
}
