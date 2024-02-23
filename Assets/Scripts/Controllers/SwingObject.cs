using UnityEngine;
using DG.Tweening;

public class SwingObject : MonoBehaviour
{
    public Transform pilot; // Reference to the pilot or character
    public float swingDuration = 2f;
    public Vector3 swingAxis = Vector3.up;
    public float swingAngle = 90;

    void Start()
    {
        // Calculate the pivot axis based on the pilot's position
        Vector3 pivotAxis = Vector3.Cross(transform.position - pilot.position, Vector3.up).normalized;

        // Define the rotation tween
        transform.DOLocalRotate(swingAngle * swingAxis, swingDuration, RotateMode.FastBeyond360)
            .SetEase(Ease.InOutQuad)
            .SetLoops(-1, LoopType.Yoyo) // Yoyo for a back-and-forth motion
            .SetRelative(true)
            .OnUpdate(() => RotateAroundPivot(pivotAxis));
    }

    void RotateAroundPivot(Vector3 pivotAxis)
    {
        transform.position = Quaternion.AngleAxis(swingAngle, pivotAxis) * (transform.position - pilot.position) + pilot.position;
    }
}
