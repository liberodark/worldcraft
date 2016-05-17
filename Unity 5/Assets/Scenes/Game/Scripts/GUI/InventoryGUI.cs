using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InventoryGUI : MonoBehaviour {
	
	private BlockSet blockSet;
	private Builder builder;
	
	private bool show = false;
	private Vector2 scrollPosition = Vector3.zero;

	public static bool IsBlockUnderSelection{ get; set; }
	public static Block BlockUnderSelection{ get; set; }

	public static bool IsItemUnderSelection{ get; set; }
	public static Item ItemUnderSelection{ get; set; }

	
	void Start () {
		Map map = (Map) GameObject.FindObjectOfType( typeof(Map) );
		IsBlockUnderSelection = false;
		blockSet = map.GetBlockSet();

		Block[] blocks = blockSet.GetBlocks();
		for (int i = 0; i < blocks.Length; ++i)
			blocks[i].Index = i;
		blockSet.SetBlocks(blocks);

		if (MultiplayerGame.Player != null)
			builder = MultiplayerGame.Player.GetComponent<Builder>();
	}
	
	void Update () {
		if(InputManager.inputManager().inventory && !GameStateManager.IsMenuPaused)
		{
			show = !show;
			GameStateManager.IsPaused = show;
			if (builder == null)
				builder = MultiplayerGame.Player.GetComponent<Builder>();
		}
		else if (!GameStateManager.IsPaused)
		{
			show = false;
		}
	}
	
	void OnGUI() {
		if(show && builder != null)
		{
			Rect window = new Rect(0, 0, Screen.width * 0.5f, Screen.height * 0.6f);
			window.center = new Vector2(Screen.width, Screen.height) / 2f;
			GUILayout.Window(0, window, DoInventoryWindow, "Inventory");
		}
	}
	
	
	private void DoInventoryWindow(int windowID) {
		Entity selected = null;
		Entity new_selection = DrawInventory(blockSet, ref scrollPosition, selected);
		if (new_selection != null)
		{
			if(IsBlockUnderSelection)BlockUnderSelection = (Block)new_selection;
			if(IsItemUnderSelection)ItemUnderSelection = (Item)new_selection;
		}
    } 
	
	private static Entity DrawInventory(BlockSet blockSet, ref Vector2 scrollPosition, Entity selected) {
		scrollPosition = GUILayout.BeginScrollView(scrollPosition);
		int y = 0;
		for(int i=0; i<blockSet.GetBlockCount(); y++) {
			GUILayout.BeginHorizontal();
			for(int x=0; x<8; x++, i++) {
				Block block = blockSet.GetBlock(i);
				if( DrawBlock(block, block == BlockUnderSelection && BlockUnderSelection != null) ) {
					selected = block;
					IsBlockUnderSelection = true;
				}
			}
			GUILayout.EndHorizontal();
		}
		for(int i=0; i<blockSet.GetItemCount(); y++) {
			GUILayout.BeginHorizontal();
			for(int x=0; x<8; x++, i++) {
				Item item = blockSet.GetItem(i);
				if( DrawBlock(item, item == ItemUnderSelection && ItemUnderSelection != null) ) {
					selected = item;
					IsItemUnderSelection = true;
				}
			}
			GUILayout.EndHorizontal();
		}
		GUILayout.EndScrollView();
		return selected;
	}
	
	private static bool DrawBlock(Entity block, bool selected) {
		Rect rect = GUILayoutUtility.GetAspectRect(1f);
		
		if(selected) GUI.Box(rect, GUIContent.none);
		
		Vector3 center = rect.center;
		rect.width -= 8;
		rect.height -= 8;
		rect.center = center;

		if (block != null)
			return (block.DrawPreview(rect));
		return false;
	}
	
}
