using UnityEngine;

/// <summary>
/// Clase encargada de manejar los sonidos que pueden reproducir los botones.
/// </summary>
public class SonidoBotones : MonoBehaviour
{
    AudioSource reproductor;

    void Start()
    { reproductor = GetComponent<AudioSource>(); }

    /// <summary>
    /// Método para reproducir un sonido de "beep".
    /// </summary>
    public void ReproducirBeep()
    { reproductor.Play(); }
}