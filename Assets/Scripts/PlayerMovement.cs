using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.NetworkVariable;
public class PlayerMovement : NetworkBehaviour
{
    public PlayerController playerContr;
    //
    public string playerName;
    public int Score = 0;
    //
    public NetworkVariableInt KillCount = new NetworkVariableInt(new NetworkVariableSettings
    {
        WritePermission = NetworkVariablePermission.OwnerOnly,
        ReadPermission = NetworkVariablePermission.Everyone
    });
    public NetworkVariableInt DeathCount = new NetworkVariableInt(new NetworkVariableSettings
    {
        WritePermission = NetworkVariablePermission.OwnerOnly,
        ReadPermission = NetworkVariablePermission.Everyone
    });
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
    float xRotation, yRotation;
    Quaternion prev_X_Rot, prev_Y_Rot;
    public float clampVal = 55f;
    public GameObject playerCanvas;
    // Start is called before the first frame update
    void Start()
    {
      
     //   GameObject playersParent = GameObject.FindWithTag("Players");
      //  transform.parent = playersParent.transform.GetChild(1).transform;
        foreach (var pc in FindObjectsOfType<PlayerController>())
            {
            
            if (transform.GetComponent<NetworkObject>().OwnerClientId == pc.OwnerClientId)
            {
                playerContr = pc;
                playerName = pc.playerName.Value;
                transform.name = playerName;
          // transform.parent =pc.transform;
            }
               
            }
        //Transform playersParent = GameObject.Find("Players").transform;
        //    transform.parent = playersParent.transform;
       


        Application.targetFrameRate = 30;
        if (!IsLocalPlayer)
        {
            playerCamera.GetComponent<Camera>().enabled = false;
            playerCamera.GetComponent<AudioListener>().enabled = false;
            playerCanvas.SetActive(false);
            return;
        }

       
        
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.isGameOver) return;

        if (!IsLocalPlayer) return;
        GetInput();
        Move();
        Jump();
        MouseLook();
    }
    void GetInput()
    {
        xMove = Input.GetAxisRaw("Horizontal") * speed * Time.deltaTime;
        yMove = Input.GetAxisRaw("Vertical") * speed * Time.deltaTime;
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
