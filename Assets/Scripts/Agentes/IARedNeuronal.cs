using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class IARedNeuronal : Agent
{
    public Vector2 mov;
    public float tamanyo;
    public ObjetoDeRayCast[] objetos;

    public float maxDistancia = 5.0f;

    public void Awake()
    { objetos = new ObjetoDeRayCast[0]; }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(mov);
        foreach (var observation in objetos)
        {
            float relTamanyo = observation.tamanyo / tamanyo;
            if (relTamanyo <= 0.8f) relTamanyo *= 1.25f;
            else if (relTamanyo < 1.0f) relTamanyo = -1.0f;
            else relTamanyo *= -1.0f;
            sensor.AddObservation((int) (observation.tipo == TipoObjetoRayCast.Agente ? TipoObjetoRayCast.Alimento : observation.tipo));
            sensor.AddObservation(observation.distancia / maxDistancia);
            sensor.AddObservation(relTamanyo);
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        mov = new Vector2(actions.ContinuousActions[0], actions.ContinuousActions[1]).normalized;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> acciones = actionsOut.ContinuousActions;
        acciones[0] = mov.x;
        acciones[1] = mov.y;
    }
}
