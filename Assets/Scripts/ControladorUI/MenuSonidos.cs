using UnityEngine;

/// <summary>
/// Clase encargada de manejar los sonidos que pueden reproducir los menús.
/// </summary>
public class MenuSonidos : MonoBehaviour
{
    AudioSource reproductor;

    void Start()
    { reproductor = GetComponent<AudioSource>(); }

    /// <summary>
    /// Método para reproducir un sonido de "woosh".
    /// </summary>
    public void ReproducirWhoosh()
    { reproductor.Play(); }
}