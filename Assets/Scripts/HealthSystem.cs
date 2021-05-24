using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI.NetworkVariable;
using MLAPI;
using UnityEngine.UI;
using MLAPI.Messaging;

public class HealthSystem : NetworkBehaviour
{ //player Script
    public PlayerMovement playerMovementScript;
    //
    public int maxHealth = 100;
    public Text playerHealthText;
    public NetworkVariableInt playerHealth = new NetworkVariableInt(100);
    private Renderer[] renderes;
    private Collider[] colliders;
    private CharacterController cc;
    [SerializeField] private List<Transform> spawnPoints;
    // Start is called before the first frame update
    void Start()
    {
        //get player script
        playerMovementScript = GetComponentInParent<PlayerMovement>();

        renderes = GetComponentsInChildren<MeshRenderer>();
        colliders = GetComponentsInChildren<Collider>();
        cc = GetComponent<CharacterController>();
        playerHealthText.text = "" + playerHealth.Value;
 

    }
    void OnEnable()
    {
        playerHealth.OnValueChanged += OnHealthChanged;
    }

    void OnDisable()
    {
        playerHealth.OnValueChanged -= OnHealthChanged;
    }

    public bool TakeDamage(int damageAmount)
    {
        bool dead = false;
        if (playerHealth.Value > 0)
            playerHealth.Value -= damageAmount;
        if (playerHealth.Value <= 0)
        {
            playerMovementScript.DeathCount.Value++;
            playerHealth.Value = maxHealth;
            RespawnPlayerClientRpc();
            dead = true;
        }
        return dead;




    }

    void OnHealthChanged(int prevVal, int newVal)
    {


        playerHealthText.text = "" + newVal;


    }
    [ClientRpc]
    void RespawnPlayerClientRpc()
    {
        StartCoroutine(Respawn());
    }
    IEnumerator Respawn()
    {
       
        foreach (var collider in colliders)
        {
            collider.enabled = false;
        }
        foreach (var renderer in renderes)
        {
            renderer.enabled = false;
        }
       
        cc.enabled = false;
        GetComponent<NetworkObject>().enabled = false;
        yield return new WaitForSeconds(3f);
      
        int randIndex = (int)Random.Range(0, 3);
        print(randIndex);
        transform.position = spawnPoints[randIndex].position;
        yield return new WaitForSeconds(3f);
        GetComponent<NetworkObject>().enabled = true;
        foreach (var renderer in renderes)
        {
            renderer.enabled = true;
        }
        foreach (var collider in colliders)
        {
            collider.enabled = true;
        }
        cc.enabled = true;
        
    }
}
