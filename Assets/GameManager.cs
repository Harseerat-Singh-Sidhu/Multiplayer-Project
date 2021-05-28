using MLAPI;
using MLAPI.SceneManagement;
using MLAPI.Connection;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using MLAPI.Prototyping;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

using UnityEngine.UI;

public class GameManager: NetworkBehaviour
{
    public bool isGameOver;
    //Game Over
   // public NetworkVariableBool isGameOver = new NetworkVariableBool(new NetworkVariableSettings
  //  { WritePermission = NetworkVariablePermission.OwnerOnly }, false);
   
    public NetworkVariableInt TotalGameTime = new NetworkVariableInt(new NetworkVariableSettings
    {
        WritePermission = NetworkVariablePermission.ServerOnly,
        ReadPermission = NetworkVariablePermission.Everyone
    });
    public Text timerText;
    private int totaltime = 10;

    public GameObject localPlayerCard;

   public List<PlayerMovement> players;
    public List<GameObject> playerCardList;
    public Canvas leaderBoardCanvas,gameOverCanvas;
    public Text GameOverText;
    public Button leaveButton;
    public GameObject playerCard;

    public static GameManager Instance;
    private void Awake()
    {
        Instance = this;
    }
    void OnEnable()
    {
        TotalGameTime.OnValueChanged += RefreshGUI;
    }

    void OnDisable()
    {
        TotalGameTime.OnValueChanged -= RefreshGUI;
    }
    private void Start()
    {
        leaderBoardCanvas.enabled = false;
        GameOverText.gameObject.SetActive(false);
        leaveButton.gameObject.SetActive(false);
        Invoke("GetPlayers",0.2f);
        TotalGameTime.Value = totaltime;
        timerText.text= (totaltime / 60).ToString("00") + ":" + (totaltime % 60).ToString("00");
        if (IsServer)
        {
            StartCoroutine("Timer");
        }
      
    }
    private void Update()
    {
        if (isGameOver) return;
        CheckTime();
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            updateLeaderBoard();
            leaderBoardCanvas.enabled = true;

        }
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            leaderBoardCanvas.enabled = false;    
        }
    }
    void updateLeaderBoard()
    {

        GetPlayers();
        SortPlayers();
        SetPlayerStats();
        
    }

    void SetPlayerStats()
    {
       
        for (int i = 0; i < playerCardList.Count; i++)
        {
            // if (playerCardList[i] != localPlayerCard)
            {
                playerCardList[i].transform.SetSiblingIndex(i + 1);
                playerCardList[i].GetComponent<PlayerCard>().SetStats(i+1);
               


            }
        /*   else
            {
                playerCardList[i].GetComponent<PlayerCard>().SetStats("You", players[i].KillCount.Value, players[i].DeathCount.Value);
                //playerCardList[i].transform.SetSiblingIndex(1);
            }*/


        }
       
      
    }
    void SortPlayers()
    {

         playerCardList.Sort(CompareByScore);

    }


    private int CompareByScore(GameObject b,GameObject a)
    {

  
        return a.GetComponent<PlayerCard>().playerScript.KillCount.Value.CompareTo(b.GetComponent<PlayerCard>().playerScript.KillCount.Value);
    }

    void GetPlayers()
    {
        foreach (var player in GameObject.FindObjectsOfType<PlayerMovement>())
        {
            if (!players.Contains(player))
            {
               
                GameObject p_Card = Instantiate(playerCard, leaderBoardCanvas.transform.GetChild(1));
                p_Card.GetComponent<PlayerCard>().playerScript = player.GetComponent<PlayerMovement>();
               
                if (player.IsLocalPlayer)
                {
                    localPlayerCard = p_Card;
                }
                playerCardList.Add(p_Card);
                p_Card.GetComponent<PlayerCard>().SetStats(playerCardList.Count);
                players.Add(player);
            }
        }

    }
    void RefreshGUI(int prevVal, int newVal)
    {
       if(newVal >=0)
        timerText.text = (newVal / 60).ToString("00") + ":" + (newVal % 60).ToString("00");


    }

    IEnumerator Timer()
    {

        yield return new WaitForSeconds(1f);
       TotalGameTime.Value--;
        if (TotalGameTime.Value >= 0)
        {
            StartCoroutine("Timer");
        }
        
    }
 
    public void CheckTime()
    {
        if (TotalGameTime.Value <= 0)
        {
            GameOver();

        }

    }

    public void GameOver()
    {
        isGameOver = true;
        StartCoroutine("shutNetwork");
       
       
        GameOverText.gameObject.SetActive(true);
        leaveButton.gameObject.SetActive(true);
        updateLeaderBoard();
        
        playerCardList[0].GetComponent<Image>().color = Color.yellow;
      
    }
    IEnumerator shutNetwork()
    {
        yield return new WaitForSeconds(1f);
        NetworkManager.Singleton.Shutdown();
        leaderBoardCanvas.enabled = true;
        Cursor.lockState = CursorLockMode.None;
    }
    public void LeaveButton()
    {
        SceneManager.LoadScene("Lobby");
    }
}
