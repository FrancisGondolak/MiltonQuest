using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Collections;

public class InventoryManager : MonoBehaviour
{
    public AudioClip sfxGetWater;

    public List<Image> slots = new List<Image>(); //Lista para el inventario
    public Sprite waterBottleSprite; //Icono para la botella de agua
    public Sprite appleHeartSprite;  //Icono para la coranzana
    public int coins = 50; //monedas iniciales del jugador
    public TextMeshProUGUI coinsText; //texto de las monedas que tenemos en el HUD
    public TextMeshProUGUI messageText; //texto para mostrar mensajes en pantalla
    public Image messageTextBackground; //imagen de fondo de los mensajes para el usuario
    public MiltonLogic milton; //variable para acceder al script de Milton y afectar a su vida al usar objetos 
    public WaterCounterUI waterCounter;//variable para acceder al script del contador de agua y aumentarlo al usar objetos
    public Image key;

    private List<Sprite> items = new List<Sprite>(); //Lista interna para los objetos del inventario
    public bool hasKey = false;//variable para controlar si tenemos la llave o no. La cambiamos desde el Script de Milton al recoger la llave o usarla en la puerta

    private void Start()
    {
        UpdateInventoryUI();
        messageText.text = ""; //asegurar que el mensaje de indicaciones está vacío al comenzar el juego
        messageTextBackground.gameObject.SetActive(false); //al principio del juego, el background del texto no está activo
    }

    private void Update()
    {
        //Detecta la pulsación de las teclas 1 al 5 para usar el item que se encuentra en ese espacio del inventario
        if (Input.GetKeyDown(KeyCode.Alpha1)) UseItemAt(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) UseItemAt(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) UseItemAt(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) UseItemAt(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) UseItemAt(4);

    }

    //Método que añade un objeto al inventario
    public bool AddItem(string itemType)
    {
        //Controlar qué icono agregamos al inventario
        Sprite itemSprite = null;
        if (itemType == "WaterBottle")
        {
            itemSprite = waterBottleSprite;
        }
        if (itemType == "AppleHeart")
        {
            itemSprite = appleHeartSprite;
        }

        if (itemSprite != null)
        {
            items.Add(itemSprite); 
            UpdateInventoryUI();  
            return true;
        }

        return false;
    }

    //método para consumir un objeto en una casilla específica
    public void UseItemAt(int index)
    {
        if (index < items.Count) //verifica si la casilla seleccionada tiene un objeto
        {
            Sprite usedItem = items[index]; //obtiene el objeto que se ha usado

            //comprueba si es una corazana o una botella de agua
            if (usedItem == appleHeartSprite && milton.currentHealth < milton.maxHealth)
            {
                milton.Heal(); //llama al método Heal() situado en MiltonLogic
                items.RemoveAt(index); //elimina el objeto seleccionado
                UpdateInventoryUI();  //actualiza la UI
            }
            else if (usedItem == waterBottleSprite)
            {
                AudioManager.Instance.PlayLouderSFX(sfxGetWater);
                waterCounter.AddWater(25);
                items.RemoveAt(index); //elimina el objeto seleccionado
                UpdateInventoryUI();  //actualiza la UI
            }
            else
            {
                ShowMessage("Tienes la vida completa");
            }   
        }
        else
        {
            ShowMessage("No hay objetos en ese espacio");
        }
    }

    //método para comprar un objeto (llamado desde la tienda)
    public bool BuyItem(string itemType, int price)
    {
        if (coins >= price)  //si tiene suficientes monedas
        {
            //verificamos si hay espacio en el inventario
            if (items.Count >= 5)  //si el inventario está lleno
            {
                ShowMessage("Inventario lleno");
                return false; //retorna false, no nos permite comprar más objetos
            }

            coins -= price;  //descontamos las monedas
            AddItem(itemType);  //añadimos el objeto al inventario
            return true;  //compra exitosa
        }
        else
        {
            ShowMessage("No tienes suficiente dinero");
            return false;
        }
    }

    //Método para actualizar la UI del inventario
    public void UpdateInventoryUI()
    {
        //actualiza el número de monedas
        coinsText.text = coins.ToString();

        for (int i = 0; i < slots.Count; i++)
        {
            if (i < items.Count)
            {
                slots[i].sprite = items[i];  //asigna el icono del objeto
                slots[i].enabled = true;     //activa la imagen
            }
            else
            {
                slots[i].sprite = null;  //borra el icono
                slots[i].enabled = false; //oculta la imagen
            }
        }

        SetKeyTransparency(hasKey ? 1f : 0.1f);
    }

    //método para cambiar la transparencia de la llave dependiendo de si la hemos recogido o no
    private void SetKeyTransparency(float alpha)
    {
        Color keyColor = key.color;
        keyColor.a = alpha;
        key.color = keyColor;
    }

    //método para mostrar los mensajes de texto en pantalla
    public void ShowMessage(string message)
    {
        messageText.text = message;
        messageTextBackground.gameObject.SetActive(true);
        StartCoroutine(ClearMessageAfterDelay(2f));
    }
    
    //método para limpiar el mensaje en pantalla pasado un tiempo
    private IEnumerator ClearMessageAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        messageText.text = "";
        messageTextBackground.gameObject.SetActive(false);
    }
}

