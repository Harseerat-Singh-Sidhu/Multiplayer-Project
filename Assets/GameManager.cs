using MLAPI;
using MLAPI.Connection;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using MLAPI.Prototyping;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager: MonoBehaviour
{
   public List<PlayerMovement> players;
    public List<GameObject> playerCardList;
    public Canvas leaderBoardCanvas;
    public GameObject playerCard;
    private void Start()
    {
        leaderBoardCanvas.enabled = false;
        Invoke("GetPlayers",0.2f);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            leaderBoardCanvas.enabled = true;
            SetPlayerStats();

        }
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            leaderBoardCanvas.enabled = false;
    
        }
    }

    void SetPlayerStats()
    {
        for (int i = 0; i < playerCardList.Count; i++)
        {
            playerCardList[i].GetComponent<PlayerCard>().SetStats(players[i].playerName, players[i].KillCount.Value, players[i].DeathCount.Value);
        }
    }

    void GetPlayers()
    {
        foreach (var player in GameObject.FindObjectsOfType<PlayerMovement>())
        {
            if (!players.Contains(player))
            {
                GameObject p_Card = Instantiate(playerCard, leaderBoardCanvas.transform.GetChild(0));
                p_Card.GetComponent<PlayerCard>().SetStats(player.playerName,player.KillCount.Value,player.DeathCount.Value);
                playerCardList.Add(p_Card);
                players.Add(player);
            }
        }

    }
}
