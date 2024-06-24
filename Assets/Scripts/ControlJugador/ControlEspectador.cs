using UnityEngine;

/// <summary>
/// Clase encargada de controlar el ente espectador de una partida.
/// </summary>
public class ControlEspectador : MonoBehaviour
{
    [Header("Velocidades")]

    [Tooltip("Velocidad de movimiento por defecto.")]
    [SerializeField] float velocidadNormal = 6.0f;

    [Tooltip("Velocidad de movimiento con sprint.")]
    [SerializeField] float velocidadSprint = 12.0f;

    [Header("Zooms")]

    [Tooltip("Máximo zoom que puede alcanzar la cámara.")]
    [SerializeField] float maxZoom   = 60.0f;

    [Tooltip("Mínimo zoom que debe tener la cámara.")]
    [SerializeField] float minZoom   = 3.0f;

    Camera camara;
    Rigidbody2D cuerpo;

    bool zoomHabilitado;
    float velocidad;
    Vector2 movimiento;

    void Start()
    {
        cuerpo = GetComponent<Rigidbody2D>();
        camara = Camera.main;
        velocidad = velocidadNormal;
        zoomHabilitado = true;
    }

    void Update()
    {
        movimiento = Vector2.right * Input.GetAxis("Horizontal") + Vector2.up * Input.GetAxis("Vertical");
        if (movimiento.magnitude > 1.0f) movimiento.Normalize();

        velocidad = Input.GetKey(KeyCode.LeftShift) ? velocidadSprint : velocidadNormal;

        cuerpo.velocity = movimiento * velocidad;

        if (zoomHabilitado && Input.GetAxis("Mouse ScrollWheel") > 0.0f && camara.orthographicSize < maxZoom)
            camara.orthographicSize++;
        else if (zoomHabilitado && Input.GetAxis("Mouse ScrollWheel") < 0.0f && camara.orthographicSize > minZoom)
            camara.orthographicSize--;
    }

    //** Getters y Setters **//

    /// <summary>Booleano que controla si es posible hacer zoom con la rueda del ratón.</summary>
    public bool ZoomHabilitado
    { set { zoomHabilitado = value; } }

    //** FIN Getters y Setters **//
}