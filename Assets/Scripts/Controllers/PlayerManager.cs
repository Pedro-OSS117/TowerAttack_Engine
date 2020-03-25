using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    MapManager m_Map = null;
    EntityManager m_Entity = null;

    private void Awake()
    {
        m_Map = FindObjectOfType<MapManager>();
        m_Entity = FindObjectOfType<EntityManager>();
    }
}
