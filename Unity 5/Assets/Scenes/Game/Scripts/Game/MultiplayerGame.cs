using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class				MultiplayerGame : MonoBehaviour
{
	#region Input Data
	public Transform		_spawn;
	public Vector3			_cam_offset = new Vector3(0, 2.8f, 0.4f);
	public Transform		_network_player_prefab;
	public NetworkView		networkView;
	#endregion

	#region Members
	#endregion

	#region Unity
	IEnumerator				Start()
	{
		bool				is_network_game;

		is_network_game = Network.peerType != NetworkPeerType.Disconnected;
		this.Spawn(is_network_game);

		//new Client ("localhost", 1250);
		yield return new WaitForEndOfFrame();

		Network.isMessageQueueRunning = true;
	}

	void					OnPlayerConnected(NetworkPlayer network_player)
	{
		Debug.Log("Player Connected"+network_player.ipAddress);
		if (Network.peerType != NetworkPeerType.Server)
			networkView.RPC("OnTimeSyncRequest", network_player, TimeManager.getTime());
	}



	void					OnPlayerDisconnected(NetworkPlayer player)
	{
		Network.DestroyPlayerObjects(player);
		Debug.Log("Player Disconnected");
	}

	void					OnDestroy()
	{
		if (Network.peerType != NetworkPeerType.Disconnected)
			Network.Disconnect();
	}

	[RPC]
	void OnTimeSyncRequest(float time){
		TimeManager.setTime (time);
	}
	#endregion

	#region Helpers
	void					Spawn(bool is_network_game)
	{
		Transform			player;

		this._spawn.position.Set(this._spawn.position.x,200,this._spawn.position.z);

		if (is_network_game)
			player = (Transform)Network.Instantiate(this._network_player_prefab, this._spawn.position, this._spawn.localRotation, 0);
		else
			player = (Transform)Instantiate(this._network_player_prefab, this._spawn.position, this._spawn.localRotation);

		Camera.main.transform.parent = player;
		Camera.main.transform.localPosition = this._cam_offset;
		//Camera.main.GetComponent<SmoothFollow>().target = player;
		MultiplayerGame.Player = player;
	}
	#endregion

	#region Props
	public static Transform Player { get; set; }
	#endregion
}

public class Client{

	private Socket SocketClient = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);

	public Client(string ip, int port){
		IPHostEntry ipHostEntry = Dns.Resolve(ip);
		IPAddress ipAddress = ipHostEntry.AddressList[0];
		try
		{
			SocketClient.Connect(new IPEndPoint(ipAddress, port));
		}
		catch (SocketException e)
		{
			//Debug.Log(e);
		}
	}

	public void startSyncing(){

	}
}
