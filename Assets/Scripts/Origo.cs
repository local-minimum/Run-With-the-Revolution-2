using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Origo : MonoBehaviour
{
    static Origo instance { get; set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    public static Vector3 GetPoint(float radius, float angle)
    {
        var origin = instance.transform.position;
        return origin + radius * new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
    }

    public static Vector3 GetForwardVector(float radius, float angle, float lookAhead = Mathf.PI / 10f)
    {
        return (GetPoint(radius, angle + lookAhead) - GetPoint(radius, angle)).normalized;
    }

    public static float GetAngle(Vector3 position)
    {
        Vector3 offset = position - instance.transform.position;
        return Mathf.Atan2(offset.y, offset.x);
    }

    public static Vector3 GetUpwardsVector(Vector3 pos)
    {
        return (instance.transform.position - pos).normalized;
    }

    public static Vector3 GetOutwardsVector(float angle)
    {
        return new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
    }

    public static Vector3 Position
    {
        get { return instance.transform.position; }
    }
}
