using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI.NetworkVariable;
using MLAPI;
using UnityEngine.UI;
using MLAPI.Messaging;

public class HealthSystem : NetworkBehaviour 
{
    public Text playerHealthText;
    public GameObject playerCanvas;
    public NetworkVariableInt playerHealth = new NetworkVariableInt(100);
    // Start is called before the first frame update
    void Start()
    {
           
            playerHealthText.text = "" + playerHealth.Value;

    }
   public struct ClientRpcSendParams
    {
        // who are the target clients?
        public ulong TargetClientId;
    }

    struct ClientRpcReceiveParams
    {
    }

    struct ClientRpcParams
    {
        ClientRpcSendParams Send;
        ClientRpcReceiveParams Receive;
    }
    public ClientRpcSendParams setWho(ulong client)
    {
        ClientRpcSendParams clientSendParams = new ClientRpcSendParams();
        clientSendParams.TargetClientId =client;
        return clientSendParams;
    }
    public void TakeDamage(ulong clientId)
    {
        setWho(clientId);
         playerHealth.Value -= 10;
        updateHealthTextClientRpc();
        



    }
 

    [ClientRpc]
    public void updateHealthTextClientRpc()
    {
       
            playerHealthText.text = "" + playerHealth.Value;

       
    }
}
