using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject gameOverMenu;
    public MiltonLogic milton;

    [Header("Audio Sources")]
    public AudioClip gameMusic;
    public AudioClip pauseMusic;

    private void Start()
    {
        pauseMenu.SetActive(false);
        gameOverMenu.SetActive(false);
    }

    //m�todo para abrir el men� de pausa
    public void OpenPauseMenuCanvas()
    {
        
        milton.GamePaused = true;
        pauseMenu.SetActive(true);
        AudioManager.Instance.PlayMusic(pauseMusic);
        Time.timeScale = 0;
    }

    //m�todo para cerrar el men� de pausa
    public void ClosePauseMenuCanvas()
    {
        milton.GamePaused = false;
        pauseMenu.SetActive(false);
        AudioManager.Instance.PlayMusic(gameMusic);
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
        BasicMudlerMovement.enemiesDefeated = 0;
        AudioManager.Instance.PlayMusic(gameMusic);
        SceneManager.LoadScene("GameScene");
    }

    //m�todo para salir del juego
    public void QuitGame()
    {
        Application.Quit();
    }
}
