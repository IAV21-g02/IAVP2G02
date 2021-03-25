using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class PlayerMovable : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 velocity;

    [Tooltip("Combinar por peso.")]
    public float maxVelocity;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        velocity.x = Input.GetAxis("Horizontal");
        velocity.z = Input.GetAxis("Vertical");
        velocity *= maxVelocity;
    }

    private void FixedUpdate()
    {
        rb.AddForce(velocity, ForceMode.Force);
    }
}
