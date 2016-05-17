using UnityEngine;
using System.Collections;

public class GameGUI : MonoBehaviour {

	private float lastTimeUpdate = 0;
	private float statsModeTime;
	private int fps;
	private bool statsMode = false;
	private bool screenShot = false;
	public static bool noHud = false;
	private float screenShotTime = 0;
	private GUIStyle style = new GUIStyle();
	private bool spellLaunched = false;
	

	void OnResume() {
		enabled = true;
	}
	
	void OnPause() {
		enabled = false;
	}

	void OnScreenShot(){
		screenShotTime = Time.time;
		screenShot = true;
	}

	void Start(){
		style.fontSize = 30;
		style.normal.textColor = Color.white;

		RenderSettings.fogMode = FogMode.Linear;
		RenderSettings.fogStartDistance = 3f * 10f;
		RenderSettings.fogEndDistance = 30f * 10f;
		RenderSettings.fogDensity = 1/10f;
		RenderSettings.fog = true;
		RenderSettings.flareFadeSpeed = 100f;
		RenderSettings.flareStrength = 1f;
		RenderSettings.haloStrength = 1f;
		
		MapRayIntersection.MaxDistance = 150f * 10f;
		audio.volume = InputManager.inputManager().soundLevel;
	}
	
	void Update(){
		if(InputManager.inputManager().statsInput) {
			statsMode = !statsMode;
		}
		if(InputManager.inputManager().noHUD){
			noHud = !noHud;
		}
		if (InputManager.inputManager ().isSpellLaunched) {
			spellLaunched = true;
		}

		if (spellLaunched && !Camera.main.particleSystem.isPlaying) {
			spellLaunched = false;
			Camera.main.particleSystem.maxParticles = 100;
			Camera.main.particleSystem.startLifetime = 2f;
			Camera.main.particleSystem.simulationSpace = ParticleSystemSimulationSpace.World;
			Camera.main.particleSystem.loop = false;
			Camera.main.particleSystem.startSpeed = 50f;
			Camera.main.particleSystem.startDelay = 0;
			Camera.main.particleSystem.Play ();
		}
	}

	void OnApplicationFocus(bool focus){
		if (!focus && !GameStateManager.IsPaused) {
			GameStateManager.IsMenuPaused = true;
			GameStateManager.manager.ToggleMouse();
		} else if (focus && GameStateManager.IsPaused) {
			Screen.showCursor = true;
			Screen.lockCursor = false;
		}
	}


	void OnGUI() {

		if (statsMode && !noHud) {
			Vector3 position = Camera.main.gameObject.transform.position;
			if (Time.time - lastTimeUpdate > 0.5f) {
				fps = (int)(1f / Time.deltaTime * Time.timeScale);
				lastTimeUpdate = Time.time;
			}
			GUILayout.Box ("FPS " + fps);
			GUILayout.Box (" position: x: " + position.x + " y:" + position.y + " z:" + position.z);
		}

		if(screenShot && (Time.time - screenShotTime > 0.2f) &&(Time.time - screenShotTime < 1f) && !noHud)GUILayout.Box("Screenshot taken");

		if(GameStateManager.IsPlaying && !noHud){
			Vector2 size = style.CalcSize( new GUIContent("+") );
			Rect rect = new Rect(0, 0, size.x, size.y);
			rect.center = new Vector2(Screen.width, Screen.height)/2f;
			GUI.Label( rect, "+", style );
		}
	}
	
	
}
