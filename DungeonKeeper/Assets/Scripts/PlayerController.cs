using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Rigidbody rb;
    
    public float speed, maxForce, jumpForce;
    private Vector2 move;
    public bool grounded;
    public LayerMask groundLayer;
    public float Range;
    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        Jump();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        RaycastHit hit;
        // Cast a ray downwards from the player's position
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Range, groundLayer))
        {
            Debug.DrawRay(transform.position, Vector3.down * Range, Color.green);
            grounded = true;
        }
        else
        {
            Debug.DrawRay(transform.position, Vector3.down * Range, Color.red);
            grounded = false;
        }
        Move();
    }
    void Jump()
    {
        Vector3 jumpForces = Vector3.zero;

        if (grounded)
        {
            jumpForces = Vector3.up * jumpForce;
        }

        rb.AddForce(jumpForces, ForceMode.VelocityChange);
    }

    void Move()
    {
        if (grounded)
        {
            Vector3 currentVelocity = rb.velocity;
            Vector3 targetVelocity = new Vector3(move.x, 0, move.y);
            targetVelocity *= speed;

            targetVelocity = transform.TransformDirection(targetVelocity);

            Vector3 velocityChange = (targetVelocity - currentVelocity);
            velocityChange = new Vector3(velocityChange.x, 0, velocityChange.z);

            Vector3.ClampMagnitude(velocityChange, maxForce);

            rb.AddForce(velocityChange, ForceMode.VelocityChange);
        }
    }

    public void SetGrounded(bool state)
    {
        grounded = state;
    }
}
