using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillfeedManager : MonoBehaviour
{
    public Transform killFeedCanvas;
    public KillFeed killFeedSript;
    [SerializeField]
    GameObject chatItemPrefab;
    public static KillfeedManager  instance;
    public delegate void OnMessageReceivedCallback(string h);
    public OnMessageReceivedCallback onMessageReceivedCallback;
    void Awake()
    {
      
        if (instance != null)
        {
            Debug.LogError("More than one GameManager in scene.");
        }
        else
        {
            instance = this;
        }
    }
    void Start()
    {
        onMessageReceivedCallback += OnMessageReceived;
    }

    public void onKillFunction(string dataString)
    {
        killFeedSript.Send(dataString);

    }
    public void OnMessageReceived(string msg)
    {
        GameObject go = (GameObject)Instantiate(chatItemPrefab, killFeedCanvas);
        go.GetComponent<KillFeedItem>().Setup(msg);
        Destroy(go, 4f);
    }
}
