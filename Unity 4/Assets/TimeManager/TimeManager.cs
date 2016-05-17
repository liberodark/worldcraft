using UnityEngine;
using System.Collections;

public class TimeManager : MonoBehaviour {

	private Material dawn;
	private Material dusk;
	private Material day;
	private Material night;
	private Material currentMaterial;

	private float currentTime = 0;
	private float nightLength;
	private float dayLength;
	private float dawnDuskLength;

	private static TimeManager _timeManager;
	public static TimeManager timeManager {
		get {
			if(_timeManager == null) _timeManager = (TimeManager) GameObject.FindObjectOfType( typeof(TimeManager) );
			return _timeManager;
		}
	}
	
	void Start () {
		dayLength = 2400f;
		nightLength = 1800f;
		dawnDuskLength = 180f;
		dawn = Resources.Load ("Skyboxes/DawnDusk Skybox", typeof(Material)) as Material;
		day = Resources.Load ("Skyboxes/Sunny3 Skybox", typeof(Material)) as Material;
		night = Resources.Load ("Skyboxes/MoonShine Skybox", typeof(Material)) as Material;
		dusk = dawn;
		currentMaterial = day;
		RenderSettings.skybox = day;
	}

	public static float getTime(){
		return timeManager.currentTime;
	}

	public static float setTime(float t){
		return timeManager.currentTime = t;
	}



	void Update () {
		currentTime += (Time.time - currentTime);
		currentTime %= (dayLength + nightLength);

		if(currentTime < (dayLength - dawnDuskLength)){
			if(currentMaterial != day){
				RenderSettings.ambientLight = new Color(1f, 1f, 1f, 1f);
				currentMaterial = day;
				RenderSettings.skybox = day;
			}
		}
		else if(currentTime < dayLength && currentTime > (dayLength - dawnDuskLength)){
			if(currentMaterial != dawn){
				RenderSettings.ambientLight = new Color(0.5f, 0.5f, 0.5f, 1f);
				currentMaterial = dawn;
				RenderSettings.skybox = dawn;
			}
		}
		else if( currentTime > dayLength && (currentTime < (dayLength + nightLength - dawnDuskLength))){
			if(currentMaterial != night){
				RenderSettings.ambientLight = new Color(0.1f, 0.1f, 0.1f, 1f);
				currentMaterial = night;
				RenderSettings.skybox = night;
			}
		}
		else if(currentTime > dayLength && currentTime < (dayLength + nightLength) && currentTime > (dayLength + nightLength - dawnDuskLength)){
			if(currentMaterial != dusk){
				RenderSettings.ambientLight = new Color(0.5f, 0.5f, 0.5f, 1f);
				currentMaterial = dusk;
				RenderSettings.skybox = dusk;
			}
		}
	}
}
