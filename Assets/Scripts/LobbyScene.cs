using MLAPI;
using MLAPI.SceneManagement;
using TMPro;
using UnityEngine;

public class LobbyScene : MonoSingleton<LobbyScene>
{
    [SerializeField] private Animator animator; 
    // Lobby UI
    [SerializeField] public GameObject playerListItemPrefab;
    [SerializeField] public Transform playerListContainer;
    [SerializeField] public TMP_InputField playerNameInput;
    //Chat
    public static LobbyScene instance;
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
    #region Buttons
    // Main
    public void OnMainHostButton()
    {
        NetworkManager.Singleton.StartHost();
        animator.SetTrigger("Lobby");
    }
    public void OnMainConnectButton()
    {
        NetworkManager.Singleton.StartClient();
        animator.SetTrigger("Lobby");
    }

    // Lobby
    public void OnLobbyBackButton()
    {
        animator.SetTrigger("Main");
        NetworkManager.Singleton.Shutdown();
    }
    public void OnLobbyStartButton()
    {
        NetworkSceneManager.SwitchScene("Game");
    }
    public void OnLobbySubmitNameChange()
    {
        string newName = playerNameInput.text;

        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(NetworkManager.Singleton.LocalClientId, out var networkedClient))
        {
            var player = networkedClient.PlayerObject.GetComponent<PlayerController>();
            if (player)
                player.ChangeName(newName);
        }
    }
    
    #endregion
}