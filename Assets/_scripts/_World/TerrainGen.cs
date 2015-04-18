using UnityEngine;
using System.Collections;
using CoherentNoise.Generation.Fractal;

public class TerrainGen {

	//base layer
	float stoneBaseHeight = -24;
	float stoneBaseNoise = 0.05f;//noise frequency of layer
	float stoneBaseNoiseHeight = 4;//max amplitude of noise

	//mountain/hill layer
	float stoneMountainHeight = 48;
	float stoneMountainFrequency = 0.004f;//frequency of mountain layer
	float stoneMinHeight = -12;

	//dirt layer
	float dirtBaseHeight = 4;
	float dirtNoise = 0.04f;
	float dirtNoiseHeight = 2;

	//caves
	float caveFrequency = 0.02f;
	int caveSize = 20;

	public Chunk ChunkGen(Chunk chunk) {
		for(int x = chunk.pos.x; x < chunk.pos.x + Chunk.chunkSize; x++) {//using world coords
			for(int z = chunk.pos.z; z < chunk.pos.z + Chunk.chunkSize; z++) {
				chunk = ChunkColumnGen(chunk, x, z);
			}
		}
		return chunk;
	}

	public Chunk ChunkColumnGen(Chunk chunk, int x, int z) {
		int stoneHeight = Mathf.FloorToInt(stoneBaseHeight);																			//set base height
		stoneHeight += GetPinkNoise(x, 0, z, chunk.world.seed, stoneMountainFrequency, Mathf.FloorToInt(stoneMountainHeight), 0.6f);		//add mountain noise
		if(stoneHeight < stoneMinHeight)
			stoneHeight = Mathf.FloorToInt(stoneMinHeight);																				//raise all stone up to min height (if needed)
		stoneHeight += GetPinkNoise(x, 0, z, chunk.world.seed, stoneBaseNoise, Mathf.FloorToInt(stoneBaseNoiseHeight), 0.2f);				//add base noise

		int dirtHeight = stoneHeight + Mathf.FloorToInt(dirtBaseHeight);																//set base dirt height
		dirtHeight += GetPinkNoise(x, 100, z, chunk.world.seed, dirtNoise, Mathf.FloorToInt(dirtNoiseHeight), 0.2f);						//add noise to dirt height																							//add layer of grass on top

		for(int y = chunk.pos.y; y < chunk.pos.y + Chunk.chunkSize; y++) {															//decide which block goes where based on height
			int caveChance = GetRidgeNoise(x, y, z, chunk.world.seed, caveFrequency, 100);

			if(y <= stoneHeight && caveSize < caveChance) {
				chunk.SetBlock(x - chunk.pos.x, y - chunk.pos.y, z - chunk.pos.z, new BlockStone());
			} else if(y <= dirtHeight && caveSize < caveChance) {
				chunk.SetBlock(x - chunk.pos.x, y - chunk.pos.y, z - chunk.pos.z, new BlockDirt());
			} else if(y <= dirtHeight + 1 && caveSize < caveChance) {
				chunk.SetBlock(x - chunk.pos.x, y - chunk.pos.y, z - chunk.pos.z, new BlockGrass());
			} else {
				chunk.SetBlock(x - chunk.pos.x, y - chunk.pos.y, z - chunk.pos.z, new BlockAir());
			}
		}

		return chunk;
	}

	public static int GetPinkNoise(int x, int y, int z, string seed, float scale, int max, float persistence) {
		PinkNoise n = new PinkNoise(seed.GetHashCode());
		n.Persistence = persistence;
		return Mathf.FloorToInt((n.GetValue(x*scale, y*scale, z*scale) + 1f) * (max/2f));
	}

	public static int GetRidgeNoise(int x, int y, int z, string seed, float scale, int max) {
		RidgeNoise n = new RidgeNoise(seed.GetHashCode());
		n.Exponent = 0.8f;
		return Mathf.FloorToInt(((1f - n.GetValue(x*scale, y*scale, z*scale)) + 1f) * (max/2f));
	}
}
