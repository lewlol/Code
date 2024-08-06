using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SpaceshipStats : UnityEngine.MonoBehaviour
{
    public float health;
    [HideInInspector]public float maxHealth;

    public float fuel;
    [HideInInspector]public float maxFuel;

    public float nuggets;

    [SerializeField] private GameObject particles;
    [SerializeField] private GameObject retryMenu;
    [SerializeField] private AudioSource dSound;
    [SerializeField] private AudioSource hitSound;

    [SerializeField] private Text outOfFuel;

    private void Start()
    {
        health = 150;
        maxHealth = 150;

        fuel = 100;
        maxFuel = 100;

        nuggets = 0;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        hitSound.Play();
        if(health <= 0)
        {
            dSound.Play();
            StartCoroutine(Death());
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "BlackHole")
        {
            dSound.Play();
            StartCoroutine(Death());
        }
    }
    private void FixedUpdate()
    {
        if(fuel > maxFuel)
        {
            fuel = maxFuel;
        }
        if(fuel < 0)
        {
            fuel = 0;
        }
        if(health > maxHealth)
        {
            health = maxHealth;
        }

        if(fuel < 10 && fuel >= 0.001)
        {
            outOfFuel.text = "YOU ARE LOW ON FUEL";
        }else if (fuel <= 0)
        {
            outOfFuel.text = "OUT OF FUEL";
        }else
        {
            outOfFuel.text = null;
        }
    }

    IEnumerator Death()
    {
        CircleCollider2D cc = GetComponent<CircleCollider2D>();
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        cc.enabled = false;
        sr.enabled = false;

        var part = Instantiate(particles, gameObject.transform.position, Quaternion.identity);

        yield return new WaitForSeconds(3f);

        retryMenu.SetActive(true);

        Destroy(part);
        Destroy(gameObject);
    }
}
