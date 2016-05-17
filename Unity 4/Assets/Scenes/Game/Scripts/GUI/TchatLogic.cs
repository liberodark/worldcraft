using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(NetworkView))]
public class					TchatLogic : MonoBehaviour
{
	#region Members
	#endregion

	#region Unity
	void						Start()
	{
		if (this.networkView.isMine && Network.peerType != NetworkPeerType.Disconnected)
			TchatGUI.Instance.SetLogic(this);
	}
	#endregion

	#region Network
	public void SendNickname(string nickname)
	{
		if (nickname.Length > 0)
		{
			networkView.RPC("SetNickname", RPCMode.All, nickname);
		}
	}

	[RPC]
	void						SetNickname(string nickname)
	{
		TchatGUI.Instance.SetNickname(this.networkView.viewID, nickname);
	}

	public void					SendTchatMessage(NetworkViewID to, string message)
	{
		if (message.Length > 0)
		{
			if (to == NetworkViewID.unassigned)
				networkView.RPC("TchatMessage", RPCMode.All, networkView.viewID, message);
			else
				networkView.RPC("Whisper", to.owner, networkView.viewID, message);
		}
	}

	[RPC]
	void						TchatMessage(NetworkViewID from, string message)
	{
		this.AddMessage(from, message, false);
	}

	[RPC]
	void						Whisper(NetworkViewID from, string message)
	{
		this.AddMessage(from, message, true);
	}
	#endregion

	#region Helpers
	void						AddMessage(NetworkViewID from, string message, bool whisper)
	{
		TchatGUI.Instance.AddMessage(from, message, whisper);
	}
	#endregion
}
