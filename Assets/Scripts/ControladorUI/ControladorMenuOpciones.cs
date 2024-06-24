using System;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ControladorMenuOpciones : MonoBehaviour
{
    readonly int[] resolucionAnchos   = { 640, 1280, 1366, 1600, 1920, 2560, 3200, 3840, 5120, 7680 };
    readonly int[] resolucionAltos    = { 360, 720, 768, 900, 1080, 1440, 1800, 2160, 2880, 4320 };
    readonly float[] velocidadPartida = { 1.0f, 1.5f, 2.0f, 2.5f, 3.0f };
    int velocidadPartidaActual = 0;

    [Header("Informacion de la partida")]
    [SerializeField] InformacionMapa info;

    [Header("Menu de opciones")]
    [SerializeField] GameObject menuOpciones;
    [SerializeField] GameObject menuConfirmacion;
    [SerializeField] Slider volumenMusica;
    [SerializeField] Slider volumenSFX;
    [SerializeField] TextMeshProUGUI musicaTexto;
    [SerializeField] TextMeshProUGUI SFXTexto;
    [SerializeField] AudioMixer mixer;
    [SerializeField] Toggle pantallaCompleta;
    [SerializeField] TMP_Dropdown resolucion;
    Animator menuOpcionesAnimator;

    [Header("Contador Entes")]
    [SerializeField] TextMeshProUGUI numeroVivos;

    [Header("Menu Final Partida")]
    [SerializeField] GameObject menuFinalPartida;
    [SerializeField] TextMeshProUGUI textoFinalPartida;

    [Header("Velocidad Partida")]
    [SerializeField] GameObject botonAcelerar;
    [SerializeField] TextMeshProUGUI textoVelocidad;

    [Header("Lista Top")]
    [SerializeField] GameObject listaTop;

    void Start()
    {
        menuOpcionesAnimator = menuOpciones.GetComponent<Animator>();

        volumenMusica.value = PlayerPrefs.GetFloat("VolumenMusica");
        mixer.SetFloat("Musica", Mathf.Log10(PlayerPrefs.GetFloat("VolumenMusica")) * 20.0f);
        musicaTexto.text = (int)(PlayerPrefs.GetFloat("VolumenMusica") * 100.0f) + "%";

        volumenSFX.value = PlayerPrefs.GetFloat("VolumenSFX");
        mixer.SetFloat("Sonidos", Mathf.Log10(PlayerPrefs.GetFloat("VolumenSFX")) * 20.0f);
        SFXTexto.text = (int)(PlayerPrefs.GetFloat("VolumenSFX") * 100.0f) + "%";

        pantallaCompleta.isOn = PlayerPrefs.GetInt("PantallaCompleta") != 0;
        Screen.fullScreen = pantallaCompleta.isOn;

        resolucion.value = PlayerPrefs.GetInt("Resolucion");
        Screen.SetResolution(resolucionAnchos[resolucion.value], resolucionAltos[resolucion.value], Screen.fullScreen);

        if (info.tipoPartida == TipoPartida.JugadorVSMaquina) botonAcelerar.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) menuOpcionesAnimator.SetBool("Abierto", !menuOpcionesAnimator.GetBool("Abierto"));
        if (Input.GetKeyDown(KeyCode.Tab)) listaTop.SetActive(!listaTop.activeSelf);
    }

    public void OpcionesPulsado()
    {
        menuOpcionesAnimator.SetBool("Abierto", true);
    }

    public void CerrarPulsado()
    {
        menuOpcionesAnimator.SetBool("Abierto", false);
    }

    public void CambiarNumeroAgentes(int nuevoNumero)
    { 
        numeroVivos.text = nuevoNumero.ToString();
    }

    public void SalirPulsado()
    {
        menuConfirmacion.SetActive(true);
    }

    public void AceptarPulsado()
    {
        SceneManager.LoadScene("MenuPrincipal");
    }

    public void CancelarPulsado()
    {
        menuConfirmacion.SetActive(false);
    }

    public void ReiniciarPulsado()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void AbrirMenuFinalPartida()
    {
        menuFinalPartida.SetActive(true);
        Time.timeScale = 1.0f;
    }

    public void CambiarTextoFinal(string textoFinal, Color color)
    {
        textoFinalPartida.text = textoFinal;
        textoFinalPartida.color = color;
    }

    public void ActualizarBarraAudioMusica(float nuevoValor)
    {
        PlayerPrefs.SetFloat("VolumenMusica", nuevoValor);
        PlayerPrefs.Save();
        mixer.SetFloat("Musica", Mathf.Log10(nuevoValor) * 20.0f);
        musicaTexto.text = (int)(nuevoValor * 100.0f) + "%";
    }

    public void ActualizarBarraAudioSFX(float nuevoValor)
    {
        PlayerPrefs.SetFloat("VolumenSFX", nuevoValor);
        PlayerPrefs.Save();
        mixer.SetFloat("Sonidos", Mathf.Log10(nuevoValor) * 20.0f);
        SFXTexto.text = (int)(nuevoValor * 100.0f) + "%";
    }

    public void CambiarPantallaCompleta(bool valor)
    {
        if (valor)
        {
            PlayerPrefs.SetInt("PantallaCompleta", 1);
            PlayerPrefs.Save();
        }
        else
        {
            PlayerPrefs.SetInt("PantallaCompleta", 0);
            PlayerPrefs.Save();
        }
        Screen.fullScreen = pantallaCompleta.isOn;
    }

    public void CambiarResolucion(Int32 valor)
    {
        PlayerPrefs.SetInt("Resolucion", valor);
        PlayerPrefs.Save();

        resolucion.value = PlayerPrefs.GetInt("Resolucion");
        Screen.SetResolution(resolucionAnchos[resolucion.value], resolucionAltos[resolucion.value], Screen.fullScreen);
    }

    public void AcelerarPartida()
    {
        velocidadPartidaActual = (velocidadPartidaActual + 1) % velocidadPartida.Length;
        Time.timeScale = velocidadPartida[velocidadPartidaActual];
        textoVelocidad.text = "x" + Time.timeScale.ToString("0.0");
    }
}