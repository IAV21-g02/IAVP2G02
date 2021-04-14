using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class MinotaurMovable : MonoBehaviour
{
    public Transform raycastStart;
    public float force = 10;
    public float raycastDistance = 2.0f;

    private Rigidbody rb;
    private Vector3 target;
    private Vector3 velocity;
    private Vector3 bound;
    private GameObject Player;
    private bool follow = false;
    private const int minotaurLayer = 9;
    private float timer = 1.0f;
    private Renderer rend;
    public ParticleSystem ptc;
    private AudioSource clip;
    private Animator animCtl;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        velocity = new Vector3(1, 0, 0);
        Player = GameObject.FindGameObjectWithTag("Player");
        rend = gameObject.GetComponent<Renderer>();
        ptc.Stop();
        clip = GetComponent<AudioSource>();
        animCtl = GetComponentInChildren<Animator>();
    }


    private void FixedUpdate()
    {
        if (!follow)
        {
            RaycastHit hit;
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
        if (follow)
        {
            transform.forward = Vector3.Normalize(Player.transform.position - transform.position);
        }
    }

    public void startFollow(bool start)
    {
        follow = start;
        if (follow)
        {
            rend.material.color = Color.black;
            ptc.Play();
            clip.Play();
            animCtl.Play("Shout");
        }
        else { 
            rend.material.color = Color.yellow;
            ptc.Stop();
        }
    }
}
