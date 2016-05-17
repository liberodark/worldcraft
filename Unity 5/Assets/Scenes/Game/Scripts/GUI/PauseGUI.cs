using UnityEngine;
using System;
using System.Collections;

public class PauseGUI : MonoBehaviour {
	
	private const string help = "Esc - Pause/Resume\n" +
								"E - Open the inventory";
	private static float distance = 10f;

	private static GUIStyle horStyle = new GUIStyle ();

	void OnResume() {
		enabled = false;
	}
	
	void OnMenuPause() {
		enabled = true;
	}

	void Start(){
		horStyle.fixedWidth = 490;
	}
	

	void OnGUI() {
		Rect menu_area = this.GetMenuArea();
		GUILayout.BeginArea(menu_area,GUI.skin.box);
		{
			GUILayout.BeginHorizontal();
			{
				GUILayout.BeginVertical();
				GUILayout.FlexibleSpace();
				if(GameStateManager.isInConfigs)
					InputManagerGUI.DrawInGameOptions();
				else
					DrawMenu();
				GUILayout.EndVertical();
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}
		GUILayout.EndArea();
		GameStateManager.MouseEventLocked = menu_area.Contains(InputManager.inputManager().getMousePosition());
	}
	
	private void DrawMenu() {

		GUILayout.Box(help, GUILayout.ExpandWidth(true));

		DrawSoundSlider();
		DrawFOVSlider ();
		DrawSensitivitySlider ();

		if(GUILayout.Button("Resume", GUILayout.ExpandWidth(true)) ) {
			GameStateManager.IsMenuPaused = false;
		}
		if( GUILayout.Button("Controls", GUILayout.ExpandWidth(true)) ) {
			GameStateManager.isInConfigs = true;
		}
		if(GUILayout.Button("Menu", GUILayout.ExpandWidth(true)) ) {
			GameStateManager.IsPlaying = true;
			Application.LoadLevel("MainMenu");
		}
		if(GUILayout.Button("Quit", GUILayout.ExpandWidth(true)) ) {
			Application.Quit();
		}
	}
	
	
	private Rect GetMenuArea() {
		Rect rect = new Rect(Screen.width/2 - 250, Screen.height/2 - 120, 500, 240);
		return rect;
	}

	private void DrawSoundSlider() {
		const float min = 0f;
		const float max = 1f;
		
		GUILayout.BeginHorizontal(horStyle,GUILayout.ExpandWidth(true));
			GUILayout.Label("Music");
		InputManager.inputManager().soundLevel = GUILayout.HorizontalSlider(InputManager.inputManager().soundLevel, min, max);
		GUILayout.EndHorizontal();

		GetComponent<AudioSource>().volume = InputManager.inputManager().soundLevel;

	}

	private void DrawSensitivitySlider() {
		const float min = 0f;
		const float max = 10f;
		
		GUILayout.BeginHorizontal(horStyle,GUILayout.ExpandWidth(true));
			GUILayout.Label("Mouse sensitivity");
			MouseLook.setSentivity (GUILayout.HorizontalSlider(MouseLook.getSentivity(), min, max));
		GUILayout.EndHorizontal();


	}

	private void DrawFOVSlider() {
		const float min = 1f;
		const float max = 100f;

		GUILayout.BeginHorizontal(horStyle,GUILayout.ExpandWidth(true));
		GUILayout.Label("Rend. dist.");
		distance = GUILayout.HorizontalSlider(distance, min, max);
		GUILayout.EndHorizontal();
		
		RenderSettings.fogMode = FogMode.Linear;
		RenderSettings.fogStartDistance = 3f * distance;
		RenderSettings.fogEndDistance = 30f * distance;
		RenderSettings.fogDensity = 1/distance;

		MapRayIntersection.MaxDistance = 150f * distance;

		WorldGenerator.RenderDistance = (int)(Math.Floor(3 + distance / 5));
	}
	
}
