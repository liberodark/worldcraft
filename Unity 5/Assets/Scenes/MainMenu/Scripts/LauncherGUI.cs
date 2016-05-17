using UnityEngine;
using System.Collections;
using System.Net;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;

public class LauncherGUI : AbstractMenu {

	private static float lastLoginFailed = 0;
	private static bool loginFailed = false;
	private static string failedMessage;
	public static bool disconnected = false;
	public bool confirm = false;
	public static bool isHashNeeded = true;
	private string startPassword;

	public static bool internetConnection = false;
	public static float lastInternetChecking = 0;

	public void OnDisconnected(){
		GameStateManager.Login = "";
		GameStateManager.Email = "";
		GameStateManager.Password = "";
		loginFailedMessage ("Connected from another place");
		loginFailed = true;
		GameStateManager.version = GameVersion.FREE;
	}

	public void Start(){
		CheckForInternetConnection ();
		PlayerLogs pl = PlayerLogs.LoadLogs();
		if (pl != null) {
			GameStateManager.Login = pl.Login;
			GameStateManager.Password = pl.Password;
			startPassword = pl.Password;
			isHashNeeded = false;
		}
	}

	protected override void OnGUI(){
		base.OnGUI ();
		Event e = Event.current;
		if (e.isKey && e.keyCode == KeyCode.Return) {
			Debug.Log("Confirm");
			confirm = true;
		}
	}

	public void Update(){
		if (Time.time - lastInternetChecking > 10f) {
			CheckForInternetConnection();
			lastInternetChecking = Time.time;
		}
	}
	
	protected override void OnMenuGUI(){

		Screen.lockCursor = false;
		Cursor.visible = true;

		GUIStyle centeredStyle = new GUIStyle (GUI.skin.label);
		centeredStyle.alignment = TextAnchor.MiddleCenter;
		centeredStyle.normal.textColor = Color.red;
		
		if (loginFailed && Time.time - lastLoginFailed < 5f) {
			GUILayout.Label (failedMessage,centeredStyle,GUILayout.Width(490));
		} else if (Time.time - lastLoginFailed >= 5f) {
			loginFailed = false;
		}

		if (!internetConnection) {
			failedMessage = "No internet connection";
			loginFailed = true;

			if(GUILayout.Button ("Play offline",GUILayout.Width(490))){
				SwitchTo<MainMenu>();
			}
		}
		else{
			if (disconnected) {
				disconnected = false;
				OnDisconnected ();
			} else if (GameStateManager.loggedIn) {
				SwitchTo<MainMenu>();
			}

			GUILayout.BeginHorizontal (GUILayout.Width(490));
				GUILayout.Label ("Login: ",GUILayout.Width(245));
				GameStateManager.Login = GUILayout.TextField(GameStateManager.Login, GUILayout.Width(245));
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal (GUILayout.Width(490));
				GUILayout.Label ("Password: ",GUILayout.Width(245));
			GameStateManager.Password = GUILayout.PasswordField(GameStateManager.Password, '*', GUILayout.Width(245));
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal (GUILayout.Width(490));
				if(GUILayout.Button("Signup",GUILayout.ExpandWidth(true))){
					SwitchTo<Signup>();
				}


			if(GameStateManager.Password != "" && GameStateManager.Login != ""){
				if(GUILayout.Button("Login",GUILayout.Width(245)))confirm = true;

				if(confirm){
					RegexUtilities rg = new RegexUtilities();
					confirm = false;
						
						using (WebClient wb = new WebClient())
						{
							string hashString = GameStateManager.Password;
							if(hashString != startPassword)isHashNeeded = true;
							NameValueCollection data = new NameValueCollection();
							if(isHashNeeded)hashString = Signup.HashString(GameStateManager.Password);
							
							data["id"] = GameStateManager.Login;
							data["psw"] = hashString;
							data["mode"] = "in";
							
							byte[] response = wb.UploadValues(MainMenu.url, "POST", data);
							using (MD5 md5Hash = MD5.Create())
							{
								if (!Signup.VerifyMd5Hash(md5Hash, "no", System.Text.Encoding.UTF8.GetString(response)))
								{
									GameStateManager.connectionID = System.Text.Encoding.UTF8.GetString(response);
									GameStateManager.loggedIn = true;
									GameStateManager.Password = hashString;
									GameStateManager.startProcess = true;
									GameStateManager.version = checkVersion();
									PlayerLogs pl = new PlayerLogs();
									pl.Login = GameStateManager.Login;
									pl.Password = GameStateManager.Password;
									pl.SerializeLogs();
									SwitchTo<MainMenu>();
								}
								else
								{
									loginFailed = true;
									failedMessage = "Login failed";
									lastLoginFailed = Time.time;
								}
							}
						}
				}
			}
			GUILayout.EndHorizontal ();
		}
	}

	[Serializable]
	public class PlayerLogs{

		public string Login{ get; set; }
		public string Password{ get; set; }

		public PlayerLogs(){}

		public void SerializeLogs(){
			BinaryFormatter serializer = new BinaryFormatter();
			System.IO.Directory.CreateDirectory(Application.dataPath+"/Options/");
			using (FileStream wr = new FileStream(Application.dataPath+"/Options/logs.wci", FileMode.Create)) {
				serializer.Serialize (wr, this);
			}
		}
		
		public static PlayerLogs LoadLogs(){
			PlayerLogs pl;
			try{
				BinaryFormatter serializer = new BinaryFormatter();
				using (FileStream rd = new FileStream(Application.dataPath+"/Options/logs.wci", FileMode.Open))
				{
					pl = (PlayerLogs)serializer.Deserialize(rd) as PlayerLogs;
				}
			}
			catch(Exception){
				return null;
			}
			return pl;
		}

	}
	

	public GameVersion checkVersion(){
		using (WebClient wb = new WebClient())
		{
			NameValueCollection data = new NameValueCollection();

			data["id"] = GameStateManager.Login;
			data["psw"] = GameStateManager.Password;
			data["mode"] = "vip";
			
			byte[] response = wb.UploadValues(MainMenu.url, "POST", data);
			using (MD5 md5Hash = MD5.Create())
			{
				if (Signup.VerifyMd5Hash(md5Hash, "vip", System.Text.Encoding.UTF8.GetString(response)))
				{
					return GameVersion.VIP;
				}
				return GameVersion.FREE;
			}
		}
	}

	public static void loginFailedMessage(string message){
		failedMessage = message;
		loginFailed = false;
		lastLoginFailed = Time.time;
	}

	public static void CheckForInternetConnection()
	{
		try
		{
			using (WebClient client = new WebClient())
			using (System.IO.Stream stream = client.OpenRead("http://www.google.com"))
			{
				internetConnection = true;
			}
		}
		catch
		{
			internetConnection = false;
		}
	}
}
