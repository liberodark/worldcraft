using UnityEngine;
using System.Collections;
using System.Net;
using System.Threading;
using System.Security.Cryptography;
using System.Collections.Specialized;

public enum GameVersion{
	VIP, FREE
}

public class GameStateManager : MonoBehaviour {
	private bool paused;
	private bool menuPaused;
	private bool inventoryPaused;

	private static float lastCheck = 0;
	public static bool RequireDisconnexion = false;
	public static GameVersion version = GameVersion.FREE;
	public static bool loggedIn{ get; set;}
	public static string connectionID{ get; set;}
	public static bool startProcess = false;
	private static bool checking{ get; set;}
	public static bool Multiplayer = true;
	public static bool isInConfigs = false;

	public static string Login = "";
	public static string Password = "";
	public static string Email = "";

	private static GameStateManager _manager;
	public static GameStateManager manager {
		get {
			if(_manager == null) _manager = (GameStateManager) GameObject.FindObjectOfType( typeof(GameStateManager) );
			return _manager;
		}
	}

	public static bool IsMenuPaused{
		set{
			//Set the game "In menu pause", avoiding menu opening while other pause operations
			manager.menuPaused = value;
			IsPaused = value;
		}
		get{
			return manager.menuPaused;
		}
	}
	
	public static bool IsPaused {
		set {
			//TODO : Check behaviour, it is considered bad habit to change the timescale, and it doesn't belong in a game with multiplayer
			//if(value) Time.timeScale = 1f/10000f;
			//if(!value) Time.timeScale = 1;
			InventoryGUI.BlockUnderSelection = null; 
			InventoryGUI.IsBlockUnderSelection = false;
			InventoryGUI.ItemUnderSelection = null; 
			InventoryGUI.IsItemUnderSelection = false;

			if(!value){
				InputManager.inputManager().SerializeInputs();
			}

			manager.paused = value;
			if(value && IsMenuPaused) manager.SendMessage("OnMenuPause", SendMessageOptions.DontRequireReceiver);
			if(!value) manager.SendMessage("OnResume", SendMessageOptions.DontRequireReceiver);
			manager.ToggleMouse();
		}
		get {
			return manager.paused;
		}
	}
	public static bool IsPlaying {
		set {
			IsPaused = !value;
		}
		get {
			return !IsPaused;
		}
	}

	public static bool MouseEventLocked { get; set; }

	
	void Start() {
		GameStateManager.MouseEventLocked = false;
		Cursor.visible = false;
		Screen.lockCursor = true;
		paused = false;
	}

	void OnGUI()
	{
		if(startProcess)
			this.StartCoroutine(checkLogin());

		if (RequireDisconnexion) {
			RequireDisconnexion = false;
			Application.LoadLevel("MainMenu");
		}

		if (InputManager.inputManager().isKeyPressed && InputManager.inputManager().escapeInput)
		{
			if(!IsPaused || IsMenuPaused){
				IsMenuPaused = !IsMenuPaused;
			}
			else if(IsPaused && !IsMenuPaused){ IsPaused = !IsPaused;}
			Event.current.Use();
		}
	}

	public void ToggleMouse()
	{
		Cursor.visible = !Cursor.visible;
		Screen.lockCursor = !Screen.lockCursor;
	}

	public IEnumerator checkLogin(){
		startProcess = false;
		checking = false;
		lastCheck = Time.time;
		while (GameStateManager.loggedIn) {
			if(Time.time - lastCheck > 5f){
				lastCheck = Time.time;
				if(!checking)	
					new Thread(new ThreadStart(isLoggedIn)).Start ();
			}
			yield return null;
		}
		
	}
	
	public void isLoggedIn(){
		checking = true;
		LauncherGUI.CheckForInternetConnection ();
		if (!LauncherGUI.internetConnection) {
			GameStateManager.loggedIn = false;
			LauncherGUI.disconnected = true;
			GameStateManager.RequireDisconnexion = true;
			checking = false;
			return;
		}
		using (WebClient wb = new WebClient())
		{
			NameValueCollection data = new NameValueCollection();
			
			data["id"] = GameStateManager.Login;
			data["connectid"] = GameStateManager.connectionID;
			data["mode"] = "is";
			
			byte[] response = wb.UploadValues(MainMenu.url, "POST", data);
			using (MD5 md5Hash = MD5.Create())
			{
				if (!Signup.VerifyMd5Hash(md5Hash, "true", System.Text.Encoding.UTF8.GetString(response)))
				{
					GameStateManager.loggedIn = false;
					LauncherGUI.disconnected = true;
					GameStateManager.RequireDisconnexion = true;
				}
			}
		}
		checking = false;
	}

}