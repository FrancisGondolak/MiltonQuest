using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public List<Image> slots = new List<Image>(); //Lista para el inventario
    public Sprite waterBottleSprite; //Icono para la botella de agua
    public Sprite appleHeartSprite;  //Icono para la coranzana

    private List<Sprite> items = new List<Sprite>(); //Lista interna para los objetos del inventario

    void Start()
    {
        UpdateInventoryUI();
        AddItem("WaterBottle");
        AddItem("AppleHeart");
        AddItem("AppleHeart");
        AddItem("WaterBottle");
        AddItem("AppleHeart");
    }

    void Update()
    {
        //Detecta la pulsaci�n de las teclas 1 al 5 para usar el item que se encuentra en ese espacio del inventario
        if (Input.GetKeyDown(KeyCode.Alpha1)) UseItemAt(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) UseItemAt(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) UseItemAt(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) UseItemAt(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) UseItemAt(4);
    }

    //M�todo que a�ade un objeto al inventario
    public bool AddItem(string itemType)
    {
        if (items.Count >= 5)
        {
            Debug.Log("Inventario lleno. No puedes comprar m�s objetos.");
            return false;
        }

        //Controlar qu� icono agregamos al inventario
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

    //M�todo para consumir un objeto en una casilla espec�fica
    public void UseItemAt(int index)
    {
        if (index < items.Count) //verifica si la casilla seleccionada tiene un objeto
        {
            Debug.Log($"Usaste el objeto en la casilla {index + 1}");
            items.RemoveAt(index); //elimina el objeto seleccionado
            UpdateInventoryUI();  //actualiza la UI
        }
        else
        {
            Debug.Log("No hay objeto en esta casilla.");
        }
    }

    //M�todo para actualizar la UI del inventario
    void UpdateInventoryUI()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (i < items.Count)
            {
                slots[i].sprite = items[i];  //Asigna el icono del objeto
                slots[i].enabled = true;     //Activa la imagen
            }
            else
            {
                slots[i].sprite = null;  //Borra el icono
                slots[i].enabled = false; //Oculta la imagen
            }
        }
    }
}

