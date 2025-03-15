using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    public GameObject menuPrincipal;
    public GameObject historiaCanvas;
    public GameObject controlesCanvas;
    public GameObject pausaCanvas;
    public GameObject gameOverCanvas;

    void Start()
    {
        //mostrar el Menú Inicial al principio del juego, los demás canvas que no se muestren
        menuPrincipal.SetActive(true);
        historiaCanvas.SetActive(false);
        controlesCanvas.SetActive(false);
        pausaCanvas.SetActive(false);
        gameOverCanvas.SetActive(false);
    }

    //método para ir al canvas de historia
    public void IrAHistoria()
    {
        menuPrincipal.SetActive(false);
        historiaCanvas.SetActive(true);
    }

    //método para ir al canvas de controles
    public void IrAControles()
    {
        historiaCanvas.SetActive(false);
        controlesCanvas.SetActive(true);
    }

    //método para comenzar la partida
    public void ComenzarJuego()
    {
        controlesCanvas.SetActive(false);
        SceneManager.LoadScene("JuegoScene");
    }

    //método para mostrar el menú de pausa
    public void MostrarPausa()
    {
        pausaCanvas.SetActive(true);
        Time.timeScale = 0;  //pausar el juego
    }

    //método para continuar el juego
    public void ContinuarJuego()
    {
        pausaCanvas.SetActive(false);
        Time.timeScale = 1;  //reanudar el juego
    }

    //método para ir al Menú Principal desde el de pausa
    public void VolverMenuPrincipal()
    {
        pausaCanvas.SetActive(false);
        menuPrincipal.SetActive(true);
        SceneManager.LoadScene("MainMenu");
    }

    //método para mostrar el menú de Game Over
    public void MostrarGameOver()
    {
        gameOverCanvas.SetActive(true);
    }

    //método para reiniciar el juego
    public void ReiniciarJuego()
    {
        gameOverCanvas.SetActive(false);
        SceneManager.LoadScene("JuegoScene");
    }

    //método para salir del juego
    public void SalirDelJuego()
    {
        Application.Quit();
    }
}
