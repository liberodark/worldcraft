using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Collections.Specialized;

[AddComponentMenu("VoxelEngine/Map")]
public class Map : MonoBehaviour {
	
	[SerializeField] private BlockSet blockSet;
	private List3D<Chunk> chunks = new List3D<Chunk>();
	private SunLightMap sunLightmap = new SunLightMap();
	private LightMap lightmap = new LightMap();
	public static Map Instance { get; set; }


	void Start()
	{
		if (Network.isClient) {
			//getRemoteMap();
		} else {
			Map.Instance = this;
		}
		BeltGUI.setMap (this);
	}
	
	public void SetBlockAndRecompute(BlockData block, Vector3i pos) {
		SetBlock( block, pos );
		
		Vector3i chunkPos = Chunk.ToChunkPosition(pos);
		Vector3i localPos = Chunk.ToLocalPosition(pos);
		
		SetDirty( chunkPos );
		
		if(localPos.x == 0) SetDirty( chunkPos - Vector3i.right );
		if(localPos.y == 0) SetDirty( chunkPos - Vector3i.up );
		if(localPos.z == 0) SetDirty( chunkPos - Vector3i.forward );
		
		if(localPos.x == Chunk.SIZE_X-1) SetDirty( chunkPos + Vector3i.right );
		if(localPos.y == Chunk.SIZE_Y-1) SetDirty( chunkPos + Vector3i.up );
		if(localPos.z == Chunk.SIZE_Z-1) SetDirty( chunkPos + Vector3i.forward );
		
		SunLightComputer.RecomputeLightAtPosition(this, pos);
		LightComputer.RecomputeLightAtPosition(this, pos);
	}
	
	private void SetDirty(Vector3i chunkPos) {
		Chunk chunk = GetChunk( chunkPos );
		if(chunk != null) chunk.GetChunkRendererInstance().SetDirty();
	}
	
	public void SetBlock(Block block, Vector3i pos) {
		SetBlock(new BlockData(block), pos);
	}
	public void SetBlock(Block block, int x, int y, int z) {
		SetBlock(new BlockData(block), x, y, z);
	}
	
	public void SetBlock(BlockData block, Vector3i pos) {
		SetBlock(block, pos.x, pos.y, pos.z);
	}
	public void SetBlock(BlockData block, int x, int y, int z) {
		Chunk chunk = GetChunkInstance( Chunk.ToChunkPosition(x, y, z) );
		if(chunk != null) chunk.SetBlock( block, Chunk.ToLocalPosition(x, y, z) );
	}

	public void remoteBlockData(Vector3i pos){
		//this.networkView.RPC("getRemoteBlock", RPCMode.Server, new Vector3(pos.x,pos.y,pos.z), Network.player);
		using (WebClient wb = new WebClient())
		{
			NameValueCollection data = new NameValueCollection();			
			string response = wb.DownloadString("http://localhost:1992/map?x="+pos.x+"&y="+pos.y+"&z"+pos.z);
		}
	}

/*	public void remoteChunkData(Vector3i pos){
		this.networkView.RPC("getRemoteChunk", RPCMode.Server, new Vector3(pos.x,pos.y,pos.z), Network.player);
	}

	[RPC]
	public void getRemoteBlock(Vector3 pos, NetworkPlayer player){
		BlockData c = this.GetBlock (new Vector3i ((int)pos.x, (int)pos.y, (int)pos.z));
		if(c.block != null)
			networkView.RPC("receiveRemoteBlock", player, c.block.Index,pos);
	}

	[RPC]
	public void getRemoteChunk(Vector3 pos, NetworkPlayer player){
		Chunk c = this.GetChunk (new Vector3i ((int)pos.x, (int)pos.y, (int)pos.z));
		if(c != null){
			networkView.RPC("receiveRemoteChunk", player, c.getChunkInInt(),pos);
		}
	}
	
	[RPC]
	public void receiveRemoteBlock(int id, Vector3 pos){
		this.SetBlock( new BlockData(Map.Instance.GetBlockSet().GetBlock(id)), new Vector3i((int)pos.x,(int)pos.y,(int)pos.z ));
	}

	[RPC]
	public void receiveRemoteChunk(byte[] chks, Vector3 pos){
		Chunk chunk = GetChunk(new Vector3i((int)pos.x,(int)pos.y,(int)pos.z));
		if(chunk == null) {
			chunk = new Chunk(this, new Vector3i((int)pos.x,(int)pos.y,(int)pos.z));
			chunks.AddOrReplace(chunk, new Vector3i((int)pos.x,(int)pos.y,(int)pos.z));
		}
		chunk.intInChunk (chks);
	}*/
	
	public BlockData GetBlock(Vector3i pos) {
		return GetBlock(pos.x, pos.y, pos.z);
	}

	public BlockData GetBlock(int x, int y, int z) {
		//if (Network.peerType == NetworkPeerType.Client)
			remoteBlockData (new Vector3i(x,y,z));
		Chunk chunk = GetChunk( Chunk.ToChunkPosition(x, y, z) );
		if(chunk == null) return default(BlockData);
		return chunk.GetBlock( Chunk.ToLocalPosition(x, y, z) );
	}

	public int GetMaxY(int x, int z) {
		Vector3i chunkPos = Chunk.ToChunkPosition(x, 0, z);
		chunkPos.y = chunks.GetMax().y;
		Vector3i localPos = Chunk.ToLocalPosition(x, 0, z);
		
		for(;chunkPos.y >= 0; chunkPos.y--) {
			localPos.y = Chunk.SIZE_Y-1;
			for(;localPos.y >= 0; localPos.y--) {
				Chunk chunk = chunks.SafeGet(chunkPos);
				if(chunk == null) break;
				BlockData block = chunk.GetBlock(localPos);
				if(!block.IsEmpty()) return Chunk.ToWorldPosition(chunkPos, localPos).y;
			}
		}
		
		return 0;
	}
	
	private Chunk GetChunkInstance(Vector3i chunkPos) {
		if(chunkPos.y < 0) return null;
		Chunk chunk = GetChunk(chunkPos);
		if(chunk == null) {
			chunk = new Chunk(this, chunkPos);
			chunks.AddOrReplace(chunk, chunkPos);
		}
		return chunk;
	}

	public Chunk GetChunk(Vector3i chunkPos) {
		return chunks.SafeGet(chunkPos);
	}
	
	public List3D<Chunk> GetChunks() {
		return chunks;
	}
	
	public SunLightMap GetSunLightmap() {
		return sunLightmap;
	}
	
	public LightMap GetLightmap() {
		return lightmap;
	}
	
	public void SetBlockSet(BlockSet blockSet) {
		this.blockSet = blockSet;
	}
	public BlockSet GetBlockSet() {
		return blockSet;
	}
	
}