using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Clase encargada de controlar la música que se reproduce de fondo en una partida.
/// </summary>
public class ControladorMusica : MonoBehaviour
{
    [Tooltip("Clips de música que se reproducirán de forma aleatoria durante la partida.")]
    [SerializeField] AudioClip[] clipsMusica;

    [Tooltip("Mezclador donde mandar la señal de audio.")]
    [SerializeField] AudioMixerGroup mixer;

    AudioSource[] reproductor;
    int reproductorActual;

    void Start()
    {
        reproductor = new AudioSource[clipsMusica.Length];
        for (int i = 0; i < reproductor.Length; i++)
        {
            reproductor[i] = gameObject.AddComponent<AudioSource>();
            reproductor[i].clip = clipsMusica[i];
            reproductor[i].outputAudioMixerGroup = mixer;
            reproductor[i].playOnAwake = false;
            reproductor[i].volume = 0.1f;
        }
        reproductorActual = Random.Range(0, clipsMusica.Length);
        reproductor[reproductorActual].Play();
    }

    void Update()
    {
        if (!reproductor[reproductorActual].isPlaying)
        {
            reproductorActual = Random.Range(0, clipsMusica.Length);
            reproductor[reproductorActual].Play();
        }
    }
}