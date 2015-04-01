using UnityEngine;
using System.Collections;
using CoherentNoise.Generation.Fractal;

public class TerrainGen {

	//base layer
	float stoneBaseHeight = -24;
	float stoneBaseNoise = 0.05f;//noise frequency of layer
	float stoneBaseNoiseHeight = 4;//amplitude of noise

	//mountain/hill layer
	float stoneMountainHeight = 48;
	float stoneMountainFrequency = 0.008f;//frequency of mountain layer
	float stoneMinHeight = -12;

	//dirt layer
	float dirtBaseHeight = 1;
	float dirtNoise = 0.04f;
	float dirtNoiseHeight = 3;

	public Chunk ChunkGen(Chunk chunk) {
		for(int x = chunk.pos.x; x < chunk.pos.x + Chunk.chunkSize; x++) {//using world coords
			for(int z = chunk.pos.z; z < chunk.pos.z + Chunk.chunkSize; z++) {
				chunk = ChunkColumnGen(chunk, x, z);
			}
		}
		return chunk;
	}

	public Chunk ChunkColumnGen(Chunk chunk, int x, int z) {
		int stoneHeight = Mathf.FloorToInt(stoneBaseHeight);													//set base height
		stoneHeight += GetNoise(x, 0, z, stoneMountainFrequency, Mathf.FloorToInt(stoneMountainHeight));		//add mountain noise
		if(stoneHeight < stoneMinHeight)
			stoneHeight = Mathf.FloorToInt(stoneMinHeight);														//raise all stone up to min height (if needed)
		stoneHeight += GetNoise(x, 0, z, stoneBaseNoise, Mathf.FloorToInt(stoneBaseNoiseHeight));				//add base noise

		int dirtHeight = stoneHeight + Mathf.FloorToInt(dirtBaseHeight);										//set base dirt height
		dirtHeight += GetNoise(x, 100, z, dirtNoise, Mathf.FloorToInt(dirtNoiseHeight));						//add noise to dirt height

		for(int y = chunk.pos.y; y < chunk.pos.y + Chunk.chunkSize; y++) {										//decide which block goes where based on height
			if(y <= stoneHeight) {
				chunk.SetBlock(x - chunk.pos.x, y - chunk.pos.y, z - chunk.pos.z, new Block());
			} else if(y <= dirtHeight) {
				chunk.SetBlock(x - chunk.pos.x, y - chunk.pos.y, z - chunk.pos.z, new BlockGrass());
			} else {
				chunk.SetBlock(x - chunk.pos.x, y - chunk.pos.y, z - chunk.pos.z, new BlockAir());
			}
		}
		return chunk;
	}

	public static int GetNoise(int x, int y, int z, float scale, int max) {
		string seed = "hello world";
		PinkNoise n = new PinkNoise(seed.GetHashCode());
		return Mathf.FloorToInt((n.GetValue(x*scale, y*scale, z*scale) + 1f) * (max/2f));
	}
}
