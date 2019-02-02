using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;

public class GuestAI : MonoBehaviour
{
    NavMeshAgent _agent;
    NavMeshAgent agent { get { if (!_agent) _agent = GetComponent<NavMeshAgent>(); return _agent; } }

    public NavMeshObstacle obstacle;

    public Transform exitDestination;
    public Transform zitoDestination;
    public Transform seatDestination;

    void Start()
    {
        GoToSeat();
    }

    public void GoToSeat()
    {
        obstacle.enabled = false;
        agent.enabled = true;

        agent.SetDestination(seatDestination.position);
    }

    public void GoToExit()
    {
        obstacle.enabled = false;
        agent.enabled = true;

        agent.SetDestination(seatDestination.position);
    }

    public void Stop()
    {
        agent.enabled = false;
        obstacle.enabled = true;
    }

    public float VelocityX()
    {
        Debug.DrawLine(
            transform.position,
            transform.position + agent.desiredVelocity.normalized, Color.red);

        Vector3 v = Camera.main.transform.InverseTransformDirection(agent.desiredVelocity.normalized);

        Debug.Log(v.x);
        return v.x;
    }

    void Update()
    {
        Debug.DrawLine(transform.position, agent.destination);
    }
}
