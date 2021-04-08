/*    
    Obra original:
        Copyright (c) 2018 Packt
        Unity 2018 Artificial Intelligence Cookbook - Second Edition, by Jorge Palacios
        https://github.com/PacktPublishing/Unity-2018-Artificial-Intelligence-Cookbook-Second-Edition
        MIT License

    Modificaciones:
        Copyright (C) 2020-2021 Federico Peinado
        http://www.federicodespeinado.com

        Este fichero forma parte del material de la asignatura Inteligencia Artificial para Videojuegos.
        Esta asignatura se imparte en la Facultad de Informática de la Universidad Complutense de Madrid (España).
        Contacto: email@federicopeinado.com
*/
namespace UCM.IAV.Navegacion
{

    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using priority_queue;
    using UnityEngine.UI;

    // Posibles algoritmos para buscar caminos en grafos
    public enum PathAlgorithmType
    {
        BFS, DFS, DIJKSTRA, ASTAR

    }

    /// <summary>
    /// Abstract class for graphs
    /// </summary>
    public abstract class Graph : MonoBehaviour
    {

        public GameObject vertexPrefab;
        protected List<Vertex> vertices;
        protected List<List<Vertex>> neighbors;
        protected List<List<float>> costs;
        protected Dictionary<int, int> instIdToId;
        private GameObject minotaur;

        private List<Vertex> allMinoVertex;
        private int nodesExplored = 0;
        private float timeToExplore = 0.0f;
        Text[] dataText;

        public List<Vertex> GetMinotaurVertex() { return allMinoVertex; }

        //// this is for informed search like A*
        public delegate float Heuristic(Vertex a, Vertex b);

        // Used for getting path in frames
        public List<Vertex> path;
        public bool isFinished;

        public virtual void Start()
        {
            Load();
            minotaur = GameObject.FindGameObjectWithTag("Minotaur");
            dataText = GameObject.FindGameObjectWithTag("Canvas").GetComponentsInChildren<Text>();
        }

        public virtual void Load() { }

        public virtual int GetSize()
        {
            if (ReferenceEquals(vertices, null))
                return 0;
            return vertices.Count;
        }

        public virtual Vertex GetNearestVertex(Vector3 position)
        {
            return null;
        }


        public virtual Vertex[] GetNeighbours(Vertex v)
        {
            if (ReferenceEquals(neighbors, null) || neighbors.Count == 0)
                return new Vertex[0];
            if (v.id < 0 || v.id >= neighbors.Count)
                return new Vertex[0];
            return neighbors[v.id].ToArray();
        }

        public virtual Edge[] GetEdges(Vertex v)
        {
            if (ReferenceEquals(neighbors, null) || neighbors.Count == 0)
                return new Edge[0];
            if (v.id < 0 || v.id >= neighbors.Count)
                return new Edge[0];
            int numEdges = neighbors[v.id].Count;
            Edge[] edges = new Edge[numEdges];
            List<Vertex> vertexList = neighbors[v.id];
            List<float> costList = costs[v.id];
            for (int i = 0; i < numEdges; i++)
            {
                edges[i] = new Edge();
                edges[i].cost = costList[i];
                edges[i].vertex = vertexList[i];
            }
            return edges;
        }

        // Encuentra caminos óptimos
        public List<Vertex> GetPathBFS(GameObject srcO, GameObject dstO)
        {
            if (srcO == null || dstO == null)
                return new List<Vertex>();
            Vertex[] neighbours;
            Queue<Vertex> q = new Queue<Vertex>();
            Vertex src = GetNearestVertex(srcO.transform.position);
            Vertex dst = GetNearestVertex(dstO.transform.position);
            Vertex v;
            int[] previous = new int[vertices.Count];
            for (int i = 0; i < previous.Length; i++)
                previous[i] = -1;
            previous[src.id] = src.id;
            q.Enqueue(src);
            while (q.Count != 0)
            {
                v = q.Dequeue();
                if (ReferenceEquals(v, dst))
                {
                    return BuildPath(src.id, v.id, ref previous);
                }

                neighbours = GetNeighbours(v);
                foreach (Vertex n in neighbours)
                {
                    if (previous[n.id] != -1)
                        continue;
                    previous[n.id] = v.id;
                    q.Enqueue(n);
                }
            }
            return new List<Vertex>();
        }

        // No encuentra caminos óptimos
        public List<Vertex> GetPathDFS(GameObject srcO, GameObject dstO)
        {
            if (srcO == null || dstO == null)
                return new List<Vertex>();
            Vertex src = GetNearestVertex(srcO.transform.position);
            Vertex dst = GetNearestVertex(dstO.transform.position);
            Vertex[] neighbours;
            Vertex v;
            int[] previous = new int[vertices.Count];
            for (int i = 0; i < previous.Length; i++)
                previous[i] = -1;
            previous[src.id] = src.id;
            Stack<Vertex> s = new Stack<Vertex>();
            s.Push(src);
            while (s.Count != 0)
            {
                v = s.Pop();
                if (ReferenceEquals(v, dst))
                {
                    return BuildPath(src.id, v.id, ref previous);
                }

                neighbours = GetNeighbours(v);
                foreach (Vertex n in neighbours)
                {
                    if (previous[n.id] != -1)
                        continue;
                    previous[n.id] = v.id;
                    s.Push(n);
                }
            }
            return new List<Vertex>();
        }

        public List<Vertex> GetPathDijkstra(GameObject srcO, GameObject dstO)
        {
            // IMPLEMENTACIÓN DEL ALGORITMO DE DIJKSTRA
            return new List<Vertex>();
        }

        public struct NodeRecord
        {
            public Vertex nodo;
            public List<Vertex> connection;
            public float costSoFar;
            public float estimatedTotalCost;

        }

        public class Minor : IComparer<NodeRecord>
        {
            public int Compare(NodeRecord a, NodeRecord b)
            {
                if (a.estimatedTotalCost < b.estimatedTotalCost) return 1;
                else return 0;
            }
        }

        public List<Vertex> GetPathAstar(GameObject srcO, GameObject dstO, Heuristic h = null)
        {
            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            //Comprobación de GOs
            if (!srcO || !dstO)
                return new List<Vertex>();

            //Si no se le pasa ninguna heurística, entonces se coge una por defecto
            if (ReferenceEquals(h, null))
                h = EuclidDist;

            //Asignación de GOs. Son los vértices más cercanos al GO src
            Vertex src = GetNearestVertex(srcO.transform.position);
            Vertex dst = GetNearestVertex(dstO.transform.position);

            //Vertices del minotauro y sus vecinos
            Vector3 posMino = minotaur.transform.position;
            posMino.y = 0;
            Vertex minoVertex = GetNearestVertex(posMino);
            Edge[] minoEdges = GetEdges(minoVertex);
            allMinoVertex = new List<Vertex>();
            allMinoVertex.Add(minoVertex);

            foreach (Edge e in minoEdges)
            {
                allMinoVertex.Add(e.vertex);
            }

            //Cola de prioridad de los nodos y sus costes
            Priority_Queue<Edge> pqEdge = new Priority_Queue<Edge>();

            Edge[] edges;
            Edge node, child;

            //Array de distancias
            float[] distances = new float[vertices.Count];
            //Array del vértice anterior
            int[] prevVertex = new int[vertices.Count];

            //Nodo inicial con distancia = 0
            node = new Edge(src, 0);
            pqEdge.Add(node);
            distances[src.id] = 0;
            prevVertex[src.id] = src.id;

            //Inicialización del resto de vértices, con valores por defecto
            //para que, posteriormente, se actualicen
            for (int i = 0; i < vertices.Count; i++)
            {
                if (i != src.id)
                {
                    prevVertex[i] = -1;
                    distances[i] = Mathf.Infinity;
                }
            }

            //Bucle ppal
            while (!pqEdge.Empty())
            {
                nodesExplored++;

                //Obtenemos el nodo más prioritario
                node = pqEdge.Remove();
                int nodeId = node.vertex.id;

                //Si es el que se busca, devolvemos el camino construido
                if (ReferenceEquals(node.vertex, dst))
                {
                    watch.Stop();
                    timeToExplore = watch.ElapsedMilliseconds;
                    ShowInCanvas();
                    //reseteo para la siguiente vez
                    nodesExplored = 0;

                    return BuildPath(src.id, node.vertex.id, ref prevVertex);
                }

                //Se guardan los nodos vecinos del nodo actual y se recorren
                edges = GetEdges(node.vertex);
                foreach (Edge neigh in edges)
                {
                    int nID = neigh.vertex.id;
                    //Si es != -1 es que ha sido visitado, por tanto se le salta
                    //Si es la casilla del minotauro, tampoco se cuenta
                    if (prevVertex[nID] == -1)
                    {
                        if (node.vertex == minoVertex)
                        {
                            continue;
                        }
                        //Se calcula el coste del nodo
                        float cost = distances[nodeId] + neigh.cost;

                        //Se comprueba si es una de las casillas vecinas al minotauro
                        //de manera que si es así, entonces aumenta 5 veces su coste
                        //foreach (Vertex mino in allMinoVertex)
                        //{
                        //    if (neigh.vertex == mino && mino != minoVertex)
                        //            cost *= 5;
                        //}
                        foreach (Edge mino in minoEdges)
                        {
                            if (neigh.vertex == mino.vertex)
                                cost *= 5;
                        }

                        //Se le suma el coste estimado al coste del nodo
                        cost += h(node.vertex, neigh.vertex);


                        //Si el coste es menor que el que ya estaba almacenado,
                        //entonces es mejor solución
                        if (cost < distances[neigh.vertex.id])
                        {
                            distances[nID] = cost;
                            prevVertex[nID] = nodeId;
                            pqEdge.Remove(neigh);
                            child = new Edge(neigh.vertex, cost);
                            if (!pqEdge.Contains(child))
                            {
                                pqEdge.Add(child);
                            }
                        }
                    }
                }
            }

            //Si llega aquí es que no ha encontrado camino hasta la salida
            watch.Stop();
            timeToExplore = watch.ElapsedMilliseconds;
            ShowInCanvas();
            //reseteo para la siguiente vez
            nodesExplored = 0;

            return new List<Vertex>();
        }

        public List<Vertex> Smooth(List<Vertex> path)
        {
            // IMPLEMENTACIÓN DEL ALGORITMO DE SUAVIZADO
            return null; //newPath
        }

        private List<Vertex> BuildPath(int srcId, int dstId, ref int[] prevList)
        {
            List<Vertex> path = new List<Vertex>();
            int prev = dstId;
            do
            {
                path.Add(vertices[prev]);
                prev = prevList[prev];
            } while (prev != srcId);
            return path;
        }

        // Heurística de distancia euclídea
        public float EuclidDist(Vertex a, Vertex b)
        {
            Vector3 posA = a.transform.position;
            Vector3 posB = b.transform.position;
            return Vector3.Distance(posA, posB);
        }

        // Heurística de distancia Manhattan
        public float ManhattanDist(Vertex a, Vertex b)
        {
            Vector3 posA = a.transform.position;
            Vector3 posB = b.transform.position;
            return Mathf.Abs(posA.x - posB.x) + Mathf.Abs(posA.y - posB.y);
        }

        private void ShowInCanvas()
        {
            dataText[0].text = "Nodos Expl: " + nodesExplored;
            dataText[1].text = "Tiempo: " + timeToExplore + " ms";
        }
    }
}