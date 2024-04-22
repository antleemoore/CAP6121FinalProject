using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    private void Awake()
    {
        RedirectionManager m = FindObjectOfType<RedirectionManager>();
        Dropdown RDWSelection = GameObject.FindGameObjectWithTag("RDWDropdown").GetComponent<Dropdown>();
        RDWSelection.onValueChanged.AddListener(m.SelectRDW);
    }

    public static void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public static void QuitGame()
    {
        //If we are running in a standalone build of the game
        #if UNITY_STANDALONE
        //Quit the application
        Application.Quit();
        #endif

        //If we are running in the editor
        #if UNITY_EDITOR
        //Stop playing the scene
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public void HideMenu()
    {
        gameObject.SetActive(false);
    }

    public void ShowMenu()
    {
        gameObject.SetActive(true);
    }
    
}
