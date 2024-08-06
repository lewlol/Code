using UnityEngine;

public class  ISOPlayerMovement : MonoBehaviour
{
    private Rigidbody _rb;
    [SerializeField] private float _turnSpeed = 360;
    private Vector3 _input;

    private PlayerStats stats;
    private PlayerBattery pb;

    public bool canMove = true;

    private void Start()
    {
        //Rigidbody, Player Stats, and Battery Script References
        _rb = GetComponent<Rigidbody>();
        stats = GetComponent<PlayerStats>();
        pb = GetComponent<PlayerBattery>();
    }
    private void Update()
    {
        if (canMove) //Check if the player can move
        {
            GatherInput(); //Determine which way the character is trying to move
            Look(); //Rotate According to the character input
        }
    }

    private void FixedUpdate()
    {
        if (canMove)
        {
            Move();//Actually Move the Character
        }
    }

    private void GatherInput()
    {
        _input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
    }

    private void Look()
    {
        if (_input == Vector3.zero) return;

        var rot = Quaternion.LookRotation(_input.ToIso(), Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, _turnSpeed * Time.deltaTime);

        pb.RemoveBattery(1);
    }

    private void Move()
    {
        _rb.MovePosition(transform.position + transform.forward * _input.normalized.magnitude * stats.speed * Time.deltaTime);
    }
}

public static class Helpers
{
    private static Matrix4x4 _isoMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));
    public static Vector3 ToIso(this Vector3 input) => _isoMatrix.MultiplyPoint3x4(input);
}
