using UnityEngine;
using System.Collections;

#pragma warning disable 0649 // Field is never assigned to, and will always have its default value
public class ActivableItem : Item {
	
	[SerializeField] private int activeFace, inactiveFace;
	private bool active = false;
	private Vector2[] _face;
	
	public override void Init(BlockSet blockSet) {
		base.Init(blockSet);
		_face = ToTexCoords(inactiveFace);
	}

	public override Rect GetPreviewFace() {
		if(active)
			return ToRect(activeFace);
		return ToRect (inactiveFace);
	}
	
	public override Vector2[] GetFaceUV() {
		return _face;
	}
	
	public void Build(Vector3i localPos, Vector3i worldPos, Map map, MeshBuilder mesh, bool onlyLight) {
		//ItemBuilder.Build(localPos, worldPos, map, mesh, onlyLight);
	}
	
	public override MeshBuilder Build() {
		return ItemBuilder.Build(this);
	}

	public void toggleActivation(){
		active = !active;
	}

	public void setActivation(bool a){
		active = a;
	}

	public bool isActive(){
		return active;
	}
	
	/*public override bool IsSolid() {
		return false;
	}*/
	
}
