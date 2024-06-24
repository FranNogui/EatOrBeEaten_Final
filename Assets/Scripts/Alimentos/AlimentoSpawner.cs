using System.Collections;
using UnityEngine;

/// <summary>
/// Clase encargada de controlar la aparición de los alimentos en la partida.
/// </summary>
public class AlimentoSpawner : MonoBehaviour
{
    [Header("Objetos necesarios")]

    [Tooltip("Transform donde crear y almacenar todos los alimentos necesarios.")]
    [SerializeField] Transform almacenAlimentos;

    [Tooltip("Prefab de un alimento.")]
    [SerializeField] GameObject alimento;

    [Header("Variables de delay")]
    
    [Tooltip("Mínimo tiempo que puede tardar en spawnear un alimento.")]
    [SerializeField] float minimoDelay;

    [Tooltip("Máximo tiempo que puede tardar en spawnear un alimento.")]
    [SerializeField] float maximoDelay;

    float maximoDelayPre;
    float minimoDelayPre;
    int maximosAlimentos;
    int numeroAlimentos;
    int primerAlimentoDisponible;
    Vector2 minimaPosicion;
    Vector2 maximaPosicion;

    GameObject[]          alimentos;
    ControladorAlimento[] alimentosAlimentoControlador;

    void Start()
    {
        alimentos = new GameObject[maximosAlimentos];
        alimentosAlimentoControlador = new ControladorAlimento[maximosAlimentos];

        for (int i = 0; i < maximosAlimentos; i++)
        {
            alimentos[i] = Instantiate(alimento);
            alimentosAlimentoControlador[i] = alimentos[i].GetComponent<ControladorAlimento>();

            alimentos[i].transform.parent = almacenAlimentos;
            alimentosAlimentoControlador[i].AlimentoSpawner = this;
            alimentosAlimentoControlador[i].ID = i;
            alimentos[i].SetActive(false);
            alimentos[i].transform.position = Vector3.zero;
        }

        maximoDelayPre = maximoDelay;
        minimoDelayPre = minimoDelay;
        primerAlimentoDisponible = 0;
        numeroAlimentos = 0;
        StartCoroutine(SpawnearAlimento());
    }

    /// <summary>
    /// Corutina encargada de spwnear alimentos de forma periódica.
    /// </summary>
    IEnumerator SpawnearAlimento()
    {
        Vector3 vectorRandom = Vector3.zero;
        float delay;

        while (true)
        {
            for (int i = primerAlimentoDisponible; i < maximosAlimentos; i++)
            {
                if (!alimentos[i].activeSelf)
                {
                    alimentos[i].SetActive(true);
                    alimentosAlimentoControlador[i].ReiniciarAlimento();

                    vectorRandom.x = Random.Range(minimaPosicion.x, maximaPosicion.x);
                    vectorRandom.y = Random.Range(minimaPosicion.y, maximaPosicion.y);
                    alimentos[i].transform.position = vectorRandom;

                    maximoDelay -= Time.deltaTime * 0.001f; 
                    maximoDelay = Mathf.Max(maximoDelay, 0.0f);
                    minimoDelay -= Time.deltaTime * Time.deltaTime; 
                    minimoDelay = Mathf.Max(minimoDelay, 0.0f);

                    numeroAlimentos++;
                    primerAlimentoDisponible++;
                    break;
                }
            }

            delay = ((float) numeroAlimentos / maximosAlimentos) * (maximoDelay - minimoDelay) + minimoDelay;
            yield return new WaitForSeconds(delay);
        }
    }

    public void Resetear()
    {
        maximoDelay = maximoDelayPre;
        minimoDelay = minimoDelayPre;
        for (int i = 0; i < maximosAlimentos; i++)
        {  
            alimentos[i].SetActive(false);
            primerAlimentoDisponible = 0;
            numeroAlimentos = 0;
        }
    }

    /// <summary>
    /// Método para marcar un alimento de la lista como eliminado.
    /// </summary>
    /// <param name="id">Identificador del alimento en este Alimento Spawner.</param>
    public void DestruirAlimento(int id)
    {
        alimentos[id].SetActive(false);
        if (id < primerAlimentoDisponible) primerAlimentoDisponible = id;
        numeroAlimentos--;
    }

    //** Getters y Setters **//

    /// <summary>Vector2 que representa la esquina inferior izquierda del área donde spawnean los alimentos.</summary>
    public Vector2 MinimaPosicion { set { minimaPosicion = value; } }

    /// <summary>Vector2 que representa la esquina superior derecha del área donde spawnean los alimentos.</summary>
    public Vector2 MaximaPosicion { set { maximaPosicion = value; } }

    /// <summary>Cantidad máxima de alimentos simultáneos que pueden haber en la partida.</summary>
    public int MaximosAlimentos { set { maximosAlimentos = value; } }

    //** FIN Getters y Setters **//
}