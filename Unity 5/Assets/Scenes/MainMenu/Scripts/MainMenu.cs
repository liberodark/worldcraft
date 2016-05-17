using UnityEngine;
using System.Collections;
using System.Net;
using System.Collections.Specialized;
using System.Security.Cryptography;

public class MainMenu : AbstractMenu {
	[SerializeField] private BlockSet[] blockSetList;
	private bool activateMode = false;
	private string key = "";
	public static string url = "http://www.worldcraftgame.altervista.org/buy/log.php";
	private static bool keyFailed = false;
	private static float lastKeyFailed = 0;
	private static string keyMessage = "";

	protected override void OnMenuGUI() {

		//Free the cursor after quitting the game
		Screen.lockCursor = false;
		Cursor.visible = true;

		GUIStyle centeredStyle = new GUIStyle (GUI.skin.label);
		centeredStyle.alignment = TextAnchor.MiddleCenter;
		centeredStyle.normal.textColor = Color.red;
		
		if (keyFailed && Time.time - lastKeyFailed < 5f) {
			GUILayout.Label (keyMessage,centeredStyle,GUILayout.Width(490));
		} else if (Time.time - lastKeyFailed >= 5f) {
			keyFailed = false;
		}

		if( GUILayout.Button("Play", GUILayout.Width(490)) ) {
			activateMode = false;
			SwitchTo<StartGameMenu>();
		}
		if (GameStateManager.version == GameVersion.VIP && GameStateManager.loggedIn && GUILayout.Button ("Multiplayer", GUILayout.Width (490))) {
			activateMode = false;
			SwitchTo<MultiplayerMenu> ();
		} else {
			if(GameStateManager.version == GameVersion.FREE && !activateMode && GameStateManager.loggedIn && GUILayout.Button("Activate",GUILayout.Width(490))){
				activateMode = true;
			}
			if(GameStateManager.version == GameVersion.FREE && activateMode){
				GUILayout.BeginHorizontal();
				key = GUILayout.TextField(key,40,GUILayout.Width(456));
				if(GUILayout.Button("OK",GUILayout.Width(30)) && key.Length == 40){
					activateKey();
				}
				GUILayout.EndHorizontal();
			}
		}
		if( GUILayout.Button("Options", GUILayout.Width(490)) ) {
			activateMode = false;
			SwitchTo<OptionMenu>();
		}
		if( GUILayout.Button("Quit", GUILayout.Width(490)) ) {
			activateMode = false;
			Application.Quit();
		}
	}

	public void activateKey(){
		using (WebClient wb = new WebClient())
		{
			NameValueCollection data = new NameValueCollection();
			

			data["id"] = GameStateManager.Login;
			data["psw"] = GameStateManager.Password;
			data["key"] = key;
			data["mode"] = "activate";
			
			byte[] response = wb.UploadValues(MainMenu.url, "POST", data);
			using (MD5 md5Hash = MD5.Create())
			{
				if (GameStateManager.loggedIn && Signup.VerifyMd5Hash(md5Hash, "ok", System.Text.Encoding.UTF8.GetString(response)))
				{
					GameStateManager.version = GameVersion.VIP;
					activateMode = false;
				}
				else if(GameStateManager.loggedIn && Signup.VerifyMd5Hash(md5Hash, "badUser", System.Text.Encoding.UTF8.GetString(response))){
					keyFailed = true;
					keyMessage = "Account already activated!";
					lastKeyFailed = Time.time;
				}
				else if(GameStateManager.loggedIn && Signup.VerifyMd5Hash(md5Hash, "badLogin", System.Text.Encoding.UTF8.GetString(response))){
					keyFailed = true;
					keyMessage = "Invalid user!";
					lastKeyFailed = Time.time;
				}
				else if(GameStateManager.loggedIn && Signup.VerifyMd5Hash(md5Hash, "badKey", System.Text.Encoding.UTF8.GetString(response))){
					keyFailed = true;
					keyMessage = "Invalid key!";
					lastKeyFailed = Time.time;
				}
				else{
					Debug.Log(System.Text.Encoding.UTF8.GetString(response));
					keyFailed = true;
					keyMessage = "Internal error!";
					lastKeyFailed = Time.time;
				}
			}
		}
	}
	
}
