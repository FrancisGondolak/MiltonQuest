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

    //m�todo para ir al canvas de historia
    public void OpenPauseMenuCanvas()
    {
        pauseMenu.SetActive(true);
    }

    //M�todo para ir al canvas de controles
    public void OpenGameOverMenuCanvas()
    {
        gameOverMenu.SetActive(true);
    }

    //m�todo para reiniciar el juego
    public void ReloadGame()
    {
        gameOverMenu.SetActive(false);
        SceneManager.LoadScene("GameScene");
    }

    //m�todo para salir del juego
    public void QuitGame()
    {
        Application.Quit();
    }
}
