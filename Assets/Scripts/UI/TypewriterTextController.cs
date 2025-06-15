using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    private List<TextMeshProUGUI> textComponents = new List<TextMeshProUGUI>();
    private List<string> originalTexts = new List<string>();
    private int currentTextIndex = 0;
    private bool isTyping = false;
    private bool isComplete = false;

    private void Start()
    {
        InitializeTextComponents();

        if (autoStart)
        {
            StartTypewriterSequence();
        }
    }

    private void InitializeTextComponents()
    {
        // Obtener todos los componentes TextMeshProUGUI en los hijos
        TextMeshProUGUI[] childTexts = GetComponentsInChildren<TextMeshProUGUI>(true);

        foreach (TextMeshProUGUI textComponent in childTexts)
        {
            textComponents.Add(textComponent);
            originalTexts.Add(textComponent.text);

            // Ocultar el texto y el GameObject inicialmente
            textComponent.text = "";
            textComponent.gameObject.SetActive(false);
        }

        Debug.Log($"Se encontraron {textComponents.Count} componentes de texto");
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
        Debug.Log("Secuencia de texto completada");

        if (loop)
        {
            StartCoroutine(RestartSequence());
        }
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

        currentTextIndex = 0;
        isComplete = false;
        isTyping = false;
    }

    // Propiedades para verificar el estado
    public bool IsTyping => isTyping;
    public bool IsComplete => isComplete;
    public int CurrentTextIndex => currentTextIndex;
    public int TotalTexts => textComponents.Count;

    // Métodos para controlar desde el input del usuario
    private void Update()
    {
        // Ejemplo: Presionar Espacio para saltar texto actual
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                SkipCurrentText();
            }
        }

        // Ejemplo: Presionar Enter para saltar toda la secuencia
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SkipToEnd();
        }
    }
}