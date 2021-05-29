using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MLAPI;
using MLAPI.NetworkVariable;
using MLAPI.Messaging;
public class GunScript : NetworkBehaviour
{
    public NetworkVariableBool isFiring = new NetworkVariableBool(new NetworkVariableSettings
    { WritePermission = NetworkVariablePermission.OwnerOnly }, false);

    public Animator gunAnim;
    //  public bool isFiring;
    public Transform playerCam;
    public Transform muzzleAnchor;

    public GameObject muzzleFlash, hitEffect;
    
   
    public bool isReloading;

    public float timer = 0f;
    public float fireRate=5f;
    public float verticalRecoil = 1f;
    public float horizontalRecoil = 1f;

    public float nextTimeToFire = 0;
    public float maxAmmo = 30;
    public float currentAmmo;
    public float reloadTime;
    //player Script
    public PlayerMovement playerMovementScript;

    //UI
    public Text ammoText;
  

    // Start is called before the first frame update
    void Start()

    {
        currentAmmo = maxAmmo;
        ammoText.text = currentAmmo + "/" + maxAmmo;
        playerMovementScript = GetComponentInParent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.isGameOver) return;

        if (IsLocalPlayer)
        {
            isFiring.Value = false;
            if (isReloading) return;
            if (currentAmmo <= 0)
            {
                StartCoroutine("Reload");
                return;
            }
            //   isFiring.Value = Input.GetMouseButtonDown(0);
            if (Input.GetMouseButton(0) && Time.time >= nextTimeToFire)
            {
           
                nextTimeToFire = Time.time + 1 / fireRate;
                print(nextTimeToFire);
                ShootServerRpc();
                currentAmmo--;
                ammoText.text = currentAmmo + "/" + maxAmmo;
                playerMovementScript.MouseLook(Random.Range(horizontalRecoil,-horizontalRecoil), verticalRecoil); ;
                
                isFiring.Value = true;
            }
            /*      if (isFiring.Value)
                  {
                      ShootServerRpc();

                  }*/
           
        }
        if (isFiring.Value)
        {
            GameObject muzzle = Instantiate(muzzleFlash, muzzleAnchor.position, muzzleAnchor.rotation);
            muzzle.transform.parent = muzzleAnchor;

        }



    }

   IEnumerator Reload()
    {
        isReloading = true;
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        ammoText.text = currentAmmo+"/" + maxAmmo;
        isReloading = false;
    }

    [ServerRpc]
    public void ShootServerRpc()
    {

        RaycastHit hit;
        if (Physics.Raycast(playerCam.position, playerCam.forward, out hit, 100f))
        {
            if (hit.transform.tag == "Player")
            {

                ulong clientId = hit.transform.GetComponent<NetworkObject>().OwnerClientId;
                // print("Hit player" + clientId);
                bool isEnemyDead = hit.transform.GetComponent<HealthSystem>().TakeDamage(10);
                if (isEnemyDead)
                {
                    playerMovementScript.KillCount.Value++;
                   //killfeed
                    string username = playerMovementScript.playerName;
                    string enemyUserName = hit.transform.GetComponent<PlayerMovement>().playerName;
                    KillfeedManager.instance.onKillFunction("<b>" + username + "</b>" + " killed " + enemyUserName);
                }
            }
            hitMarkClientRpc(hit.point, hit.normal);


        }

    }
    [ClientRpc]
    void hitMarkClientRpc(Vector3 impactPos, Vector3 impactRot)
    {
        gunAnim.Play("FireAnim", 0, 0f);
        GameObject _hitEffect = Instantiate(hitEffect, impactPos, Quaternion.identity);
        _hitEffect.transform.rotation = Quaternion.LookRotation(impactRot);

        Destroy(_hitEffect, 2f);

    }


}
