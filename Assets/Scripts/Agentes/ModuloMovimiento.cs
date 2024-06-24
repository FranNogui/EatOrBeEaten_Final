using UnityEngine;

/// <summary>
/// Enumerable para distingir los distintos elementos que pueden controlar a los agentes.
/// </summary>
public enum TipoAgente
{
    Jugador, 
    IAProgramada,
    RedNeuronal
}

/// <summary>
/// Clase encargada de decidir que módulo moverá al agente.
/// </summary>
public class ModuloMovimiento : MonoBehaviour
{
    [Tooltip("Elemento que moverá al agente.")]
    [SerializeField] TipoAgente tipoAgente;
    [SerializeField] IARedNeuronal redNeuronal;
    [SerializeField] bool grabandoDemos;

    IAProgramada iaProgramada;

    //float tamanyoPrev;
    //float tiempo;
    //float peligro;
    //readonly float maxTiempo = 2.0f;

    void Awake()
    { 
        if (RedNeuronalActiva) redNeuronal.enabled = true;
        else redNeuronal.enabled = false;
        iaProgramada = new IAProgramada(GetComponent<ControladorAgente>(), transform);
        //tamanyoPrev = 0.0f;
        //tiempo = 0.0f;
    }

    /// <summary>
    /// Método para cambiar el elemento que mueve al agente.
    /// </summary>
    /// <param name="nuevoTipo">Nuevo elemento.</param>
    public void CambiarA(TipoAgente nuevoTipo)
    {
        tipoAgente = nuevoTipo;
        if (RedNeuronalActiva) redNeuronal.enabled = true;
        else redNeuronal.enabled = false;
    }

    /// <summary>
    /// Método para obtener el vector resultado de ejecutar un paso de movimiento.
    /// </summary>
    /// <param name="objetos">Lista de objetos que el agente ve a través de los RayCast.</param>
    public Vector2 ActualizarMovimiento(ObjetoDeRayCast[] objetos, float tamanyo)
    {
        
        if (RedNeuronalActiva)
        {
            /*
            if (tamanyo == tamanyoPrev && tiempo <= maxTiempo) tiempo += Time.deltaTime;
            else if (tamanyo != tamanyoPrev) tiempo = 0.0f;
            if (tiempo >= maxTiempo && redNeuronal.GetCumulativeReward() > -1.0f)
                redNeuronal.AddReward(- Time.deltaTime * (1.0f + peligro));
            */
            redNeuronal.objetos = objetos;
            //tamanyoPrev = tamanyo;
        }

        /*
        if (tamanyoPrev == tamanyo)
        {  tiempo += Time.deltaTime; }
        else
        {
            tamanyoPrev = tamanyo;
            tiempo = 1.0f;
        }
        redNeuronal.SetReward(tamanyo / tiempo);
        redNeuronal.objetos = objetos;
        */


        switch (tipoAgente)
        {
            case TipoAgente.Jugador:
                return Vector2.up * Input.GetAxis("Vertical") + Vector2.right * Input.GetAxis("Horizontal");
            case TipoAgente.IAProgramada:
                Vector2 res = iaProgramada.Resultado(objetos);
                /*
                if (grabandoDemos)
                {
                    redNeuronal.mov = res;
                    redNeuronal.RequestDecision();
                }       
                */
                return res;
            case TipoAgente.RedNeuronal:
                redNeuronal.RequestDecision();
                return redNeuronal.mov;
            default:
                return Vector2.zero;
        }
    }

    public bool RedNeuronalActiva
    { get { return tipoAgente == TipoAgente.RedNeuronal || grabandoDemos; } }

    /*
    public float Peligro
    { set { peligro = value; } }
    */
}