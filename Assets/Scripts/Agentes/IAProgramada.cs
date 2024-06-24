using UnityEngine;

public class IAProgramada
{
    const float maximoTiempo = 2.0f;
    const float valorDistancia = 2.0f;
    const float valorTamanyo = 0.5f;
    
    readonly ControladorAgente agente;
    readonly Transform miPosicion;
    ObjetoDeRayCast mejorAlimento;
    ObjetoDeRayCast peorEnemigo;

    bool esquivandoPared;
    Vector2 vectorRandom;
    Vector2 vectorDireccion;
    Vector3 huida;
    Vector3 recogida;
    float tiempoRandomActual;
    float dirPared;

    public IAProgramada(ControladorAgente agente, Transform miPosicion)
    {
        this.agente = agente;
        this.miPosicion = miPosicion;
        esquivandoPared = false;
        vectorRandom = Vector2.zero;
        vectorDireccion = Vector2.zero;
        huida = Vector3.zero;
        recogida = Vector3.zero;
        tiempoRandomActual = 2.0f;
        dirPared = 1.0f;
        mejorAlimento = null;
        peorEnemigo = null;
    }

    public Vector2 Resultado(ObjetoDeRayCast[] objetos)
    {
        mejorAlimento = null;
        peorEnemigo = null;
        foreach (var obj in objetos)
        {
            if (obj.tipo == TipoObjetoRayCast.Nada || obj.tipo == TipoObjetoRayCast.Borde) continue;
            if (obj.tipo == TipoObjetoRayCast.Agente && agente.Volumen * agente.MargenVolumenPorcentual < obj.tamanyo)
            {
                if (peorEnemigo == null || obj.distancia < peorEnemigo.distancia) peorEnemigo = obj;
            }
            if (mejorAlimento == null ||
                mejorAlimento.distancia * valorDistancia - mejorAlimento.tamanyo * valorTamanyo > obj.distancia * valorDistancia - obj.tamanyo * valorTamanyo)
            {
                if (obj.tipo == TipoObjetoRayCast.Agente && obj.tamanyo * agente.MargenVolumenPorcentual >= agente.Volumen) continue;
                mejorAlimento = obj;
            }
        }

        if (mejorAlimento == null && peorEnemigo == null)
        {
            if (tiempoRandomActual >= maximoTiempo)
            {
                tiempoRandomActual = 0.0f;
                dirPared *= -1.0f;
                if (!esquivandoPared)
                {
                    vectorRandom.x = Random.Range(-1.0f, 1.0f);
                    vectorRandom.y = Random.Range(-1.0f, 1.0f);
                }
            }
            else tiempoRandomActual += Time.deltaTime;

            esquivandoPared = false;
            for (int i = 0; i < objetos.Length; i++)
            {
                if (objetos[i].tipo == TipoObjetoRayCast.Borde && objetos[i].distancia < 5.0f)
                {
                    vectorDireccion.x = agente.DirX[i];
                    vectorDireccion.y = agente.DirY[i];
                    vectorDireccion.Normalize();

                    if (Vector2.Angle(vectorDireccion, vectorRandom) <= (360.0f / objetos.Length))
                    {
                        vectorRandom.Normalize();
                        vectorRandom.x += dirPared * vectorRandom.y;
                        vectorRandom.y -= dirPared * vectorRandom.x;
                        vectorRandom.Normalize();
                        esquivandoPared = true;
                        break;
                    }
                }
            }

            Debug.DrawRay(miPosicion.position, vectorRandom.normalized, Color.blue);
            return vectorRandom.normalized;
        }
        else if (peorEnemigo != null)
        {
            if (mejorAlimento != null && mejorAlimento.distancia * 5.0f < peorEnemigo.distancia)
            {
                recogida = ((mejorAlimento.posicion - miPosicion.localPosition)).normalized;
                esquivandoPared = false;
                Debug.DrawRay(miPosicion.localPosition, recogida, Color.green);
                vectorRandom = recogida;
                return recogida;
            }

            if (!esquivandoPared)
            {
                huida = ((peorEnemigo.posicion - miPosicion.localPosition) * -1.0f).normalized;
            }
            else huida = vectorRandom;

            vectorRandom = huida;

            esquivandoPared = false;
            for (int i = 0; i < objetos.Length; i++)
            {
                if (objetos[i].tipo == TipoObjetoRayCast.Borde && objetos[i].distancia < 10.0f)
                {
                    vectorDireccion.x = agente.DirX[i];
                    vectorDireccion.y = agente.DirY[i];
                    vectorDireccion.Normalize();

                    if (Vector2.Angle(vectorDireccion, huida) <= (360.0f / objetos.Length))
                    {
                        huida.Normalize();
                        huida.x += dirPared * huida.y;
                        huida.y -= dirPared * huida.x;
                        huida.Normalize();
                        vectorRandom = huida;
                        esquivandoPared = true;
                        if (Vector2.Angle(huida, ((peorEnemigo.posicion - miPosicion.localPosition) * -1.0f).normalized) >= 100.0f) dirPared *= -1.0f;
                        break;
                    }
                }
            }

            Debug.DrawRay(miPosicion.localPosition, huida, Color.red);
            return huida;
        }
        else
        {
            recogida = ((mejorAlimento.posicion - miPosicion.localPosition)).normalized;
            esquivandoPared = false;
            Debug.DrawRay(miPosicion.localPosition, recogida, Color.green);
            vectorRandom = recogida;
            return recogida;
        }
    }
}