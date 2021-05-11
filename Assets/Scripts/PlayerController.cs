using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField, Range(0, 10)] float lateralAcceleration = 2;
    [SerializeField, Range(0, 30)] float forwardAcceleration = 5;

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
        alive = true;
    }

    private void Update()
    {
        float a = Origo.GetAngle(transform.position);
        Vector3 forward = Origo.GetForwardVector(builder.Radius, a);
        transform.rotation = Quaternion.LookRotation(forward, Origo.GetUpwardsVector(transform.position));
        if (!alive) return;        
        rb.AddForce(Input.GetAxis("Horizontal") * lateralAcceleration * -1 * Vector3.forward, ForceMode.Acceleration);
        float acceleration = Input.GetAxis("Vertical");
        if (acceleration > 0)
        {
            rb.AddForce(acceleration * forwardAcceleration * Origo.GetForwardVector(builder.Radius, a), ForceMode.Acceleration);
        }
    }
}
