using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class PlayerMovement : MonoBehaviour
{
    //movement shit
    public float moveSpeed = 5f;
    public float jumpForce;
    public Transform groundCheck;
    public LayerMask groundLayer;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool isGrounded;
    public float fallMultiplier;
    public float lowjumpMultiplier;

    //pickaxe and sprite rotation
    public Transform playerCenter; // Empty GameObject at the center of the player
    public GameObject pickaxe; // The pickaxe GameObject

    //Upgrades and shit
    public static int coins = 100; //used for upgrades
    public static int breakingpower = 0; // determins what blocks player can break, each level changes pickaxe sprite too
    public static int fullstamina = 35;
    public static int stamina = 2; //each block broke loses stamina 
    public static int fortune = 0; //chance to get mroe ores from mining

    //Health
    public int maxHealth = 100;
    public int health;

    //Animation
    private Animator anim;

    public Color damagedPlayer;
    public AudioSource walking;
    public AudioSource playerHurt;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        fullstamina = 50;
        fortune = 0;
        breakingpower = 0;
        coins = 100;
        stamina = fullstamina;
        health = maxHealth;
    }

    private void Update()
    {
        jumping();
        pickaxerotation();
       
        //Jump More Smooth
        if(rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }else if(rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowjumpMultiplier - 1) * Time.deltaTime;
        }

        if (!walking.isPlaying && isGrounded && rb.velocity != Vector2.zero)
            walking.Play();
        if(walking.isPlaying && !isGrounded || walking.isPlaying && rb.velocity == Vector2.zero)
            walking.Stop();
    }

    private void FixedUpdate()
    {
        movement();
    }
    void pickaxerotation()
    {
        {
            // Calculate the angle between playerCenter and mouse position
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 direction = mousePos - playerCenter.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Rotate the playerCenter (and the attached pickaxe)
            playerCenter.rotation = Quaternion.Euler(0f, 0f, angle);

            if (mousePos.x < playerCenter.position.x)
            {
                GetComponent<SpriteRenderer>().flipX = true;
                pickaxe.GetComponent<SpriteRenderer>().flipY = true;
            }
            else
            {
                GetComponent<SpriteRenderer>().flipX = false;
                pickaxe.GetComponent<SpriteRenderer>().flipY = false;
            }

        }
    }

    void jumping()
    {
        // Check if the character is grounded
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);

        // Jumping
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    void movement()
    {
        // Movement
        float moveDirection = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveDirection * moveSpeed, rb.velocity.y);

        if(rb.velocity != Vector2.zero)
        {
            anim.SetBool("isWalking", true);
        }
        else
        {
            anim.SetBool("isWalking", false);
        }
    }

    public void TakeDamage(int damage)
    {
        Death d = GetComponent<Death>();

        health -= damage;
        d.AffectHealthUI(maxHealth, health);
        StartCoroutine(DamagedPlayerRed());
        playerHurt.Play();
        if(health <= 0)
        {    
            d.Respawn();
        }
    }

    public void HealPlayer()
    {
        health = maxHealth;
    }

    IEnumerator DamagedPlayerRed()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.color = damagedPlayer;
        yield return new WaitForSeconds(0.25f);
        sr.color = Color.white;
    }

}