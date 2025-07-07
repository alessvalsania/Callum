using UnityEngine;

public class DoorMinigame : MonoBehaviour, IInteractable
{
    [SerializeField] private string interactText = "Examinar objeto";
    [SerializeField] private bool requiresSpecificItem = true;
    [SerializeField] private Item.ItemType requiredItemType;

    // AÑADE ESTA LÍNEA: Referencia al GameObject de tu Canvas o Panel
    // Asegúrate de arrastrar el Canvas (o el Panel dentro) a esta variable en el Inspector de Unity.
    [SerializeField] private GameObject minigameCanvas; // O un Panel, si es lo que quieres mostrar/ocultar
    [SerializeField] private GameObject minigameTextUI; // UI para el texto de interacción
    [SerializeField] private string correctItemMessage = "Presiona F para interactuar";
    [SerializeField] private string wrongItemMessage = "Falta encontrar el mineral para activar el sistema";
    private TMPro.TextMeshProUGUI minigameText;

    private bool canOpenMinigame = false;
    private bool playerInside = false;
    private Player currentPlayer;

    public string GetInteractText()
    {
        if (requiresSpecificItem)
        {
            return $"{interactText} (Requiere: {requiredItemType})";
        }
        return interactText;
    }

    public void Interact(Player player)
    {
        Item playerItem = player.GetSelectedItem();

        if (requiresSpecificItem)
        {
            if (playerItem != null && playerItem.itemType == requiredItemType)
            {
                PerformSpecialInteraction(player);
            }
            else
            {
                Debug.Log($"Necesitas {requiredItemType} para interactuar con {gameObject.name}");
            }
        }
        else // Si no requiere un ítem específico, simplemente realiza la interacción especial
        {
            PerformSpecialInteraction(player);
        }
    }

    public void PerformSpecialInteraction(Player player)
    {
        // Aquí es donde mostramos el Canvas/Panel del minijuego
        if (minigameCanvas != null)
        {
            minigameCanvas.SetActive(true); // Activa el GameObject, haciéndolo visible
            Debug.Log("Canvas del minijuego mostrado.");
            
        }
        else
        {
            Debug.LogError("minigameCanvas no está asignado en el Inspector de " + gameObject.name);
        }

        // Originalmente tenías esto para desactivar el objeto interactuable (la puerta).
        // Decide si quieres que la puerta se desactive inmediatamente o después del minijuego.
        // gameObject.SetActive(false); // Desactivar el objeto interactuable (la puerta)
    }

    public void OnPlayerEnter(Player player)
    {
        // Cambiar color del sprite o agregar outline
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            // Cambiar el color del primer SpriteRenderer encontrado
            // Verifica si el jugador tiene el ítem, no solo el seleccionado
            if (player.GetInventory() != null && player.GetInventory().HasItem(requiredItemType))
            {
                spriteRenderer.color = Color.yellow; // Cambiar a un color de resaltado si tiene el ítem
            }
            else
            {
                spriteRenderer.color = Color.red; // Cambiar a un color de advertencia si no tiene el ítem
            }

            // Si el ítem seleccionado es el requerido
            if (player.GetSelectedItem() != null && player.GetSelectedItem().itemType == requiredItemType)
            {
                spriteRenderer.color = Color.green; // Cambiar a un color de éxito si tiene el ítem seleccionado
            }
        }
    }

    public void OnPlayerExit(Player player)
    {
        // Restaurar color original
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            // Restaurar el color del primer SpriteRenderer encontrado
            spriteRenderer.color = Color.white; // Restaurar al color original
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Opcional: Asegúrate de que el Canvas del minijuego esté oculto al inicio
        if (minigameCanvas != null)
        {
            minigameCanvas.SetActive(false);
        }
        // Obtener el componente TextMeshProUGUI del UI
        if (minigameTextUI != null)
        {
            minigameText = minigameTextUI.GetComponent<TMPro.TextMeshProUGUI>();
            if (minigameText == null)
                minigameText = minigameTextUI.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Si necesitas una forma de cerrar el minijuego, podrías añadir algo aquí,
        // por ejemplo, si el minigameCanvas está activo y se presiona una tecla.
        // Sin embargo, lo más común es que el propio script del minijuego lo cierre.
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canOpenMinigame = true;
            playerInside = true;
            currentPlayer = other.GetComponent<Player>();
            UpdateMinigameText();
            if (minigameTextUI != null)
                minigameTextUI.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canOpenMinigame = false;
            playerInside = false;
            currentPlayer = null;
            if (minigameTextUI != null)
                minigameTextUI.SetActive(false);
        }
    }

    private void UpdateMinigameText()
    {
        if (minigameText == null || currentPlayer == null) return;

        Item playerItem = currentPlayer.GetSelectedItem();

        if (requiresSpecificItem)
        {
            if (playerItem != null && playerItem.itemType == requiredItemType)
            {
                // El jugador tiene el item correcto
                minigameText.text = correctItemMessage;
                minigameText.color = Color.green; // Color verde para indicar que puede interactuar
            }
            else
            {
                // El jugador no tiene el item correcto o no tiene ningún item
                minigameText.text = wrongItemMessage;
                minigameText.color = Color.red; // Color rojo para indicar que no puede interactuar
            }
        }
        else
        {
            // No requiere item específico
            minigameText.text = correctItemMessage;
            minigameText.color = Color.white;
        }
    }
}
