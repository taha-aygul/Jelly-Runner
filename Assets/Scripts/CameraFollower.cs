using UnityEngine;
using DG.Tweening;
public class CameraFollower : MonoBehaviour
{
    public BlobStateController blobStateController;
    public float followSpeed = 5f;
    [SerializeField] private bool autoOffset;
    [SerializeField] private Vector3 offset;

    private void Start()
    {
        blobStateController = BlobStateController.Instance;
        if (autoOffset)
        {
            offset = blobStateController.GetAverageBlobPosition() - transform.position;
        }
    }

    private void FixedUpdate()
    {
        FollowJellos();
    }

    void FollowJellos()
    {
        Vector3 averageJelloPosition = blobStateController.GetAverageBlobPosition();
        Vector3 targetPosition = new Vector3(averageJelloPosition.x, averageJelloPosition.y - offset.y, averageJelloPosition.z - offset.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
       // transform.LookAt(averageJelloPosition);
    }
}
