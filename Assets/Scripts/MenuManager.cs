using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject gameOverMenu;

    private void Start()
    {
        pauseMenu.SetActive(false);
        gameOverMenu.SetActive(false);
    }

    //método para ir al canvas de historia
    public void OpenPauseMenuCanvas()
    {
        pauseMenu.SetActive(true);
    }

    //Método para ir al canvas de controles
    public void OpenGameOverMenuCanvas()
    {
        gameOverMenu.SetActive(true);
    }

    //método para reiniciar el juego
    public void ReloadGame()
    {
        gameOverMenu.SetActive(false);
        SceneManager.LoadScene("GameScene");
    }

    //método para salir del juego
    public void QuitGame()
    {
        Application.Quit();
    }
}
