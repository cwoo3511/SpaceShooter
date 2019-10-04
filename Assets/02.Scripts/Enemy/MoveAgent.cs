using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// NavMeshAgent 컴퍼넌트가 없으면 실행이 되지 않음
[RequireComponent(typeof(NavMeshAgent))]
public class MoveAgent : MonoBehaviour
{
    public List<Transform> wayPoints;
    public int nextIdx = 0;

    private NavMeshAgent agent;
    private Transform enemyTr;
    private readonly float patrolSpeed = 1.5f;
    private readonly float traceSpeed = 4.0f;
    private float damping = 1.0f;

    private bool patrolling;
    public bool Patrolling //patrolling 의 속성(property)
    {
        get { return patrolling; }
        set
        {
            patrolling = value;
            if (patrolling)
            {
                agent.speed = patrolSpeed;
                damping = 1.0f;
                MoveWayPoint();
            }
        }
    }

    private Vector3 traceTarget;

    public Vector3 TraceTarget
    {
        get { return traceTarget; }
        set
        {
            traceTarget = value;
            agent.speed = traceSpeed;
            damping = 7.0f;
            TargetTrace(traceTarget);
        }
    }

    public float Speed
    {
        get { return agent.velocity.magnitude; }
    }

    void Start()
    {
        enemyTr = GetComponent<Transform>();
        agent = GetComponent<NavMeshAgent>();
        //목적지 접근 시 자동으로 감속하는 기능 정지
        agent.autoBraking = false;
        //자동으로 회전하는 기능 정지
        agent.updateRotation = false;
        var group = GameObject.Find("WayPointGroup");
        if (group != null)
        {
            group.GetComponentsInChildren<Transform>(wayPoints);
            wayPoints.RemoveAt(0);
            nextIdx = Random.Range(0, wayPoints.Count);
        }
        //MoveWayPoint();
        this.Patrolling = true;
    }

    private void MoveWayPoint()
    {
        //최단거리 경로 계산이 끝나지 않으면 true 값 반환
        if (agent.isPathStale) return;
        agent.destination = wayPoints[nextIdx].position;
        agent.isStopped = false; //NavMeshAgent의 이동을 활성화
    }

    private void TargetTrace(Vector3 pos)
    {
        if (agent.isPathStale) return;
        agent.destination = pos;
        agent.isStopped = false;
    }

    public void Stop()
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        Patrolling = false;
    }

    void Update()
    {
        if (agent.isStopped == false)
        {
            Quaternion rot = Quaternion.LookRotation(agent
                .desiredVelocity);
            enemyTr.rotation = Quaternion.Slerp(enemyTr.rotation
                , rot, Time.deltaTime * damping);
        }
        if (!Patrolling) return;
        if (agent.velocity.sqrMagnitude >= (0.2f * 0.2f) 
            && agent.remainingDistance <= 0.5f)
        {
            //nextIdx = ++nextIdx % wayPoints.Count;
            nextIdx = Random.Range(0, wayPoints.Count);
            MoveWayPoint();
        }
    }
}
