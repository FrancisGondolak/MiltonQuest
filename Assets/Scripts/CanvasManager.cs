using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject historyMenu;
    public GameObject controlsMenu;
    public GameObject gameOverMenu;

    void Start()
    {
        //mostrar el men� principal al inicio
        mainMenu.SetActive(true);
        historyMenu.SetActive(false);
        controlsMenu.SetActive(false);
        gameOverMenu.SetActive(false);
    }

    //m�todo para ir al canvas de historia
    public void OpenHistoryCanvas()
    {
        mainMenu.SetActive(false);
        historyMenu.SetActive(true);
    }

    //M�todo para ir al canvas de controles
    public void OpenControlsCanvas()
    {
        historyMenu.SetActive(false);
        controlsMenu.SetActive(true);
    }

    //m�todo para comenzar el juego
    public void BeginGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    //m�todo para mostrar el canvas de Game Over
    public void OpenGameOverCanvas()
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
