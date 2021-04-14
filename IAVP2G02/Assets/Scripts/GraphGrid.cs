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
    using System;
    using System.Collections.Generic;
    using System.IO;
    using UnityEngine.UI;

    public class GraphGrid : Graph
    {
        public GameObject obstaclePrefab;
        public GameObject playerPrefab;
        public GameObject minotaurPrefab;

        public string mapsDir = "Maps"; // Directorio por defecto
        public string mapName = "arena.map"; // Fichero por defecto
        public bool get8Vicinity = false;
        public float cellSize = 1f;
        [Range(0, Mathf.Infinity)]
        public float defaultCost = 1f;
        [Range(0, Mathf.Infinity)]
        public float maximumCost = Mathf.Infinity;
        public TextAsset file;

        int numCols;
        int numRows;
        GameObject[] vertexObjs;
        TypeOfBox[,] mapVertices;

        //Diferentes tipos de casillas que tendremos, Enter marca la posicion en la que empieza a moverse Teseo
        //y Minotaur la posición inicial del minotauro
        public enum TypeOfBox
        {
            Wall, Floor, Enter, Minotaur
        }

        private int GridToId(int x, int y)
        {
            return Math.Max(numRows, numCols) * y + x;
        }

        private Vector2 IdToGrid(int id)
        {
            Vector2 location = Vector2.zero;
            location.y = Mathf.Floor(id / numCols);
            location.x = Mathf.Floor(id % numCols);
            return location;
        }

        private void LoadMap(string filename)
        {
            string path = Application.dataPath + "/" + mapsDir + "/" + filename;

            //Se guarda el archivo de texto en un array
            string[] mapa = file.text.Split('\n');

            //string path = "Assets/Maps/" + filename;
            try
            {
                int j = 0;
                int i = 0;
                int id = 0;

                Vector3 position = Vector3.zero;
                position.y = -30.0f;

                Vector3 scale = Vector3.zero;
                numRows = int.Parse(mapa[1]);
                numCols = int.Parse(mapa[2]);

                vertices = new List<Vertex>(numRows * numCols);
                neighbors = new List<List<Vertex>>(numRows * numCols);
                costs = new List<List<float>>(numRows * numCols);
                vertexObjs = new GameObject[numRows * numCols];
                mapVertices = new TypeOfBox[numRows, numCols];

                for (i = 4; i < numRows + 4; i++)
                {
                    for (j = 0; j < numCols; j++)
                    {
                        TypeOfBox casilla = TypeOfBox.Floor;

                        if (mapa[i][j] == '.')
                            casilla = TypeOfBox.Floor;
                        else if (mapa[i][j] == 'T')
                            casilla = TypeOfBox.Wall;
                        else if (mapa[i][j] == 'E')
                            casilla = TypeOfBox.Enter;
                        else if (mapa[i][j] == 'M')
                            casilla = TypeOfBox.Minotaur;


                        mapVertices[i - 4, j] = casilla;
                        position.x = j * cellSize;
                        //position.x += 10;
                        position.z = i - 4 * cellSize;
                        id = GridToId(j, i - 4);

                        if (casilla != TypeOfBox.Wall)
                        {
                            vertexObjs[id] = Instantiate(vertexPrefab, position, Quaternion.identity) as GameObject;
                            Vector3 aux = position;
                            aux.y += 1;
                            if (casilla == TypeOfBox.Enter)
                            {
                                vertexObjs[id].name = "Salida";
                                Instantiate(playerPrefab, aux, Quaternion.identity);
                            }
                            else if (casilla == TypeOfBox.Minotaur)
                                Instantiate(minotaurPrefab, aux, Quaternion.identity);
                        }
                        else
                        {
                            vertexObjs[id] = Instantiate(obstaclePrefab, position, Quaternion.identity) as GameObject;
                        }

                        vertexObjs[id].name = vertexObjs[id].name.Replace("(Clone)", id.ToString());
                        Vertex v = vertexObjs[id].AddComponent<Vertex>();
                        v.id = id;
                        vertices.Add(v);
                        neighbors.Add(new List<Vertex>());
                        costs.Add(new List<float>());
                        float y = vertexObjs[id].transform.localScale.y;
                        scale = new Vector3(cellSize, y, cellSize);
                        vertexObjs[id].transform.localScale = scale;
                        vertexObjs[id].transform.parent = gameObject.transform;
                    }
                }

                // now onto the neighbours
                for (i = 0; i < numRows; i++)
                {
                    for (j = 0; j < numCols; j++)
                    {
                        SetNeighbours(j, i);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public override void Load()
        {
            LoadMap(mapName);
        }

        protected void SetNeighbours(int x, int y, bool get8 = false)
        {
            int col = x;
            int row = y;
            int i, j;
            int vertexId = GridToId(x, y);
            neighbors[vertexId] = new List<Vertex>();
            costs[vertexId] = new List<float>();
            Vector2[] pos = new Vector2[0];
            if (get8)
            {
                pos = new Vector2[8];
                int c = 0;
                for (i = row - 1; i <= row + 1; i++)
                {
                    for (j = col - 1; j <= col; j++)
                    {
                        pos[c] = new Vector2(j, i);
                        c++;
                    }
                }
            }
            else
            {
                pos = new Vector2[4];
                pos[0] = new Vector2(col, row - 1);
                pos[1] = new Vector2(col - 1, row);
                pos[2] = new Vector2(col + 1, row);
                pos[3] = new Vector2(col, row + 1);
            }
            foreach (Vector2 p in pos)
            {
                i = (int)p.y;
                j = (int)p.x;
                if (i < 0 || j < 0)
                    continue;
                if (i >= numRows || j >= numCols)
                    continue;
                if (i == row && j == col)
                    continue;
                if (mapVertices[i, j] == TypeOfBox.Wall)
                    continue;
                int id = GridToId(j, i);
                neighbors[vertexId].Add(vertices[id]);
                costs[vertexId].Add(defaultCost);
            }
        }

        public override Vertex GetNearestVertex(Vector3 position)
        {
            int col = (int)(position.x / cellSize);
            int row = (int)(position.z / cellSize);
            Vector2 p = new Vector2(col, row);
            List<Vector2> explored = new List<Vector2>();
            Queue<Vector2> queue = new Queue<Vector2>();
            queue.Enqueue(p);
            do
            {
                p = queue.Dequeue();
                col = (int)p.x;
                row = (int)p.y;
                int id = GridToId(col, row);
                if (mapVertices[row, col] != TypeOfBox.Wall)
                    return vertices[id];

                if (!explored.Contains(p))
                {
                    explored.Add(p);
                    int i, j;
                    for (i = row - 1; i <= row + 1; i++)
                    {
                        for (j = col - 1; j <= col + 1; j++)
                        {
                            if (i < 0 || j < 0)
                                continue;
                            if (j >= numCols || i >= numRows)
                                continue;
                            if (i == row && j == col)
                                continue;
                            queue.Enqueue(new Vector2(j, i));
                        }
                    }
                }
            } while (queue.Count != 0);
            return null;
        }

    }
}
