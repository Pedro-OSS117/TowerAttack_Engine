using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("UI Components")]
    public Button startButton;
    public GameObject contentAllEntities;
    public GameObject contentPlayerDeck;

    [Header("DATA")]
    public Deck allEntitiesDeck, playerDeck;

    [SerializeField]
    private List<EntityData> m_CurrentSelectedEntities;

    [Range(4, 10)]
    public int nbrDeckCard = 8;

    public void Awake()
    {
        m_CurrentSelectedEntities = new List<EntityData>();

        // Recuperation de l'instance de toggle
        Toggle toggle = contentAllEntities.GetComponentInChildren<Toggle>();
        // Set de la fonction associée à l'event de click sur le toggle
        SetToggleButton(toggle);
        // Set du content de l'image et du text du gameobject du toggle
        SetEntityContentUI(toggle.gameObject, allEntitiesDeck.allEntities[0]);

        // Parcours des entités pour creation d'un toggle pour chaque entité
        for (int i = 1; i < allEntitiesDeck.allEntities.Count; i++)
        {
            // Creation d'une nouveau toggle pour l'entité courante
            Toggle newToggle = Instantiate(toggle, contentAllEntities.transform);
            // Set de la fonction associée à l'event de click sur le toggle
            SetToggleButton(newToggle);
            // Set du content de l'image et du text du gameobject du toggle
            SetEntityContentUI(newToggle.gameObject, allEntitiesDeck.allEntities[i]);
        }

        Image playerCard = contentPlayerDeck.GetComponentInChildren<Image>();
        playerCard.gameObject.SetActive(false); 
        for (int i = 1; i < nbrDeckCard; i++)
        {
            Image newPlayerCard = Instantiate(playerCard, contentPlayerDeck.transform);
            newPlayerCard.gameObject.SetActive(false);
        }

        startButton.onClick.AddListener(StartGame);
    }

    private void SetToggleButton(Toggle toggle)
    {
        toggle.onValueChanged.AddListener(SelectEntity);
    }

    private void SetEntityContentUI(GameObject toSet, EntityData data)
    {
        Image img = toSet.GetComponent<Image>();
        if (img != null)
        {
            img.color = data.debugColor;
        }

        Text text = toSet.GetComponentInChildren<Text>();
        if(text != null)
        {
            text.text = data.name;
        }
    }

    private void SelectEntity(bool arg0)
    {
        m_CurrentSelectedEntities.Clear();

        for(int i = 0; i < contentAllEntities.transform.childCount 
            && m_CurrentSelectedEntities.Count < 8; i++)
        {
            Transform child = contentAllEntities.transform.GetChild(i);
            if(child.GetComponent<Toggle>().isOn)
            {
                m_CurrentSelectedEntities.Add(allEntitiesDeck.allEntities[i]);
            }
        }

        // Parcourir les 8 card
        // Les activés en fonction de la liste m_CurrentSelectedEntities
        // Mettre a jour les données de vue (name et color)
        for(int i = 0; i < contentPlayerDeck.transform.childCount; i++)
        {
            Transform child = contentPlayerDeck.transform.GetChild(i);
            if (i < m_CurrentSelectedEntities.Count)
            {
                child.gameObject.SetActive(true);
                SetEntityContentUI(child.gameObject, m_CurrentSelectedEntities[i]);
            }
            else
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    private void StartGame()
    {
        Debug.Log("========= StartGame ===========");
        playerDeck.allEntities = m_CurrentSelectedEntities;
        SceneManager.LoadScene("Level1");
    }
}
