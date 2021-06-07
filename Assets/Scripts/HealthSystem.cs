using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI.NetworkVariable;
using MLAPI;
using MLAPI.Prototyping;
using UnityEngine.UI;
using MLAPI.Messaging;

public class HealthSystem : NetworkBehaviour
{ //player Script
    public PlayerMovement playerMovementScript;
    //
    public GameObject playerCanvas;
    public int maxHealth = 100;
    public Text playerHealthText;
    public Slider playerHealthSlider;
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
        playerHealthSlider.value = playerHealth.Value;


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
            //   playerHealth.Value = maxHealth;
            // RespawnPlayerClientRpc();
            dead = true;
        }
        return dead;
    }

    void OnHealthChanged(int prevVal, int newVal)
    {

        playerHealthSlider.value = newVal;
        playerHealthText.text = "" + newVal;
        if (newVal <= 0)
        {
            StartCoroutine(Respawn());

        }

    }
    [ServerRpc]
    void RespawnPlayerServerRpc()
    {
        RespawnPlayerClientRpc();
    }
    [ClientRpc]
    void RespawnPlayerClientRpc()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        StartCoroutine(Respawn());
    }
    IEnumerator Respawn()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        //  gameObject.SetActive(false);
        playerCanvas.SetActive(false);
        cc.enabled = false;
        playerMovementScript.enabled = false;
      //  this.enabled = false;
        
      //  GetComponent<NetworkTransform>().enabled = false;
      /*  foreach (var renderer in renderes)
        {
            renderer.enabled = false;
        }*/
    /*    foreach (var collider in colliders)
        {
            collider.enabled = false;
        }*/
      
       
        
      
      
        int randIndex = (int)Random.Range(0, 3);
        print(randIndex);
        transform.position = spawnPoints[randIndex].position;
        yield return new WaitForSeconds(3f);
        playerCanvas.SetActive(true);
        playerHealth.Value = maxHealth;
        //gameObject.SetActive(true);
        transform.GetChild(0).gameObject.SetActive(true);
        cc.enabled = true;
        playerMovementScript.enabled = true;
      //  this.enabled = true;
        //   GetComponent<NetworkObject>().enabled = true;
        /*   foreach (var renderer in renderes)
           {
               renderer.enabled = true;
           }*/
        /*   foreach (var collider in colliders)
           {
               collider.enabled = true;
           }
           gameObject.SetActive(false);
        */
        //  cc.enabled = true;
        // transform.GetChild(0).gameObject.SetActive(true);
    }
}
