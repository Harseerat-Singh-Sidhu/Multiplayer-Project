using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FirstScene : MonoBehaviour
{

    public void Play()
    {
        SceneManager.LoadScene(1);
    }

}
