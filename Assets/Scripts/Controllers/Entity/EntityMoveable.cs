using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EntityMoveable : Entity
{
    [Range(1, 50)]
    public float moveSpeed = 1;

    [Header("Target")]
    // Variable target
    public GameObject globalTarget;
    
    [Header("Stop Time")]
    // Variable de temps d'arret
    public float timeWaitBeforeMove = 1;
    private float m_CurrentTimeBeforeNextMove = 0;
    
    private NavMeshAgent m_NavMeshAgent;

    public override void Awake()
    {
        base.Awake();
        m_NavMeshAgent = GetComponent<NavMeshAgent>();
        m_NavMeshAgent.speed = moveSpeed;
        SetDestination();
    }

    public void SetGlobalTarget(GameObject target)
    {
        globalTarget = target;
        SetDestination();
    }

    private void SetDestination()
    {
        if (globalTarget)
        {
            m_NavMeshAgent.SetDestination(globalTarget.transform.position);
        }
    }

    public override void Update()
    {
        base.Update();
        if(m_NavMeshAgent.isStopped)
        {
            if (m_CurrentTimeBeforeNextMove < timeWaitBeforeMove)
            {
                m_CurrentTimeBeforeNextMove += Time.deltaTime;
            }
            else
            {
                m_NavMeshAgent.isStopped = false;
                SetDestination();
            }
        }
    }

    protected override bool DoAttack(Entity targetEntity)
    {
        if(base.DoAttack(targetEntity))
        {
            m_NavMeshAgent.isStopped = true;
            m_CurrentTimeBeforeNextMove = 0;
            return true;
        }
        return false;
    }
}
