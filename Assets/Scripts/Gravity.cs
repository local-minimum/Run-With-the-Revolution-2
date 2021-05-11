using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Gravity : MonoBehaviour
{
    [SerializeField, Range(0, 20)] float gravityAcceleration = 10;
    Rigidbody rb;
    GroundBuilder builder;

    [SerializeField, Range(0, Mathf.PI)] float gravityLookAhead = Mathf.PI / 4f;
    [SerializeField, Range(0, Mathf.PI)] float rotationalVelocityPerSecond = Mathf.PI / 4f;

    float lastAngle = 0;

    public bool activeGravity
    {
        get;
        private set;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
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
        activeGravity = true;
    }

    private void Update()
    {
        if (!activeGravity) return;
        float angle = NextAngle(Origo.GetAngle(transform.position) + gravityLookAhead);
        Vector3 direction = Origo.GetOutwardsVector(angle);
        rb.AddForce(direction * gravityAcceleration, ForceMode.Acceleration);
        lastAngle = angle % (2 * Mathf.PI);
    }

    private float NextAngle(float selfAngle)
    {
        /*
        float minAngle = lastAngle + Time.deltaTime * rotationalVelocityPerSecond;
        float forwardAngle = (selfAngle - minAngle) % (2 * Mathf.PI);
        float backwardAngle = (minAngle - selfAngle) % (2 * Mathf.PI);
        return Mathf.Abs(forwardAngle) < Mathf.Abs(backwardAngle) ? selfAngle : minAngle;
        */
        return selfAngle;
    }

    private void OnDrawGizmosSelected()
    {
        var r = new Ray(transform.position, Origo.GetOutwardsVector(lastAngle + gravityLookAhead));
        Gizmos.DrawLine(r.origin, r.GetPoint(5));
    }
}
