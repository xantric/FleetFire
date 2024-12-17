using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;
    public CharacterController characterController;

    public float gravity = -9.81f;
    public float jumpHeight = 3f;
    private Vector2 velocity;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    private bool isGrounded;

    public LayerMask groundLayer;
    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        characterController.Move(move * moveSpeed * Time.deltaTime);

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance);

        if (isGrounded && velocity.y < 0) {
            velocity.y = -2f;
        }
        if (Input.GetButtonDown("Jump") && isGrounded) {
            velocity.y = Mathf.Sqrt(-2f * jumpHeight * gravity);
        }
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity*Time.deltaTime);
    }
}
