using UnityEngine;

public interface IInteractable
{
    /// <summary>
    /// Método llamado cuando el jugador interactúa con el objeto
    /// </summary>
    /// <param name="player">Referencia al jugador que interactúa</param>
    void Interact(Player player);

    /// <summary>
    /// Texto que describe la interacción disponible
    /// </summary>
    /// <returns>Texto descriptivo del objeto</returns>
    string GetInteractText();

    /// <summary>
    /// Llamado cuando el jugador entra en rango de interacción
    /// </summary>
    /// <param name="player">Referencia al jugador</param>
    void OnPlayerEnter(Player player);

    /// <summary>
    /// Llamado cuando el jugador sale del rango de interacción
    /// </summary>
    /// <param name="player">Referencia al jugador</param>
    void OnPlayerExit(Player player);
}