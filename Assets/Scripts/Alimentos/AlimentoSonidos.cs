using UnityEngine;

/// <summary>
/// Módulo del alimento encargado de controlar los sonidos.
/// </summary>
public class AlimentoSonidos : MonoBehaviour
{
    [Tooltip("Lista de sonidos de diferentes \'pop\'.")]
    [SerializeField] AudioClip[] pops;

    AudioSource reproductor;

    void Start()
    { reproductor = GetComponent<AudioSource>(); }

    /// <summary>
    /// Método para reproducir un 'pop' aleatorio.
    /// </summary>
    public void ReproducirPop()
    {
        reproductor.clip = pops[Random.Range(0, pops.Length)];
        reproductor.Play();
    }
}