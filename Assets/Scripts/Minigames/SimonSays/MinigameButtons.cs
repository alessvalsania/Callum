using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MinigameSimon : MonoBehaviour
{
    public InteractableObject interactableObject;

    public Button[] gridButtons; // Asigna los 9 botones desde el inspector
    public float highlightTime = 0.5f;
    public float waitTime = 0.3f;

    private List<int> pattern = new List<int>();
    private int playerStep = 0;
    private bool playerTurn = false;

    void OnEnable()
    {
        GeneratePattern(4); // Por ejemplo, patrón de 4 pasos
        StartCoroutine(ShowPattern());
    }

    void Start()
    {
        for (int i = 0; i < gridButtons.Length; i++)
        {
            int index = i;
            gridButtons[i].onClick.AddListener(() => PlayerPress(index));
        }
    }

    void GeneratePattern(int length)
    {
        pattern.Clear();
        for (int i = 0; i < length; i++)
            pattern.Add(Random.Range(0, 9));
    }

    System.Collections.IEnumerator ShowPattern()
    {
        playerTurn = false;
        foreach (int id in pattern)
        {
            gridButtons[id].image.color = Color.yellow;
            yield return new WaitForSecondsRealtime(highlightTime);
            gridButtons[id].image.color = Color.white;
            yield return new WaitForSecondsRealtime(waitTime);
        }
        playerTurn = true;
        playerStep = 0;
    }

    void PlayerPress(int index)
    {
        if (!playerTurn) return;
        if (pattern[playerStep] == index)
        {
            playerStep++;
            if (playerStep >= pattern.Count)
            {
                // Ganaste el minijuego
                Debug.Log("Minijuego completado!");
                EndMinigame();
            }
        }
        else
        {
            // Fallaste
            Debug.Log("¡Has fallado!");
            EndMinigame();
        }
    }

    void EndMinigame()
    {
        // Desactiva el panel principal del minijuego (minigameUI)
        interactableObject.minigameUI.SetActive(false);

        // Llama a EndMinigame en el objeto interactuable
        interactableObject.EndMinigame();

        // Opcional: Si usaste Time.timeScale = 0 en algún momento, aquí puedes reanudarlo
        // Time.timeScale = 1f;
    }
}