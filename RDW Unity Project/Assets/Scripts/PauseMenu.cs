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
        Dropdown rdwSelection = null;//GameObject.FindGameObjectWithTag("RDWDropdown").GetComponent<Dropdown>();
        Dropdown[] dropdowns = gameObject.GetComponentsInChildren<Dropdown>();
        foreach (Dropdown d in dropdowns)
        {
            if (d.gameObject.CompareTag("RDWDropdown"))
            {
                rdwSelection = d;
                break; // in future could make env a dropdown and check here too
            }
        }
        if (rdwSelection is not null) rdwSelection.onValueChanged.AddListener(m.SelectRDW);
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
        //gameObject.SetActive(false);
    }

    public void ShowMenu()
    {
        gameObject.SetActive(true);
    }
    
}
