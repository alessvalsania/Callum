using UnityEngine;
using UnityEngine.UI; // Necesario para Image

public class LightSwitch : MonoBehaviour
{
    // Referencias a los GameObjects de los estados visuales del interruptor
    [SerializeField] private GameObject switchUpVisual;
    [SerializeField] private GameObject switchDownVisual;

    // Referencias a los GameObjects de los estados visuales de la luz
    [SerializeField] private GameObject lightOnVisual;
    [SerializeField] private GameObject lightOffVisual;

    // Estado actual del interruptor y la luz
    public bool IsLightOn { get; private set; }

    // Evento para notificar al MinigameManager que un interruptor ha cambiado
    public event System.Action<LightSwitch> OnSwitchToggled;

    void Start()
    {
        // Asegúrate de que el estado inicial sea el correcto (por defecto, apagado)
        SetLightState(false);
    }

    public void ToggleSwitch()
    {
        // Cambia el estado de la luz
        SetLightState(!IsLightOn);

        // Notifica a quienes estén escuchando (el MinigameManager)
        OnSwitchToggled?.Invoke(this);
    }

    private void SetLightState(bool on)
    {
        IsLightOn = on;

        // Actualiza la visibilidad de las imágenes del interruptor
        switchUpVisual.SetActive(!on);
        switchDownVisual.SetActive(on);

        // Actualiza la visibilidad de las imágenes de la luz
        lightOnVisual.SetActive(on);
        lightOffVisual.SetActive(!on);
    }
}