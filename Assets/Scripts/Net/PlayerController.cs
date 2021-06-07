using MLAPI;
using MLAPI.SceneManagement;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : NetworkBehaviour
{
    // Networked fields
    public NetworkVariableString playerName = new NetworkVariableString(new NetworkVariableSettings
    {
        WritePermission = NetworkVariablePermission.OwnerOnly,
        ReadPermission = NetworkVariablePermission.Everyone
    });
    public NetworkVariableColor playerColor = new NetworkVariableColor(new NetworkVariableSettings
    {
        WritePermission = NetworkVariablePermission.OwnerOnly,
        ReadPermission = NetworkVariablePermission.Everyone
    });
    //Spawner
    public GameObject Player;
  
    public int connectedPlayers = 0;
    public Transform parent;
    [SerializeField] private List<Transform> spawnPoints;

    // Fields
    private GameObject myPlayerListItem;
   
    private TextMeshProUGUI playerNameLabel;

    public override void NetworkStart()
    {
        RegisterEvents();

        Debug.Log($"NetworkStart:: {NetworkManager.Singleton.LocalClientId} OWNER:{OwnerClientId} OS:{IsOwnedByServer}");
        myPlayerListItem = Instantiate(LobbyScene.Instance.playerListItemPrefab, Vector3.zero, Quaternion.identity);
        Manager.instance.Setup(myPlayerListItem.transform);
      //  myPlayerListItem.transform.SetParent(LobbyScene.Instance.playerListContainer);
        //connected players
            connectedPlayers = NetworkManager.Singleton.ConnectedClients.Count;
       
        playerNameLabel = myPlayerListItem.GetComponentInChildren<TextMeshProUGUI>();
        if (IsClient)
        {
            LobbyScene.instance.startButton.interactable = false;
        }
        if(IsHost)
        {
            LobbyScene.instance.startButton.interactable = true;

        }
        
        if (IsOwner)
        {
           
            playerName.Value = UnityEngine.Random.Range(1000, 9999).ToString();
         
            
            if(PlayerPrefs.GetString("PlayerName")==string.Empty)
            {
                print("hello");
            //    PlayerPrefs.SetString("PlayerName", UnityEngine.Random.Range(1000, 9999).ToString());
            }
            //  playerName.Value = PlayerPrefs.GetString("PlayerName");
            playerName.Value = LobbyScene.instance.playerNameInput.text;
            playerColor.Value = new Color(UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f));
        }
        else
        { 
            playerNameLabel.text = playerName.Value;
        }
    }
    
    public void OnDestroy()
    {
        Destroy(myPlayerListItem);
        UnregisterEvents();
    }

    public void ChangeName(string newName)
    {
        if (IsOwner)
        {
            PlayerPrefs.SetString("PlayerName",newName);
            playerName.Value = newName;
        }
            
    }

    // Events
    private void RegisterEvents()
    {
        playerName.OnValueChanged += OnPlayerNameChange;
        NetworkSceneManager.OnSceneSwitched += OnSceneChange;
    }
    private void UnregisterEvents()
    { 
        playerName.OnValueChanged -= OnPlayerNameChange;
        NetworkSceneManager.OnSceneSwitched -= OnSceneChange;
    }

    private void OnPlayerNameChange(string previousValue, string newValue)
    {
        playerNameLabel.text = playerName.Value;
        
    }
    void OnSceneChange()
    {
        Transform playersParent = GameObject.Find("Players").transform;
        transform.parent = playersParent.GetChild(0).transform;
        if (IsLocalPlayer)
        {
            print(NetworkManager.Singleton.LocalClientId);
            print("Scene of" + playerName.Value);
            SpawnMeServerRpc(NetworkManager.Singleton.LocalClientId);
        }
     
    }
    [ServerRpc(RequireOwnership = false)]
    private void SpawnMeServerRpc(ulong clientId)
    {
        Vector3 spawnPos = Vector3.zero;
        GameObject go = Instantiate(Player, spawnPoints[connectedPlayers-1].position, Quaternion.identity).gameObject;
        go.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, null, true);
        
       
         //connectedPlayers++;
       // print(connectedPlayers);
    }
}
