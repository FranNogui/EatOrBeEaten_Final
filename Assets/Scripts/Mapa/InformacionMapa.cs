using UnityEngine;

/// <summary>
/// Enumerable para distinguir los distintos tamanyos del mapa.
/// </summary>
public enum TamanyoMapa
{
    Pequenyo, 
    Mediano, 
    Grande
}

/// <summary>
/// Enumerable para distinguir el tipo de partida según si juega una persona o no
/// </summary>
public enum TipoPartida
{
    MaquinaVSMaquina, 
    JugadorVSMaquina
}

/// <summary>
/// ScriptableObject con la información necesaria para iniciar las partidas.
/// </summary>
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/InfoMapa", order = 1)]
public class InformacionMapa : ScriptableObject
{
    public TipoPartida tipoPartida;
    public TamanyoMapa tamanyo;
    public int numeroJugadores;
    public int numeroJugadoresIA;
}