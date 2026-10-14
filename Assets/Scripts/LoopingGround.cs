using UnityEngine;

public class LoopingGround : MonoBehaviour
{
    [Tooltip("Reference to the paper plane")]
    public Transform plane;

    [Tooltip("The width of a single ground tile")]
    public float groundWidth = 6.4f;

    [Tooltip("Total number of ground tiles in the scene")]
    public int totalGrounds = 10;

    private float totalWidth;
    private float halfTotalWidth;

    void Start()
    {
        // Pre-calculate the total width and half width to avoid redundant calculations in Update
        totalWidth = groundWidth * totalGrounds;
        halfTotalWidth = totalWidth / 2f;
    }

    // Use LateUpdate to ensure the ground moves after the plane's physics update, preventing visual jitter
    void LateUpdate()
    {
        if (plane == null) return;

        // Calculate the X-axis distance between the plane and this ground tile
        float distanceToPlane = plane.position.x - transform.position.x;

        // Case 1: The plane flies forward, and this tile is left far behind (exceeds half the total width)
        if (distanceToPlane > halfTotalWidth)
        {
            // Instantly move the tile to the front
            transform.position += new Vector3(totalWidth, 0, 0);
        }
        // Case 2: The plane flies backward, and this tile is too far ahead
        else if (distanceToPlane < -halfTotalWidth)
        {
            // Instantly move the tile to the back
            transform.position -= new Vector3(totalWidth, 0, 0);
        }
    }
}