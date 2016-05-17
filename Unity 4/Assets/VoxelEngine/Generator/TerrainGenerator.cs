using UnityEngine;
using System.Collections;

#pragma warning disable 0414 // The private field is assigned but its value is never used
public class TerrainGenerator {
	
	private const int WATER_LEVEL = 50;
	private bool generating = false;
	
	/*private NoiseArray2D terrainNoise = new NoiseArray2D(1/50f).SetOctaves(1);
	private NoiseArray3D terrainNoise3D = new NoiseArray3D(1/30f);*/
	
	private Map map;
	private BiomeManager bmanager;
	
	private Block water;
	private Block grass;
	private Block dirt;

	public TerrainGenerator(Map map) {
		this.map = map;
		this.bmanager = new BiomeManager (map);
		BlockSet blockSet = map.GetBlockSet();

		water = blockSet.GetBlock("Water");
		grass = blockSet.GetBlock("Grass");
		dirt = blockSet.GetBlock("Dirt");

	}
	
	public IEnumerator Generate(int cx, int cz) {
		if (generating)yield return null;
		generating = true;
		//terrainNoise.GenerateNoise(cx*Chunk.SIZE_X, cz*Chunk.SIZE_Z);
		Biome.generateHeightNoise(cx*Chunk.SIZE_X, cz*Chunk.SIZE_Z);
		
		for(int cy=0; true; cy++) {
			Vector3i worldPos = Chunk.ToWorldPosition( new Vector3i(cx, cy, cz), Vector3i.zero );
			//terrainNoise3D.GenerateNoise(worldPos);
			bmanager.generateNoise(worldPos);
			
			bool generated = GenerateChunk( new Vector3i(cx, cy, cz) );
			if(!generated) break;
			
			yield return null;
		}
		generating = false;
		
	}
	
	private bool GenerateChunk(Vector3i chunkPos) {
		bool generated = false;
		for(int z=-1; z<Chunk.SIZE_Z+1; z++) {
			for(int x=-1; x<Chunk.SIZE_X+1; x++) {
				for(int y=0; y<Chunk.SIZE_Y; y++) {
					Vector3i worldPos = Chunk.ToWorldPosition(chunkPos, new Vector3i(x, y, z));

					if(worldPos.y <= WATER_LEVEL && map.GetBlock(worldPos).IsEmpty()) {
						map.SetBlock( water, worldPos );
						generated = true;
					}

					int islandHeight = BiomeManager.getCurrentBiome().getHeight(worldPos.x, worldPos.z);
					if( worldPos.y <= islandHeight) {
						GenerateBlockForIsland(worldPos, islandHeight-worldPos.y, islandHeight);
						generated = true;
						continue;
					}
					
				}
			}
		}
		return generated;
	}
	
	
	public IEnumerator GeneratePlants(int cx, int cz) {
		for(int z=-1; z<Chunk.SIZE_Z+1; z++) {
			for(int x=-1; x<Chunk.SIZE_X+1; x++) {
				Vector3i worldPos = new Vector3i(cx*Chunk.SIZE_X+x, 0, cz*Chunk.SIZE_Z+z);
				worldPos.y = map.GetMaxY(worldPos.x, worldPos.z);
				for(; worldPos.y>=WATER_LEVEL; worldPos.y--) {
					if(map.GetBlock(worldPos).block == dirt && map.GetSunLightmap().GetLight(worldPos+Vector3i.up) > 5) {
						map.SetBlock( grass, worldPos );
					}
				}
			}
		}
		yield return null;
	}

	private void GenerateBlockForIsland(Vector3i worldPos, int deep, int height) {

			Biome biome = bmanager.getBiome (worldPos);
			if(biome != null) map.SetBlock(biome.surfaceBlock, worldPos);

	}
	
	
	/*private int GetTerrainHeight(int x, int z) {
		float height = terrainNoise.GetNoise(x, z) * 10;
		return (int) height;
	}

	private void GenerateBlockForBaseTerrain(Vector3i worldPos) {
		Block block = null;
		int terrainHeight = GetTerrainHeight(worldPos.x, worldPos.z);
		block = bmanager.getBiome (worldPos,terrainHeight).surfaceBlock;
		if(block != null) map.SetBlock(block, worldPos);
	}*/

}

