using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 12f;
    public CharacterController controller;

    public float gravity = -9.81f;
    public float jumpHeight = 3f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;

    public LayerMask groundLayer;

    Vector3 velocity;
    AudioManager audioManager;

private void Awake()

{
audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
}


    bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * moveSpeed * Time.deltaTime);

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayer);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            audioManager.PlaySFX (audioManager.jump);
            
            velocity.y = Mathf.Sqrt(-2f * gravity * jumpHeight);
        }
    if (move!= new Vector3(0,0,0)){ 
audioManager.PlaySFX (audioManager.Movement);
    }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
       // Debug.Log(velocity.y);

    }
}
