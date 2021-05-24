using MLAPI;
using UnityEngine;

public class GameScene : MonoSingleton<GameScene>
{
    [SerializeField] public Material[] playerMats;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
