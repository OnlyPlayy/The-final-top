using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathPoints : MonoBehaviour
{
    public Color LineColour;
    public float CheckpointDistance = 50f;

    private List<Transform> Waypoints = new List<Transform>();

    void OnDrawGizmosSelected()
    {
        Gizmos.color = LineColour;

        Transform[] PathTransforms = GetComponentsInChildren<Transform>();
        Waypoints = new List<Transform>();

        for (int i = 0; i < PathTransforms.Length; i++)
        {
            if (PathTransforms[i] != transform)
            {
                Waypoints.Add(PathTransforms[i]);
            }
        }
        for (int i = 0; i < Waypoints.Count; i++)
        {
            Vector3 currentWaypoint = Waypoints[i].position;
            Vector3 previousWaypoint = Vector3.zero;
            if (i > 0)
            {
                previousWaypoint = Waypoints[i - 1].position;
            }
            else if (i == 0 && Waypoints.Count > 1)
            {
                previousWaypoint = Waypoints[Waypoints.Count - 1].position;
            }
            Gizmos.DrawLine(previousWaypoint, currentWaypoint);
            Gizmos.DrawWireSphere(currentWaypoint, CheckpointDistance);
        }
    }

}
