using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCard : MonoBehaviour
{
    public Text playerName, killCount, deathCount;
    // Start is called before the first frame update
  public void SetStats(string pName,int kCount,int dCount)
    {
        playerName.text = pName;
        killCount.text = string.Empty + kCount;
        deathCount.text = string.Empty + dCount;
    }
}
