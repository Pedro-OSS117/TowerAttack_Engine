using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private Camera m_CurrentCamera;
    private MapManager m_MapManager = null;
    private EntityManager m_EntityManager = null;

    [Header("Props Player Data")]
    public Deck deck;

    [Header("Props FeedBack Pos Drop")]
    public GameObject feedbackPosDrop;

    [Header("Debug Pop Entity")]
    public EntityData dataToInstantiate;

    private int m_CurrentIndex = -1;

    private void Awake()
    {
        m_MapManager = FindObjectOfType<MapManager>();
        m_EntityManager = FindObjectOfType<EntityManager>();
        m_CurrentCamera = FindObjectOfType<Camera>();
    }

    private void Start()
    {
        m_MapManager.DisplayDropFeedBack(false);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            UpdateCurrentPopEntityIndex();
        }

        PopPlayerEntity();
    }

    private void UpdateCurrentPopEntityIndex()
    {
        m_CurrentIndex++;
        if (m_CurrentIndex >= deck.allEntities.Count)
        {
            m_CurrentIndex = -1;
        }
        m_MapManager.DisplayDropFeedBack(m_CurrentIndex != -1);
        if(m_CurrentIndex == -1)
        {
            UnDisplayFBDrop();
        }
    }

    private void DisplayFBDrop(Vector3 pos, Color colorFB)
    {
        feedbackPosDrop.SetActive(true);
        feedbackPosDrop.GetComponent<MeshRenderer>().material.color = colorFB;  ;
        feedbackPosDrop.transform.position = pos; ;
    }

    private void UnDisplayFBDrop()
    {
        feedbackPosDrop.SetActive(false);
    }

    #region PLAYER INPUT
    // Fonction de debug pour lancer les entités du players
    private void PopPlayerEntity()
    {
        if(m_CurrentIndex != -1)
        {
            // Creation d'un Ray à partir de la camera
            Ray ray = m_CurrentCamera.ScreenPointToRay(Input.mousePosition);
            float mult = 1000;
            Debug.DrawRay(ray.origin, ray.direction * mult, Color.green);

            if (Physics.Raycast(ray, out RaycastHit hit, mult, LayerMask.GetMask("Default")))
            {
                // On verifie si on peut dropper à cette position de la map
                bool canDrop = m_MapManager.TestIfCanDropAtPos(hit.point);

                // On set le feedback en fonction de can drop
                DisplayFBDrop(new Vector3(hit.point.x, 0.4f, hit.point.z), canDrop ? Color.blue : Color.red);

                // Recuperation du bouton droit de la souris.
                if (canDrop && Input.GetMouseButtonDown(0))
                {
                    // On recupère un élement depuis le poolmanager
                    m_EntityManager.PopElementFromData(deck.allEntities[m_CurrentIndex], hit.point);
                }
            }
            else
            {
                // On desactive le feedback si on est pas sur la map
                UnDisplayFBDrop();
            }
        }
    }
    #endregion PLAYER INPUT
}
