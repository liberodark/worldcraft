using UnityEngine;
using System;
public class OptionMenu : AbstractMenu
{
	private Resolution[] res;
	private ComboBox<Resolution> combobox;
	private bool Initialized = false;
	private GUIContent[] content;
	private bool fullScreen;

	public void init(){
		if(!Initialized){
			Initialized = true;
			fullScreen = Screen.fullScreen;
			res = Screen.resolutions;
			content = new GUIContent[res.Length];
			for(int i = 0; i < res.Length; i++){
				content[i] = new GUIContent(res[i].width + "x" + res[i].height);
			}
			combobox = new ComboBox<Resolution>(res, content);
		}
	}
	

	protected override void OnMenuGUI() {
		init ();

		combobox.Show ();

		GUILayout.BeginHorizontal (GUILayout.Width(490));

		fullScreen = GUILayout.Toggle (!fullScreen, "Fullscreen");

		if (GUILayout.Button ("Commands")) {
			SwitchTo<InputManagerGUI>();
		}

		GUILayout.EndHorizontal ();

		GUILayout.BeginHorizontal ();

			if (GUILayout.Button ("Apply")) {
			Screen.SetResolution(combobox.SelectedItem.width,combobox.SelectedItem.height,fullScreen);
				SwitchTo<MainMenu>();
			}
			if (GUILayout.Button ("Back")) {
				SwitchTo<MainMenu>();
			}

		GUILayout.EndHorizontal ();
	}
}

public class ComboBox<T>
{
	private int selectedItem = 0;
	private GUIStyle style;
	private GUIContent[] content;
	private T[] items;
	private bool editMode = false;
	private Vector2 scrollPosition;

	public ComboBox(T[] list, GUIContent[] cont){
		items = list;
		content = cont;

		style = new GUIStyle (GUI.skin.box);
		style.border = new RectOffset (0, 0, 0, 30);
	}

	public T SelectedItem{
		get{
			return items[selectedItem];
		}
	}

	public GUIContent SelectedContent{
		get{
			return content[selectedItem];
		}
	}

	public void Show(){
		scrollPosition = GUILayout.BeginScrollView(scrollPosition,GUILayout.Width(490));

		if (GUILayout.Button(SelectedContent,style, GUILayout.ExpandWidth(true)))
		{
			editMode = !editMode;
		}
		
		if (editMode)
		{
			for (int x = 0; x < items.Length; x++)
			{
				if (GUILayout.Button( content[x],style, GUILayout.ExpandWidth(true)))
				{
					selectedItem = x;
					editMode = false;
				}
			}
		}
		GUILayout.EndScrollView ();
	}

}