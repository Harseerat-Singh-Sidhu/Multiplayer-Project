
using UnityEngine;
using MLAPI;
public class MenuScript : NetworkBehaviour
{
    public GameObject menuPanel;
    public GameObject menuCam;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
   public void Host()
    {
        NetworkManager.Singleton.StartHost();
        menuPanel.SetActive(false);
        menuCam.SetActive(false);
    }
    public void Join()
    {
        NetworkManager.Singleton.StartClient();
        menuPanel.SetActive(false);
        menuCam.SetActive(false);
    }
}
