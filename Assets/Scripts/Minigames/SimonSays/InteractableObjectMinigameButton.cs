using UnityEngine;
using TMPro;

public class InteractableObject : MonoBehaviour, IInteractable
{
    public GameObject minigameUI;
    public GameObject minigameTextUI;
    private bool canOpenMinigame = false;
    private bool playerInside = false;
    private Player currentPlayer; // Referencia al jugador actual

    [SerializeField] private string interactText = "Empieza el minijuego";
    [SerializeField] private bool requiresSpecificItem = true;
    [SerializeField] private Item.ItemType requiredItemType;
    [SerializeField] private GameObject highlightEffect;

    // Mensajes personalizados
    [SerializeField] private string correctItemMessage = "Presiona F para interactuar";
    [SerializeField] private string wrongItemMessage = "Falta encontrar el mineral para activar el sistema";

    // Referencia al componente TextMeshPro
    private TextMeshProUGUI minigameText;

    private void Awake()
    {
        // Obtener el componente TextMeshPro del UI
        if (minigameTextUI != null)
        {
            minigameText = minigameTextUI.GetComponent<TextMeshProUGUI>();
            if (minigameText == null)
            {
                minigameText = minigameTextUI.GetComponentInChildren<TextMeshProUGUI>();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canOpenMinigame = true;
            playerInside = true;
            currentPlayer = other.GetComponent<Player>();
            UpdateMinigameText();
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
            minigameTextUI.SetActive(false);
        }
    }

    private void Update()
    {
        // Actualizar el texto mientras el jugador está dentro del trigger
        // Esto es útil si el jugador cambia de item mientras está en el área
        if (playerInside && currentPlayer != null)
        {
            UpdateMinigameText();
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

    public void EndMinigame()
    {
        minigameUI.SetActive(false);
        if (playerInside)
        {
            canOpenMinigame = true;
            minigameTextUI.SetActive(true);
            UpdateMinigameText(); // Actualizar el texto al cerrar el minijuego
        }
    }

    public void Interact(Player player)
    {
        Item playerItem = player.GetSelectedItem();

        if (!requiresSpecificItem || (playerItem != null && playerItem.itemType == requiredItemType))
        {
            OpenMinigame();
        }
        else
        {
            Debug.Log($"Necesitas {requiredItemType} para interactuar con {gameObject.name}");

            // Opcional: hacer que el texto parpadee o cambie de color brevemente
            StartCoroutine(FlashText());
        }
    }

    private System.Collections.IEnumerator FlashText()
    {
        if (minigameText != null)
        {
            Color originalColor = minigameText.color;

            // Parpadear en rojo
            for (int i = 0; i < 3; i++)
            {
                minigameText.color = Color.red;
                yield return new WaitForSeconds(0.2f);
                minigameText.color = originalColor;
                yield return new WaitForSeconds(0.2f);
            }
        }
    }

    private void OpenMinigame()
    {
        if (minigameUI != null)
        {
            minigameUI.SetActive(true);
            if (minigameTextUI != null)
                minigameTextUI.SetActive(false);
        }
    }

    public string GetInteractText()
    {
        if (requiresSpecificItem)
        {
            return $"{interactText} (Requiere: {requiredItemType})";
        }
        return interactText;
    }

    public void OnPlayerEnter(Player player)
    {
        currentPlayer = player;

        // Activar efecto visual si existe
        if (highlightEffect != null)
        {
            highlightEffect.SetActive(true);
        }

        // Cambiar color del sprite o agregar outline
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.yellow;
        }

        if (minigameTextUI != null)
        {
            minigameTextUI.SetActive(true);
            UpdateMinigameText();
        }
    }

    public void OnPlayerExit(Player player)
    {
        currentPlayer = null;

        // Desactivar efecto visual
        if (highlightEffect != null)
        {
            highlightEffect.SetActive(false);
        }

        // Restaurar color original
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
        }

        if (minigameTextUI != null)
            minigameTextUI.SetActive(false);
    }

    // Métodos públicos para configurar los mensajes desde el Inspector o código
    public void SetCorrectItemMessage(string message)
    {
        correctItemMessage = message;
        if (playerInside) UpdateMinigameText();
    }

    public void SetWrongItemMessage(string message)
    {
        wrongItemMessage = message;
        if (playerInside) UpdateMinigameText();
    }

    public void SetRequiredItem(Item.ItemType itemType)
    {
        requiredItemType = itemType;
        requiresSpecificItem = true;
        if (playerInside) UpdateMinigameText();
    }
}