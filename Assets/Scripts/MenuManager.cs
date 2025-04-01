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

    //m�todo para abrir el men� de pausa
    public void OpenPauseMenuCanvas()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
    }

    //m�todo para cerrar el men� de pausa
    public void ClosePauseMenuCanvas()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }

    //M�todo para abrir el men� de game over
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
