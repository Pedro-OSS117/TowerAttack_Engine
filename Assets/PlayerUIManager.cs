using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    public GameObject pauseBackground;

    public Text staminaLabel;

    public GameObject dropButtonContainer;

    private PlayerManager m_PlayerManager;

    public void Start()
    {
        m_PlayerManager = FindObjectOfType<PlayerManager>();
        PopButton button = dropButtonContainer.GetComponentInChildren<PopButton>();
        button.index = 0;
        for(int i = 1; i < m_PlayerManager.deck.allEntities.Count; i++)
        {
            PopButton newButton = Instantiate(button, dropButtonContainer.transform);
            newButton.index = i; 
        }
    }

    public void Update()
    {
        UpdateStaminaContent();
    }

    private void UpdateStaminaContent()
    {
        if(staminaLabel != null)
        {
            if (m_PlayerManager != null)
            {
                staminaLabel.text = "S X " + (int)m_PlayerManager.GetCurrentStamina();
            }
            //staminaLabel.text = "S X " + m_PlayerManager.GetCurrentStamina().ToString("00.00f");
        }
    }

    public void PauseGame()
    {
        bool isEnable = Time.timeScale != 0;
        Time.timeScale = isEnable ? 0 : 1;

        if(pauseBackground)
        {
            pauseBackground.SetActive(!(Time.timeScale != 0));
        }
    }
}
