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

        if (!rb.isKinematic)
        {
            velocity.x = Input.GetAxis("Horizontal");
            velocity.z = Input.GetAxis("Vertical");
            velocity *= maxVelocity;
            Debug.Log(velocity);
        }
    }

    public void SetPlayerAsKinematicObject(bool kinematic)
    {
        rb.isKinematic = kinematic;
    }

    public void AddExitPath(List<Vertex> exit)
    {
        path = exit;
        index = path.Count - 1;
        dest = path[index].GetComponent<Transform>();
    }


    public void MoveToExit(Vertex currPlayerVert)
    {
        Vertex next = path[index];
        if (currPlayerVert == next && index > 0)
        {
            Debug.Log("Llegamos al objetivo");
            Renderer r = next.GetComponent<Renderer>();
            r.material.color = Color.white;
            index--;
            next = path[index];
            dest = next.GetComponent<Transform>();
        }

        Vector3 dir = (dest.position - transform.position).normalized;
        dir *= maxIAVelocity;
        dir.y = 0;

        transform.Translate(dir * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (!rb.isKinematic)
        {
            rb.AddForce(velocity, ForceMode.Force);
        }
    }
}
