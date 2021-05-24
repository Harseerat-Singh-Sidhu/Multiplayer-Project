
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.Transports.UNET;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;

public class KillFeed : NetworkBehaviour
{






  

    public void Send(string msgData)
	{
		
		serServerRpc(msgData);
	}
	[ServerRpc(RequireOwnership = false)]
	public void serServerRpc(string msg)
	{
		SendMsgClientRpc(msg);
	}
	[ClientRpc]
	public void SendMsgClientRpc(string msg)
	{

		KillfeedManager.instance.onMessageReceivedCallback.Invoke(msg);

	}
	

}
