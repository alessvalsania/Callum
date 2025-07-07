using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MineralCounter : MonoBehaviour
{
    public int totalMinerals = 15; // Cambia según cuántos haya en la mina
    private int collectedMinerals = 0;

    public TextMeshProUGUI counterText; // Asigna en el inspector

    private void Start()
    {
        UpdateCounter();
    }

    public void AddMineral()
    {
        collectedMinerals++;
        UpdateCounter();
        if (collectedMinerals >= totalMinerals)
        {
            SceneManager.LoadScene("Continue");
        }
    }

    private void UpdateCounter()
    {
        if (counterText != null){
            counterText.text = $"{collectedMinerals}/{totalMinerals}";
        }
    }
}