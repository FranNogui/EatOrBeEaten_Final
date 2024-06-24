using UnityEngine;

/// <summary>
/// Clase encargada de controlar el movimiento de los ojos.
/// </summary>
public class ControladorOjos : MonoBehaviour
{
    [SerializeField] Transform[] ojos;
    [SerializeField] float maximoRadio = 5.0f;

    /// <summary>
    /// Método para actualizar la posición actual del ojo de forma interpolada.
    /// </summary>
    /// <param name="direccion">Dirección a la que se va a encontrar el ojo.</param>
    public void ActualizarPosicion(Vector2 direccion)
    {
        foreach (var ojo in ojos) 
        { ojo.localPosition = Vector2.Lerp(ojo.localPosition, direccion * maximoRadio, 5.0f * Time.deltaTime); }
    }
}