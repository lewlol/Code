using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteRotation : MonoBehaviour
{
    public float rotationSpeed = 5f;
    public float maxRotationAngle = 90f; // Adjust this as needed

    private void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 dirToMouse = mousePos - transform.position;

        float angle = Mathf.Atan2(dirToMouse.y, dirToMouse.x) * Mathf.Rad2Deg;

        // Calculate the clamped angle based on the maxRotationAngle
        float clampedAngle = Mathf.Clamp(angle, -maxRotationAngle, maxRotationAngle);

        // Apply rotation only to the Z-axis of the arm's local rotation using Transform.Rotate
        transform.rotation = Quaternion.Euler(0f, 0f, clampedAngle);
    }
}