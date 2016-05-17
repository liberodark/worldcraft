using UnityEngine;
using System;
using System.Collections;

public class BeltGUI : MonoBehaviour
{
	private static ArrayList Blocks;
	private static int BeltSize = 7;
	private static int boxSize = 50;
	private static GUIStyle emptyBoxStyle = new GUIStyle ();
	private static Texture2D emptyTexture;
	private static Texture2D backTexture;
	private static Texture2D selected;
	private static Map map;
	private static int selectedPosition = 0;

	public static void setMap(Map m){
		map = m;
	}

	public static Entity getSelectedBlock(){
		return (Entity)(Blocks.ToArray () [selectedPosition%Blocks.Count]);
	}

	public static void SetItem(object b, int index){
		if(Blocks.Count > index ){
			Blocks.RemoveAt(index);
		}

		Blocks.Insert (index, b);
	}

	public static void SetItemAtSelectedPosition(Entity b){
		Blocks.RemoveAt (selectedPosition%BeltSize);
		Blocks.Insert (selectedPosition%BeltSize, b);
	}
	
	void Start()
	{
		Blocks = new ArrayList (BeltSize);

		for (int i = 0; i < BeltSize; i++) {
			SetItem (null,i);
		}

		backTexture = createTexture(new Color (0.3f, 0.3f, 0.0f,0.4f),GetBeltArea().width,GetBeltArea().height);
		emptyTexture = createTexture (new Color (0.3f, 0.3f, 0.3f,0.3f));
		selected = createTexture(new Color (0.0f, 0.8f, 0.8f,0.3f));

		emptyBoxStyle.fixedHeight = boxSize;
		emptyBoxStyle.fixedWidth = boxSize;
		emptyBoxStyle.normal.background = emptyTexture;
		emptyBoxStyle.margin = new RectOffset (10, 10, 10, 10);

	}

	void Update () {
		if(GameStateManager.IsPlaying){
			if (Input.GetAxis("Mouse ScrollWheel") > 0) // back
			{
				deactivateItem(selectedPosition, selectedPosition - 1);
				selectedPosition --;
				if(selectedPosition < 0){
					selectedPosition += BeltSize;
				}
			}
			if (Input.GetAxis("Mouse ScrollWheel") < 0) // forward
			{
				deactivateItem(selectedPosition, selectedPosition + 1);
				selectedPosition ++;
				if(selectedPosition >= BeltSize){
					selectedPosition -= BeltSize;
				}
			}
		}
	}

	private void deactivateItem(int i, int next){
		if (Blocks.ToArray () [i] is ActivableItem && Blocks.ToArray () [i] != Blocks.ToArray () [next]) {
			((ActivableItem)Blocks.ToArray () [i]).setActivation(false);
		}
	}
	
	void OnGUI(){
		if(!GameGUI.noHud){
			int cp = 0;
			Rect menu_area = this.GetBeltArea();
			GUILayout.BeginArea(menu_area);
			{
				GUILayout.BeginVertical();
				{
					GUILayout.BeginHorizontal();
					GUILayout.FlexibleSpace();
					DrawBelt();
					GUILayout.EndHorizontal();
				}
				GUILayout.FlexibleSpace();
				GUILayout.EndVertical();
			}
			GUILayout.EndArea();

			foreach (Entity b in Blocks) {
				cp++;
				if(b != null){
					GUI.DrawTextureWithTexCoords(getArea(cp),b.GetTexture(),b.GetPreviewFace());
				}
			}
	}
	}
	
	private Rect GetBeltArea() {
		Rect rect = new Rect(Screen.width/2 - (boxSize * (BeltSize + 1))/2 - 10 * (BeltSize + 2), Screen.height - 40 - boxSize / 2, boxSize * BeltSize + 20 * BeltSize, boxSize);
		return rect;
	}

	private Rect getArea(int index){
		Rect rect = new Rect((Screen.width/2 - (boxSize * (BeltSize + 1))/2 - 10 * (BeltSize)) + index * (boxSize + 10) + 5, Screen.height - 40 - boxSize / 2 + 5, boxSize-10, boxSize-10);
		return rect;
	}
	
	private void DrawBelt() {
		for(int i = 0; i < BeltSize; i++){

			if(i == selectedPosition){
				if(GUILayout.Button (selected, emptyBoxStyle)){
					if(InventoryGUI.IsBlockUnderSelection){
						SetItem(InventoryGUI.BlockUnderSelection,i);
						InventoryGUI.IsBlockUnderSelection = false;
						InventoryGUI.BlockUnderSelection = null;
					}
					else if(InventoryGUI.IsItemUnderSelection){
						SetItem(InventoryGUI.ItemUnderSelection,i);
						InventoryGUI.IsItemUnderSelection = false;
						InventoryGUI.ItemUnderSelection = null;
					}
				}
			}
			else{
				if(GUILayout.Button (emptyTexture, emptyBoxStyle)){
					if(InventoryGUI.IsBlockUnderSelection){
						SetItem(InventoryGUI.BlockUnderSelection,i);
						InventoryGUI.IsBlockUnderSelection = false;
						InventoryGUI.BlockUnderSelection = null;
					}
					else if(InventoryGUI.IsItemUnderSelection){
						SetItem(InventoryGUI.ItemUnderSelection,i);
						InventoryGUI.IsItemUnderSelection = false;
						InventoryGUI.ItemUnderSelection = null;
					}
				}

			}
		
		}
	}

	private Texture extractTextureFromColor(Color[] p){
		Texture2D tex = new Texture2D(boxSize,boxSize);
		tex.SetPixels (p);
		tex.Apply ();
		return tex as Texture;

	}

	private Texture2D createTexture(Color col){

		Texture2D tex;
		Color[] pix = new Color[boxSize*boxSize];

		for(int i = 0; i < pix.Length; i++)
			pix[i] = col;
		
		tex = new Texture2D (boxSize, boxSize);
		tex.SetPixels(pix);
		tex.Apply();
		return tex;
	}

	private Texture2D createTexture(Color col,float height, float width){
		
		Texture2D tex;
		Color[] pix = new Color[(int)height*(int)width];
		
		for(int i = 0; i < pix.Length; i++)
			pix[i] = col;
		
		tex = new Texture2D ((int)height, (int)width);
		tex.SetPixels(pix);
		tex.Apply();
		return tex;
	}
}
