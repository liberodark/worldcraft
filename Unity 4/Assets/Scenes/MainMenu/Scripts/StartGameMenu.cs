using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StartGameMenu : AbstractMenu {
	
	[SerializeField] private BlockSet[] blockSetList;
	
	protected override void OnMenuGUI() {
		/*foreach(BlockSet blockset in blockSetList) {
			if(GUILayout.Button(blockset.name)) {
				GameSetup.blockSet = blockset;
				Application.LoadLevel("Game");
				return;
			}
		}*/
		GameSetup.blockSet = blockSetList [0];
		Application.LoadLevel("Game");
		return;
	}
}
