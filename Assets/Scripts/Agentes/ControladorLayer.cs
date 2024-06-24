using UnityEngine;

/// <summary>
/// Clase encargada de controlar las layers donde se dibujan los agentes.
/// </summary>
public class ControladorLayer : MonoBehaviour
{
    readonly int valorMaximoCapa = 32767;

    [Header("Capas")]

    [Tooltip("Número máximo de capas para agentes.")]
    [SerializeField] int numeroCapas = 4;

    [Header("SpriteRenderers necesarios")]

    [Tooltip("SpriteRender del color base del agente.")]
    [SerializeField] SpriteRenderer Base;

    [Tooltip("SpriteRender del outline del agente.")]
    [SerializeField] SpriteRenderer Outline;

    [Tooltip("SpriteRenderes de los ojos del agente.")]
    [SerializeField] SpriteRenderer[] Ojos;

    [Tooltip("SpriteRenderes de los outlines de los ojos del agente.")]
    [SerializeField] SpriteRenderer[] OjosOutline;

    [Tooltip("SpriteRenderes de las pupilas del agente.")]
    [SerializeField] SpriteRenderer[] Pupilas;

    int layerBase, layerBaseActual;
    int layerOutline, layerOutlineActual;
    int layerOjo, layerOjoActual;
    int layerOjoOutline, layerOjoOutlineActual;
    int layerPupila, layerPupilaActual;
    int tamanyoExtra;

    ControladorTamanyo tamanyo;

    void Start()
    {
        tamanyo = GetComponent<ControladorTamanyo>();
        layerBase       = Base.sortingOrder;
        layerOutline    = Outline.sortingOrder;
        layerOjo        = Ojos[0].sortingOrder;
        layerOjoOutline = OjosOutline[0].sortingOrder;
        layerPupila     = Pupilas[0].sortingOrder;
    }

    void Update()
    {
        int nuevoExtra = (int)(tamanyo.VolumenActual * 10.0f);
        if (nuevoExtra == tamanyoExtra) return;
        ActualizarLayersActuales(nuevoExtra);
        Base.sortingOrder = layerBaseActual % valorMaximoCapa;
        Base.sortingLayerName = "Agentes" + Mathf.Min(layerBaseActual / valorMaximoCapa, numeroCapas);
        Outline.sortingOrder = layerOutlineActual % valorMaximoCapa;
        Outline.sortingLayerName = "Agentes" + Mathf.Min(layerOutlineActual / valorMaximoCapa, numeroCapas);
        foreach (var ojo in Ojos)
        {
            ojo.sortingOrder = layerOjoActual % valorMaximoCapa;
            ojo.sortingLayerName = "Agentes" + Mathf.Min(layerOjoActual / valorMaximoCapa, numeroCapas);
        }
        foreach (var ojoOutline in OjosOutline)
        {
            ojoOutline.sortingOrder = layerOjoOutlineActual % valorMaximoCapa;
            ojoOutline.sortingLayerName = "Agentes" + Mathf.Min(layerOjoOutlineActual / valorMaximoCapa, numeroCapas);
        }
        foreach (var pupila in Pupilas)
        {
            pupila.sortingOrder = layerPupilaActual % valorMaximoCapa;
            pupila.sortingLayerName = "Agentes" + Mathf.Min(layerPupilaActual / valorMaximoCapa, numeroCapas);
        }
    }

    /// <summary>
    /// Método para actualizar los valores de las layers actuales.
    /// </summary>
    /// <param name="nuevoExtra">Valor extra a añadir a las layers actuales.</param>
    void ActualizarLayersActuales(int nuevoExtra)
    {
        tamanyoExtra = nuevoExtra;
        layerBaseActual       = layerBase + tamanyoExtra;
        layerOutlineActual    = layerOutline + tamanyoExtra;
        layerOjoActual        = layerOjo + tamanyoExtra;
        layerOjoOutlineActual = layerOjoOutline + tamanyoExtra;
        layerPupilaActual     = layerPupila + tamanyoExtra;
    }
}