using UnityEngine;

/// <summary>
/// Clase encargada de controlar los sonidos del agente.
/// </summary>
public class ControladorSonidosAgente : MonoBehaviour
{
    [Tooltip("Lista de sonidos de diferentes \'pop\'.")]
    [SerializeField] AudioClip[] pops;

    AudioSource reproductor;

    void Start()
    {  reproductor = GetComponent<AudioSource>(); }

    /// <summary>
    /// Método para reproducir un 'pop' aleatorio.
    /// </summary>
    public void ReproducirPop()
    {
        reproductor.clip = pops[Random.Range(0, pops.Length)];
        reproductor.Play();
    }
}