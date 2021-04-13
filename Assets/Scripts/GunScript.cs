using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.NetworkVariable;
public class GunScript : NetworkBehaviour
{
    public NetworkVariableInt bullets = new NetworkVariableInt(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.OwnerOnly, ReadPermission =NetworkVariablePermission.Everyone }, 14);
    public Animator gunAnim;
    public bool isFiring;
    public Transform playerCam;
    public GameObject muzzleFlash,hitEffect;
    public Transform muzzleAnchor;
    public float timer, fireRate ;
    public int bulletCount = 14;
    // Start is called before the first frame update
    void Start()
    {
        fireRate = 1f / 2f;
        print(fireRate);
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsLocalPlayer)
        {
            bulletCount = bullets.Value;
            return;
        }
        if (Input.GetButtonDown("Fire1") && timer <=0)
        {
            isFiring = true;
            timer = fireRate;
            gunAnim.SetBool("Fire", true);

            bulletCount--;
        }
        else
        {
            isFiring = false;
            timer -= Time.deltaTime;
            gunAnim.SetBool("Fire", false);


        }

        if (isFiring)
        {
            bullets.Value--;
            GameObject muzzle = Instantiate(muzzleFlash, muzzleAnchor.position, muzzleAnchor.rotation);
            muzzle.transform.parent = muzzleAnchor;
            RaycastHit hit;
            if(Physics.Raycast(playerCam.position, playerCam.forward,  out hit,100f))
            {
                GameObject _hitEffect = Instantiate(hitEffect, hit.point, Quaternion.identity);
                _hitEffect.transform.rotation = Quaternion.LookRotation(hit.normal);             
                Destroy(_hitEffect, 2f);
            }
        }
    }
}
