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

    void Update()
    {
        Debug.DrawLine(transform.position, agent.destination);
        Debug.DrawRay(transform.position, Vector3.up, ReachedDestination ? Color.yellow : Color.blue);
    }
}
