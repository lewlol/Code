using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    bool canDamage;

    private void Awake()
    {
        canDamage = true;
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player" && canDamage)
        {
            collision.gameObject.GetComponent<PlayerMovement>().TakeDamage(10);
            StartCoroutine(DamageNo());
        }
    }
    IEnumerator DamageNo()
    {
        canDamage = false;
        yield return new WaitForSeconds(1);
        canDamage = true;
    }
}
