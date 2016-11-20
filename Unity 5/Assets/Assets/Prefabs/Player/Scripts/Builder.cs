using UnityEngine;
using System.Collections;

public class Builder : MonoBehaviour {

	[SerializeField]
	private GameObject cursor;
	[SerializeField]
	private int cursor_range = 20;

	[SerializeField]
	private int SelectedBlockIndex = 0;

	private Transform cameraTrans;
	private CharacterCollider characterCollider;
	private Map map;
	private Block selectedBlock;
	private ArrayList spreadingList;
	private float lastTimeDestroy = 0;
	private float lastTimeBuild = 0;
	
	void Awake() {
		cameraTrans = Camera.main.transform;
		characterCollider = GetComponent<CharacterCollider>();
		map = (Map) GameObject.FindObjectOfType( typeof(Map) );
		cursor = (GameObject)GameObject.Instantiate(cursor);
		spreadingList = new ArrayList ();
		selectedBlock = Map.Instance.GetBlockSet().GetBlock(0);
	}
	
	public void SetSelectedBlock(Block block) {
		selectedBlock = block;
		SelectedBlockIndex = block.Index;
		if (this.GetComponent<NetworkView>().isMine && Network.peerType != NetworkPeerType.Disconnected)
			GetComponent<NetworkView>().RPC("SetSelectedBlockIndex", RPCMode.AllBuffered, block.Index);
	}

	public void SetSelectedBlock(Item item) {
		selectedBlock = null;
		SelectedBlockIndex = item.Index;
		if (this.GetComponent<NetworkView>().isMine && Network.peerType != NetworkPeerType.Disconnected)
			GetComponent<NetworkView>().RPC("SetSelectedBlockIndex", RPCMode.AllBuffered, item.Index);
	}

	[RPC]
	void				SetSelectedBlockIndex(int index)
	{
		selectedBlock = Map.Instance.GetBlockSet().GetBlock(index);
		SelectedBlockIndex = index;
	}

	public Block GetSelectedBlock() {
		return selectedBlock;
	}

	void Update(){
		Entity selected = BeltGUI.getSelectedBlock ();
		if (selected != null && !(selected is Item)) {
			SetSelectedBlock ((Block)selected);
		} else
			selectedBlock = null;

		if (selected is ActivableItem){
			if(InputManager.inputManager ().isItemActivated){
				((ActivableItem)selected).toggleActivation();
				Camera.main.GetComponent<Light>().enabled = !Camera.main.GetComponent<Light>().enabled;
			}
		}else if(Camera.main.GetComponent<Light>().enabled)
			Camera.main.GetComponent<Light>().enabled = false;
	}



	// Update is called once per frame
	void OnGUI () {
		if(GameStateManager.IsPaused) return;

		if (InputManager.inputManager().IsKeyDown(KeyCode.LeftControl))
		{
			Vector3i? point = GetCursor(false);
			if(point.HasValue) {
				byte sun = map.GetSunLightmap().GetLight(point.Value);
				byte light = map.GetLightmap().GetLight(point.Value);
				Debug.Log("Sun "+" "+sun+"  Light "+light);
			}
		}

		if (InputManager.inputManager().IsKeyDown(KeyCode.RightControl))
		{
			Vector3i? point = GetCursor(true);
			if(point.HasValue) {
				byte sun = map.GetSunLightmap().GetLight(point.Value);
				byte light = map.GetLightmap().GetLight(point.Value);
				Debug.Log("Sun "+sun+"  Light "+light);
			}
		}
		
		//Remove block
		if (InputManager.inputManager().canBreakBlocks && ((InputManager.inputManager().LeftClick && (Time.time - lastTimeDestroy > 0.1f)) || (InputManager.inputManager().isLeftClickHold && (Time.time - lastTimeDestroy > 0.200f))))
		{
			Vector3i? point = GetCursor(true);
			if(point.HasValue && point.Value.y >= 2) {
                if (Network.peerType == NetworkPeerType.Disconnected)
                    map.SetBlockAndRecompute(new BlockData(), point.Value);
                else
                    this.DestroyBlock(point.Value);

				Vector3i around = new Vector3i(point.Value.x,point.Value.y,point.Value.z);

				around.y += 1;
				if(map.GetBlock(around).IsFluid()){
					spreadingList.Add(new SpreadingInstruction(0.5f,around,this,map,map.GetBlock(around),15));
				}
				around.y -= 1;
				
				for (int j = -1; j <= 1; j+=2) {
					around.x += j;
					if(map.GetBlock(around).IsFluid()){
						spreadingList.Add(new SpreadingInstruction(0.5f,around,this,map,map.GetBlock(around),15));
					}
					around.x -= j;
				}
				for (int j = -1; j <= 1; j+=2) {
					around.z += j;
					if(map.GetBlock(around).IsFluid()){
						spreadingList.Add(new SpreadingInstruction(0.5f,around,this,map,map.GetBlock(around),15));
					}
					around.z -= j;
				}
			}
			lastTimeDestroy = Time.time;
		}

		//Spawn block
		if ((InputManager.inputManager().RightClick && (Time.time - lastTimeBuild > 0.1f)) || (InputManager.inputManager().isRightClickHold && (Time.time - lastTimeBuild > 0.200f)))
		{
			Vector3i? point = GetCursor(false);
			if(point.HasValue) {
				bool empty = !BlockCharacterCollision.GetContactBlockCharacter(point.Value, transform.position, characterCollider).HasValue;
				if(empty && selectedBlock != null) {
					BlockData block = new BlockData( selectedBlock );
					block.SetDirection( GetDirection(-transform.forward) );

					if (Network.peerType == NetworkPeerType.Disconnected)
						map.SetBlockAndRecompute(block, point.Value);
					else
						this.SpawnBlock(point.Value);

					if(block.IsFluid())spreadingList.Add(new SpreadingInstruction(0.5f,point.Value,this,map,block,15));
				}
			}
			lastTimeBuild = Time.time;
		}

		ArrayList delList = new ArrayList();
		foreach(SpreadingInstruction si in spreadingList){
			if(si.canSpread()){
				si.spreading();
			}else{
				delList.Add(si);
			}
		}

		foreach(SpreadingInstruction si in delList){
			spreadingList.Remove(si);
		}
		
		Vector3i? cursor = GetCursor(true);
		this.cursor.SetActive(cursor.HasValue);
		if(cursor.HasValue) {
			this.cursor.transform.position = cursor.Value;
		}
		
	}

	void DestroyBlock(Vector3i cursor_pos)
	{
		GetComponent<NetworkView>().RPC("RemoteDestroyBlock", RPCMode.All, (Vector3)cursor_pos);
	}

	[RPC]
	void RemoteDestroyBlock(Vector3 cursor_pos)
	{
		Map.Instance.SetBlockAndRecompute(new BlockData(), new Vector3i(Mathf.RoundToInt(cursor_pos.x), Mathf.RoundToInt(cursor_pos.y), Mathf.RoundToInt(cursor_pos.z)));
	}

	public void						SpawnBlock(Vector3i cursor_pos)
	{
		GetComponent<NetworkView>().RPC("RemoteSpawnBlock", RPCMode.All, (Vector3)cursor_pos);
	}

	[RPC]
	public void						RemoteSpawnBlock(Vector3 cursor_pos)
	{
		Map.Instance.SetBlockAndRecompute(new BlockData(selectedBlock), new Vector3i(Mathf.RoundToInt(cursor_pos.x), Mathf.RoundToInt(cursor_pos.y), Mathf.RoundToInt(cursor_pos.z)));
	}
	
	private Vector3i? GetCursor(bool inside) {
		Ray ray = new Ray(cameraTrans.position, cameraTrans.forward);
		Vector3? point =  MapRayIntersection.Intersection(map, ray, this.cursor_range);
		if( point.HasValue ) {
			Vector3 pos = point.Value;
			if(inside) pos += ray.direction*0.01f;
			if(!inside) pos -= ray.direction*0.01f;
			int posX = Mathf.RoundToInt(pos.x);
			int posY = Mathf.RoundToInt(pos.y);
			int posZ = Mathf.RoundToInt(pos.z);
			return new Vector3i(posX, posY, posZ);
		}
		return null;
	}
	
	private static BlockDirection GetDirection(Vector3 dir) {
		if( Mathf.Abs(dir.z) >= Mathf.Abs(dir.x) ) {
			// 0 Ð¸Ð»Ð¸ 180
			if(dir.z >= 0) return BlockDirection.Z_PLUS;
			return BlockDirection.Z_MINUS;
		} else {
			// 90 Ð¸Ð»Ð¸ 270
			if(dir.x >= 0) return BlockDirection.X_PLUS;
			return BlockDirection.X_MINUS;
		}
	}
	
}

public class SpreadingInstruction{

	private float creationTime;
	private float requiredDelay;
	private Builder builder; 
	private Map map;
	private ArrayList points;
	private BlockData block;
	private int requiredDepth;
	private int currentDepth;
	private bool processing = false;


	public SpreadingInstruction(float delay, Vector3i vect, Builder builder, Map map, BlockData block, int requiredDepth){

		this.creationTime = Time.time;
		this.requiredDelay = delay;
		this.points = new ArrayList ();
		this.points.Add (vect);
		this.builder = builder;
		this.map = map;
		this.block = block;
		this.requiredDepth = requiredDepth;
		this.currentDepth = 0;
	}

	public bool canSpread(){
		return currentDepth < requiredDepth;
	}

	public void spreading(){
		if(Time.time - creationTime > requiredDelay && !processing){
			foreach (Vector3i around in points) {
				if (Network.peerType == NetworkPeerType.Disconnected){
					map.SetBlockAndRecompute (block, around);
				}
				else if (Network.peerType == NetworkPeerType.Server){
					builder.SpawnBlock (around);
					map.SetBlockAndRecompute (block, around);
				}
				else{
					map.SetBlockAndRecompute (block, around);
				}
			}
			new System.Threading.Thread(fluidCalculate).Start();
			creationTime = Time.time;
		}
	}

	void fluidCalculate(){
		processing = true;
		ArrayList newPoints = points;
		points = new ArrayList();
		if (canSpread()) {
			foreach (Vector3i around in newPoints) {
				applyFluidPhysics(around);
			}
			currentDepth++;
		}
		processing = false;
	}

	void applyFluidPhysics(Vector3i point){

		Vector3i around = new Vector3i(point.x,point.y,point.z);
		
		around.y -= 1;
		if(map.GetBlock(around).IsEmpty()){
			points.Add(around);
			return;
		}
		around.y += 1;
		
		for (int j = -1; j <= 1; j+=2) {
			around.x += j;
			if(map.GetBlock(around).IsEmpty()){
				points.Add(around);
			}
			around.x -= j;
		}
		for (int j = -1; j <= 1; j+=2) {
			around.z += j;
			if(map.GetBlock(around).IsEmpty()){
				points.Add(around);
			}
			around.z -= j;
		}
	}
}
