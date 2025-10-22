using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WaypointWalker : MonoBehaviour
{
    public List<Transform> waypoints;
    public int currentWaypointIndex = 0;
    public float stoppingDistance = 2f;
    public bool loop = false;
    public NavMeshAgent agent;

    public void StartWalking()
    {
        if (waypoints == null || agent == null || waypoints.Count == 0)
        {
            Debug.LogError("No waypoints assigned!");
            return;
        }

        if (agent == null)
        {
            Debug.LogError("NavMeshAgent is missing!");
            return;
        }

        agent.SetDestination(waypoints[currentWaypointIndex].position);
    }

    public void GoToNextWaypoint()
    {
        currentWaypointIndex++;

        if (currentWaypointIndex >= waypoints.Count)
        {
            Debug.Log("No more waypoints for " + gameObject.name);
            return;
        }

        StartWalking();
    }

    public bool HasReachedDestination()
    {
        if (agent == null) return false;

        return !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance;
    }

}
