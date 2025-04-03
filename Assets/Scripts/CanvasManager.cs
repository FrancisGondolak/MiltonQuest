using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject historyMenu;
    public GameObject controlsMenu;
    public AudioClip gameMusic;

    void Start()
    {
        //mostrar el men� principal al inicio
        mainMenu.SetActive(true);
        historyMenu.SetActive(false);
        controlsMenu.SetActive(false);
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
        AudioManager.Instance.PlayMusic(gameMusic);
        SceneManager.LoadScene("GameScene");
    }

    //m�todo para salir del juego
    public void QuitGame()
    {
        Application.Quit();
    }
}
