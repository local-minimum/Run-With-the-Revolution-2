using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    PlayerController pc;

    void Start()
    {
        pc = FindObjectOfType<PlayerController>();
    }

    void Update()
    {
        transform.LookAt(pc.transform);
    }
}
