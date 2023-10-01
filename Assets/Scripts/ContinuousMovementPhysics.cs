using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ContinuousMovementPhysics : MonoBehaviour
{
    public AudioSource footstepSource;
    public float speed = 1;
    public float turnSpeed = 60;
    public bool onlyMoveWhenGrounded = false;
    public InputActionProperty turnInputSource;
    public InputActionProperty moveInputSource;
    public Rigidbody rb;
    private bool isGrounded;
    public LayerMask groundLayer;
    public Transform directionSource;
    public CapsuleCollider bodyCollider;
    private Vector2 inputMoveAxis;
    private float inputTurnAxis;
    public Transform turnSource;

    private bool isMoving = false; // Track if the player is moving.

    // Update is called once per frame
    void Update()
    {
        inputMoveAxis = moveInputSource.action.ReadValue<Vector2>();
        inputTurnAxis = turnInputSource.action.ReadValue<Vector2>().x;

        // Check if the player is moving.
        isMoving = inputMoveAxis.magnitude > 0.1f;
    }

    private void FixedUpdate()
    {
        isGrounded = CheckIfGrounded();
        if (!onlyMoveWhenGrounded || (onlyMoveWhenGrounded && isGrounded))
        {
            Quaternion yaw = Quaternion.Euler(0, 1 * directionSource.eulerAngles.y, 0);
            Vector3 direction = yaw * new Vector3(inputMoveAxis.x, 0, inputMoveAxis.y);
            Vector3 targetMovePosition = rb.position + direction * Time.fixedDeltaTime * speed;
            rb.MovePosition(targetMovePosition);
        }

        // Play footstep sound when moving, stop it when not moving.
        if (isMoving)
        {
            if (!footstepSource.isPlaying)
            {
                footstepSource.Play();
                footstepSource.loop = true;
            }
        }
        else
        {
            footstepSource.loop = false;
        }
    }

    public bool CheckIfGrounded()
    {
        Vector3 start = bodyCollider.transform.TransformPoint(bodyCollider.center);
        float rayLength = bodyCollider.height / 2 - bodyCollider.radius + 0.05f;

        bool hasHit = Physics.SphereCast(start, bodyCollider.radius, Vector3.down, out RaycastHit hitInfo, groundLayer);

        return hasHit;
    }
}

