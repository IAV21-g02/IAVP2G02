using System.Collections;
using System.Collections.Generic;
using UnityEngine;


struct box
{
    public Vector3  pos;
    public bool     walled;
    public box(Vector3 newPos,bool status)
    {
        pos = newPos;
        walled = status;
    }
}


public class LabyrinthManager : MonoBehaviour
{
    [Tooltip("Columnas del plano.")]
    public int cols;
    [Tooltip("Filas del plano.")]
    public int fils;

    //  Representa a la familia 
    private box[,] matriz;
    // Start is called before the first frame update
    void Start()
    {
        Collider planCol = GetComponent<Collider>();
        Vector3 size = planCol.bounds.size;
        Vector3 boxSize = new Vector3(size.x / cols, 0.1f, size.z / fils);
        matriz = new box[fils, cols];
        Vector3 auxSize = planCol.bounds.min + boxSize / 2;
        for (int i = 0; i < cols; ++i)
        {
            for (int j = 0; j < fils; ++j) 
            {
                matriz[i, j] = new box(auxSize, false);
                auxSize.x += boxSize.x;
            }
            auxSize.x = planCol.bounds.min.x + (boxSize.x / 2);
            auxSize.z += boxSize.z;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //
    public Vector3 getRandomPos()
    {
        bool finded = false;
        do
        {
            int x = Random.Range(0, cols - 1);
            int y = Random.Range(0, fils - 1);
            if (!matriz[x, y].walled)
            {
                finded = true;
                return matriz[x, y].pos;
            }
        }
        while (!finded);
        return new Vector3(-1, -1, -1);
    }
}
