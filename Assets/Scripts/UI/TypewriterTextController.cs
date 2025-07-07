using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class TypewriterTextController : MonoBehaviour
{
    [Header("Configuración de Animación")]
    [SerializeField] private float typingSpeed = 0.05f; // Velocidad entre caracteres
    [SerializeField] private float delayBetweenTexts = 2f; // Pausa entre textos
    [SerializeField] private bool autoStart = true; // Iniciar automáticamente
    [SerializeField] private bool loop = false; // Repetir la secuencia

    [Header("Sonidos (Opcional)")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip typingSound;

    [Header("Configuración del Contador")]
    [SerializeField] private float countdownTime = 120f; // 2 minutos en segundos
    [SerializeField] private TextMeshProUGUI countdownText; // Asignar en el inspector
    [SerializeField] private TextMeshProUGUI gameOverText; // Texto "Perdiste"
    [SerializeField] private string gameOverMessage = "¡PERDISTE!";
    [SerializeField] private string nextSceneName = ""; // Nombre de la siguiente escena (opcional)

    private List<TextMeshProUGUI> textComponents = new List<TextMeshProUGUI>();
    private List<string> originalTexts = new List<string>();
    private int currentTextIndex = 0;
    private bool isTyping = false;
    private bool isComplete = false;
    private bool countdownStarted = false;
    private bool gameOver = false;
    private float currentCountdownTime;

    private void Start()
    {
        InitializeTextComponents();
        InitializeCountdown();

        if (autoStart)
        {
            StartTypewriterSequence();
        }
    }

    private void InitializeTextComponents()
    {
        // Obtener todos los componentes TextMeshProUGUI en los hijos (excluyendo el contador y game over)
        TextMeshProUGUI[] childTexts = GetComponentsInChildren<TextMeshProUGUI>(true);

        foreach (TextMeshProUGUI textComponent in childTexts)
        {
            // No incluir el texto del contador ni el de game over en la secuencia
            if (textComponent != countdownText && textComponent != gameOverText)
            {
                textComponents.Add(textComponent);
                originalTexts.Add(textComponent.text);

                // Solo ocultar y limpiar si NO es Help
                if (textComponent.gameObject.name != "Help")
                {
                    textComponent.text = "";
                    textComponent.gameObject.SetActive(false);
                }
            }
        }

        Debug.Log($"Se encontraron {textComponents.Count} componentes de texto");
    }

    private void InitializeCountdown()
    {
        currentCountdownTime = countdownTime;

        // Ocultar el contador y el texto de game over al inicio
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }

        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(false);
        }
    }

    public void StartTypewriterSequence()
    {
        if (textComponents.Count == 0)
        {
            Debug.LogWarning("No se encontraron componentes de texto para animar");
            return;
        }

        isComplete = false;
        currentTextIndex = 0;
        StopAllCoroutines();
        StartCoroutine(TypewriterSequence());
    }

    private IEnumerator TypewriterSequence()
    {
        while (currentTextIndex < textComponents.Count)
        {
            yield return StartCoroutine(ShowAndTypeText(currentTextIndex));

            // Pausa entre textos (excepto en el último)
            if (currentTextIndex < textComponents.Count - 1)
            {
                yield return new WaitForSeconds(delayBetweenTexts);

                // Ocultar el texto actual antes de pasar al siguiente
                textComponents[currentTextIndex].gameObject.SetActive(false);
            }

            currentTextIndex++;
        }

        isComplete = true;
        OnSequenceComplete();
    }

    private IEnumerator ShowAndTypeText(int textIndex)
    {
        if (textIndex >= textComponents.Count) yield break;

        TextMeshProUGUI currentText = textComponents[textIndex];
        string fullText = originalTexts[textIndex];

        // Mostrar el GameObject y limpiar el texto
        currentText.gameObject.SetActive(true);
        currentText.text = "";

        isTyping = true;

        // Animar carácter por carácter
        for (int i = 0; i <= fullText.Length; i++)
        {
            currentText.text = fullText.Substring(0, i);

            // Reproducir sonido de tecleo (opcional)
            if (audioSource != null && typingSound != null && i < fullText.Length)
            {
                audioSource.PlayOneShot(typingSound, 0.1f);
            }

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    private void OnSequenceComplete()
    {
        Debug.Log("Secuencia de texto completada - Iniciando contador");

        if (loop)
        {
            StartCoroutine(RestartSequence());
        }
        else
        {
            // Solo ocultar el texto si el índice es válido y no se llama 'Help'
            if (currentTextIndex > 0 && currentTextIndex <= textComponents.Count)
            {
                TextMeshProUGUI currentText = textComponents[currentTextIndex - 1];
                if (currentText.gameObject.name != "Help")
                {
                    currentText.gameObject.SetActive(false);
                }
            }
            // Iniciar el contador cuando termine la secuencia
            StartCountdown();
        }
    }

    private void StartCountdown()
    {
        if (countdownText == null)
        {
            Debug.LogWarning("No se asignó el TextMeshProUGUI para el contador");
            return;
        }

        // Ocultar el texto de ayuda 'Help' si existe
        foreach (TextMeshProUGUI text in textComponents)
        {
            if (text.gameObject.name == "Help")
            {
                text.gameObject.SetActive(false);
            }
        }

        countdownStarted = true;
        countdownText.gameObject.SetActive(true);
        StartCoroutine(CountdownCoroutine());
    }

    private IEnumerator CountdownCoroutine()
    {
        while (currentCountdownTime > 0 && !gameOver)
        {
            // Actualizar el texto del contador
            UpdateCountdownDisplay();

            yield return new WaitForSeconds(1f);
            currentCountdownTime--;
        }

        // Si el tiempo se agotó
        if (currentCountdownTime <= 0 && !gameOver)
        {
            ShowGameOver();
        }
    }

    private void UpdateCountdownDisplay()
    {
        if (countdownText != null)
        {
            int minutes = Mathf.FloorToInt(currentCountdownTime / 60f);
            int seconds = Mathf.FloorToInt(currentCountdownTime % 60f);
            countdownText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    private void ShowGameOver()
    {
        gameOver = true;

        // Ocultar el contador
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }

        // Mostrar mensaje de game over
        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(true);
            gameOverText.text = gameOverMessage;
        }

        Debug.Log("¡Tiempo agotado! Game Over");
        ChangeToNextScene();
    }

    // Método público para cambiar de escena (llamar desde otro script o botón)
    public void ChangeToNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
    }

    // Método público para cambiar de escena por índice
    public void ChangeToNextScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    // Método público para cambiar de escena por nombre
    public void ChangeToNextScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator RestartSequence()
    {
        yield return new WaitForSeconds(delayBetweenTexts);

        // Ocultar todos los textos
        foreach (TextMeshProUGUI text in textComponents)
        {
            text.gameObject.SetActive(false);
            text.text = "";
        }

        StartTypewriterSequence();
    }

    // Métodos públicos para control externo
    public void SkipCurrentText()
    {
        if (isTyping && currentTextIndex < textComponents.Count)
        {
            StopAllCoroutines();

            // Mostrar el texto completo inmediatamente
            TextMeshProUGUI currentText = textComponents[currentTextIndex];
            currentText.text = originalTexts[currentTextIndex];

            isTyping = false;

            // Continuar con el siguiente texto después de la pausa
            StartCoroutine(ContinueAfterSkip());
        }
    }

    private IEnumerator ContinueAfterSkip()
    {
        yield return new WaitForSeconds(delayBetweenTexts);

        // Ocultar el texto actual antes de continuar
        if (currentTextIndex < textComponents.Count)
        {
            textComponents[currentTextIndex].gameObject.SetActive(false);
        }

        currentTextIndex++;

        if (currentTextIndex < textComponents.Count)
        {
            StartCoroutine(TypewriterSequence());
        }
        else
        {
            isComplete = true;
            OnSequenceComplete();
        }
    }

    public void SkipToEnd()
    {
        StopAllCoroutines();

        // Mostrar todos los textos inmediatamente
        for (int i = 0; i < textComponents.Count; i++)
        {
            textComponents[i].gameObject.SetActive(true);
            textComponents[i].text = originalTexts[i];
        }

        currentTextIndex = textComponents.Count;
        isComplete = true;
        isTyping = false;

        OnSequenceComplete();
    }

    public void ResetSequence()
    {
        StopAllCoroutines();

        // Ocultar todos los textos
        foreach (TextMeshProUGUI text in textComponents)
        {
            text.gameObject.SetActive(false);
            text.text = "";
        }

        // Reiniciar contador
        currentCountdownTime = countdownTime;
        countdownStarted = false;
        gameOver = false;

        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }

        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(false);
        }

        currentTextIndex = 0;
        isComplete = false;
        isTyping = false;
    }

    // Propiedades para verificar el estado
    public bool IsTyping => isTyping;
    public bool IsComplete => isComplete;
    public bool IsCountdownActive => countdownStarted && !gameOver;
    public bool IsGameOver => gameOver;
    public float RemainingTime => currentCountdownTime;
    public int CurrentTextIndex => currentTextIndex;
    public int TotalTexts => textComponents.Count;

    // Métodos para controlar desde el input del usuario
    private void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     SkipCurrentText();
        // }

        // if (Input.GetKeyDown(KeyCode.R))
        // {
        //     ResetSequence();
        // }

        // Salta directo al contador con la tecla 'C'
        if (!isComplete && Input.GetKeyDown(KeyCode.C))
        {
            SkipToCounter();
        }
    }

    // Método para saltar directo al contador
    private void SkipToCounter()
    {
        StopAllCoroutines();
        // Oculta todos los textos excepto el que se llama 'Help'
        foreach (TextMeshProUGUI text in textComponents)
        {
            if (text.gameObject.name != "Help")
            {
                text.gameObject.SetActive(false);
            }
        }
        isComplete = true;
        OnSequenceComplete();
    }
}