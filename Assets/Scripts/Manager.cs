using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour
{
    public Transform playerListContainer;
    public Transform playerListItem;
    public static Manager instance;
    private void Awake()
    {
        instance = this;
    }
    public void Play()
    {
        SceneManager.LoadScene(1);
    }
    public void Setup(Transform playerItem)
    {
        playerItem.parent = playerListContainer;
        playerItem.transform.localScale = new Vector3(1, 1, 1);
    }
    public void Quit()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                 Application.Quit();
        #endif
    }
}
