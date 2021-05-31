using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cinemachine;
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
    public Animator playerAnim;
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
    public CinemachineFreeLook cinMac;


    public float mouseSensitivity = 50f;
    public float SensiMultiplier = 0.01f;
    public GameObject aim;
    public float camRotateSpeed = 15f;
    float xRotation, yRotation;
    Quaternion prev_X_Rot, prev_Y_Rot;
    public float clampVal = 55f;
    public GameObject playerCanvas;

    //Audio
    public AudioSource _audioSource;
   
    public AudioClip walkingSound;
    public AudioClip runningSound;
    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
     
        _audioSource.clip = walkingSound;
        _audioSource.loop = true;

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
            cinMac.enabled = false;
            playerCamera.GetComponent<Camera>().enabled = false;
            playerCamera.GetComponent<CinemachineBrain>().enabled = false;
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
        PlayFootstepSounds();
        if (!IsLocalPlayer) return;
        GetInput();
      
        SetAnimation();
        Move();
        Jump();
       
    }
    private void FixedUpdate()
    {
        MouseLook(mouseX, mouseY);
    }
    void SetAnimation()
    {
        float xAnimMove = Input.GetAxisRaw("Horizontal");
        float yAnimMove = Input.GetAxisRaw("Vertical");
        playerAnim.SetFloat("yVelocity", velocity.y);

        if (isGrounded)
        {
            playerAnim.SetFloat("Vertical", yAnimMove, 0, Time.deltaTime);
            playerAnim.SetFloat("Horizontal", xAnimMove, 0, Time.deltaTime);
           
        }
        else if (!isGrounded)
        {
            playerAnim.SetFloat("Vertical", 0, 0, Time.deltaTime);
            playerAnim.SetFloat("Horizontal", 0, 0, Time.deltaTime);
        }

        playerAnim.SetBool("isGrounded", isGrounded);
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

        // Vector3 move = transform.right * xMove + transform.forward * yMove;
        if (!isGrounded)
        {
            yMove = yMove * 0.5f;
            xMove = xMove * 0.5f;
        }
        // Calculate the Direction to Move based on the tranform of the Player
        Vector3 moveDirectionForward = transform.forward * yMove;
        Vector3 moveDirectionSide = transform.right * xMove;//find the direction
        Vector3 direction = (moveDirectionForward + moveDirectionSide).normalized;
        //find the distance
        Vector3 distance = direction * speed * Time.deltaTime;
        
        // Apply Movement to Player

        characterController.Move(distance);



    }
    void Jump()
    {
       // playerAnim.SetBool("isJumping", isJumping);
        velocity.y += gravity * Time.deltaTime;
        if (isJumping && isGrounded)
        {
         
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }
        characterController.Move(velocity * Time.deltaTime);
    }
    public void MouseLook(float MouseX, float MouseY)

    {
        float CamYax = playerCamera.transform.rotation.eulerAngles.y;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, CamYax, 0), camRotateSpeed * Time.fixedDeltaTime);        //  transform.Rotate(Vector3.up * MouseX);

        xRotation += MouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
      
    }

    private void PlayFootstepSounds()
    {
        // print(isGrounded);
        if (!isGrounded)
        {
            if (_audioSource.isPlaying)
            {
                _audioSource.Pause();
            }
        }
       else if (xMove != 0 || yMove != 0)
        {
          
        
          
            {
                print(isGrounded);
                _audioSource.clip = runningSound;
                if (!_audioSource.isPlaying)
                {
                    _audioSource.Play();
                }
            }
        }

       
        else
        {
            if (_audioSource.isPlaying)
            {
                _audioSource.Pause();
            }
        }
     
        /*  float xAxMove = Input.GetAxisRaw("Horizontal");
          float yAxMove = Input.GetAxisRaw("Vertical");
          if (isGrounded && xMove == 1 || yMove == 1)
          {


                  _audioSource.clip = runningSound;

             // _audioSource.clip = input.Run ? runningSound : walkingSound;
              if (!_audioSource.isPlaying)
              {
                  _audioSource.Play();
              }
          }
          else
          {
              if (_audioSource.isPlaying)
              {
                  _audioSource.Pause();
              }
          }*/
    }

}
