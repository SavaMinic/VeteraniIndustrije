using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;

public class GuestAI : MonoBehaviour
{
    NavMeshAgent _agent;
    NavMeshAgent agent { get { if (!_agent) _agent = GetComponent<NavMeshAgent>(); return _agent; } }

    public bool ReachedDestination => agent.isStopped;

    public Transform exitDestination;
    public Transform zitoDestination;
    public Transform seatDestination;

    void Start()
    {
        GotoSeat();
    }

    void GotoSeat()
    {
        agent.SetDestination(seatDestination.position);
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
        Debug.DrawRay(transform.position, Vector3.up, ReachedDestination ? Color.yellow : Color.blue);
    }
}
