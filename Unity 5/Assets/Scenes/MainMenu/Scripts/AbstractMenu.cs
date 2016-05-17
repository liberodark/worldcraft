using UnityEngine;
using System.Collections;

public class AbstractMenu : MonoBehaviour {

	[SerializeField] protected Font font;
	private GUIStyle style = new GUIStyle ();
	private static GUIStyle newStyle;
	private Rect titlePosition;
	private bool initialized = false;

	private void init(){
		if(!initialized){
			initialized = true;
			newStyle = new GUIStyle(GUI.skin.box);
			newStyle.normal.background = style.normal.background;
			newStyle.normal.textColor = new Color (0f, 0f, 0f);
			newStyle.fontSize = 160;
			newStyle.fontStyle = FontStyle.Bold;
		}
		titlePosition = new Rect (Screen.width / 2 - 400, Screen.height * 0.05f, 800, 200);
	}

	protected virtual void OnGUI() {
		GUI.depth = -1;

		Font oldFont = GUI.skin.font;
		GUI.skin.font = font;

		GUIStyle oldStyle = GUI.skin.box;
		
		init ();

		GUI.Box(titlePosition,"World Craft",newStyle);

		GUI.skin.font = oldFont;
		//GUI.skin.box = oldStyle;

		GUILayout.BeginArea( GetMenuArea());
		{
			GUILayout.BeginHorizontal();
			{
				GUILayout.BeginVertical();
				GUILayout.FlexibleSpace();
					OnMenuGUI();
				GUILayout.EndVertical();
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}
		GUILayout.EndArea();
	}
	
	private static Rect GetMenuArea() {
		Rect rect = new Rect(Screen.width/2 - 260, Screen.height/2-135, 500, 270);
		return rect;
	}
	
	protected virtual void OnMenuGUI() {
		
	}
	
	protected void SwitchTo<T>() where T : MonoBehaviour {
		enabled = false;
		GetComponent<T>().enabled = true;
	}
	
	
}
