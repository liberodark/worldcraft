using UnityEngine;
using System;
using System.Threading;
using System.Collections;

public class BiomeManager{

	private static SortedList biomes = new SortedList();
	private Map map;
	private static bool generating = false;
	private static Biome currentBiome = null;

	//Perlin noises
	private static NoiseArray3D biomeNoise3D;
	private static NoiseArray3D caveNoise3D;

	private Vector3i done;

	public BiomeManager (Map m){

		map = m;

		currentBiome = new Biome (700, map.GetBlockSet ().GetBlock ("Sand"),false,65,40);//Desert
		new Biome (700, map.GetBlockSet ().GetBlock ("Dirt"),true,65,40);//Plain
		new Biome(700, map.GetBlockSet ().GetBlock ("Snow"),false,65,40);//Snow
		//new Biome(700, map.GetBlockSet ().GetBlock ("Dirt"),false,30);//Ocean

		initNoises ();

	}

	public void initNoises(){
		biomeNoise3D = new NoiseArray3D(0.005f);
		caveNoise3D = new NoiseArray3D(1/50f);
	}

	public void generateNoise(Vector3i worldPos){
		biomeNoise3D.GenerateNoise (worldPos);
		caveNoise3D.GenerateNoise(worldPos);
	}

	public static Biome getCurrentBiome(){
		return currentBiome;
	}

	public static void addBiome(Biome b){
		biomes.Add(b.getBiomeId(),b);
	}

	public Biome getBiome(Vector3i position){
		if(caveNoise3D.GetNoise(position.x, position.y, position.z) > 0.7f && position.y >= 2) return null;

		int noise = (int) Math.Floor(biomeNoise3D.GetNoise(position.x, position.y, position.z)/(1.0f/biomes.Count));
		if (noise == biomes.Count)noise --;
		Biome tmpBiome = (Biome)biomes.GetByIndex (noise);
		if (tmpBiome != currentBiome) {
						biomeNoise3D.setScale (1.0f / tmpBiome.persistence);
		}
		if(noise >= 0 && noise < biomes.Count)
			currentBiome = tmpBiome;
		return currentBiome;
	}
	
}