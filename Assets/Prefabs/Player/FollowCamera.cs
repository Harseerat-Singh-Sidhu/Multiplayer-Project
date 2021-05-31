using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public GameObject aim;
    public float rotateSpeed = 15f;
    public Camera p_camera;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
       
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float CamYax = p_camera.transform.rotation.eulerAngles.y;
        transform.rotation = Quaternion.Slerp(transform.rotation,Quaternion.Euler(0,CamYax,0),rotateSpeed*Time.fixedDeltaTime);
      //  transform.localEulerAngles = new Vector3(0,transform.localEulerAngles.y, 0);
    }
}
