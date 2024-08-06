using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipMovement : UnityEngine.MonoBehaviour
{
    Rigidbody2D body;

    float horizontal;
    float vertical;
    float moveLimiter = 0.7f;

    public float runSpeed = 10.0f;

    private SpaceshipStats stats;
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        stats = GetComponent<SpaceshipStats>();
    }

    void Update()
    {
        // Gives a value between -1 and 1
        horizontal = Input.GetAxisRaw("Horizontal"); // -1 is left
        vertical = Input.GetAxisRaw("Vertical"); // -1 is down
    }

    void FixedUpdate()
    {
        if (horizontal != 0 && vertical != 0) // Check for diagonal movement
        {
            // limit movement speed diagonally, so you move at 70% speed
            horizontal *= moveLimiter;
            vertical *= moveLimiter;
        }
        FuelDeplete();
        body.velocity = new Vector2(horizontal * runSpeed, vertical * runSpeed);
    }

    void FuelDeplete()
    {
        if(horizontal != 0 || vertical != 0)
        {
            stats.fuel -= Time.deltaTime;
        }
    }
}
