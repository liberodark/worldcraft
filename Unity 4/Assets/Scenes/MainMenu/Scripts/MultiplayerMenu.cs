using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class							MultiplayerMenu : AbstractMenu
{
	#region Input Data
	public int						_nickname_min = 3;
	public string					_room_name = "WorldCraftMultiplayerRoom";
	#endregion

	#region Members
	Page							_page = Page.Main;
	State							_state = State.Idle;
	string							_nickname = GameStateManager.Login;
	int								_max_players = 8;
	int								_port = 1337;
	string							_server_name = "My WorldCraft Server";
	string							_server_desc = "A very cool WorldCraft Server";
	Vector2							_rooms_scroll_pos = Vector2.zero;
	HostData						_selected_room = null;
	#endregion

	#region GUI
	protected override void			OnMenuGUI()
	{
		switch (this._page)
		{
			case Page.Main: this.PageMain(); break;
			case Page.Create: this.PageCreate(); break;
			case Page.Join: this.PageJoin(); break;
		}
		
		if(GUILayout.Button("Back"))
		{
			switch (this._page)
			{
				case Page.Main: SwitchTo<MainMenu>(); break;
				case Page.Create: this._page = Page.Main; break;

				case Page.Join:
					this._page = Page.Main;
					if (this._state != State.Idle)
					{
						this._state = State.Idle;
						Network.Disconnect();
					}
					break;
			}
		}
	}

	private void					PageMain()
	{
		GUILayout.BeginVertical(GUILayout.Width(490));
		if (GUILayout.Button("Create"))
			this._page = Page.Create;
		else if (GUILayout.Button("Join"))
		{
			MasterServer.RequestHostList(this._room_name);
			this._page = Page.Join;
		}
		GUILayout.EndVertical();
	}

	private void					PageCreate()
	{
		GUILayout.Label("Server name:");
		this._server_name = GUILayout.TextField(this._server_name);

		GUILayout.BeginHorizontal(GUILayout.Width(490));

		GUILayout.BeginVertical();
		GUILayout.Label("Max Players:");
		this._max_players = int.Parse(GUILayout.TextField(this._max_players.ToString()));
		GUILayout.EndVertical();

		GUILayout.BeginVertical();
		GUILayout.Label("Port:");
		this._port = int.Parse(GUILayout.TextField(this._port.ToString()));
		GUILayout.EndVertical();

		GUILayout.EndHorizontal();

		if (GUILayout.Button("Start",GUILayout.Width(490)))
		{
			this._state = State.Starting;

			//Network.InitializeSecurity();
			Network.InitializeServer(this._max_players, this._port, !Network.HavePublicAddress());
			this._server_desc = (UnityEngine.Random.seed = Mathf.RoundToInt(Time.realtimeSinceStartup * 1000)).ToString();
			MasterServer.RegisterHost(this._room_name, this._server_name, this._server_desc);
			Network.isMessageQueueRunning = true;
			Application.LoadLevel("Game");
		}
	}

	private void					PageJoin()
	{
		HostData[] rooms = MasterServer.PollHostList();
		this._rooms_scroll_pos = GUILayout.BeginScrollView(this._rooms_scroll_pos);
		foreach (HostData room in rooms)
		{
			bool selected = this._selected_room != null ? room.guid == this._selected_room.guid : false;
			if (GUILayout.Toggle(selected, room.gameName + " @" + String.Format("{0}", room.ip) + ":" + room.port + " (" + room.connectedPlayers + "/" + room.playerLimit + " players)", GUI.skin.button))
			{
				this._selected_room = room;
			}
		}
		GUILayout.EndScrollView();
		switch (this._state)
		{
			case State.Idle:
				if (this._selected_room == null)
				GUILayout.Label(rooms.Length > 0 ? "Select a game to join." : "No game currently running",GUILayout.Width(490));
			else if (GUILayout.Button("Join " + this._selected_room.gameName,GUILayout.Width(490)))
				{
					UnityEngine.Random.seed = int.Parse(this._selected_room.comment);
					Network.isMessageQueueRunning = false;
					Network.Connect(this._selected_room);
					this._state = State.Connecting;
				}
				break;

		case State.Connecting: GUILayout.Label("Connecting".PadRight((int)Time.realtimeSinceStartup % 3, '.'),GUILayout.Width(490)); break;
		case State.Starting: GUILayout.Label("Starting Game...",GUILayout.Width(490)); break;
		}
	}
	#endregion

	#region Unity
	void							Start()
	{
		_nickname = GameStateManager.Login;
	}

	void							OnConnectedToServer()
	{
		Application.LoadLevel("Game");
	}
	#endregion

	#region Enums
	enum							Page
	{
		Main, Create, Join
	}

	enum							State
	{
		Idle, Connecting, Starting
	}
	#endregion
}
