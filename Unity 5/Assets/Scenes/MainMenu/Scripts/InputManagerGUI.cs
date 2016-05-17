using UnityEngine;
using System.Collections;

public class InputManagerGUI : AbstractMenu {

	private static KeyCode keyPressed;
	private static bool initial = false;
	private static bool inLoop = false;
	private static int currentIndex = 0;
	private static Vector2 position = new Vector2();

	private static GUIStyle[] styles;
	private static Color activeColor = new Color(0f,1f,1f);
	private static Color inactiveColor;
	
	private static void initialize(){
		if (!initial) {
			InputManager.inputManager();
			initial = true;

			styles = new GUIStyle[InputManager.dictionnary.Count];
			inactiveColor = GUI.skin.button.normal.textColor;

			for(int i = 0; i < InputManager.dictionnary.Count; i++){
				styles[i] = new GUIStyle(GUI.skin.button);
				//styles[i].border = new RectOffset(0,0,0,10);
			}

		}
	}

	protected override void OnMenuGUI() {

		initialize ();

		//Start of GUI part
		position = GUILayout.BeginScrollView(position,GUILayout.Width(500),GUILayout.Height(Screen.height/5));
		foreach (string name in InputManager.dictionnary.Keys) {
			GUILayout.BeginHorizontal(GUILayout.Width(460));
			GUILayout.Label(name,GUILayout.Width (200));
			if (GUILayout.Button (InputManager.dictionnary[name].ToString(),styles[InputManager.dictionnary.IndexOfKey(name)],GUILayout.Width (200))) {

				styles[InputManager.dictionnary.IndexOfKey(name)].normal.textColor = activeColor;
				styles[InputManager.dictionnary.IndexOfKey(name)].focused.textColor = activeColor;
				styles[InputManager.dictionnary.IndexOfKey(name)].active.textColor = activeColor;
				styles[InputManager.dictionnary.IndexOfKey(name)].hover.textColor = activeColor;

				StartCoroutine (WaitForAKey ());
				currentIndex = InputManager.dictionnary.IndexOfKey(name);
		
			}
			GUILayout.EndHorizontal();
		}
		GUILayout.EndScrollView();

		GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
			GUILayout.Label("Mouse sensitivity");
			InputManager.inputManager ().sensitivity = GUILayout.HorizontalSlider (InputManager.inputManager ().sensitivity, 0f, 10f);
		GUILayout.EndHorizontal ();

		GUILayout.BeginHorizontal ();
			if(GUILayout.Button("Back")){
				InputManager.inputManager().SerializeInputs();
				inLoop = false;
				SwitchTo<OptionMenu>();
			}
		
		GUILayout.EndHorizontal();

		//End of GUI part

		Event e = Event.current;
		if (inLoop && (e.type == EventType.keyDown)) {
			inLoop = false;
			keyPressed = e.keyCode;
			setKey();

		} else if (inLoop && Input.GetKey (KeyCode.LeftShift)) {
			inLoop = false;
			keyPressed = KeyCode.LeftShift;
			setKey();

		}
		else if(inLoop && Input.GetKey(KeyCode.RightShift)){
			inLoop = false;
			keyPressed = KeyCode.RightShift;
			setKey();
		}

	}

	public static void setKey(){
		InputManager.dictionnary[InputManager.dictionnary.Keys[currentIndex]] = keyPressed;
		keyPressed = KeyCode.None;
		
		styles [currentIndex].normal.textColor = inactiveColor;
		styles [currentIndex].active.textColor = inactiveColor;
		styles [currentIndex].focused.textColor = inactiveColor;
		styles [currentIndex].hover.textColor = inactiveColor;
	}

	public static IEnumerator WaitForAKey(){
		inLoop = true;
		while (inLoop) {
			yield return null;
		}
	}

	public static void DrawInGameOptions(){
		initialize ();

		//Start of GUI part
		position = GUILayout.BeginScrollView(position,GUILayout.Width(490),GUILayout.Height(170));
		foreach (string name in InputManager.dictionnary.Keys) {
			GUILayout.BeginHorizontal(GUILayout.Width(460));
			GUILayout.Label(name,GUILayout.Width (200));
			if (GUILayout.Button (InputManager.dictionnary[name].ToString(),styles[InputManager.dictionnary.IndexOfKey(name)],GUILayout.Width (200))) {
				
				styles[InputManager.dictionnary.IndexOfKey(name)].normal.textColor = activeColor;
				styles[InputManager.dictionnary.IndexOfKey(name)].focused.textColor = activeColor;
				styles[InputManager.dictionnary.IndexOfKey(name)].active.textColor = activeColor;
				styles[InputManager.dictionnary.IndexOfKey(name)].hover.textColor = activeColor;
				
				GameStateManager.manager.StartCoroutine (WaitForAKey ());
				currentIndex = InputManager.dictionnary.IndexOfKey(name);
				
			}
			GUILayout.EndHorizontal();
		}
		GUILayout.EndScrollView();
		
		GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
		GUILayout.Label("Mouse sensitivity");
		InputManager.inputManager ().sensitivity = GUILayout.HorizontalSlider (InputManager.inputManager ().sensitivity, 0f, 10f);
		GUILayout.EndHorizontal ();
		
		GUILayout.BeginHorizontal ();
		if(GUILayout.Button("Back")){
			InputManager.inputManager().SerializeInputs();
			inLoop = false;
			GameStateManager.isInConfigs = false;
		}
		
		GUILayout.EndHorizontal();
		
		//End of GUI part
		
		Event e = Event.current;
		if (inLoop && (e.type == EventType.keyDown)) {
			inLoop = false;
			keyPressed = e.keyCode;
			setKey();
			
		} else if (inLoop && Input.GetKey (KeyCode.LeftShift)) {
			inLoop = false;
			keyPressed = KeyCode.LeftShift;
			setKey();
			
		}
		else if(inLoop && Input.GetKey(KeyCode.RightShift)){
			inLoop = false;
			keyPressed = KeyCode.RightShift;
			setKey();
		}
	}
}
