﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EntityMoveable : Entity
{
    [Header("Move Props")]
    [Range(1, 50)]
    public float moveSpeed = 1;

    [Header("Target")]
    // Variable target
    public GameObject globalTarget;
    
    [Header("Stop Time")]
    // Variable de temps d'arret
    public float timeWaitBeforeMove = 1;
    private float m_CurrentTimeBeforeNextMove = 0;

    [Header("Go To Target")]
    public int rangeToDoAttack = 1;
    private Entity m_CurrentMoveToTarget = null;

    private NavMeshAgent m_NavMeshAgent;

    // Initialisation - Construction de l'entité
    public override void InitEntity()
    {
        // Initialisation - Construction
        m_NavMeshAgent = GetComponent<NavMeshAgent>();

        base.InitEntity();
    }

    public override void RestartEntity()
    {
        base.RestartEntity();

        // Set/Restart properties
        m_NavMeshAgent.speed = moveSpeed;
        SetGlobalDestination();
        m_CurrentMoveToTarget = null;
    }

    #region GLOBAL TARGET
    public void SetGlobalTarget(GameObject target)
    {
        globalTarget = target;
        SetGlobalDestination();
    }

    private void SetGlobalDestination()
    {
        if (globalTarget)
        {
            m_NavMeshAgent.SetDestination(globalTarget.transform.position);
        }
    }

    private void ResetToGlobalDestination()
    {
        m_CurrentMoveToTarget = null;
        SetGlobalDestination();
    }
    #endregion

    public override void Update()
    {
        base.Update();

        UpdateMoveToTarget();

        UpdateStopState();
    }

    #region ATTACK
    protected override bool DoAttack(Entity targetEntity)
    {
        // On verifie si le range To Do attack est valide
        if(rangeToDoAttack < rangeDetect)
        {
            // On test si on est assez prêt
            if(!(Vector3.Distance(targetEntity.transform.position, transform.position) <= rangeToDoAttack))
            {
                // Si non on save la target pour bouger vers la target
                m_CurrentMoveToTarget = targetEntity;
                m_NavMeshAgent.SetDestination(targetEntity.transform.position);
                // On sort de la fonction pour empecher l'attack
                return false;
            }
        }
        if(base.DoAttack(targetEntity))
        {
            m_NavMeshAgent.isStopped = true;
            m_CurrentTimeBeforeNextMove = 0;
            return true;
        }
        return false;
    }
    #endregion ATTACK

    #region MOVE
    void UpdateStopState()
    {
        if (m_NavMeshAgent.isStopped)
        {
            if (m_CurrentTimeBeforeNextMove < timeWaitBeforeMove)
            {
                m_CurrentTimeBeforeNextMove += Time.deltaTime;
            }
            else
            {
                m_NavMeshAgent.isStopped = false;
                ResetToGlobalDestination();
            }
        }
    }

    void UpdateMoveToTarget()
    {
        if (m_CurrentMoveToTarget != null)
        {
            // On test si la cible est valide
            if(m_CurrentMoveToTarget.IsValidEntity())
            {
                // On test si on est assez proche
                if (Vector3.Distance(m_CurrentMoveToTarget.transform.position, transform.position) <= rangeToDoAttack)
                {
                    // Si oui on fait l'attack
                    DoAttack(m_CurrentMoveToTarget);

                    // On reset pour que l'entité reparte sur la destination global
                    ResetToGlobalDestination();
                }
                else
                {
                    // On continue à setter la destination car elle peut avoir bougé
                    m_NavMeshAgent.SetDestination(m_CurrentMoveToTarget.transform.position);
                }
            }
            else
            {
                // Cas ou l'entité vers laquelle on se dirige n'est plus valide.
                ResetToGlobalDestination();
            }
        }
        else
        {
            ResetToGlobalDestination();
        }
    }
    #endregion MOVE
}
