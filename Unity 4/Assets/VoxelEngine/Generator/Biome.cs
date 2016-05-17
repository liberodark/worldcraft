using System;
using UnityEngine;

public class Biome{
	
	private float _persistence;
	private Block _surfaceBlock;
	private static int biomeCount = 0;
	private int biomeId = -1;
	private bool _isFlat;
	private int _maxMountainHeight;
	private int _offset = 0;

	private static NoiseArray2D heightNoise = new NoiseArray2D(1/120f).SetOctaves(2);

	public Biome (float pers, Block surface, bool isFlat, int maxMountainHeight, int offset){
		_persistence = pers;
		biomeId = biomeCount;
		biomeCount ++;
		_surfaceBlock = surface;
		_isFlat = isFlat;
		_maxMountainHeight = maxMountainHeight;
		_offset = offset;
		BiomeManager.addBiome (this);
	}

	public static void generateHeightNoise(int x, int z){
		heightNoise.GenerateNoise (x, z);
	}

	public int getHeight(int x, int z){
		float height;
		lock (heightNoise) {
			height = heightNoise.GetNoise (x, z) * _maxMountainHeight + _offset;
		}
		return (int)height;
	}

	public float persistence{
		get{
			return _persistence;
		}
	}

	public bool isFlat{
		get{
			return _isFlat;
		}
	}

	public int maxMountainHeight{
		get{
			return _maxMountainHeight;
		}
	}

	public int getBiomeId(){
		return biomeId;
	}

	public Block surfaceBlock{
		get{
			return _surfaceBlock;
		}
	}
}