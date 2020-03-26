using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : SingletonMono<EntityManager>
{
    // Ref vers la global target des entités Player
    public GameObject globalTargetPlayer;
    // Ref vers la global target des entités IA
    public GameObject globalTargetMonster;
    
    public void PopElementFromData(EntityData entityData, Vector3 position)
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
            if (moveable.entityData.alignment == Alignment.IA)
            {
                moveable.SetGlobalTarget(globalTargetMonster);
            }
            else if (moveable.entityData.alignment == Alignment.Player)
            {
                moveable.SetGlobalTarget(globalTargetPlayer);
            }
            entity.RestartEntity();
        }
    }
}
