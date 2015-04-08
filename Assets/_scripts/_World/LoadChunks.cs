﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class LoadChunks : MonoBehaviour {

	public World world;

	List<WorldPos>updateList = new List<WorldPos>();
	List<WorldPos>buildList = new List<WorldPos>();

	int deleteTimer = 0;

	static  WorldPos[] chunkPositions= {   new WorldPos( 0, 0,  0), new WorldPos(-1, 0,  0), new WorldPos( 0, 0, -1), new WorldPos( 0, 0,  1), new WorldPos( 1, 0,  0),
		new WorldPos(-1, 0, -1), new WorldPos(-1, 0,  1), new WorldPos( 1, 0, -1), new WorldPos( 1, 0,  1), new WorldPos(-2, 0,  0),
		new WorldPos( 0, 0, -2), new WorldPos( 0, 0,  2), new WorldPos( 2, 0,  0), new WorldPos(-2, 0, -1), new WorldPos(-2, 0,  1),
		new WorldPos(-1, 0, -2), new WorldPos(-1, 0,  2), new WorldPos( 1, 0, -2), new WorldPos( 1, 0,  2), new WorldPos( 2, 0, -1),
		new WorldPos( 2, 0,  1), new WorldPos(-2, 0, -2), new WorldPos(-2, 0,  2), new WorldPos( 2, 0, -2), new WorldPos( 2, 0,  2),
		new WorldPos(-3, 0,  0), new WorldPos( 0, 0, -3), new WorldPos( 0, 0,  3), new WorldPos( 3, 0,  0), new WorldPos(-3, 0, -1),
		new WorldPos(-3, 0,  1), new WorldPos(-1, 0, -3), new WorldPos(-1, 0,  3), new WorldPos( 1, 0, -3), new WorldPos( 1, 0,  3),
		new WorldPos( 3, 0, -1), new WorldPos( 3, 0,  1), new WorldPos(-3, 0, -2), new WorldPos(-3, 0,  2), new WorldPos(-2, 0, -3),
		new WorldPos(-2, 0,  3), new WorldPos( 2, 0, -3), new WorldPos( 2, 0,  3), new WorldPos( 3, 0, -2), new WorldPos( 3, 0,  2),
		new WorldPos(-4, 0,  0), new WorldPos( 0, 0, -4), new WorldPos( 0, 0,  4), new WorldPos( 4, 0,  0), new WorldPos(-4, 0, -1),
		new WorldPos(-4, 0,  1), new WorldPos(-1, 0, -4), new WorldPos(-1, 0,  4), new WorldPos( 1, 0, -4), new WorldPos( 1, 0,  4),
		new WorldPos( 4, 0, -1), new WorldPos( 4, 0,  1), new WorldPos(-3, 0, -3), new WorldPos(-3, 0,  3), new WorldPos( 3, 0, -3),
		new WorldPos( 3, 0,  3), new WorldPos(-4, 0, -2), new WorldPos(-4, 0,  2), new WorldPos(-2, 0, -4), new WorldPos(-2, 0,  4),
		new WorldPos( 2, 0, -4), new WorldPos( 2, 0,  4), new WorldPos( 4, 0, -2), new WorldPos( 4, 0,  2), new WorldPos(-5, 0,  0),
		new WorldPos(-4, 0, -3), new WorldPos(-4, 0,  3), new WorldPos(-3, 0, -4), new WorldPos(-3, 0,  4), new WorldPos( 0, 0, -5),
		new WorldPos( 0, 0,  5), new WorldPos( 3, 0, -4), new WorldPos( 3, 0,  4), new WorldPos( 4, 0, -3), new WorldPos( 4, 0,  3),
		new WorldPos( 5, 0,  0), new WorldPos(-5, 0, -1), new WorldPos(-5, 0,  1), new WorldPos(-1, 0, -5), new WorldPos(-1, 0,  5),
		new WorldPos( 1, 0, -5), new WorldPos( 1, 0,  5), new WorldPos( 5, 0, -1), new WorldPos( 5, 0,  1), new WorldPos(-5, 0, -2),
		new WorldPos(-5, 0,  2), new WorldPos(-2, 0, -5), new WorldPos(-2, 0,  5), new WorldPos( 2, 0, -5), new WorldPos( 2, 0,  5),
		new WorldPos( 5, 0, -2), new WorldPos( 5, 0,  2), new WorldPos(-4, 0, -4), new WorldPos(-4, 0,  4), new WorldPos( 4, 0, -4),
		new WorldPos( 4, 0,  4), new WorldPos(-5, 0, -3), new WorldPos(-5, 0,  3), new WorldPos(-3, 0, -5), new WorldPos(-3, 0,  5),
		new WorldPos( 3, 0, -5), new WorldPos( 3, 0,  5), new WorldPos( 5, 0, -3), new WorldPos( 5, 0,  3), new WorldPos(-6, 0,  0),
		new WorldPos( 0, 0, -6), new WorldPos( 0, 0,  6), new WorldPos( 6, 0,  0), new WorldPos(-6, 0, -1), new WorldPos(-6, 0,  1),
		new WorldPos(-1, 0, -6), new WorldPos(-1, 0,  6), new WorldPos( 1, 0, -6), new WorldPos( 1, 0,  6), new WorldPos( 6, 0, -1),
		new WorldPos( 6, 0,  1), new WorldPos(-6, 0, -2), new WorldPos(-6, 0,  2), new WorldPos(-2, 0, -6), new WorldPos(-2, 0,  6),
		new WorldPos( 2, 0, -6), new WorldPos( 2, 0,  6), new WorldPos( 6, 0, -2), new WorldPos( 6, 0,  2), new WorldPos(-5, 0, -4),
		new WorldPos(-5, 0,  4), new WorldPos(-4, 0, -5), new WorldPos(-4, 0,  5), new WorldPos( 4, 0, -5), new WorldPos( 4, 0,  5),
		new WorldPos( 5, 0, -4), new WorldPos( 5, 0,  4), new WorldPos(-6, 0, -3), new WorldPos(-6, 0,  3), new WorldPos(-3, 0, -6),
		new WorldPos(-3, 0,  6), new WorldPos( 3, 0, -6), new WorldPos( 3, 0,  6), new WorldPos( 6, 0, -3), new WorldPos( 6, 0,  3),
		new WorldPos(-7, 0,  0), new WorldPos( 0, 0, -7), new WorldPos( 0, 0,  7), new WorldPos( 7, 0,  0), new WorldPos(-7, 0, -1),
		new WorldPos(-7, 0,  1), new WorldPos(-5, 0, -5), new WorldPos(-5, 0,  5), new WorldPos(-1, 0, -7), new WorldPos(-1, 0,  7),
		new WorldPos( 1, 0, -7), new WorldPos( 1, 0,  7), new WorldPos( 5, 0, -5), new WorldPos( 5, 0,  5), new WorldPos( 7, 0, -1),
		new WorldPos( 7, 0,  1), new WorldPos(-6, 0, -4), new WorldPos(-6, 0,  4), new WorldPos(-4, 0, -6), new WorldPos(-4, 0,  6),
		new WorldPos( 4, 0, -6), new WorldPos( 4, 0,  6), new WorldPos( 6, 0, -4), new WorldPos( 6, 0,  4), new WorldPos(-7, 0, -2),
		new WorldPos(-7, 0,  2), new WorldPos(-2, 0, -7), new WorldPos(-2, 0,  7), new WorldPos( 2, 0, -7), new WorldPos( 2, 0,  7),
		new WorldPos( 7, 0, -2), new WorldPos( 7, 0,  2), new WorldPos(-7, 0, -3), new WorldPos(-7, 0,  3), new WorldPos(-3, 0, -7),
		new WorldPos(-3, 0,  7), new WorldPos( 3, 0, -7), new WorldPos( 3, 0,  7), new WorldPos( 7, 0, -3), new WorldPos( 7, 0,  3),
		new WorldPos(-6, 0, -5), new WorldPos(-6, 0,  5), new WorldPos(-5, 0, -6), new WorldPos(-5, 0,  6), new WorldPos( 5, 0, -6),
		new WorldPos( 5, 0,  6), new WorldPos( 6, 0, -5), new WorldPos( 6, 0,  5) };

	void FindChunksToLoad() {
		WorldPos playerPos = new WorldPos(Mathf.FloorToInt(transform.position.x / Chunk.chunkSize) * Chunk.chunkSize, 
		                                  Mathf.FloorToInt(transform.position.y / Chunk.chunkSize) * Chunk.chunkSize, 
		                                  Mathf.FloorToInt(transform.position.z / Chunk.chunkSize) * Chunk.chunkSize);

		//if there aren't already chunks to generate
		if(buildList.Count == 0) {
			for(int i = 0; i < chunkPositions.Length; i++) {
				//translate player and array position into chunk position
				WorldPos newChunkPos = new WorldPos(chunkPositions[i].x * Chunk.chunkSize + playerPos.x, 0, chunkPositions[i].z * Chunk.chunkSize + playerPos.z);
				Chunk newChunk = world.GetChunk(newChunkPos.x, newChunkPos.y, newChunkPos.z);

				//if the chunk already exists and it's already rendered, continue
				if(newChunk != null && (newChunk.rendered || updateList.Contains(newChunkPos)))
					continue;

				//otherwise load a column of chunks in this position
				for(int y = -4; y < 4; y++) {
					buildList.Add(new WorldPos(newChunkPos.x, y*Chunk.chunkSize, newChunkPos.z));
				}
				return;//only one column added to list at a time
			}
		}
	}

	void BuildChunk(WorldPos pos) {//builds chunk and the surrounding ones
		List<Chunk> chunkData = new List<Chunk>();
		for(int y = pos.y - Chunk.chunkSize; y <= pos.y + Chunk.chunkSize; y+= Chunk.chunkSize) {
			if(y > 64 || y < -64)
				continue;

			for(int x = pos.x - Chunk.chunkSize; x <= pos.x + Chunk.chunkSize; x += Chunk.chunkSize) {
				for(int z = pos.z - Chunk.chunkSize; z <= pos.z + Chunk.chunkSize; z += Chunk.chunkSize) {
					if((y == pos.y || x == pos.x || z == pos.z) && world.GetChunk(x, y, z) == null) {
						Chunk c = new Chunk();
						var thread = new Thread( () => {c = world.LoadChunk(x, y, z);});
						thread.Start();
						thread.Join();
						chunkData.Add(c);
					}
				}
			}
		}

		foreach(Chunk c in chunkData) {
			world.CreateChunk(c);
		}
		updateList.Add(pos);
	}

	void LoadAndRenderChunks() {
		for(int i = 0; i < 2; i++) {//# = how many chunks to build per frame, should eventually be variable based on distance from player
			if(buildList.Count != 0) {
				BuildChunk(buildList[0]);
				buildList.RemoveAt(0);
			}
		}

		for(int i = 0; i < updateList.Count; i++) {
			Chunk chunk = world.GetChunk(updateList[0].x, updateList[0].y, updateList[0].z);
			if(chunk != null) {
				chunk.container.Init();
				chunk.update = true;
			}
			updateList.RemoveAt(0);
		}
	}

	void DeleteChunks() {
		if(deleteTimer == 60) {
			deleteTimer = 0;
			var chunksToDelete = new List<WorldPos>();
			foreach(var chunk in world.chunks) {
				float dist = Vector3.Distance(new Vector3(chunk.Value.pos.x, 0, chunk.Value.pos.z), new Vector3(transform.position.x, 0, transform.position.z));
				if(dist > 64) {
					chunksToDelete.Add(chunk.Key);
				}
			}

			foreach(var chunk in chunksToDelete) {
				world.DestroyChunk(chunk.x, chunk.y, chunk.z);
			}
		}
		deleteTimer++;
	}

	void Update () {
		DeleteChunks();
		FindChunksToLoad();
		LoadAndRenderChunks();
	}
}
