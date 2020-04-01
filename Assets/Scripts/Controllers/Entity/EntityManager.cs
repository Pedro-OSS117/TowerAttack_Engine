using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : SingletonMono<EntityManager>
{
    // Ref vers la global target des entités Player
    public Entity towerIA;
    // Ref vers la global target des entités IA
    public Entity towerPlayer;

    public List<Entity> outPostsPlayer;

    public List<Entity> outPostsIA;

    public Action<Alignment> OnTowerDestroy;
    public Action<bool> OnOutPostIADestroy;

    public void PopElementFromData(EntityData entityData, Vector3 position)
    {
        for(int i = 0; i < entityData.nbrEntityAtPop; i++)
        {
            GameObject newInstantiate = PoolManager.Instance.GetElement(entityData);
            if (newInstantiate != null)
            {
                SetPopElement(newInstantiate, position);
            }
            else
            {
                Debug.LogError("NO POOLED DATA PREFAB : " + entityData.name);
            }
        }
    }

    public void PopElementFromPrefab(GameObject prefabToPop, Vector3 position)
    {
        GameObject newInstantiate = PoolManager.Instance.GetElement(prefabToPop);
        if (newInstantiate != null)
        {
            SetPopElement(newInstantiate, position);
        }
        else
        {
            Debug.LogError("NO POOLED PREFAB : " + prefabToPop.name);
        }
    }


    // Fonction centrale.
    // Toute instantiation d'entité doit passer par cette fonction.
    // Elle centralise l'initialisation de l'entité.
    private void SetPopElement(GameObject newInstantiate, Vector3 position)
    {
        newInstantiate.transform.position = position;
        newInstantiate.SetActive(true);
        Entity entity = newInstantiate.GetComponent<Entity>();
        if (entity is EntityMoveable moveable)
        {
            moveable.SetGlobalTarget(GetGlobalTarget(position, moveable.entityData.alignment));
            entity.RestartEntity();
        }
    }

    public void PoolElement(GameObject toPool)
    {
        Entity entity = toPool.GetComponent<Entity>();
        if (towerPlayer == entity)
        {
            OnTowerDestroy?.Invoke(Alignment.Player);
        }
        else if (towerIA == entity)
        {
            OnTowerDestroy?.Invoke(Alignment.IA);
        }
        else if(outPostsIA.Contains(entity))
        {
            OnOutPostIADestroy(outPostsIA[0] == entity);
        }

        PoolManager.Instance.PoolElement(toPool);
    }

    public GameObject GetGlobalTarget(Vector3 position, Alignment alignment)
    {
        GameObject globalTarget = null;
        if (alignment == Alignment.IA)
        {
            globalTarget = GetGlobalTargetFromList(position, outPostsPlayer, towerPlayer);
        }
        else if (alignment == Alignment.Player)
        {
            globalTarget = GetGlobalTargetFromList(position, outPostsIA, towerIA);
        }
        if (globalTarget == null)
        {
            //Debug.LogError("No Target => End Game");
        }
        return globalTarget;
    }

    private GameObject GetGlobalTargetFromList(Vector3 position, List<Entity> outposts, Entity main)
    {
        GameObject target = null;
        foreach (Entity entity in outposts)
        {
            if (entity != null && entity.IsValidEntity())
            {
                if (target == null || (target != null
                    && (Vector3.Distance(target.transform.position, position) > Vector3.Distance(entity.transform.position, position))))
                {
                    target = entity.gameObject;
                }
            }
        }

        if (target == null && main != null && main.IsValidEntity())
        {
            target = main.gameObject;
        }
        return target;
    }
}
