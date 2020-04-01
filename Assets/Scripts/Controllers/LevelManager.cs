using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Classe Globale qui a la vue sur tous les autres managers.
/// Elle gère l'état global du niveau.
/// Notamment la game loop, l'état de Pause.
/// </summary>
public class LevelManager : SingletonMono<LevelManager>
{
    // Enum permettant de gérer l'état du level
    public enum LevelState
    {
        StartingGame, // le jeu n'a pas demarré (pas codé)
        InGame, // le jeu est en cours
        Pause, // le jeu est en pause
        EndGame // La partie est terminée
    }

    private MapManager m_MapManager;
    private PlayerUIManager m_PlayerUIManager;

    [Range(0f, 5f)]
    public float timeGameInMin = 3;
    private float m_CurrentTimeRemain = 0;

    // Etat courant du level
    private LevelState m_CurrentLevelState = LevelState.InGame;

    // Initilisation par rapport aux autres systèmes
    private void Start()
    {
        // Recupération pour connexion aux events
        EntityManager entityManager = FindObjectOfType<EntityManager>();
        if(entityManager != null)
        {
            entityManager.OnTowerDestroy += EndGame;
            entityManager.OnOutPostIADestroy += OnOutPostDestroy;
        }

        m_MapManager = FindObjectOfType<MapManager>();
        m_PlayerUIManager = FindObjectOfType<PlayerUIManager>();

        // Initialisation du timer, convretion en seconde
        m_CurrentTimeRemain = timeGameInMin * 60;
    }

    private void Update()
    {
        UpdateTimer();
    }
    
    private void UpdateTimer()
    {
        if(m_CurrentLevelState == LevelState.InGame)
        {
            if (m_CurrentTimeRemain > 0)
            {
                m_CurrentTimeRemain -= Time.deltaTime;
                if (m_CurrentTimeRemain < 0)
                {
                    m_CurrentTimeRemain = 0;
                }

                // Update de l'UI
                if (m_PlayerUIManager)
                {
                    m_PlayerUIManager.UdpateTimer(m_CurrentTimeRemain);
                }
            }

            // Si le timer est terminé
            if (m_CurrentTimeRemain <= 0)
            {
                // On appel End Game
                EndGame(Alignment.Player);
            }
        }
    }

    // Permet d'appeler le map manager pour
    // setter l'alignement de la zone de l'outpost.
    // La methode est très spécifique, il faudrait améliorer la généricité.
    //  => Rendre l'update de zone en fonction de la position de l'outpost.
    private void OnOutPostDestroy(bool isLeft)
    {
        if(m_MapManager)
        {
            m_MapManager.SetAlignementTopRightOrLeftZone(Alignment.Player, isLeft);
        }
    }

    private void EndGame(Alignment alignment)
    {
        m_CurrentLevelState = LevelState.EndGame;

        switch(alignment)
        {
            case Alignment.Player:
                Debug.Log("LOOOOOOOOOOSE ! GAME OVER !");
                break;
            case Alignment.IA:
                Debug.Log("WIN ! YOU'RE THE BEST");
                break;
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void PauseGame()
    {
        bool isEnable = Time.timeScale != 0;
        Time.timeScale = isEnable ? 0 : 1;
        m_CurrentLevelState = Time.timeScale == 0 ? LevelState.Pause : LevelState.InGame;
    }
}
