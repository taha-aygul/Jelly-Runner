using UnityEngine;
using DG.Tweening;

public class RotateObject : MonoBehaviour
{
    public float rotationDuration = 2f;
    public Vector3 rotationAmount = new Vector3(0f, 360f, 0f);

    void Start()
    {
        // Define the rotation tween
        transform.DOLocalRotate(rotationAmount, rotationDuration, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Incremental); // Infinite loop for continuous rotation
    }
}
