using System.Collections;
using System.Collections.Generic;
using UCM.IAV.Navegacion;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class PlayerMovable : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 velocity;
    List<Vertex> path; // La variable con el camino calculado
    int index = 0;

    //Variable que sirve para almacenar el transform de la casilla a la que nos tenemos que
    //mover cuando la IA maneja nuestro movimiento
    private Transform dest;

    [Tooltip("Velocidad maxima de movimiento por fisica.")]
    public float maxVelocity;

    [Tooltip("Velocidad maxima de movimiento por IA")]
    public float maxIAVelocity;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        path = new List<Vertex>();
    }

    // Update is called once per frame
    void Update()
    {
        //Cuando el player no esté bajo el control de la IA, manejaremos su movimiento mediante WADS o las flechas de dirección
        if (!rb.isKinematic)
        {
            velocity.x = Input.GetAxis("Horizontal");
            velocity.z = Input.GetAxis("Vertical");
            velocity *= maxVelocity;
        }
    }

    //Método para cambiar el control del player (true -> mediante IA, false-> mediante flechas o WADS) 
    public void SetPlayerAsKinematicObject(bool kinematic)
    {
        rb.isKinematic = kinematic;
    }

    //Método que asigna una lista de vertices con el camino que deberá seguir el player
    public void AddExitPath(List<Vertex> exit)
    {
        path = exit;
        index = path.Count - 1;
        dest = path[index].GetComponent<Transform>();
    }

    //Método que maneja el movimiento del player cuando este está bajo el control de la IA
    public void MoveToExit()
    {
        if (!rb.isKinematic) return;

        //Hacemos copias de las posiciones para luego no tener en cuenta su distancia en y para
        //el calculo de pasar de baldosa
        Vector3 playerP = transform.position;
        playerP.y = 0;
        Vector3 destP = dest.position;
        destP.y = 0;

        float distance = (destP - playerP).magnitude;
        //Si el player está lo suficientemente cerca de la siguiente baldosa a la que debería llegar 
        //siguiendo el path pasamos a la siguiente
        if ( distance< 0.5f && index >= 0)
        {
            Vertex next = path[index];
            
            //Cambiamos color de la baldosa por la que hemos pasado(recogemos hilo)
            Renderer r = next.GetComponent<Renderer>();
            r.material.color = Color.white;

            //Pasamos al siguiente objetivo si todavía nos faltan baldosas por recorrer
            if (index > 0)
            {
                index--;
                next = path[index];
                dest = next.GetComponent<Transform>();
            }
            else rb.isKinematic = false;
            
        }

        //Calculamos hacia que dirección nos tendríamos que mover 
        Vector3 dir = (dest.position - transform.position);
        //No tenemos en cuenta la diferencia en Y que pueda haber entre el suelo y el player
        dir.y = 0; 
        dir.Normalize();
        dir *= maxIAVelocity;

        //Movemos
        transform.Translate(dir * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        //Cuando el player no esté bajo el control de la IA, el movimiento se controla mediante físicas
        if (!rb.isKinematic)
        {
            rb.AddForce(velocity, ForceMode.Force);
        }
    }
}
