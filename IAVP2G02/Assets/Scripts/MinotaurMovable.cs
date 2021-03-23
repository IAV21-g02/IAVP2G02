using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class MinotaurMovable : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 target;
    private Vector3 velocity;
    private LabyrinthManager lManager;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        lManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<LabyrinthManager>();
        target = lManager.getRandomPos();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        
    }
}
