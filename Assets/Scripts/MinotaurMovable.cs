using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class MinotaurMovable : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 target;
    private Vector3 velocity;
    private Vector3 bound;
    private GameObject Player;
    private bool follow = false;
    public float force = 10;
    public float raycastDistance = 2.0f;
    private const int minotaurLayer = 9;
    public Transform raycastStart;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        velocity = new Vector3(1, 0, 0);
        Player = GameObject.FindGameObjectWithTag("Player");
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

                    if (transform.forward == Vector3.right) transform.forward = Vector3.back;
                    else if (transform.forward == Vector3.back) transform.forward = Vector3.left;
                    else if (transform.forward == Vector3.left) transform.forward = Vector3.forward;
                    else transform.forward = transform.right;
                }

            }


        }

        rb.AddForce(transform.forward * force, ForceMode.Force);


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

    }


}
