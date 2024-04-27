using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public TMP_Dropdown rdwSelection;
    private void Awake()
    {
        RedirectionManager m = FindObjectOfType<RedirectionManager>();
        if (rdwSelection is not null) rdwSelection.onValueChanged.AddListener(m.SelectRDW);
        else Debug.LogWarning("Failed to assign RDW Selection method");
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
