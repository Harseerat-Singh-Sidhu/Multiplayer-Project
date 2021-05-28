using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCard : MonoBehaviour
{
    public Text playerRank,playerName, killCount, deathCount;
    public PlayerMovement playerScript;
    public int Score;
    // Start is called before the first frame update
  public void SetStats(int rank)
    {
        playerRank.text =""+ rank;
        playerName.text = playerScript.playerName;
        killCount.text = string.Empty + playerScript.KillCount.Value;
        deathCount.text = string.Empty + playerScript.DeathCount.Value;
        Score = playerScript.KillCount.Value;
    }
}
