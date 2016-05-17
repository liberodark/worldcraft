using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;


[AddComponentMenu("VoxelEngine/WorldGenerator")]
public class WorldGenerator : MonoBehaviour {
	
	private Map map;
	private ColumnMap columnMap = new ColumnMap();
	private TerrainGenerator terrainGenerator;
	private TreeGenerator treeGenerator;
	private bool building = false;

	public static int RenderDistance{ get; set; }
	
	
	void Awake() {
		map = GetComponent<Map>();
		RenderDistance = 3;

		StartCoroutine ("initGenerator");
	}

	public void initGenerator(){
		terrainGenerator = new TerrainGenerator(map);
		treeGenerator = new TreeGenerator(map);

	}
	
	void Update() {
		if(!building) StartCoroutine( Building() );
	}
	
	private IEnumerator Building() {
		building = true;
		Vector3 pos = Camera.main.transform.position;
		Vector3i current = Chunk.ToChunkPosition( (int)pos.x, (int)pos.y, (int)pos.z );
		Vector3i? column = columnMap.GetClosestEmptyColumn(current.x, current.z, RenderDistance);

		if(column.HasValue) {
			int cx = column.Value.x;
			int cz = column.Value.z;
			columnMap.SetBuilt(cx, cz);
			
			yield return StartCoroutine( GenerateColumn(cx, cz) );
			yield return null;
			ChunkSunLightComputer.ComputeRays(map, cx, cz);
			ChunkSunLightComputer.Scatter(map, columnMap, cx, cz);
			yield return StartCoroutine( terrainGenerator.GeneratePlants(cx, cz));
			
			yield return StartCoroutine( BuildColumn(cx, cz) );
		}
		building = false;
	}
	
	private IEnumerator GenerateColumn(int cx, int cz) {
		yield return StartCoroutine( terrainGenerator.Generate(cx, cz) );
		yield return null;

		int x = cx * Chunk.SIZE_X + Chunk.SIZE_X/2;
		int z = cz * Chunk.SIZE_Z + Chunk.SIZE_Z/2;
		int y = map.GetMaxY(x, z)+1;
		treeGenerator.Generate(x, y, z);
	}
	
	public IEnumerator BuildColumn(int cx, int cz) {
		List3D<Chunk> chunks = map.GetChunks();
		for(int cy=chunks.GetMinY(); cy<chunks.GetMaxY(); cy++) {
			Chunk chunk = map.GetChunk( new Vector3i(cx, cy, cz) );
			if(chunk != null) chunk.GetChunkRendererInstance().SetDirty();
			if(chunk != null) yield return null;
		}
	}
	
	
}
