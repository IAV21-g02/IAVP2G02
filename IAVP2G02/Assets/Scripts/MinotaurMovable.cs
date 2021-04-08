using System.Collections;
using System.Collections.Generic;
using UCM.IAV.Navegacion;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class MinotaurMovable : MonoBehaviour
{
    public PathAlgorithmType algorithm;
    public Transform raycastStart;
    public float force = 10;
    public float raycastDistance = 2.0f;

    private Rigidbody rb;
    private Vector3 target;
    private Vector3 velocity;
    private Vector3 bound;
    private GameObject Player;
    private const int minotaurLayer = 9;
    private Graph graph;
    private List<Vertex> path; // La variable con el camino calculado
    private int index = 0;
    private Transform dest;
    private float timer = 1.0f;
    private CapsuleCollider coll;
    bool follow = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        velocity = new Vector3(1, 0, 0);
        Player = GameObject.FindGameObjectWithTag("Player");
        graph = GameObject.Find("GraphGrid").GetComponent<GraphGrid>();
        coll = gameObject.GetComponent<CapsuleCollider>();
        //coll.radius = 0.1f;
        //follow = true;
        //
        //AddPath();
    }

    private void FixedUpdate()
    {
        if (!follow)
        {

            RaycastHit hit;
            int rnd = Random.Range(0, 2);
            if (Physics.Raycast(raycastStart.position, transform.forward, out hit, raycastDistance))
            {
                if (hit.transform.gameObject.tag == "Wall")
                {
                    Debug.DrawLine(transform.position, hit.point, Color.green);
                    rb.velocity = Vector3.zero;

                    RandomTurn();
                }
            }

            //Evita que el minotauro se quede atascado en un punto
            //de manera que si la velocidad es muy baja, gire al pasar un tiempo
            if (rb.velocity.magnitude < 1.0f)
            {
                timer -= Time.deltaTime;
                if (timer <= 0.0f)
                {
                    RandomTurn();
                }
            }
        }

        rb.AddForce(transform.forward * force, ForceMode.Force);
    }

    private void RandomTurn()
    {
        timer = 1.0f;
        int rnd = Random.Range(0, 4);
        if (rnd == 0)
            transform.forward = Vector3.back;
        else if (rnd == 1)
            transform.forward = Vector3.left;
        else if (rnd == 2)
            transform.forward = Vector3.forward;
        else if (rnd == 3)
            transform.forward = transform.right;
    }

    /// <summary>
    /// Gira al minotauro hacia la derecha
    /// </summary>
    private void TurnRight()
    {
        timer = 1.0f;
        Debug.Log("TIMER: " + timer);
        if (transform.forward == Vector3.right) transform.forward = Vector3.back;
        else if (transform.forward == Vector3.back) transform.forward = Vector3.left;
        else if (transform.forward == Vector3.left) transform.forward = Vector3.forward;
        else transform.forward = transform.right;
    }

    private void TurnLeft()
    {
        timer = 1.0f;
        Debug.Log("TIMER: " + timer);
        if (transform.forward == Vector3.right) transform.forward = Vector3.forward;
        else if (transform.forward == Vector3.back) transform.forward = Vector3.right;
        else if (transform.forward == Vector3.left) transform.forward = Vector3.back;
        else transform.forward = Vector3.left;

    }

    private void Update()
    {
        if (follow && index >= 0)
        {
            //Se coge la posicion destino
            dest = path[index].GetComponent<Transform>();

            //No se tiene en cuenta la y
            Vector3 pos = transform.position;
            pos.y = 0;
            Vector3 destP = dest.position;
            destP.y = 0;

            float distance = (destP - pos).magnitude;
            //Si el minotauro está lo suficientemente cerca, entonces pasa al siguiente nodo
            if (distance < 0.5f && index > 0)
            {
                Vertex next = path[index];

                //Pasamos al siguiente objetivo si todavía nos faltan baldosas por recorrer
                if (index > 0)
                {
                    index--;
                    next = path[index];
                    dest = next.GetComponent<Transform>();
                }
                else rb.isKinematic = false;
            }

            //Movemos
            transform.forward = Vector3.Normalize(destP - pos);
        }
    }

    //Añade un nuevo camino para ir hacia Teseo
    public void AddPath()
    {
        switch (algorithm)
        {
            case PathAlgorithmType.ASTAR:
                path = graph.GetPathAstar(gameObject, Player, graph.ManhattanDist);
                break;
            default:
            case PathAlgorithmType.BFS:
                path = graph.GetPathBFS(gameObject, Player);
                break;
            case PathAlgorithmType.DFS:
                path = graph.GetPathDFS(gameObject, Player);
                break;
            case PathAlgorithmType.DIJKSTRA:
                path = graph.GetPathDijkstra(gameObject, Player);
                break;
        }

        //Pintamos el camino a seguir
        ShowPath(path, Color.black);
        index = path.Count - 1;
    }

    // Muestra el camino calculado
    public void ShowPath(List<Vertex> path, Color color)
    {
        int i;
        for (i = 0; i < path.Count; i++)
        {
            Vertex v = path[i];
            Renderer r = v.GetComponent<Renderer>();
            if (ReferenceEquals(r, null))
                continue;
            r.material.color = color;
        }
    }

    public void startFollow(bool start)
    {
        follow = start;
        if (follow)
        {
            //coll.isTrigger = true;
            AddPath();
        }
        else
        {
            //coll.isTrigger = false;
            path = new List<Vertex>();
        }
    }
}
