using UnityEngine;

[RequireComponent(typeof(Camera))]
public sealed class CameraFollow2D : MonoBehaviour
{
    [SerializeField] private float smoothTime = 0.18f;
    [SerializeField] private Vector2 minimum = new(8.9f, 2f);
    [SerializeField] private Vector2 maximum = new(135.1f, 2f);

    private Transform target;
    private Vector3 velocity;

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        Vector3 desired = new(
            Mathf.Clamp(target.position.x, minimum.x, maximum.x),
            Mathf.Clamp(target.position.y + 1.5f, minimum.y, maximum.y),
            transform.position.z);

        transform.position = Vector3.SmoothDamp(
            transform.position,
            desired,
            ref velocity,
            smoothTime);
    }
}
