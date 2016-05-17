using UnityEngine;
using System.Collections;
using System;

public interface Entity {

	void Init (BlockSet blockSet);
	
	Rect ToRect (int pos);

	bool DrawPreview (Rect position);

	Rect GetPreviewFace();
	

	MeshBuilder Build();

	
	void SetName (string name);
	string GetName ();
	
	void SetAtlasID (int atlas);
	int GetAtlasID ();
	Atlas GetAtlas ();
	Texture GetTexture();
	
	void SetLight (int light);
	byte GetLight ();
	Vector2[] GetFaceUV ();
	
	bool IsAlpha ();
	
}