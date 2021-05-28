using MLAPI;
using MLAPI.SceneManagement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using MLAPI.Transports.UNET;
public class LobbyScene : MonoSingleton<LobbyScene>
{
    [SerializeField] public InputField IPAddInputField;
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
      /*  UNetTransport m_Transport = (UNetTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;
     
        if (string.IsNullOrWhiteSpace(IPAddInputField.text))
        {
            m_Transport.ConnectAddress = "127.0.0.1";
        }
        else
        {
            m_Transport.ConnectAddress = IPAddInputField.text;
        }*/
       
        NetworkManager.Singleton.StartHost();
        animator.SetTrigger("Lobby");
    }
    public void OnMainConnectButton()
    {
        UNetTransport m_Transport = (UNetTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;

        if (string.IsNullOrWhiteSpace(IPAddInputField.text))
        {
            m_Transport.ConnectAddress = "127.0.0.1";
        }
        else
        {
            m_Transport.ConnectAddress = IPAddInputField.text;
        }

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