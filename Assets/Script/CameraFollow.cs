using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; 
    public float smoothTime = 0.3f; 
    public Vector2 offset = new Vector2(3f, 2f); 
    public Transform boundaryTransform;

    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPosition = target.position + new Vector3(offset.x, offset.y, -10f);

        if (boundaryTransform != null)
        {
            Vector3 minBoundary = boundaryTransform.position - boundaryTransform.localScale / 2;
            Vector3 maxBoundary = boundaryTransform.position + boundaryTransform.localScale / 2;

            targetPosition.x = Mathf.Clamp(targetPosition.x, minBoundary.x, maxBoundary.x);
            targetPosition.y = Mathf.Clamp(targetPosition.y, minBoundary.y, maxBoundary.y);
        }

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}
