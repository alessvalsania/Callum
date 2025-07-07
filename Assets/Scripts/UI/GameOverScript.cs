using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Aquí podrías agregar lógica para manejar el estado del juego, como mostrar un mensaje de Game Over
        // o reiniciar el juego si se presiona una tecla específica.
        if (Input.GetKeyDown(KeyCode.R))
        {
            // Reiniciar el juego o cargar la escena de inicio
            SceneManager.LoadScene(0);
        }

        if (Input.GetKeyDown(KeyCode.X))
        {

            SceneManager.LoadScene("Mine");
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Salir del juego
            Application.Quit();
        }
    }
}
