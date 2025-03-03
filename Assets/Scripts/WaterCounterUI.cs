using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class WaterCounterUI : MonoBehaviour
{
    public Image waterImage; //la imagen del est�mago con agua
    public TextMeshProUGUI waterText; //el texto con el n�mero de disparos
    public int maxWater = 30; //m�ximo de disparos
    private int currentWater; //disparos actuales

    void Start()
    {
        currentWater = maxWater; //empieza con el agua llena
        UpdateUI();
    }

    //getter para acceder desde el script de MiltonMovement al agua actual
    public int GetCurrentWater()
    {
        return currentWater;
    }

    //funci�n para actualizar el n�mero de disparos en pantalla
    public void UpdateUI()
    {
        waterText.text = currentWater.ToString();
    }

    //funci�n para restar los disparos al usarlos
    public void UseWater()
    {
        if (currentWater > 0)
        {
            currentWater--;
            UpdateUI();
        }
    }

    //funci�n para a�adir m�s disparos al contador al recoger botellas de agua
    public void AddWater(int amount)
    {
        currentWater = Mathf.Min(currentWater + amount, maxWater);
        UpdateUI();
    }
}
