using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class StoreManager : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioClip sfxBuyItem;
    public AudioClip sfxItemNavigate;
    public AudioClip storeMusic;
    public AudioClip gameMusic;

    [Header("Others")]
    public InventoryManager inventoryManager;  //referencia pública, una instancia, del InventoryManager (para poder usar los métodos de ese script) Se agrega en el Inspector
    public GameObject storeUI;  //referencia al canvas de la tienda que contiene todos los objetos y botones
    public float interactionRange = 6f;  //distancia a la que el jugador puede interactuar con el vendedor
    private bool isPlayerInRange = false; //indica si el jugador está cerca del vendedor
    private GameObject player;  //referencia al jugador para detectar la proximidad
    public GameObject interactionHintHolder; //bocadillo de la tienda (con botón E, para indicar cómo interactuar con ella)
    public MiltonLogic milton; //acceder a Milton para no poder disparar agua si está abierta la tienda


    //lista de objetos disponibles en la tienda y sus precios
    public List<StoreItem> storeItems = new List<StoreItem>();
    private int currentItemIndex = 0; //índice del objeto actualmente seleccionado, el 0 de entrada

    private void Start()
    {
        //asegura que la tienda está oculta al principio
        storeUI.SetActive(false);
        player = GameObject.FindGameObjectWithTag("Player");  //localiza el objeto con el Tag "Player" (Milton)
        interactionHintHolder.SetActive(false);  //ocultar bocadillo al iniciar
        UpdateSelectionHighlight();
    }

    private void Update()
    {
        //comprobar si el jugador está dentro del rango de interacción
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (distanceToPlayer <= interactionRange)
        {
            isPlayerInRange = true;
        }
        else
        {
            isPlayerInRange = false;
        }

        //verifica si el jugador está cerca y si la tienda está abierta o cerrada para mostrar el bocadillo del tendero
        if (isPlayerInRange && !storeUI.activeSelf)
        {
            interactionHintHolder.SetActive(true); //muestra el bocadillo
        }
        else
        {
            interactionHintHolder.SetActive(false); //pculta el bocadillo
        }

        //si el jugador está cerca y pulsa la tecla "Enter", abrir o cerrar la tienda
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            ToggleStore();
        }

        //navegación por los objetos usando las teclas WASD o las flechas
        if (storeUI.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                NavigateItems(-1);
            }
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                NavigateItems(1);
            }

            //comprar el objeto seleccionado con la tecla Q
            if (Input.GetKeyDown(KeyCode.Q))
            {
                TryBuyItem();
            }
        }
    }

    //método para moverse entre los objetos de la tienda
    private void NavigateItems(int direction)
    {
        AudioManager.Instance.PlayLouderSFX(sfxItemNavigate);

        //cambiar el índice del objeto seleccionado
        currentItemIndex = Mathf.Clamp(currentItemIndex + direction, 0, storeItems.Count - 1);

        //resaltar el objeto seleccionado
        UpdateSelectionHighlight();
    }

    //método para resaltar el objeto seleccionado
    private void UpdateSelectionHighlight()
    {
        for (int i = 0; i < storeItems.Count; i++)
        {
            if (i == currentItemIndex)
            {
                storeItems[i].Highlight(true); //resaltar el objeto seleccionado
            }
            else
            {
                storeItems[i].Highlight(false); //desactivar el resaltado en los otros objetos
            }
        }
    }

    //método para abrir o cerrar la tienda
    private void ToggleStore()
    {
        bool isStoreOpen =!storeUI.activeSelf;  //almacenamos en el booleano el estado de la tienda (true o false, dependiendo de si está activa o no)
        storeUI.SetActive(isStoreOpen);  //cambia el estado de la tienda (activa/desactiva)

        Time.timeScale = isStoreOpen ? 0 : 1;  //pausa el juego cuando la tienda está abierta, y lo reanuda cuando se cierra

        //cambia la música y pausa el juego dependiendo de si la tienda está abierta o cerrada
        if (isStoreOpen)
        {
            milton.gamePaused = true;
            AudioManager.Instance.PlayMusic(storeMusic);  //música de la tienda
        }
        else
        {
            milton.gamePaused = false;
            AudioManager.Instance.PlayMusic(gameMusic);  //música del juego
        }
    }

    //método para intentar comprar el objeto seleccionado
    public void TryBuyItem()
    {
        if (storeItems.Count > 0 && currentItemIndex >= 0 && currentItemIndex < storeItems.Count)
        {
            //obtener el objeto seleccionado
            StoreItem selectedItem = storeItems[currentItemIndex];

            //verificar si el InventoryManager está asignado
            if (inventoryManager != null)
            {
                bool purchased = inventoryManager.BuyItem(selectedItem.itemType, selectedItem.price);
                if (purchased)
                {
                    AudioManager.Instance.PlayLouderSFX(sfxBuyItem);
                    inventoryManager.ShowMessage("Objeto comprado");
                }
            }
            else
            {
                inventoryManager.ShowMessage("No hay objeto seleccionado");
            }
        }
    }

}

//clase para representar los objetos de la tienda
[System.Serializable]
public class StoreItem
{
    public string itemType;  //tipo de objeto (por ejemplo, "WaterBottle")
    public int price;        //precio del objeto
    public Image itemImage;  //imagen que representa el objeto

    //método para resaltar el objeto (añadir o quitar borde)
    public void Highlight(bool highlight)
    {
        Color color = itemImage.color;  //obtiene el color actual de la imagen
        if (highlight)
        {
            //hacer el objeto completamente opaco (sin transparencia)
            color.a = 1f; //1 significa opaco
        }
        else
        {
            //hacer el objeto más transparente (con menos opacidad)
            color.a = 0.5f; //0.5 significa medio transparente
        }

        //asigna el nuevo color con la transparencia ajustada
        itemImage.color = color;
    }
}
