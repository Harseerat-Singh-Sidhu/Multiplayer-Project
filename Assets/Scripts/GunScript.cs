using UnityEngine;
using MLAPI;
using MLAPI.NetworkVariable;
using MLAPI.Messaging;
public class GunScript : NetworkBehaviour
{
      public NetworkVariableBool isFiring = new NetworkVariableBool(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.OwnerOnly }, false);
   
    public Animator gunAnim;
  //  public bool isFiring;
    public Transform playerCam;
    public GameObject muzzleFlash, hitEffect;
    public Transform muzzleAnchor;
    public float timer=0f, fireRate;
    public int bulletCount = 14;


    // Start is called before the first frame update
    void Start()
    
    {
        fireRate = 1f / 2f;

    }

    // Update is called once per frame
    void Update()
    {


        if (IsLocalPlayer)
        {
            isFiring.Value = Input.GetMouseButtonDown(0);
            if (isFiring.Value)
            {
                ShootServerRpc();

            }
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
            if(hit.transform.tag =="Player")
            {
              
             ulong clientId = hit.transform.GetComponent<NetworkObject>().OwnerClientId;
               // print("Hit player" + clientId);
                hit.transform.GetComponent<HealthSystem>().TakeDamage(clientId);
            }
            hitMarkClientRpc(hit.point, hit.normal);

  
        }

    }
    [ClientRpc]
    void hitMarkClientRpc( Vector3 impactPos, Vector3 impactRot)
    {
        gunAnim.Play("FireAnim",0,0f);
        GameObject _hitEffect = Instantiate(hitEffect, impactPos, Quaternion.identity);
        _hitEffect.transform.rotation = Quaternion.LookRotation(impactRot);

        Destroy(_hitEffect, 2f);
 
    }


}
