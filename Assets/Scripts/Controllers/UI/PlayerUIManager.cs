﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    public GameObject pauseBackground;

    public Text staminaLabel;
    public Text timerLabel;

    public GameObject dropButtonContainer;

    private PlayerManager m_PlayerManager;

    public void Start()
    {
        // Recuperation PlayerManager
        m_PlayerManager = FindObjectOfType<PlayerManager>();
        if (m_PlayerManager)
        {
            // Recuperation bouton existant
            PopButton button = dropButtonContainer.GetComponentInChildren<PopButton>();
            // Set bouton existant avec première entity
            InitPopButton(button, 0, m_PlayerManager.deck.allEntities[0]);
            
            // Creation d'autant de bouton que d'entity en plus dans le deck
            for (int i = 1; i < m_PlayerManager.deck.allEntities.Count; i++)
            {
                PopButton newButton = Instantiate(button, dropButtonContainer.transform);
                InitPopButton(newButton, i, m_PlayerManager.deck.allEntities[i]);
            }
        }
    }

    private void InitPopButton(PopButton button, int index, EntityData data)
    {
        // Save index de l'entité qu'on droppera
        button.index = index;

        // Connexion au event du bouton
        button.OnBeginDragEvent += m_PlayerManager.OnStartDrag;
        button.OnDragEvent += m_PlayerManager.OnDrag;
        button.OnEndDragEvent += m_PlayerManager.OnDrop;

        // Set du text du bouton
        Text text = button.GetComponentInChildren<Text>();
        text.text = data.name + "\n" + data.popAmount;

        // Set de la couleur du bouton
        Image image = button.GetComponent<Image>();
        image.color = data.debugColor;
    }

    public void Update()
    {
        UpdateStaminaContent();
    }

    private void UpdateStaminaContent()
    {
        if(staminaLabel != null)
        {
            int valStamina = (int)m_PlayerManager.GetCurrentStamina();
            if (m_PlayerManager != null)
            {
                staminaLabel.text = "S X " + valStamina;
            }

            PopButton[] popButtons = dropButtonContainer.GetComponentsInChildren<PopButton>();
            for (int i = 0; i < popButtons.Length; i++)
            {
                EntityData data = m_PlayerManager.deck.allEntities[i];
                popButtons[i].interactable = valStamina >= data.popAmount;
            }
        }
    }

    public void PauseGame()
    {
        LevelManager.Instance.PauseGame();

        if (pauseBackground)
        {
            pauseBackground.SetActive(!(Time.timeScale != 0));
        }
    }

    public void UdpateTimer(float currentTimeInSecond)
    {
        if (timerLabel)
        {
            float currentSecond = currentTimeInSecond % 60;
            timerLabel.text = ((int)(currentTimeInSecond / 60)).ToString("00");
            timerLabel.text += " : ";
            timerLabel.text += currentSecond.ToString("00");
        }
    }
}
