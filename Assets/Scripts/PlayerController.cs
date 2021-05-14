using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RwRUtils;


public class PlayerController : MonoBehaviour
{
    [SerializeField, Range(0, 10)] float lateralAcceleration = 2;
    [SerializeField, Range(0, 30)] float forwardAcceleration = 5;
    [SerializeField, Range(0, 1)] float breakFraction = 0.5f;
    [SerializeField, Range(0, 10)] float stopSpeed = 1f;
    [SerializeField, Range(200, 1200)] float jumpImpulse = 200f;

    GroundBuilder builder;
    Rigidbody rb;
    bool alive = false;

    LayerMask groundedLayers;

    private void Awake()
    {
        groundedLayers = LayerMask.NameToLayer("Ground").AsLayerMask();
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
        Vector3 up = Origo.GetUpwardsVector(transform.position);

        transform.rotation = Quaternion.LookRotation(forward, up);
        if (!alive) return;
        
        // Side moves
        rb.AddForce(Input.GetAxis("Horizontal") * lateralAcceleration * -1 * Vector3.forward, ForceMode.Acceleration);

        // Forward / break
        float acceleration = Input.GetAxis("Vertical");        
        if (acceleration > 0)
        {
            rb.AddForce(acceleration * forwardAcceleration * forward, ForceMode.Acceleration);
        } else if (acceleration < 0)
        {
            Vector3 forwardVelocity = Vector3.Project(rb.velocity, forward);
            float speed = forwardVelocity.magnitude;
            if (speed < stopSpeed)
            {
                rb.velocity -= forwardVelocity;
            } else {
                rb.velocity -= breakFraction * Time.deltaTime * forwardVelocity;
            }
        }

        // Jump
        if (Input.GetButton("Jump") && grounded)
        {
            lastJump = Time.timeSinceLevelLoad;
            rb.AddForce(up * jumpImpulse, ForceMode.Impulse);
        }
    }

    float lastGrounded;
    float gracePeriod = 0.25f;
    float lastJump;
    float noMultiJumpPeriod = 0.3f;

    private bool grounded
    {
        get
        {
            var touchingGround = lastGrounded + gracePeriod > Time.timeSinceLevelLoad;
            var jumping = lastJump + noMultiJumpPeriod < Time.timeSinceLevelLoad;
            return (touchingGround && jumping);
        }

    }

    private void OnCollisionStay(Collision collision)
    {  
        if (groundedLayers.HasLayer(collision.gameObject.layer))
        {
            lastGrounded = Time.timeSinceLevelLoad;
        }
    }
}
