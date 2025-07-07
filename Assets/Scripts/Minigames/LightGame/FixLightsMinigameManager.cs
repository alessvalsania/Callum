using UnityEngine;
using System.Collections.Generic; // Necesario para List
using TMPro; // Si usas TextMeshPro para el mensaje de éxito

public class FixLightsMinigameManager : MonoBehaviour
{
    // Lista de todos los interruptores en el minijuego
    [SerializeField] private List<LightSwitch> allLightSwitches;

    // Referencia al GameObject del Canvas/Panel del minijuego (para ocultarlo al completar)
    [SerializeField] private GameObject minigameCanvas;

    // Referencia a un TextMeshProUGUI (o Text Legacy) para mostrar el mensaje de éxito
    [SerializeField] private TextMeshProUGUI successMessageText; // O Text si no usas TMPro
    [SerializeField] private GameObject successMessagePanel; // Un panel para el mensaje de éxito

    void Start()
    {
        // Inicializa el estado de los interruptores de forma aleatoria (o predefinida)
        InitializeSwitches();

        // Suscríbete al evento OnSwitchToggled de cada interruptor
        foreach (LightSwitch ls in allLightSwitches)
        {
            ls.OnSwitchToggled += OnLightSwitchToggled;
        }

        // Asegúrate de que el mensaje de éxito esté oculto al inicio
        if (successMessagePanel != null)
        {
            successMessagePanel.SetActive(false);
        }
    }

    private void InitializeSwitches()
    {
        // Puedes establecer un patrón inicial aquí, por ejemplo, que algunos estén encendidos y otros apagados
        // Para simplificar, vamos a asumir que necesitas encenderlos todos.
        // Podrías randomizar el estado inicial de cada uno si quieres más variedad:
        // foreach (LightSwitch ls in allLightSwitches)
        // {
        //     if (Random.value > 0.5f)
        //     {
        //         ls.ToggleSwitch(); // Esto cambiará su estado inicial
        //     }
        // }
    }

    private void OnLightSwitchToggled(LightSwitch toggledSwitch)
    {
        CheckMinigameCompletion();
    }

    private void CheckMinigameCompletion()
    {
        bool allLightsOn = true;
        foreach (LightSwitch ls in allLightSwitches)
        {
            if (!ls.IsLightOn)
            {
                allLightsOn = false;
                break; // Si encontramos una luz apagada, no necesitamos seguir revisando
            }
        }

        if (allLightsOn)
        {
            Debug.Log("¡Minijuego completado!");
            CompleteMinigame();
        }
    }

    private void CompleteMinigame()
    {
        Debug.Log("Iniciando CompleteMinigame()...");

        // 1. Iniciar la coroutine del mensaje de éxito PRIMERO
        if (successMessagePanel != null)
        {
            successMessagePanel.SetActive(true);
            if (successMessageText != null)
            {
                successMessageText.text = "¡Luces arregladas! ¡Misión cumplida!";
            }
            // Inicia la coroutine para ocultar el mensaje de éxito
            StartCoroutine(HideSuccessMessageAfterDelay(3f));
            Debug.Log("successMessagePanel mostrado y coroutine iniciada.");
        }
        else
        {
            Debug.LogError("successMessagePanel es null en CompleteMinigame().");
        }

        // 2. Desactivar la interacción de los interruptores (opcional, si quieres que no se puedan volver a clicar)
        foreach (LightSwitch ls in allLightSwitches)
        {
            if (ls != null)
            {
                ls.enabled = false; // Desactiva el script para que no respondan a más clics
                Debug.Log(ls.name + " script deshabilitado.");
            }
        }

        // 3. Finalmente, oculta el canvas del minijuego.
        // Asegúrate de que el minigameCanvas no sea el mismo GameObject que contiene este script.
        // Si lo es, la coroutine debe ser iniciada ANTES de desactivar el GameObject.
        if (minigameCanvas != null)
        {
            // Solo desactiva el canvas si este script NO está en el mismo GameObject que minigameCanvas.
            // Si este script (FixLightsMinigameManager) está en el GameObject 'Panel' y 'Panel' es tu minigameCanvas,
            // entonces desactivarlo aquí causaría que la coroutine se detenga.
            // Asumimos que FixLightsMinigameManager está en un Panel HIJO o en el mismo Canvas,
            // y que el Canvas es el que se desactiva.
            minigameCanvas.SetActive(false);
            Debug.Log("minigameCanvas ocultado.");
        }
        else
        {
            Debug.LogError("minigameCanvas es null en CompleteMinigame().");
        }

        // Puedes añadir lógica adicional aquí, como desbloquear una puerta, etc.
    }

    private System.Collections.IEnumerator HideSuccessMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (successMessagePanel != null)
        {
            successMessagePanel.SetActive(false);
        }
    }

    void OnDestroy()
    {
        // Es una buena práctica desuscribirse de los eventos para evitar Memory Leaks
        foreach (LightSwitch ls in allLightSwitches)
        {
            if (ls != null) // Asegúrate de que el objeto no ha sido destruido ya
            {
                ls.OnSwitchToggled -= OnLightSwitchToggled;
            }
        }
    }
}