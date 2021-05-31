using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
public class Spawner : NetworkBehaviour
{
    public GameObject Player;
    public int connectedPlayers = 0;
    [SerializeField] private List<Transform> spawnPoints;
    private void Start()
    {
        SpawnMeServerRpc(NetworkManager.Singleton.LocalClientId);

      /* 
        if (IsHost)
         {
            print("Hello2");
            spawnPlayerServerRpc();
        }*/
        
          
        
           
       

    }
    [ServerRpc(RequireOwnership = false)]
    private void SpawnMeServerRpc(ulong clientId)
    {

      
        Vector3 spawnPos = Vector3.zero;
        GameObject go = Instantiate(Player, spawnPoints[connectedPlayers].position, Quaternion.identity).gameObject;
        go.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, null, true);
        Transform playersParent = GameObject.Find("Players").transform;
        go.transform.parent = playersParent.transform;


        connectedPlayers++;
        print(connectedPlayers);
    }
}
