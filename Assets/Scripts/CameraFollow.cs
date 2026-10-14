using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    public float smoothSpeed = 2f;
    public float xOffset = 4f;
    public float upperThreshold = 2.5f;
    public float lowerThreshold = -3.5f;
    public float minY = 0f;

    private float currentY;

    void Start()
    {
        currentY = transform.position.y;
    }

    void LateUpdate()
    {
        if (target == null) return;

        float targetX = target.position.x + xOffset;

        float diffY = target.position.y - transform.position.y;

        // Only move up when about to fly off the top screen edge
        if (diffY > upperThreshold)
        {
            currentY = Mathf.Lerp(
                currentY,
                target.position.y,
                smoothSpeed * Time.deltaTime
            );
        }

        // Only shift downward if the plane significantly drops below the bottom screen edge
        if (diffY < lowerThreshold && target.position.y > minY)
        {
            currentY = Mathf.Lerp(
                currentY,
                target.position.y,
                smoothSpeed * Time.deltaTime
            );
        }

        // Clamp camera position to prevent it from dropping below the initial starting height
        currentY = Mathf.Max(currentY, minY);

        Vector3 targetPos = new Vector3(targetX, currentY, -10);

        transform.position = Vector3.Lerp(
            transform.position,
            targetPos,
            smoothSpeed * Time.deltaTime
        );
    }
}
