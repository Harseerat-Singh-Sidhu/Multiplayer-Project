using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.NetworkVariable;

public class MenuScript : NetworkBehaviour
{
    
    public GameObject menuPanel;
    public GameObject menuCam;
    public int connectedPlayers = 0;
    [SerializeField] private List<Transform> spawnPoints;

    public void Host()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        NetworkManager.Singleton.StartHost(spawnPoints[0].position, Quaternion.identity);
        menuPanel.SetActive(false);
        menuCam.SetActive(false);
    }

    public void Join()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        NetworkManager.Singleton.StartClient();
        menuPanel.SetActive(false);
        menuCam.SetActive(false);
    }


    private void ApprovalCheck(byte[] connectionData, ulong clientId, MLAPI.NetworkManager.ConnectionApprovedDelegate callback)
    {
        print("Hello");

        bool approveConnection = true;

        Vector3 spawnPos = Vector3.zero;
        Quaternion spawnRot = Quaternion.identity;
        int playerCount = NetworkManager.Singleton.ConnectedClients.Count;
        spawnPos = spawnPoints[playerCount].position;


        callback(true, null, approveConnection, spawnPos, spawnRot);
    }
}

