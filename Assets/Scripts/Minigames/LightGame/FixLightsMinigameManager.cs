using UnityEngine;
using System.Collections.Generic; // Necesario para List
using TMPro; // Si usas TextMeshPro para el mensaje de �xito

public class FixLightsMinigameManager : MonoBehaviour
{
    // Lista de todos los interruptores en el minijuego
    [SerializeField] private List<LightSwitch> allLightSwitches;

    // Referencia al GameObject del Canvas/Panel del minijuego (para ocultarlo al completar)
    [SerializeField] private GameObject minigameCanvas;

    // Referencia a un TextMeshProUGUI (o Text Legacy) para mostrar el mensaje de �xito
    [SerializeField] private TextMeshProUGUI successMessageText; // O Text si no usas TMPro
    [SerializeField] private GameObject successMessagePanel; // Un panel para el mensaje de �xito
    [SerializeField] private GameObject door; // La puerta que se abrirá al completar el minijuego


    void Start()
    {
        // Inicializa el estado de los interruptores de forma aleatoria (o predefinida)
        InitializeSwitches();

        // Suscr�bete al evento OnSwitchToggled de cada interruptor
        foreach (LightSwitch ls in allLightSwitches)
        {
            ls.OnSwitchToggled += OnLightSwitchToggled;
        }

        // Aseg�rate de que el mensaje de �xito est� oculto al inicio
        if (successMessagePanel != null)
        {
            successMessagePanel.SetActive(false);
        }
    }

    private void InitializeSwitches()
    {
        // Puedes establecer un patr�n inicial aqu�, por ejemplo, que algunos est�n encendidos y otros apagados
        // Para simplificar, vamos a asumir que necesitas encenderlos todos.
        // Podr�as randomizar el estado inicial de cada uno si quieres m�s variedad:
        // foreach (LightSwitch ls in allLightSwitches)
        // {
        //     if (Random.value > 0.5f)
        //     {
        //         ls.ToggleSwitch(); // Esto cambiar� su estado inicial
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
            Debug.Log("�Minijuego completado!");
            CompleteMinigame();
        }
    }

    private void CompleteMinigame()
    {
        Debug.Log("Iniciando CompleteMinigame()...");

        // 1. Iniciar la coroutine del mensaje de �xito PRIMERO
        if (successMessagePanel != null)
        {
            successMessagePanel.SetActive(true);
            if (successMessageText != null)
            {
                successMessageText.text = "�Luces arregladas! �Misi�n cumplida!";
            }
            // Inicia la coroutine para ocultar el mensaje de �xito
            StartCoroutine(HideSuccessMessageAfterDelay(3f));
            Debug.Log("successMessagePanel mostrado y coroutine iniciada.");
        }
        else
        {
            Debug.LogError("successMessagePanel es null en CompleteMinigame().");
        }

        // 2. Desactivar la interacci�n de los interruptores (opcional, si quieres que no se puedan volver a clicar)
        foreach (LightSwitch ls in allLightSwitches)
        {
            if (ls != null)
            {
                ls.enabled = false; // Desactiva el script para que no respondan a m�s clics
                Debug.Log(ls.name + " script deshabilitado.");
            }
        }

        // 3. Finalmente, oculta el canvas del minijuego.
        // Aseg�rate de que el minigameCanvas no sea el mismo GameObject que contiene este script.
        // Si lo es, la coroutine debe ser iniciada ANTES de desactivar el GameObject.
        if (minigameCanvas != null)
        {
            // Solo desactiva el canvas si este script NO est� en el mismo GameObject que minigameCanvas.
            // Si este script (FixLightsMinigameManager) est� en el GameObject 'Panel' y 'Panel' es tu minigameCanvas,
            // entonces desactivarlo aqu� causar�a que la coroutine se detenga.
            // Asumimos que FixLightsMinigameManager est� en un Panel HIJO o en el mismo Canvas,
            // y que el Canvas es el que se desactiva.
            minigameCanvas.SetActive(false);
            Debug.Log("minigameCanvas ocultado.");
            if (door != null)
            {
                Destroy(door);

            }
        }
        else
        {
            Debug.LogError("minigameCanvas es null en CompleteMinigame().");
        }

        // Puedes a�adir l�gica adicional aqu�, como desbloquear una puerta, etc.
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
        // Es una buena pr�ctica desuscribirse de los eventos para evitar Memory Leaks
        foreach (LightSwitch ls in allLightSwitches)
        {
            if (ls != null) // Aseg�rate de que el objeto no ha sido destruido ya
            {
                ls.OnSwitchToggled -= OnLightSwitchToggled;
            }
        }
    }
}