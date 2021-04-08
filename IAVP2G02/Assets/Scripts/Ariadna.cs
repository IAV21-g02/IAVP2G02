namespace UCM.IAV.Navegacion
{

    using UnityEngine;
    using System.Collections.Generic;

    //Clase encargada de gestionar el hilo de Ariadna
    public class Ariadna : MonoBehaviour
    {
        public Graph graph;
        public PathAlgorithmType algorithm;
        public bool smoothPath;
        public Color pathColor;
        public float pathNodeRadius = .3f;

        GameObject player;
        GameObject exit;
        PlayerMovable mov;
        List<Vertex> path; // La variable con el camino calculado

        // Despertar inicializando esto
        void Awake()
        {
            path = new List<Vertex>();
        }

        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            mov = player.GetComponent<PlayerMovable>();
            exit = GameObject.Find("Salida");
        }

        void Update()
        {
            //Al pulsar la barra espaciadora calculamos una sola vez el camino
            if (Input.GetKeyDown(KeyCode.Space))
            {
                switch (algorithm)
                {
                    case PathAlgorithmType.ASTAR:
                        path = graph.GetPathAstar(this.player, exit, graph.ManhattanDist);

                        //Baldosas del minotauro
                        ShowPath(graph.GetMinotaurVertex(), Color.green);
                        break;
                    default:
                    case PathAlgorithmType.BFS:
                        path = graph.GetPathBFS(this.player, exit);
                        break;
                    case PathAlgorithmType.DFS:
                        path = graph.GetPathDFS(this.player, exit);
                        break;
                    case PathAlgorithmType.DIJKSTRA:
                        path = graph.GetPathDijkstra(this.player, exit);
                        break;
                }
                if (smoothPath)
                    path = graph.Smooth(path); // Suavizar el camino, una vez calculado

                //Pintamos el camino a seguir
                ShowPath(path, Color.red);
                //Hacemos que el player pase a moverse cinemáticamente en vez de por fuerzas
                mov.SetKinematic(true);
                //Le pasamos el path que deberá seguir
                mov.AddExitPath(path);
            }
            //Mientras se mantenga pulsada la tecla espacio el player continua moviendose
            else if (Input.GetKey(KeyCode.Space)){
                mov.MoveToExit();
            }
            //Cuando se levante la tecla espacio
            else if (Input.GetKeyUp(KeyCode.Space))
            {
                //El player vuelve a controlarse mediante fisicas
                mov.SetKinematic(false);
                //Borramos los caminos pintados
                ShowPath(path, Color.white);
                ShowPath(graph.GetMinotaurVertex(), Color.white);
                //Reseteamos la lista
                path.Clear();
            }
        }

        // Dibujado de artilugios en el editor
        public void OnDrawGizmos()
        {
            if (!Application.isPlaying)
                return;

            if (ReferenceEquals(graph, null))
                return;

            Vertex v;
            if (!ReferenceEquals(player, null))
            {
                Gizmos.color = Color.green; // Verde es el nodo inicial
                v = graph.GetNearestVertex(player.transform.position);
                Gizmos.DrawSphere(v.transform.position, pathNodeRadius);
            }
            if (!ReferenceEquals(exit, null))
            {
                Gizmos.color = Color.red; // Rojo es el color del nodo de destino
                v = graph.GetNearestVertex(exit.transform.position);
                Gizmos.DrawSphere(v.transform.position, pathNodeRadius);
            }
            int i;
            Gizmos.color = pathColor;
            for (i = 0; i < path.Count; i++)
            {
                v = path[i];
                Gizmos.DrawSphere(v.transform.position, pathNodeRadius);
                if (smoothPath && i != 0)
                {
                    Vertex prev = path[i - 1];
                    Gizmos.DrawLine(v.transform.position, prev.transform.position);

                }

            }
        }

        // Mostrar el camino calculado
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
    }
}
