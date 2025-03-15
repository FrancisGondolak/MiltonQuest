using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class WaterCounterUI : MonoBehaviour
{
    public Image waterImage; //la imagen del estómago con agua
    public TextMeshProUGUI waterText; //el texto con el número de disparos
    public int currentWater; //disparos actuales

    void Start()
    {
        currentWater = 50; //empieza con el agua a 50 puntos
        UpdateUI();
    }

    //getter para acceder desde el script de MiltonMovement al agua actual
    public int GetCurrentWater()
    {
        return currentWater;
    }

    //función para actualizar el número de disparos en pantalla
    public void UpdateUI()
    {
        waterText.text = currentWater.ToString();
    }

    //función para restar los disparos al usarlos
    public void UseWater()
    {
        if (currentWater > 0)
        {
            currentWater--;
            UpdateUI();
        }
    }

    //función para añadir más disparos al contador al recoger/usar botellas de agua
    public void AddWater(int amount)
    {
        currentWater += amount;
        UpdateUI();
    }
}
