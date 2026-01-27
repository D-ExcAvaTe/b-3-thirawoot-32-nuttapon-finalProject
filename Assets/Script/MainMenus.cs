using UnityEngine;
using UnityEngine.SceneManagement; 

public class MainMenus : MonoBehaviour
{

    public GameObject playerAidPanel;
    
    public void StartGame()
    {
        
        SceneManager.LoadScene("test-feedback");
    }

    public void ExitGame()
    {
        Debug.Log("Player has quit the game");
        Application.Quit();
    }

    public void openPlayerAIDS()
    {
        playerAidPanel.SetActive(true);
    }
    public void closePlayerAIDS()
    {
        playerAidPanel.SetActive(false);
    }
}