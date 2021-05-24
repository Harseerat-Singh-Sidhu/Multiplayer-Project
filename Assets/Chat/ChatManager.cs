using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatManager : MonoBehaviour
{
    [SerializeField]
    GameObject chatItemPrefab;
    public static ChatManager instance;
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
    private void Update()
    {
	//	print(transform.childCount);
        if (transform.childCount > 13)
        {
			Destroy(transform.GetChild(0).gameObject);
        }
    }

    public void OnMessageReceived(string msg)
	{
		GameObject go = (GameObject)Instantiate(chatItemPrefab, this.transform);
		go.GetComponent<ChatItem>().Setup(msg);

	}
}
