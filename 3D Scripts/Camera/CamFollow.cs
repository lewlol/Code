using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    public Vector3 offset;

    private void Update()
    {
        transform.position = target.transform.position + offset;
    }
}
