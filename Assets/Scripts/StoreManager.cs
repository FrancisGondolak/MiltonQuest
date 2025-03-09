using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class StoreManager : MonoBehaviour
{
    public InventoryManager inventoryManager;  //referencia pública, una instancia, del InventoryManager (para poder usar los métodos de ese script) Se agrega en el Inspector
    public GameObject storeUI;  //referencia al canvas de la tienda que contiene todos los objetos y botones
    public float interactionRange = 3f;  //distancia a la que el jugador puede interactuar con el vendedor
    private bool isPlayerInRange = false; //indica si el jugador está cerca del vendedor
    private GameObject player;  //referencia al jugador para detectar la proximidad

    // Lista de objetos disponibles en la tienda y sus precios
    public List<StoreItem> storeItems = new List<StoreItem>();
    private int currentItemIndex = 0; // Índice del objeto actualmente seleccionado

    void Start()
    {
        //asegura que la tienda está oculta al principio
        storeUI.SetActive(false);
        player = GameObject.FindGameObjectWithTag("Player");  //localiza el objeto con el Tag "Player" (Milton)
        UpdateSelectionHighlight();
    }

    void Update()
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

        //si el jugador está cerca y pulsa la tecla "Enter", abrir o cerrar la tienda
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.Return))
        {
            ToggleStore();
        }

        // Navegación por los objetos usando las teclas WASD o las flechas
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

            // Comprar el objeto seleccionado con la tecla Q
            if (Input.GetKeyDown(KeyCode.Q))
            {
                TryBuyItem();
            }
        }
    }

    // Método para movernos entre los objetos
    void NavigateItems(int direction)
    {
        // Cambiar el índice del objeto seleccionado
        currentItemIndex = Mathf.Clamp(currentItemIndex + direction, 0, storeItems.Count - 1);

        // Resaltar el objeto seleccionado
        UpdateSelectionHighlight();
    }

    // Resalta el objeto seleccionado con un borde
    void UpdateSelectionHighlight()
    {
        for (int i = 0; i < storeItems.Count; i++)
        {
            if (i == currentItemIndex)
            {
                storeItems[i].Highlight(true); // Resalta el objeto seleccionado
            }
            else
            {
                storeItems[i].Highlight(false); // Desactiva el resaltado en los otros objetos
            }
        }
    }

    //método para abrir o cerrar la tienda
    void ToggleStore()
    {
        storeUI.SetActive(!storeUI.activeSelf);  //cambia el estado de la tienda (activa/desactiva)
        Time.timeScale = storeUI.activeSelf ? 0 : 1; //pausa el juego cuando la tienda está abierta, y lo reanuda cuando se cierra
    }

    // Método corregido para intentar comprar el objeto seleccionado
    public void TryBuyItem()
    {
        if (storeItems.Count > 0 && currentItemIndex >= 0 && currentItemIndex < storeItems.Count)
        {
            // Obtén el objeto seleccionado
            StoreItem selectedItem = storeItems[currentItemIndex];

            // Verifica si el InventoryManager está asignado
            if (inventoryManager != null)
            {
                bool purchased = inventoryManager.BuyItem(selectedItem.itemType, selectedItem.price);
                if (purchased)
                {
                    Debug.Log("Compra exitosa!");
                }
            }
            else
            {
                Debug.LogError("No se encontró una referencia de InventoryManager!");
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
            // Hacer el objeto completamente opaco (sin transparencia)
            color.a = 1f; // 1 significa opaco
        }
        else
        {
            // Hacer el objeto más transparente (con menos opacidad)
            color.a = 0.5f; // 0.5 significa medio transparente
        }

        // Asigna el nuevo color con la transparencia ajustada
        itemImage.color = color;
    }
}
