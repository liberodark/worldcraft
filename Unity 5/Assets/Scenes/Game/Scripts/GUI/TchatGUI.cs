using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class					TchatGUI : MonoBehaviour
{
	#region Input Data
	public bool					_bottom;
	public Rect					_tchat_area;
	public int					_message_limit = 100;
	public GUIStyle				_message_style_normal;
	public GUIStyle				_message_style_whisper;
	public float				_overlay_timeout_delay = 10;
	#endregion

	#region Members
	State						_state = State.NotReady;
	float						_overlay_timer;
	List<Message>				_messages;
	Dictionary<NetworkViewID, string> _player_names;
	string						_current_message;
	NetworkViewID				_last_whisper_sender;
	NetworkViewID				_whisper_to;
	Vector2						_message_scroll_view;
	TchatLogic					_logic;
	#endregion

	#region Unity
	void						Start()
	{
		TchatGUI.Instance = this;
		if (this._bottom)
			this._tchat_area.y = Screen.height - this._tchat_area.height;
		this._overlay_timer = 0;
		this._messages = new List<Message>();
		this._player_names = new Dictionary<NetworkViewID, string>(Network.maxConnections);
		this._current_message = "";
		this._last_whisper_sender = NetworkViewID.unassigned;
		this._whisper_to = NetworkViewID.unassigned;
		this._message_scroll_view = Vector2.zero;
		this._state = State.Idle;
	}

	void					Update()
	{
		switch (this._state)
		{
			case State.Idle:
				if (InputManager.inputManager().returnInput)
					this.OpenTchatWindow();
				else if (InputManager.inputManager().respondInput) //Respond to last whisper
				{
					this.OpenTchatWindow();
					this._whisper_to = this._last_whisper_sender;
				}
				break;

			case State.Overlay:
				if (this._overlay_timer > 0)
					this._overlay_timer -= Time.deltaTime;
				else
					this._state = State.Idle;

				if (InputManager.inputManager().returnInput)
					this.OpenTchatWindow();
				else if (InputManager.inputManager().respondInput) //Respond to last whisper
				{
					this.OpenTchatWindow();
					this._whisper_to = this._last_whisper_sender;
				}
				break;
		}
	}
	
	void					OnGUI()
	{
		if (!this.IsDisplayState(State.Idle))
		{
			if (this.IsDisplayState(State.Window))
				GUI.Box(this._tchat_area, "Tchat");
			GUILayout.BeginArea(this._tchat_area);
			GUILayout.Space(20);

			//Show messages
			this._message_scroll_view = GUILayout.BeginScrollView(this._message_scroll_view);
			GUILayout.BeginVertical();
			foreach (Message msg in this._messages)
			{
				if (this._player_names.ContainsKey(msg.From))
					GUILayout.Label(this._player_names[msg.From] + ": " + msg.Content, msg.Whisper ? this._message_style_whisper : this._message_style_normal);
				else
					GUILayout.Label(GameStateManager.Login + msg.Content, msg.Whisper ? this._message_style_whisper : this._message_style_normal);
			}
			GUILayout.EndVertical();
			GUILayout.EndScrollView();

			//Input message
			if (this.IsDisplayState(State.Window))
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label(this._whisper_to == NetworkViewID.unassigned ? "All" : (this._player_names.ContainsKey(this._whisper_to) ? this._player_names[this._whisper_to] : "..."), GUILayout.Width(50));
				GUI.SetNextControlName("input_field");
				this._current_message = GUILayout.TextField(this._current_message, 99);
				if (this.IsDisplayState(State.Opening))
				{
					this._state ^= State.Opening;
					GUI.FocusControl("input_field");
				}
				this.CheckGUIEvent();
				GUILayout.EndHorizontal();
			}
			else
				GUILayout.Space(30);
			GUILayout.EndArea();
		}
	}
	#endregion

	#region Network
	void					SendCurrentMessage()
	{
		if (this._current_message.Length > 0)
		{
			if (this._logic != null)
				this._logic.SendTchatMessage(this._whisper_to, this._current_message);
			this._current_message = "";
		}
	}
	#endregion

	#region Helpers
	public void					SetLogic(TchatLogic logic)
	{
		this._logic = logic;
		this._logic.SendNickname(GameStateManager.Login);
	}

	bool						IsDisplayState(State state)
	{
		return ((this._state & state) == state);
	}

	void						OpenTchatWindow()
	{
		this._state = State.Window | State.Opening;
		GameStateManager.manager.ToggleMouse();
	}

	void						CloseTchatWindow()
	{
		this._state = State.Idle;
		GameStateManager.manager.ToggleMouse();
	}

	void						CheckGUIEvent()
	{
		if (Event.current.isKey)
		{
			switch (Event.current.keyCode)
			{
				case KeyCode.Return:
					this.SendCurrentMessage();
					Event.current.Use();
					break;

				case KeyCode.Tab:
					//TODO
					Event.current.Use();
					break;

				case KeyCode.Escape:
					this.CloseTchatWindow();
					Event.current.Use();
					break;
			}
		}
	}

	public void					SetNickname(NetworkViewID viewID, string nickname)
	{
		this._player_names[viewID] = nickname;
	}

	public void					AddMessage(NetworkViewID from, string message, bool whisper)
	{
		Debug.Log("Incomming Tchat message + " + message);

		this._messages.Add(new Message() { Whisper = whisper, From = from, Content = message });
		if (this._messages.Count > this._message_limit)
			this._messages.RemoveAt(0);
		else
			this._message_scroll_view.y = this._tchat_area.height;

		if (this._state == State.Idle)
			this._state = State.Overlay;
		if (this._state == State.Overlay)
			this._overlay_timer = this._overlay_timeout_delay;
	}
	#endregion

	#region Enums
	enum						State
	{
		NotReady				= 0x0000,
		Idle					= 0x0001,
		Overlay					= 0x0002,
		Window					= 0x0004,
		Opening					= 0x0010
	}
	#endregion

	#region Data Types
	struct						Message
	{
		public bool				Whisper;
		public NetworkViewID	From;
		public string			Content;
	}
	#endregion

	#region Props
	public static TchatGUI		Instance { get; set; }
	#endregion
}
