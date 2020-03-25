using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : SingletonMono<EntityManager>
{
    public GameObject prefabEntityPlayer;
    public GameObject prefabEntityPlayerProjectile;
    public EntityData dataToInstantiate;

    // Ref vers la global target des entités Player
    public GameObject globalTargetPlayer;
    // Ref vers la global target des entités IA
    public GameObject globalTargetMonster;

    private Camera m_CurrentCamera;

    private void Awake()
    {
        m_CurrentCamera = FindObjectOfType<Camera>();

    }

    private void Update()
    {
        PopPlayerEntity();

        if(Input.GetKeyDown(KeyCode.Space))
        {
            // Test acces
            GameObject newInstantiate = PoolManager.Instance.GetElement(dataToInstantiate);
        }
    }

    // Fonction centrale.
    // Toute instantiation d'entité doit passer par cette fonction.
    // Elle centralise l'initialisation de l'entité.
    public void PopElement(GameObject prefabToPop, Vector3 position)
    {
        GameObject newInstantiate = PoolManager.Instance.GetElement(prefabToPop);
        newInstantiate.transform.position = position;
        newInstantiate.SetActive(true);
        if (newInstantiate != null)
        {
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
        else
        {
            Debug.LogError("NO POOLED PREFAB : " + prefabToPop.name);
        }
    }

    #region DEBUG PLAYER INPUT
    // Fonction de debug pour lancer les entités du players
    private void PopPlayerEntity()
    {
        // Creation d'un Ray à partir de la camera
        Ray ray = m_CurrentCamera.ScreenPointToRay(Input.mousePosition);
        float mult = 1000;
        Debug.DrawRay(ray.origin, ray.direction * mult, Color.green);

        // Recuperation du bouton droit de la souris.
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            // On recupère la bonne entité à instantier
            GameObject prefabToInstantiate = Input.GetMouseButtonDown(0) ? prefabEntityPlayer : prefabEntityPlayerProjectile;
            if (Physics.Raycast(ray, out RaycastHit hit, mult, LayerMask.GetMask("Default")))
            {
                // On recupère un élement depuis le poolmanager
                PopElement(prefabToInstantiate, hit.point);
            }
        }
    }
    #endregion DEBUG PLAYER INPUT
}
