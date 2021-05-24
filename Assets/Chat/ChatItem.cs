using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ChatItem : MonoBehaviour
{
    
	[SerializeField]
	Text text;

	public void Setup(string msg)
	{
		text.text = msg;
	}

}
