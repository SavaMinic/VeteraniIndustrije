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
        GoToZito();
    }

    public void GoToZito()
    {
        StartMoving();

        agent.SetDestination(zitoDestination.position);
    }

    public void GoToSeat()
    {
        StartMoving();

        agent.SetDestination(seatDestination.position);
    }

    public void GoToExit()
    {
        StartMoving();
        agent.SetDestination(exitDestination.position);
    }

    public void Stop()
    {
        agent.isStopped = true;
    }

    public void FullStop()
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

    void StartMoving()
    {
        obstacle.enabled = false;
        agent.enabled = true;
        agent.isStopped = false;
    }

    void Update()
    {
        Debug.DrawLine(transform.position, agent.destination);
    }
}
