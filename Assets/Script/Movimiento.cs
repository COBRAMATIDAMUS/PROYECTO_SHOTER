using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movimiento : MonoBehaviour
{
    public float velocidadMovimiento = 5f;     // Velocidad de movimiento del personaje
    public float sensibilidadMouse = 2f;       // Sensibilidad del rat�n
    public float fuerzaSalto = 5f;             // Fuerza del salto
    public Transform camara;                   // Referencia a la c�mara del jugador (debe ser hija del personaje)
    public Transform arma;                     // Referencia al arma del jugador
    public LayerMask capaSuelo;                // Capa que representa el suelo para detectar colisiones
    public GameObject proyectilPrefab;         // Prefab del proyectil
    public Transform puntoDisparo;             // Punto desde donde se dispara el proyectil
    public float fuerzaProyectil = 10f;        // Fuerza del proyectil al ser disparado

    private float rotacionX = 0f;              // Para almacenar la rotaci�n vertical de la c�mara
    private Rigidbody rb;                      // Referencia al Rigidbody del jugador
    private bool enSuelo;                      // Para comprobar si est� tocando el suelo
    public Transform chequeoSuelo;             // Posici�n desde donde se verifica el suelo
    public float radioChequeo = 0.4f;          // Radio para detectar el suelo

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;  // Bloquear y ocultar el cursor
        Cursor.visible = false;
    }

    void Update()
    {
        // Movimiento del personaje usando WASD o las flechas
        float movimientoHorizontal = Input.GetAxis("Horizontal");
        float movimientoVertical = Input.GetAxis("Vertical");
        Vector3 movimiento = transform.right * movimientoHorizontal + transform.forward * movimientoVertical;

        // Aplicar movimiento
        rb.MovePosition(rb.position + movimiento * velocidadMovimiento * Time.deltaTime);

        // Movimiento del rat�n (rotaci�n de la c�mara y el personaje)
        float mouseX = Input.GetAxis("Mouse X") * sensibilidadMouse;
        float mouseY = Input.GetAxis("Mouse Y") * sensibilidadMouse;

        // Rotaci�n horizontal del personaje (mirar izquierda/derecha)
        transform.Rotate(Vector3.up * mouseX);

        // Rotaci�n vertical de la c�mara (mirar arriba/abajo)
        rotacionX -= mouseY;
        rotacionX = Mathf.Clamp(rotacionX, -90f, 90f); // Limitar la rotaci�n vertical entre -90� y 90�
        camara.localRotation = Quaternion.Euler(rotacionX, 0f, 0f);

        // Chequear si est� en el suelo
        enSuelo = Physics.CheckSphere(chequeoSuelo.position, radioChequeo, capaSuelo);

        // Salto
        if (Input.GetKeyDown(KeyCode.Space) && enSuelo)
        {
            rb.AddForce(Vector3.up * fuerzaSalto, ForceMode.Impulse);
        }

        // Alinear el arma con la rotaci�n de la c�mara
        if (arma != null)
        {
            arma.rotation = camara.rotation;  // Hace que el arma mire donde est� mirando la c�mara
        }

        // Disparo
        if (Input.GetButtonDown("Fire1")) // "Fire1" es el mapeo predeterminado para el clic izquierdo
        {
            Disparar();
        }
    }

    void Disparar()
    {
        // Instanciar el proyectil
        if (proyectilPrefab != null && puntoDisparo != null)
        {
            GameObject proyectil = Instantiate(proyectilPrefab, puntoDisparo.position, puntoDisparo.rotation);
            Rigidbody rbProyectil = proyectil.GetComponent<Rigidbody>();
            if (rbProyectil != null)
            {
                rbProyectil.AddForce(puntoDisparo.forward * fuerzaProyectil, ForceMode.Impulse);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        // Dibuja una esfera en el editor para ver el �rea de chequeo del suelo
        if (chequeoSuelo != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(chequeoSuelo.position, radioChequeo);
        }
    }
}