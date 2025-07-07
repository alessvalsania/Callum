using UnityEngine;
using UnityEngine.UI;

public class UI_BatteryHealth : MonoBehaviour
{
    public Sprite[] batterySprites; // Asigna en el inspector: [0]=vacía, [1]=1/5, ..., [5]=llena
    public Image batteryImage;      // Asigna el componente Image

    // Llama a este método cuando cambie la vida
    public void SetHealth(int health)
    {
        // Asegúrate de que health esté en el rango 0-5
        health = Mathf.Clamp(health, 0, batterySprites.Length - 1);
        Debug.Log("El sprite es: " + batterySprites[health]);
        batteryImage.sprite = batterySprites[health];
    }
}