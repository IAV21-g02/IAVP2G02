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
    bool IA = false;

    [Tooltip("Combinar por peso.")]
    public float maxVelocity;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        path = new List<Vertex>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            //El jugador empieza a avanzar hasta la casilla de salida
            MoveToExit();
            Debug.Log("MOVEMENT");
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            IA = false;

            Debug.Log("IA FALSE");
        }

        if (!IA)
        {
            velocity.x = Input.GetAxis("Horizontal");
            velocity.z = Input.GetAxis("Vertical");
            velocity *= maxVelocity;
        }
    }

    public void AddExitPath(List<Vertex> exit)
    {
        path = exit;
        index = path.Count - 1;
        IA = true;
    }

    void MoveToExit()
    {
        Vertex next = path[index];
        Transform dest = next.GetComponent<Transform>();
        transform.Translate((dest.position - transform.position) * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (IA && collision.gameObject == path[index].gameObject)
        {
            index--;
            Vertex v = path[index];
            Renderer r = v.GetComponent<Renderer>();
            r.material.color = Color.white;
        }
    }

    private void FixedUpdate()
    {
        rb.AddForce(velocity, ForceMode.Force);
    }
}
