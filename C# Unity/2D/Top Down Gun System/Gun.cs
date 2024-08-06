using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Gun")]
public class Gun : ScriptableObject
{
    [Header("Gun Info")]
    public string gunName;

    [Header("Gun Stats")]
    public int damage;
    public float shootDelay;
    public float accuracy;
    public float reloadSpeed;
    public bool isAuto;

    [Header("Bullet Stats")]
    public GameObject projectile;
    public float bulletSpeed;
    public float bulletTime;
    public float magSize;
}
