
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

public class Chat : NetworkBehaviour
{
	[SerializeField] InputField inputField;
	string msgData;
	string clientName;
	string localAddress;
	UNetTransport m_Transport;

	string hostName;
	string ipAdd;



	

	public void Send()
	{
		if (!Input.GetKeyDown(KeyCode.Return)) return;
		if (string.IsNullOrWhiteSpace(inputField.text)) return;
	//	clientName = ipv4;
		
	
		if (NetworkManager.Singleton.ConnectedClients.TryGetValue(NetworkManager.Singleton.LocalClientId, out var networkedClient))
		{
			clientName = networkedClient.PlayerObject.GetComponent<PlayerController>().playerName.Value;
		}

		m_Transport = (UNetTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;
		
		msgData = clientName+" :- " +inputField.text;

		serServerRpc(msgData);
    }
	[ServerRpc(RequireOwnership =false)]
	public void serServerRpc(string msg)
    {
		SendMsgClientRpc(msg);
    }
	[ClientRpc]
	public void SendMsgClientRpc(string msg)
	{

		ChatManager.instance.onMessageReceivedCallback.Invoke(msg);
		inputField.text = string.Empty;

	}
	public static string GetIPPublic()
	{
		WebClient webClient = new WebClient();
		string publicIp = webClient.DownloadString("https://api.ipify.org");
		return publicIp;
	}

	public string GetIPLocal()
	{
		var host = Dns.GetHostEntry(Dns.GetHostName());
		string output = "";
		foreach (var ip in host.AddressList)
		{
			if (ip.AddressFamily == AddressFamily.InterNetwork)
			{
				output = ip.ToString();
				return ip.ToString();
			}
		}
		return output;
	}
	string ipv4 = GetIP(ADDRESSFAM.IPv4);
	string ipv6 = GetIP(ADDRESSFAM.IPv6);

	public static string GetIP(ADDRESSFAM Addfam)
	{
		//Return null if ADDRESSFAM is Ipv6 but Os does not support it
		if (Addfam == ADDRESSFAM.IPv6 && !Socket.OSSupportsIPv6)
		{
			return null;
		}

		string output = "";

		foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
		{
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
			NetworkInterfaceType _type1 = NetworkInterfaceType.Wireless80211;
			NetworkInterfaceType _type2 = NetworkInterfaceType.Ethernet;

			if ((item.NetworkInterfaceType == _type1 || item.NetworkInterfaceType == _type2) && item.OperationalStatus == OperationalStatus.Up)
#endif
			{
				foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
				{
					//IPv4
					if (Addfam == ADDRESSFAM.IPv4)
					{
						if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
						{
							output = ip.Address.ToString();
						}
					}

					//IPv6
					else if (Addfam == ADDRESSFAM.IPv6)
					{
						if (ip.Address.AddressFamily == AddressFamily.InterNetworkV6)
						{
							output = ip.Address.ToString();
						}
					}
				}
			}
		}
		return output;
	}


	public enum ADDRESSFAM
	{
		IPv4, IPv6
	}

}
