using System;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ControladorMenuPrincipal : MonoBehaviour
{
    readonly int[] resolucionAnchos = { 640, 1280, 1366, 1600, 1920, 2560, 3200, 3840, 5120, 7680 };
    readonly int[] resolucionAltos  = { 360, 720, 768, 900, 1080, 1440, 1800, 2160, 2880, 4320 };

    [Header("Menu de juego")]
    [SerializeField] GameObject menuJugar;
    Animator   menuJugarAnimator;
    GameObject menuJugarOpciones1;
    GameObject menuJugarOpciones2;

    [SerializeField] TextMeshProUGUI numeroJugadores;
    [SerializeField] TextMeshProUGUI porcentajeIA;

    [SerializeField] Image  imagenElegido;
    [SerializeField] Sprite imagenHumano;
    [SerializeField] Sprite imagenComputador;

    [Header("Menu de opciones")]
    [SerializeField] GameObject menuOpciones;
    [SerializeField] Slider volumenMusica;
    [SerializeField] Slider volumenSFX;
    [SerializeField] TextMeshProUGUI musicaTexto;
    [SerializeField] TextMeshProUGUI SFXTexto;
    [SerializeField] AudioMixer mixer;
    [SerializeField] Toggle pantallaCompleta;
    [SerializeField] TMP_Dropdown resolucion;
    Animator menuOpcionesAnimator;

    [Header("Informacion del mapa")]
    [SerializeField] InformacionMapa info;

    [Header("Opciones de partida")]
    [SerializeField] Slider sliderJugadores;
    [SerializeField] Slider sliderIA;
    [SerializeField] Toggle pequenyo;
    [SerializeField] Toggle mediano;
    [SerializeField] Toggle grande;

    [Header("Controles")]
    [SerializeField] GameObject menuControles;
    Animator menuControlesAnimator;

    [Header("Creditos")]
    [SerializeField] GameObject creditos;
    Animator creditosAnimator;

    [Header("Nombre de usuario")]
    [SerializeField] TMP_InputField nombreUsuario;


    void Start()
    {
        Time.timeScale = 1.0f;
        menuJugarAnimator  = menuJugar.GetComponent<Animator>();
        menuJugarOpciones1 = menuJugar.transform.Find("Opciones1").gameObject;
        menuJugarOpciones2 = menuJugar.transform.Find("Opciones2").gameObject;
        volumenMusica.value = PlayerPrefs.GetFloat("VolumenMusica");
        volumenSFX.value = PlayerPrefs.GetFloat("VolumenSFX");

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

        menuOpcionesAnimator = menuOpciones.GetComponent<Animator>();
        menuControlesAnimator = menuControles.GetComponent<Animator>();
        creditosAnimator = creditos.GetComponent<Animator>();

        if (!PlayerPrefs.HasKey("NombreJugador"))
        {
            PlayerPrefs.SetString("NombreJugador", "TuNick");
            PlayerPrefs.Save();
        }
        nombreUsuario.text = PlayerPrefs.GetString("NombreJugador");
    }

    public void JugarPulsado()
    {
        menuJugarAnimator.SetBool("Abierto", true);
        menuJugarOpciones1.SetActive(true);
        menuJugarOpciones2.SetActive(false);
    }

    public void OpcionesPulsado()
    {
        menuOpcionesAnimator.SetBool("Abierto", true);
    }

    public void SalirPulsado()
    {
        Application.Quit();
    }

    public void CerrarMenuOpcionesPulsado()
    {
        menuOpcionesAnimator.SetBool("Abierto", false);
    }

    public void CerrarMenuJugarPulsado()
    {
        menuJugarAnimator.SetBool("Abierto", false);
    }

    public void JugadorContraMaquinaPulsado()
    {
        menuJugarOpciones1.SetActive(false);
        menuJugarOpciones2.SetActive(true);
        imagenElegido.sprite = imagenHumano;
        info.tipoPartida = TipoPartida.JugadorVSMaquina;
    }

    public void MaquinaContraMaquinaPulsado()
    {
        menuJugarOpciones1.SetActive(false);
        menuJugarOpciones2.SetActive(true);
        imagenElegido.sprite = imagenComputador;
        info.tipoPartida = TipoPartida.MaquinaVSMaquina;
    }

    public void ActualizarNumeroJugadores(float nuevoNumero)
    {
        numeroJugadores.text = nuevoNumero.ToString();
    }

    public void ActualizarPorcentajeIA(float nuevoPorcentaje)
    {
        porcentajeIA.text = nuevoPorcentaje.ToString() + "%";
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

    public void CancelarPartidaPusado()
    {
        menuJugarOpciones1.SetActive(true);
        menuJugarOpciones2.SetActive(false);
    }

    public void AceptarPartidaPulsado()
    {
        if      (pequenyo.isOn) info.tamanyo = TamanyoMapa.Pequenyo;
        else if (mediano.isOn)  info.tamanyo = TamanyoMapa.Mediano;
        else if (grande.isOn)   info.tamanyo = TamanyoMapa.Grande;

        info.numeroJugadores = (int) sliderJugadores.value;
        info.numeroJugadoresIA = (int) Math.Floor(sliderJugadores.value * sliderIA.value / 100.0f);

        SceneManager.LoadScene("EscenaJuego");
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

    public void AbrirMenuControles()
    {
        menuControlesAnimator.SetBool("Abierto", true);
    }

    public void CerrarMenuControles()
    {
        menuControlesAnimator.SetBool("Abierto", false);
    }

    public void AbrirCreditos()
    {
        creditosAnimator.SetBool("Abierto", true);
    }

    public void CerrarCreditos()
    {
        creditosAnimator.SetBool("Abierto", false);
    }

    public void ActualizarNombreUsuario(string nuevoNombre)
    {
        PlayerPrefs.SetString("NombreJugador", nuevoNombre);
        PlayerPrefs.Save();
    }
}