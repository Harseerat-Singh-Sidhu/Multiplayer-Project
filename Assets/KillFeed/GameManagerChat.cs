using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerChat2 : MonoBehaviour
{
	public static GameManagerChat2 instance;
	public delegate void OnPlayerKilledCallback(string h);
	public OnPlayerKilledCallback onPlayerKilledCallback;
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

 
}
