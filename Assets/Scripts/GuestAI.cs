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

    public bool Fullstopped { get; private set; }

    void Start()
    {
        GoToZito();
    }

    public void GoToZito()
    {
        StartMoving(zitoDestination);
    }

    public void GoToSeat()
    {
        StartMoving(seatDestination);
    }

    public void GoToExit()
    {
        StartMoving(exitDestination);
    }

    public void Stop()
    {
        agent.isStopped = true;
    }

    public float VelocityX()
    {
        Debug.DrawLine(
            transform.position,
            transform.position + agent.desiredVelocity.normalized, Color.red);

        Vector3 v = Camera.main.transform.InverseTransformDirection(agent.desiredVelocity.normalized);
        
        return v.x;
    }

    void StartMoving(Transform to)
    {
        obstacle.enabled = false;
        StartCoroutine(SkipFrameAndStartMoving(to));
    }

    IEnumerator SkipFrameAndStartMoving(Transform to)
    {
        yield return null;
        agent.enabled = true;
        agent.isStopped = false;
        Fullstopped = false;

        agent.SetDestination(to.position);
    }

    void FullStop()
    {
        agent.isStopped = true;
        agent.enabled = false;
        obstacle.enabled = true;
        Fullstopped = true;
    }

    void Update()
    {
        if (!Application.isPlaying || GameController.I.IsPaused)
            return;

        Debug.DrawLine(transform.position, agent.destination);

        if (!Fullstopped && agent.isStopped && agent.velocity.sqrMagnitude < 0.01f)
        {
            FullStop();
        }
    }
}
