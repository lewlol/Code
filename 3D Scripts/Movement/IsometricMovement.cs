using UnityEngine;

public class IsometricMovement : MonoBehaviour
{
    private Rigidbody _rb;
    [SerializeField] private float _turnSpeed = 360;
    private Vector3 _input;

    public bool canMove = true;

    float speed;

    private void Start()
    {
        //Rigidbody
        _rb = GetComponent<Rigidbody>();
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
    }

    private void Move()
    {
        _rb.MovePosition(transform.position + transform.forward * _input.normalized.magnitude * speed * Time.deltaTime);
    }
}

public static class Helpers
{
    private static Matrix4x4 _isoMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));
    public static Vector3 ToIso(this Vector3 input) => _isoMatrix.MultiplyPoint3x4(input);
}
