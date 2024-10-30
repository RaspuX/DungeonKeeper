using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTimer : MonoBehaviour
{
    public float deathTimer;
    public void Start()
    {
        Destroy(gameObject, deathTimer);
    }
}
