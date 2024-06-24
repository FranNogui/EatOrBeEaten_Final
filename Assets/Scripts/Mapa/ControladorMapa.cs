using TMPro;
using UnityEngine;

/// <summary>
/// Clase encargada de iniciar ciertos parámetros del Alimento Spawner y Controlador de Agentes,
/// preparación del entorno jugable y actualización del contador de la partida. 
/// </summary>
public class ControladorMapa : MonoBehaviour
{
    [Header("Informacion necesaria")]

    [Tooltip("ScriptableObject con la información de la partida.")]
    [SerializeField] InformacionMapa informacionMapa;

    [Header("Objetos para los limites y fondo")]

    [Tooltip("Objeto que contiene los bordes del mapa.")]
    [SerializeField] Transform limites;

    [Tooltip("Objeto con las coordenadas máximas y mínimas de aparición.")]
    [SerializeField] Transform coordenadasLimites;

    [Tooltip("Sprite del fondo donde se ubican los elementos.")]
    [SerializeField] SpriteRenderer fondo;

    [Header("Controladores necesarios")]

    [Tooltip("Alimento Spawner de la partida.")]
    [SerializeField] AlimentoSpawner alimentoSpawner;

    [Tooltip("Controlador de Agentes de la partida.")]
    [SerializeField] ControladorAgentes controladorAgentes;

    [Header("Elementos de la interfaz")]

    [Tooltip("Contador de tiempo de la interfaz.")]
    [SerializeField] TextMeshProUGUI contadorTexto;

    float contador;

    void Awake()
    {
        GameObject bordesPequenyo, bordesMediano, bordesGrande;
        Vector3 limiteMinimoPequenyo, limiteMaximoPequenyo;
        Vector3 limiteMinimoMediano, limiteMaximoMediano;
        Vector3 limiteMinimoGrande, limiteMaximoGrande;

        limiteMinimoPequenyo = coordenadasLimites.Find("Pequenyo").Find("Min").position;
        limiteMaximoPequenyo = coordenadasLimites.Find("Pequenyo").Find("Max").position;
        limiteMinimoMediano  = coordenadasLimites.Find("Mediano").Find("Min").position;
        limiteMaximoMediano  = coordenadasLimites.Find("Mediano").Find("Max").position;
        limiteMinimoGrande   = coordenadasLimites.Find("Grande").Find("Min").position;
        limiteMaximoGrande   = coordenadasLimites.Find("Grande").Find("Max").position;

        bordesPequenyo = limites.Find("Pequenyo").gameObject;
        bordesMediano  = limites.Find("Mediano").gameObject;
        bordesGrande   = limites.Find("Grande").gameObject;

        bordesPequenyo.SetActive(false);
        bordesMediano.SetActive(false);
        bordesGrande.SetActive(false);

        contador = 0.0f;
        Time.timeScale = 1.0f;  
        switch (informacionMapa.tamanyo)
        {
            case TamanyoMapa.Pequenyo:
                bordesPequenyo.SetActive(true);
                EstablecerParametros(limiteMinimoPequenyo, limiteMaximoPequenyo, 1000);
                break;
            case TamanyoMapa.Mediano:
                bordesMediano.SetActive(true);
                EstablecerParametros(limiteMinimoMediano, limiteMaximoMediano, 4000);
                break;
            case TamanyoMapa.Grande:
                bordesGrande.SetActive(true);
                EstablecerParametros(limiteMinimoGrande, limiteMaximoGrande, 8000);
                break;
        }
    }

    void Update()
    {
        contador += Time.deltaTime;
        contadorTexto.text = ((int)(contador / 60.0f)).ToString("00") + ":" + ((int)(contador % 60.0f)).ToString("00");
    }

    /// <summary>
    /// Método para establecer los parametros necesarios de Alimento Spawner, Controlador de Agentes y del fondo.
    /// </summary>
    /// <param name="limiteMinimo">Límite inferior izquierdo de aparición.</param>
    /// <param name="limiteMaximo">Límite superior derecho de aparación.</param>
    /// <param name="numeroAlimentos">Número máximo de alimentos simultáneos.</param>
    void EstablecerParametros(Vector3 limiteMinimo, Vector3 limiteMaximo, int numeroAlimentos)
    {
        alimentoSpawner.MinimaPosicion = limiteMinimo;
        alimentoSpawner.MaximaPosicion = limiteMaximo;
        alimentoSpawner.MaximosAlimentos = numeroAlimentos;
        controladorAgentes.MinPos = limiteMinimo;
        controladorAgentes.MaxPos = limiteMaximo;
        fondo.size = limiteMaximo - limiteMinimo;
    }
}