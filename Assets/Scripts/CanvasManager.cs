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
        //mostrar el menú principal al inicio
        mainMenu.SetActive(true);
        historyMenu.SetActive(false);
        controlsMenu.SetActive(false);
    }

    //método para ir al canvas de historia
    public void OpenHistoryCanvas()
    {
        mainMenu.SetActive(false);
        historyMenu.SetActive(true);
    }

    //Método para ir al canvas de controles
    public void OpenControlsCanvas()
    {
        historyMenu.SetActive(false);
        controlsMenu.SetActive(true);
    }

    //método para comenzar el juego
    public void BeginGame()
    {
        AudioManager.Instance.PlayMusic(gameMusic);
        SceneManager.LoadScene("GameScene");
    }

    //método para salir del juego
    public void QuitGame()
    {
        Application.Quit();
    }
}
