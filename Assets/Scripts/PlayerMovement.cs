using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class PlayerMovement : NetworkBehaviour
{
    public CharacterController characterController;
    float xMove, yMove;
    bool isJumping;
    public float speed = 10f;
    public float jumpHeight = 3f;
    Vector3 velocity;
    public float gravity = -9.81f;
    public LayerMask groundMask;
    public Transform groundCheck;
    public float groundCheckRad = 0.2f;
    bool isGrounded;
    //Mouse Look
    float mouseX;
    float mouseY;
    public Transform playerCamera;
    public float mouseSensitivity = 50f;
    public float SensiMultiplier = 0.01f;
    float xRotation,yRotation;
    Quaternion prev_X_Rot, prev_Y_Rot;
    public float clampVal = 55f;
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 30;
        if (!IsLocalPlayer)
        {
            playerCamera.GetComponent<Camera>().enabled = false;
            playerCamera.GetComponent<AudioListener>().enabled = false;
            return;
        }
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    public float timer = 0;
    public int temp = 0;
    void Update()
    {
        if (temp < 30)
        {
            timer += Time.deltaTime;
            temp++;
            print(timer);
        }
        if (!IsLocalPlayer) return;
        GetInput();
        Move();
        Jump();
        MouseLook();
    }
    void GetInput()
    {
        xMove = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        yMove = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        isJumping = Input.GetButtonDown("Jump");
        mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity;
        mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity;

    }
    void Move()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRad, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
       
        Vector3 move = transform.right * xMove + transform.forward * yMove;
        characterController.Move(move);
        
       

    }
    void Jump()
    {
        velocity.y += gravity * Time.deltaTime;
        if (isJumping && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }
        characterController.Move(velocity * Time.deltaTime);
    }
    void MouseLook()
    {
       
        transform.Rotate(Vector3.up * mouseX);

        xRotation += mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.transform.localEulerAngles = Vector3.left * xRotation;
    }

}
