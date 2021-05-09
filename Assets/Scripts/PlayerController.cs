using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField, Range(0, 1000)] float lateralForce = 500;
    [SerializeField, Range(0, 1000)] float acceleartionForce = 200;

    GroundBuilder builder;
    Rigidbody rb;
    bool alive = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        builder = FindObjectOfType<GroundBuilder>();
        builder.OnCompleted += Builder_OnCompleted;
    }

    private void OnDisable()
    {
        builder.OnCompleted -= Builder_OnCompleted;
    }

    private void Builder_OnCompleted(GroundBuilder builder)
    {
        rb.useGravity = true;
        alive = true;
    }

    private void Update()
    {
        float a = Origo.GetAngle(transform.position);
        Vector3 forward = Origo.GetForwardVector(builder.Radius, a);
        transform.rotation = Quaternion.LookRotation(forward, Origo.GetUpwardsVector(transform.position));
        if (!alive) return;        
        rb.AddForce(Input.GetAxis("Horizontal") * lateralForce * -1 * Vector3.forward);
        float acceleration = Input.GetAxis("Vertical");
        if (acceleration > 0)
        {
            rb.useGravity = false;
            rb.AddForce(acceleration * acceleartionForce * Origo.GetForwardVector(builder.Radius, a));
        }
    }
}
