using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_GameOver_01 : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] GameObject panel;
    [SerializeField] private string sceneName;
    private void Update()
    {
        panel.SetActive(player.IsDead());
        
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(sceneName);
    }
}
