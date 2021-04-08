using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
public class MouseLook : NetworkBehaviour
{
    public Transform playerBody;
    public float mouseSensi = 100f;
    float xRotation;
    public float clampVal =55f;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsLocalPlayer)
        {
            return;
        }
        float mouseX = Input.GetAxis("Mouse X") * mouseSensi * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensi * Time.deltaTime;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -clampVal, clampVal);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f) ;
        playerBody.Rotate(Vector3.up * mouseX);

    }
}
