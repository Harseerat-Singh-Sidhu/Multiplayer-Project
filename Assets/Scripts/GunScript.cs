using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using MLAPI;
using MLAPI.NetworkVariable;
using MLAPI.Messaging;
public class GunScript : NetworkBehaviour
{
    //player Script
    public PlayerMovement playerMovementScript;

    //Net var
    public NetworkVariableBool isFiring = new NetworkVariableBool(new NetworkVariableSettings
    { WritePermission = NetworkVariablePermission.OwnerOnly }, false);
    public NetworkVariableBool _isReloading = new NetworkVariableBool(new NetworkVariableSettings
    { WritePermission = NetworkVariablePermission.OwnerOnly }, false);

    public Animator anim;
    //  public bool isFiring;
    public CinemachineFreeLook cinMac;
    public Transform playerCam;
    public Transform muzzleAnchor;

    [Header("Weapon Components")]
    public ParticleSystem muzzleflashParticles;
    public Light muzzleflashLight;

    public GameObject muzzleFlash, hitEffect;
    public Transform aimLookAt, firePos;

    public bool isReloading;

    public float timer = 0f;
    public float fireRate = 5f;
    public float verticalRecoil = 0.005f;
    public float horizontalRecoil = 0.005f;

    public float nextTimeToFire = 0;
    public float maxAmmo = 30;
    public float currentAmmo;
    public float reloadTime;


    //UI
    public Text ammoText;

    //Audio
    [Header("Audio Source")]
    //Main audio source
    public AudioSource audioSource;
    //Audio source used for shoot sound
    public AudioSource shootAudioSource;

    [System.Serializable]
    public class soundClips
    {
        public AudioClip shootSound;

        public AudioClip reloadSoundOutOfAmmo;
        public AudioClip reloadSoundAmmoLeft;
        public AudioClip aimSound;
    }
    public soundClips SoundClips;

    private void Awake()
    {
        cinMac = playerMovementScript.cinMac;
        muzzleflashParticles.Pause();
    }
    // Start is called before the first frame update
    void Start()

    {
        currentAmmo = maxAmmo;
        ammoText.text = currentAmmo + "/" + maxAmmo;

        // playerMovementScript = GetComponentInParent<PlayerMovement>();
    }
    public Vector3 worldPosition;
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

            }
            //   isFiring.Value = Input.GetMouseButtonDown(0);
            if (Input.GetMouseButton(0) && !isReloading && Time.time >= nextTimeToFire)
            {

                nextTimeToFire = Time.time + 1 / fireRate;
                ///   print(nextTimeToFire);
                anim.Play("Fire");
                ShootServerRpc();
                Recoil();
                currentAmmo--;
                ammoText.text = currentAmmo + "/" + maxAmmo;

                //   playerMovementScript.MouseLook(Random.Range(horizontalRecoil,-horizontalRecoil), verticalRecoil); ;

                isFiring.Value = true;
            }
            /* if (isFiring.Value)
             {
                 ShootServerRpc();
             }*/
            //Reload with R key for testing
            if (Input.GetKeyDown(KeyCode.R) && currentAmmo != maxAmmo)
            {
                anim.Play("Reload");
                StartCoroutine("Reload");
                //Play reload animation
                // anim.Play("Reload", 1, 0.0f);
            }

            // Ray ray = playerCam.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
            //  RaycastHit hit; // declare the RaycastHit variable
            //  if (Physics.Raycast(playerCam.position, playerCam.forward, out hit, 100f))
            {

                //    aimLookAt.position = hit.point;
            }

        }
        if (isFiring.Value)
        {
            muzzleflashParticles.Play();

            //Play light flash
            StartCoroutine(MuzzleflashLight());

            //  GameObject muzzle = Instantiate(muzzleFlash, muzzleAnchor.position, muzzleAnchor.rotation);
            //  muzzle.transform.parent = muzzleAnchor;
            //  Destroy(muzzle.gameObject, 01f); 
        }
        if (_isReloading.Value)
        {

            shootAudioSource.clip = SoundClips.reloadSoundOutOfAmmo;
            shootAudioSource.Play();
            //Play light flash

        }



    }
    public void Recoil()
    {
        cinMac.m_YAxis.Value -= verticalRecoil;

        cinMac.m_XAxis.Value += Random.Range(horizontalRecoil, -horizontalRecoil);

    }
    IEnumerator Reload()
    {
        _isReloading.Value = true;
        //Play reload animation
        //anim.Play("Reload");
        anim.Play("Reload");
        isReloading = true;
        yield return new WaitForSeconds(2.073f);
        currentAmmo = maxAmmo;
        ammoText.text = currentAmmo + "/" + maxAmmo;
        isReloading = false;
        _isReloading.Value = false;
    }
    [ServerRpc]
    public void ReloadServerRpc()
    {

    }
    [ClientRpc]
    public void ReloadClientRpc()
    {

    }
    IEnumerator MuzzleflashLight()
    {
        muzzleflashLight.enabled = true;
        yield return new WaitForSeconds(0.02f);
        muzzleflashLight.enabled = false;
    }
    [ServerRpc]
    public void ShootServerRpc()
    {
        
       
        RaycastHit hit;
        if (Physics.Raycast(firePos.position, playerCam.forward, out hit, 100f))
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
        shootAudioSource.clip = SoundClips.shootSound;
        shootAudioSource.Play();
        //gunAnim.Play("FireAnim", 0, 0f);
        GameObject _hitEffect = Instantiate(hitEffect, impactPos, Quaternion.identity);
        _hitEffect.transform.rotation = Quaternion.LookRotation(impactRot);

        Destroy(_hitEffect, 2f);

    }


}
