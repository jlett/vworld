﻿using UnityEngine;
using System.Collections;
using LibNoise.Generator;

public class TerrainGen {

	//base layer
	float stoneBaseHeight = -24;
	float stoneBaseNoise = 0.05f;//noise frequency of layer
	float stoneBaseNoiseHeight = 4;//max amplitude of noise

	//mountain/hill layer
	float stoneMountainHeight = 72;
	float stoneMountainFrequency = 0.004f;//frequency of mountain layer
	float stoneMinHeight = -12;

	//dirt layer
	float dirtBaseHeight = 6;
	float dirtNoise = 0.04f;
	float dirtNoiseHeight = 2;

	//caves
	float caveFrequency = 0.02f;
	int caveSize = 10;

	//trees
	float treeFrequency = 0.2f;
	int treeDensity = 3;

	public Chunk ChunkGen(Chunk chunk) {
		WorldPos pos = chunk.pos;
		for(int x = pos.x-3; x < pos.x + Chunk.chunkSize+3; x++) {//using world coords
			for(int z = pos.z-3; z < pos.z + Chunk.chunkSize+3; z++) {
				chunk = ChunkColumnGen(chunk, x, z);
			}
		}
		return chunk;
	}

	public Chunk ChunkColumnGen(Chunk chunk, int x, int z) {
		int stoneHeight = Mathf.FloorToInt(stoneBaseHeight);																			//set base height
		stoneHeight += GetPerlinNoise(x, 0, z, chunk.world.seed, stoneMountainFrequency, Mathf.FloorToInt(stoneMountainHeight), 0.6f);		//add mountain noise
		if(stoneHeight < stoneMinHeight)
			stoneHeight = Mathf.FloorToInt(stoneMinHeight);																				//raise all stone up to min height (if needed)
		stoneHeight += GetPerlinNoise(x, 0, z, chunk.world.seed, stoneBaseNoise, Mathf.FloorToInt(stoneBaseNoiseHeight), 0.3f);				//add base noise

		int dirtHeight = stoneHeight + Mathf.FloorToInt(dirtBaseHeight);																//set base dirt height
		dirtHeight += GetPerlinNoise(x, 100, z, chunk.world.seed, dirtNoise, Mathf.FloorToInt(dirtNoiseHeight), 0.2f);						//add noise to dirt height																							//add layer of grass on top

		for(int y = chunk.pos.y-8; y < chunk.pos.y + Chunk.chunkSize; y++) {															//decide which block goes where based on height
			if(caveSize > GetPerlinNoise(x, y, z, chunk.world.seed, caveFrequency, 100, 0.3f)) {
				SetBlock(x, y, z, new BlockAir(), chunk);
			}else if(y <= stoneHeight) {
				SetBlock(x, y, z, new BlockStone(), chunk);
			} else if(y <= dirtHeight) {
				SetBlock(x, y, z, new BlockDirt(), chunk);
			} else if(y <= dirtHeight + 1) {
				SetBlock(x, y, z, new BlockGrass(), chunk);
				if (y == dirtHeight+1 && GetPerlinNoise(x, 0, z, chunk.world.seed, treeFrequency, 100, .3f) < treeDensity)
					CreateTree(x, y+1, z, chunk);
			} else {
				SetBlock(x, y, z, new BlockAir(), chunk);
			}

		}
		return chunk;
	}

	public static void SetBlock(int x, int y, int z, Block block, Chunk chunk, bool replaceBlocks = false) {
		x -= chunk.pos.x;
		y -= chunk.pos.y;
		z -= chunk.pos.z;

		if(Chunk.CoordInRange(x) && Chunk.CoordInRange(y) && Chunk.CoordInRange(z)) {
			if (replaceBlocks || chunk.blocks[x, y, z] == null)
				chunk.SetBlock(x, y, z, block);
		}
	}

	public static int GetPerlinNoise(int x, int y, int z, string seed, float scale, int max, float persistence) {
		Perlin n = new Perlin();
		n.Persistence = persistence;
		n.Seed = seed.GetHashCode();
		int result = (int)((n.GetValue(x*scale, y*scale, z*scale) + 1f) * (max/2f));
		return result;
	}

	//---------------------------------structure generation functions-----------------------------------
	void CreateTree(int x, int y, int z, Chunk chunk) {
		//create leaves
		for (int xi = -2; xi <= 2; xi++) {
			for (int yi = 4; yi <= 8; yi++) {
				for (int zi = -2; zi <= 2; zi++) {
					SetBlock (x + xi, y + yi, z + zi, new BlockLeaf(), chunk, true);
				}
			}
		}
		
		//create trunk
		for (int yt = 0; yt < 6; yt++) {
			SetBlock (x, y + yt, z, new BlockLog(), chunk, true);
		}
	}

}
