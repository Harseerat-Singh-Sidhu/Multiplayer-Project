using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class PlayerMovement : NetworkBehaviour
{
    public CharacterController characterController;
    public float speed = 10f;
    public float jumpHeight = 3f;
    Vector3 velocity;
    public float gravity = -9.81f;
    public LayerMask groundMask;
    public Transform groundCheck;
    public float groundCheckRad = 0.4f;
    bool isGrounded;
    //Mouse Look
    public Transform playerCamera;
    public float mouseSensi = 100f;
    float xRotation;
    public float clampVal = 55f;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsLocalPlayer) return;
        Move();
        Jump();
        mouseLook();
    }
    
    void Move()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRad, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        float x = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        float y = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        Vector3 move = transform.right * x + transform.forward * y;
        characterController.Move(move);
        
       

    }
    void Jump()
    {
        velocity.y += gravity * Time.deltaTime;
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }
        characterController.Move(velocity * Time.deltaTime);
    }
    void mouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensi * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensi * Time.deltaTime;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -clampVal, clampVal);
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
}
