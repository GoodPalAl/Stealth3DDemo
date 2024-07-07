/*
 * Al A.
 * Summer 2020 (c)
 */
using UnityEngine;

/// <summary>
/// A point in the game space that draws an editor-visible wireframe around itself.
/// Enemies with movement AI will move to a specified waypoint.
/// </summary>
public class Waypoints : MonoBehaviour
{
    /// <summary>
    /// Radius of wireframe.
    /// </summary>
    [SerializeField]
    float debugDrawRadius = 0.5f;
    [SerializeField]
    Color wireframeColor = Color.white;

    public virtual void OnDrawGizmos()
    {
        Gizmos.color = wireframeColor;
        Gizmos.DrawWireSphere(transform.position, debugDrawRadius);
    }
}
