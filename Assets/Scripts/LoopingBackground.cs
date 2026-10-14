using UnityEngine;

public class LoopingBackground : MonoBehaviour
{
    [Tooltip("Reference to the paper plane")]
    public Transform plane;

    [Tooltip("The width of a single background")]
    public float backgroundWidth = 25.6f;

    [Tooltip("Total number of backgrounds in the scene")]
    public int totalBackgrounds = 3;

    private float totalWidth;
    private float halfTotalWidth;

    void Start()
    {
        // Pre-calculate the total width and half width to avoid redundant calculations in Update
        totalWidth = backgroundWidth * totalBackgrounds;
        halfTotalWidth = totalWidth / 2f;
    }

    // Use LateUpdate to ensure the background moves after the plane's physics update, preventing visual jitter
    void LateUpdate()
    {
        if (plane == null) return;

        // Calculate the X-axis distance between the plane and this background
        float distanceToPlane = plane.position.x - transform.position.x;

        // Case 1: The plane flies forward, and this background is left far behind (exceeds half the total width)
        if (distanceToPlane > halfTotalWidth)
        {
            // Instantly move the tile to the front
            transform.position += new Vector3(totalWidth, 0, 0);
        }
        // Case 2: The plane flies backward, and background is too far ahead
        else if (distanceToPlane < -halfTotalWidth)
        {
            // Instantly move background to the back
            transform.position -= new Vector3(totalWidth, 0, 0);
        }
    }
}
