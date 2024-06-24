using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Policies;

/// <summary>
/// Clase encargada del comportamineto principal de los agentes.
/// </summary>
public class ControladorAgente : MonoBehaviour
{
    [Header("Variables del agente")]

    [Tooltip("Velocidad del agente.")]
    [SerializeField] float velocidad = 6.0f;

    [Tooltip("Cuanto decrece la velocidad por unidad de volumen aumentada.")]
    [SerializeField] float decrementoVelocidadPorTamanyo = 0.01f;

    [Tooltip("Distancia de visión inicial del agente.")]
    [SerializeField] float distanciaVision = 5.0f;

    [Tooltip("Diferencia de tamaño porcentual para que un agente pueda eliminar a otro.")]
    [SerializeField] float margenVolumenPorcentual = 1.25f;

    const int numeroRayCasts = 32;
    ObjetoDeRayCast[] objetos;
    RaycastHit2D[] hits;
    float[] dirX;
    float[] dirY;
    Vector2 direccionRayCastActual;
    float distanciaRayCast;
    Transform objetoHit;
    ControladorAgente agenteHit;
    ControladorAlimento alimentoHit;
    BehaviorParameters parameters;

    int id;
    bool eliminado;
    bool ganador;
    float probabilidadParpadeo = 0.0f;
    readonly float incrementoProbabilidadParpadeo = 0.001f;
    float tamanyoProporcion = 1.0f;
    Vector2 movimiento;
    Vector2 movimientoPrevio;

    ControladorOjos controladorOjos;
    ControladorTamanyo controladorTamanyo;
    Rigidbody2D cuerpo;
    ModuloMovimiento moduloMovimiento;
    Animator animador;
    ControladorSonidosAgente controladorSonidos;
    Collider2D colision;
    Collider2D colisionInterior1;
    Collider2D colisionInterior2;
    ControladorNombre controladorNombre;
    ControladorAgentes controladorAgentes;
    List<ControladorAgente> agentesEnContacto;

    IARedNeuronal redNeuronal;
    //float controlPeligro;

    void Awake()
    {
        dirX = new float[numeroRayCasts];
        dirY = new float[numeroRayCasts];

        objetos = new ObjetoDeRayCast[numeroRayCasts];
        for (int i = 0; i < numeroRayCasts; i++)
        {
            dirX[i] = Mathf.Cos(i * (2 * Mathf.PI / numeroRayCasts));
            dirY[i] = Mathf.Sin(i * (2 * Mathf.PI / numeroRayCasts));
            objetos[i] = new ObjetoDeRayCast
            { tipo = TipoObjetoRayCast.Nada };
        }

        agentesEnContacto = new List<ControladorAgente>();
        hits = new RaycastHit2D[numeroRayCasts];
        direccionRayCastActual = Vector2.zero;
        //controlPeligro = 0.0f;
    }

    void Start()
    {
        controladorOjos    = GetComponent<ControladorOjos>();
        controladorTamanyo = GetComponent<ControladorTamanyo>();
        cuerpo             = GetComponent<Rigidbody2D>();
        animador           = GetComponent<Animator>();
        moduloMovimiento   = GetComponent<ModuloMovimiento>();
        controladorSonidos = GetComponent<ControladorSonidosAgente>();
        colision           = GetComponent<Collider2D>();
        controladorNombre  = GetComponent<ControladorNombre>();
        redNeuronal        = GetComponent<IARedNeuronal>();
        parameters         = GetComponent<BehaviorParameters>();

        colisionInterior1 = transform.Find("ColliderInterior").GetComponent<Collider2D>();
        colisionInterior2 = transform.Find("ColliderInteriorBordes").GetComponent<Collider2D>();

        movimiento = Vector2.zero;
        movimientoPrevio = movimiento;
        eliminado = false;
        ganador = false;
        parameters.TeamId = id;
        //parameters.BehaviorName = "IARedNeuronal" + (id%20).ToString();
    }

    void Update()
    {
        if (eliminado)
        {
            if (moduloMovimiento.RedNeuronalActiva) redNeuronal.enabled = false;
            return;
        }

        if (Input.GetKeyDown(KeyCode.Q)) controladorNombre.CambiarEstado();
        Parpadear();
    }

    private void FixedUpdate()
    {
        if (!eliminado)
        {
            RayCasting();
            if (moduloMovimiento.RedNeuronalActiva)
            {
                //moduloMovimiento.Peligro = controlPeligro;
                redNeuronal.tamanyo = Volumen;
                redNeuronal.maxDistancia = distanciaRayCast;
            }
            movimiento = moduloMovimiento.ActualizarMovimiento(objetos, Volumen);
            if (movimiento.magnitude > 1.0f) { movimiento.Normalize(); }
            movimiento = Vector3.Lerp(movimiento, movimientoPrevio, 0.01f * Time.deltaTime);            
            movimientoPrevio = movimiento;
            controladorOjos.ActualizarPosicion(movimiento);
            movimiento *= (velocidad * tamanyoProporcion);
            cuerpo.velocity = movimiento;
        }
    }

    void LateUpdate()
    {
        if (eliminado) return;
        ComprobarEliminaciones();
    }

    /// <summary>
    /// Método encargado de manejar el parpadeo del agente.
    /// </summary>
    void Parpadear()
    {
        if (Random.Range(0.0f, 1.0f) < probabilidadParpadeo)
        {
            probabilidadParpadeo = 0.0f;
            animador.SetTrigger("Parpadeo");
        }
        else
        { probabilidadParpadeo += Time.deltaTime * incrementoProbabilidadParpadeo; }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("ColliderInterior"))
        {
            ControladorAgente otro = collision.transform.parent.GetComponent<ControladorAgente>();
            if (!otro.eliminado)
            {
                agentesEnContacto.Add(otro);
            }
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("ColliderInterior"))
        {
            ControladorAgente otro = collision.transform.parent.GetComponent<ControladorAgente>();
            agentesEnContacto.Remove(otro);
        }
    }

    /// <summary>
    /// Método encargado de comprobar si se puede eliminar alguno de los agentes que se encuentran en contacto
    /// </summary>
    void ComprobarEliminaciones()
    {
        for (int i = agentesEnContacto.Count - 1; i >= 0; i--) 
        {
            ControladorAgente agente = agentesEnContacto[i];
            if (agente.Eliminado) agentesEnContacto.RemoveAt(i);
            else if (agente.Volumen * margenVolumenPorcentual <= Volumen)
            {
                agentesEnContacto.RemoveAt(i);
                AumentarTamanyo(agente.Volumen);
                agente.Eliminar(id);
            }
        }
    }

    /// <summary>
    /// Método para eliminar este agente.
    /// </summary>
    /// <param name="idEliminador">ID del otro agente que ha eliminado a este.</param>
    void Eliminar(int idEliminador)
    {
        if (moduloMovimiento.RedNeuronalActiva)
        { 
            //redNeuronal.SetReward(-2.0f); 
            //redNeuronal.EndEpisode();
        }
        eliminado = true;
        colision.enabled = false;
        colisionInterior1.enabled = false;
        colisionInterior2.enabled = false;
        controladorNombre.OcultarNick();
        controladorSonidos.ReproducirPop();
        controladorAgentes.EliminarAgente(id, idEliminador);
    }

    /// <summary>
    /// Método para manejar el aumento de tamaño del agente.
    /// </summary>
    /// <param name="volumen">Cantidad de volumen a aumentar.</param>
    public void AumentarTamanyo(float volumen)
    {
        controladorTamanyo.CambiarTamanyo(volumen);
        tamanyoProporcion = (1 / (Mathf.Log(controladorTamanyo.VolumenActual * decrementoVelocidadPorTamanyo + 1) + 1));
        //if (moduloMovimiento.RedNeuronalActiva) redNeuronal.AddReward(Mathf.Max((volumen / Volumen), 0.1f) * (1.0f - controlPeligro));
    }

    /// <summary>
    /// Método para obtener un color aleatorio siguiendo la regla de que al menos una de las componentes RGB debe ser máxima.
    /// </summary>
    public void RandomizarColor()
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
        transform.Find("Color").GetComponent<SpriteRenderer>().color = new Color(r, g, b);
    }

    public void ResetearEstado()
    {
        agentesEnContacto = new List<ControladorAgente>();
        if (moduloMovimiento.RedNeuronalActiva)
        {
            if (redNeuronal.enabled)
            {
                /*
                if (!eliminado && ganador) redNeuronal.AddReward(1.0f);
                else redNeuronal.SetReward(-1.0f);
                */
                redNeuronal.EndEpisode();
            }
            redNeuronal.enabled = true;
        }
        eliminado = false;
        ganador = false;
        colision.enabled = true;
        colisionInterior1.enabled = true;
        colisionInterior2.enabled = true;
        controladorNombre.MostrarNick();
        controladorTamanyo.ResetearTamanyo(); 
    }

    /// <summary>
    /// Método encargado de realizar los RayCasts para detectar los elementos que se encuentran alrededor del agente.
    /// </summary>
    void RayCasting()
    {
        distanciaRayCast = distanciaVision * transform.localScale.x / 2.0f;
        //controlPeligro = 0.0f;
        for (int i = 0; i < numeroRayCasts; i++)
        {
            direccionRayCastActual.x = dirX[i];
            direccionRayCastActual.y = dirY[i];
            hits[i] = Physics2D.Raycast(transform.position, direccionRayCastActual, distanciaRayCast);
            if (hits[i])
            {
                Debug.DrawRay(transform.position, direccionRayCastActual * hits[i].distance, Color.yellow);
                objetoHit   = hits[i].transform;
                agenteHit   = objetoHit.GetComponent<ControladorAgente>();
                alimentoHit = objetoHit.GetComponent<ControladorAlimento>();

                if (agenteHit != null)
                {
                    objetos[i].tipo = TipoObjetoRayCast.Agente;
                    objetos[i].posicion = objetoHit.localPosition;
                    objetos[i].tamanyo = agenteHit.Volumen;
                    objetos[i].distancia = hits[i].distance;
                    /*
                    float relTamanyo = objetos[i].tamanyo / Volumen;
                    float relDistancia = objetos[i].distancia / distanciaRayCast;
                    if (relTamanyo >= 1.0f)
                    {
                        relTamanyo = 1.0f / (relTamanyo * relTamanyo); 
                        controlPeligro += (relTamanyo * 0.5f + relDistancia * 0.5f) / numeroRayCasts;
                    }
                    */
                }
                else if (alimentoHit != null)
                {
                    objetos[i].tipo = TipoObjetoRayCast.Alimento;
                    objetos[i].posicion = objetoHit.localPosition;
                    objetos[i].tamanyo = alimentoHit.Volumen;
                    objetos[i].distancia = hits[i].distance;
                }
                else
                {
                    objetos[i].tipo = TipoObjetoRayCast.Borde;
                    objetos[i].posicion = Vector2.zero;
                    objetos[i].tamanyo = -1.0f;
                    objetos[i].distancia = hits[i].distance;
                }
            }
            else
            {
                objetos[i].tipo = TipoObjetoRayCast.Nada;
                objetos[i].posicion = Vector2.zero;
                objetos[i].tamanyo = 0.0f;
                objetos[i].distancia = 0.0f;
            }
        }
    }

    /// <summary>
    /// Mètodo para seleccionar al agente y que lo siga la cámara.
    /// </summary>
    public void SeleccionarAgente()
    {
        controladorAgentes.SeleccionarAgente(id);
    }

    //** Getters y Setters **//

    /// <summary>Conjunto de componentes X de los vectores RayCast.</summary>
    public float[] DirX
    { get { return dirX; } }

    /// <summary>Conjunto de componentes Y de los vectores RayCast.</summary>
    public float[] DirY 
    { get { return dirY; } }

    /// <summary>Diferencia de tamaño porcentual para que un agente pueda eliminar a otro.</summary>
    public float MargenVolumenPorcentual
    { get { return margenVolumenPorcentual; } }

    /// <summary>Velocidad del agente.</summary>
    public float Velocidad
    {
        set { velocidad = value; }
        get { return velocidad; }
    }

    /// <summary>Velocidad tras aplicar la proporcion.</summary>
    public float VelocidadProporcional
    { get { return velocidad * tamanyoProporcion; } }

    /// <summary>Booleano para indicar si el agente esta eliminado.</summary>
    public bool Eliminado
    { get { return eliminado; } }

    public bool Ganador
    { set { ganador = value; } }

    /// <summary>ID del agente dentro del Controlador Agentes.</summary>
    public int ID
    {
        get { return id; }
        set { id = value; }
    }

    /// <summary>Objeto Controlador Agentes que organiza a este agente.</summary>
    public ControladorAgentes ControladorAgentes
    { set { controladorAgentes = value; } }

    /// <summary>Volumen actual del agente.</summary>
    public float Volumen
    { get { return controladorTamanyo.VolumenActual; } }

    
    //** FIN Getters y Setters **//
}