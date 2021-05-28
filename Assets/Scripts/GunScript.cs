using UnityEngine;
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
    public GameObject muzzleFlash, hitEffect;
    public Transform muzzleAnchor;
    public float timer = 0f;
    public float fireRate=5f;

    public float nextTimeToFire = 0;

    public int bulletCount = 14;

    //player Script
    public PlayerMovement playerMovementScript;

    // Start is called before the first frame update
    void Start()

    {
   
        playerMovementScript = GetComponentInParent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.isGameOver) return;

        if (IsLocalPlayer)
        {
            isFiring.Value = false;

         //   isFiring.Value = Input.GetMouseButtonDown(0);
            if (Input.GetMouseButton(0) && Time.time >= nextTimeToFire)
            {
           
                nextTimeToFire = Time.time + 1 / fireRate;
                print(nextTimeToFire);
                ShootServerRpc();
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
