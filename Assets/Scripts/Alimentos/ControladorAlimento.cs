using System.Collections;
using UnityEngine;

/// <summary>
/// Clase encargada de controlar el comportamiento que debe tener un alimento en la partida.
/// </summary>
public class ControladorAlimento : MonoBehaviour
{
    [Header("Valores del alimento")]

    [Tooltip("Volumen mínimo que debe tener un alimento.")]
    [SerializeField] float volumenMinimo = 0.1f;

    [Tooltip("Volumen máximo que puede tener un alimento.")]
    [SerializeField] float volumenMaximo = 1.0f;

    float volumen;
    bool comido;
    int id;

    SpriteRenderer  render;
    AlimentoSonidos sonidos;
    AlimentoSpawner alimentoSpawner;
    Collider2D      colision;

    void Awake()
    {
        sonidos  = GetComponent<AlimentoSonidos>();
        colision = GetComponent<Collider2D>();
        render   = transform.Find("Color").GetComponent<SpriteRenderer>();

        ReiniciarAlimento();
    }

    /// <summary>
    /// Método para resetear el estado del alimento.
    /// </summary>
    public void ReiniciarAlimento()
    {
        volumen = Random.Range(volumenMinimo, volumenMaximo);
        transform.localScale = Vector2.one * Mathf.Sqrt(volumen);
        render.color = ObtenerColorAleatorio();
        colision.enabled = true;
        comido = false; 
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (comido) return;
        ControladorAgente agente;
        
        agente = collision.gameObject.GetComponent<ControladorAgente>();
        if (agente != null && agente.Volumen >= volumen) 
        {
            comido = true;
            colision.enabled = false;
            agente.AumentarTamanyo(volumen);
            sonidos.ReproducirPop();
            StartCoroutine(Destruir(agente.transform));
        }
    }

    /// <summary>
    /// Método encargado de controlar el comportamiento del alimento mientras se destruye.
    /// </summary>
    /// <param name="posicionAgente">Transform con la posición del agente que ha destruido el alimento.</param>
    IEnumerator Destruir(Transform posicionAgente)
    {
        Vector3 vectorDireccion;
        float distanciaActual, distanciaNueva;

        distanciaActual = (transform.position - posicionAgente.position).magnitude;
        while (true)
        {
            if (transform.localScale.x > 0.1f)
            {
                distanciaNueva = (transform.position - posicionAgente.position).magnitude;
                transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, 1.0f * Time.deltaTime);
                distanciaActual -= 2.0f * Time.deltaTime;

                if (distanciaNueva <= distanciaActual) distanciaActual = distanciaNueva;
                else
                { 
                    if (distanciaActual < 0.0f) distanciaActual = 0.0f;
                    vectorDireccion = (transform.position - posicionAgente.position).normalized;
                    vectorDireccion.z = 0.0f;
                    transform.position = posicionAgente.position + vectorDireccion * distanciaActual;
                }  
            }
            else
            {
                alimentoSpawner.DestruirAlimento(id);
                break;
            }
            yield return null;
        }
        yield break;
    }

    /// <summary>
    /// Método para obtener un color aleatorio siguiendo la regla de que al menos una de las componentes RGB debe ser máxima.
    /// </summary>
    Color ObtenerColorAleatorio()
    {
        float r = 0, g = 0, b = 0;
        switch (Random.Range(0, 3))
        {
            case 0:
                r = 1.0f;
                g = Random.Range(0.0f, 1.0f);
                b = Random.Range(0.0f, 1.0f);
                break;
            case 1:
                g = 1.0f;
                r = Random.Range(0.0f, 1.0f);
                b = Random.Range(0.0f, 1.0f);
                break;
            case 2:
                b = 1.0f;
                r = Random.Range(0.0f, 1.0f);
                g = Random.Range(0.0f, 1.0f);
                break;
        }
        return new Color(r, g, b, 1.0f);
    }

    //** Getters y Setters **//

    /// <summary>Alimento Spawner que controla el comportamiento de este alimento en el mapa.</summary>
    public AlimentoSpawner AlimentoSpawner
    { set { alimentoSpawner = value; } }

    /// <summary>Volumen del alimento.</summary>
    public float Volumen
    { get { return volumen; } }

    /// <summary>Identificador del alimento dentro de su Alimento Spawner.</summary>
    public int ID
    {
        get { return id; }
        set { id = value; }
    }

    //** FIN Getters y Setters **//
}