using UnityEngine;
using System.Collections;

public class TreeGenerator {
	
	private Map map;
	
	private Block snowWood;
	private Block dirtWood;
	private Block snowLeaves;
	private Block dirtLeaves;
	private Block sandLeaves;
	private Block sandWood;
	private Block cactus;
	
	public TreeGenerator(Map map) {
		this.map = map;
		BlockSet blockSet = map.GetBlockSet ();

		dirtWood = blockSet.GetBlock("DirtWood");
		dirtLeaves = blockSet.GetBlock("DirtLeaves");

		snowWood = blockSet.GetBlock("SnowWood");
		snowLeaves = blockSet.GetBlock("SnowLeaves");


		sandWood = blockSet.GetBlock("SandWood");
		sandLeaves = blockSet.GetBlock("SandLeaves");

		cactus = blockSet.GetBlock("Cactus");
	}	
	
	public void Generate(int x, int y, int z) {
		BlockData block = map.GetBlock(x, y-1, z);
		if(block.IsEmpty() || (!block.block.GetName().Equals("Sand") && !block.block.GetName().Equals("Dirt") && !block.block.GetName().Equals("Snow"))) return;
		float score = Random.Range (0f, 1f);
		if (score > 0.3f){
			if(block.block.GetName().Equals("Sand") && score < 0.5f) {
				GenerateCactus(x, y, z);
			}
			return;
		}
		GenerateTree(x, y, z, block.block);
	}
	
	private void GenerateTree(int x, int y, int z, Block b) {
		int treeSize = Random.Range (20, 30);
		if(b.GetName().Equals("Dirt")){
			GenerateLeaves( new Vector3i(x, y+treeSize, z), dirtLeaves,treeSize - 9 );
			for(int i=0; i<treeSize; i++) {
				map.SetBlock(new BlockData(dirtWood), new Vector3i(x, y+i, z));
			}
		}else if(b.GetName().Equals("Snow")){	
			GeneratePinLeaves( new Vector3i(x, y+treeSize, z), snowLeaves, treeSize - 9 );
			for(int i=0; i<treeSize; i++) {
				map.SetBlock(new BlockData(snowWood), new Vector3i(x, y+i, z));
			}
		}else if(b.GetName().Equals("Sand")){	
			GenerateSandLeaves( new Vector3i(x, y+treeSize, z), sandLeaves, treeSize - 9 );
			for(int i=0; i<treeSize; i++) {
				map.SetBlock(new BlockData(sandWood), new Vector3i(x, y+i, z));
			}
		}
	}

	private void GenerateCactus(int x, int y, int z) {
		int cactusSize = Random.Range (3,10);
		for(int i=0; i<cactusSize; i++) {
				map.SetBlock(new BlockData(cactus), new Vector3i(x, y+i, z));
		}
	}
	
	private void GenerateLeaves(Vector3i center, Block leaves, int size) {

		int y1 = center.y - size/3;
		int y2 = center.y + size;

		for(int y=y1; y<=y2; y++) {

			int x1 = center.x - size/3 - 1 + (y-y1)/2;
			int z1 = center.z - size/3 - 1 + (y-y1)/2;
			int x2 = center.x + size/3 + 1 - (y-y1)/2;
			int z2 = center.z + size/3 + 1 - (y-y1)/2;

			for(int x=x1; x<=x2; x++) {
				for(int z=z1; z<=z2; z++) {
					if(x >= x1 + 1 && x <= x2 - 1 || z >= z1 + 1 && z <= z2 - 1)
						map.SetBlock(leaves, new Vector3i(x, y, z));
				}
			}
		}
		y2 = center.y - 9*size/10;
		for(int y=y2; y<y1 + 1; y++) {
			
			int x1 = center.x - size/3 - 1 + Mathf.Abs(y - y1 + 1);
			int z1 = center.z - size/3 - 1 + Mathf.Abs(y - y1 + 1);
			int x2 = center.x + size/3 + 1 - Mathf.Abs(y - y1 + 1);
			int z2 = center.z + size/3 + 1 - Mathf.Abs(y - y1 + 1);
			
			for(int x=x1; x<=x2; x++) {
				for(int z=z1; z<=z2; z++) {
					if(x >= x1 + 1 && x <= x2 - 1 || z >= z1 + 1 && z <= z2 - 1)
						map.SetBlock(leaves, new Vector3i(x, y, z));
				}
			}
		}
	}

	private void GeneratePinLeaves(Vector3i center, Block leaves, int size) {
		
		int y1 = center.y - size/3;
		int y2 = center.y + size;
		
		for(int y=y1; y<=y2; y++) {
			
			int x1 = center.x - size/3 - 1 + (y-y1)/3;
			int z1 = center.z - size/3 - 1 + (y-y1)/3;
			int x2 = center.x + size/3 + 1 - (y-y1)/3;
			int z2 = center.z + size/3 + 1 - (y-y1)/3;
			
			for(int x=x1; x<=x2; x++) {
				for(int z=z1; z<=z2; z++) {
					if(x >= x1 + 1 && x <= x2 - 1 || z >= z1 + 1 && z <= z2 - 1)
						map.SetBlock(leaves, new Vector3i(x, y, z));
				}
			}
		}

		y2 = center.y - 8 * size/10;
		for(int y=y2; y<y1 + 1; y++) {
			
			int x1 = center.x - size/3 - 1 + Mathf.Abs(y - y1);
			int z1 = center.z - size/3 - 1 + Mathf.Abs(y - y1);
			int x2 = center.x + size/3 + 1 - Mathf.Abs(y - y1);
			int z2 = center.z + size/3 + 1 - Mathf.Abs(y - y1);
			
			for(int x=x1; x<=x2; x++) {
				for(int z=z1; z<=z2; z++) {
					if(x >= x1 + 1 && x <= x2 - 1 || z >= z1 + 1 && z <= z2 - 1)
						map.SetBlock(leaves, new Vector3i(x, y, z));
				}
			}
		}
	}

	private void GenerateSandLeaves(Vector3i center, Block leaves, int size) {
		
		int y1 = center.y - size/3;
		int y2 = center.y + size;
		
		for(int y=y1; y<=y2; y++) {
			
			int x1 = center.x - size/3 - 1 + (y-y1);
			int z1 = center.z - size/3 - 1 + (y-y1);
			int x2 = center.x + size/3 + 1 - (y-y1);
			int z2 = center.z + size/3 + 1 - (y-y1);
			
			for(int x=x1; x<=x2; x++) {
				for(int z=z1; z<=z2; z++) {
					if(x == x1 + 1 || x == x2 - 1 || z == z1 + 1 || z == z2 - 1)
						map.SetBlock(leaves, new Vector3i(x, y, z));
				}
			}
		}
	}
}