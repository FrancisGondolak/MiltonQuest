using UnityEngine;

public class StoreManager : MonoBehaviour
{
    public GameObject storeUI;    //referencia al canvas de la tienda que contiene todos los objetos y botones
    public float interactionRange = 3f;  //distancia a la que el jugador puede interactuar con el vendedor
    private bool isPlayerInRange = false; //indica si el jugador est� cerca del vendedor
    private GameObject player;  //referencia al jugador para detectar la proximidad

    void Start()
    {
        //asegura que la tienda est� oculta al principio
        storeUI.SetActive(false);
        player = GameObject.FindGameObjectWithTag("Player");  //localiza el objeto con el Tag "Player" (Milton)
    }

    void Update()
    {
        //comprobar si el jugador est� dentro del rango de interacci�n
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (distanceToPlayer <= interactionRange)
        {
            isPlayerInRange = true;
        }
        else
        {
            isPlayerInRange = false;
        }

        //si el jugador est� cerca y pulsa la tecla "Enter", abrir o cerrar la tienda
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.Return))
        {
            ToggleStore();
        }
    }

    //m�todo para abrir o cerrar la tienda
    void ToggleStore()
    {
        storeUI.SetActive(!storeUI.activeSelf);  //cambia el estado de la tienda (activa/desactiva)
        Time.timeScale = storeUI.activeSelf ? 0 : 1; //pausa el juego cuando la tienda est� abierta, y lo reanuda cuando se cierra
    }
}
