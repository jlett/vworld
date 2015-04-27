using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class LoadChunks : MonoBehaviour {

	public World world;

	Queue<WorldPos>updateList = new Queue<WorldPos>();
	Queue<WorldPos>buildList = new Queue<WorldPos>();

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
		if(updateList.Count == 0) {
			for(int i = 0; i < chunkPositions.Length; i++) {
				//translate player and array position into chunk position
				WorldPos newChunkPos = new WorldPos(chunkPositions[i].x * Chunk.chunkSize + playerPos.x, 0, chunkPositions[i].z * Chunk.chunkSize + playerPos.z);
				Chunk newChunk = world.GetChunk(newChunkPos.x, newChunkPos.y, newChunkPos.z);

				//if the chunk already exists and it's already rendered, continue
				if(newChunk != null && (newChunk.rendered || updateList.Contains(newChunkPos)))
					continue;

				//otherwise load a column of chunks in this position
				for(int y = -4; y < 4; y++) {
					for (int x = newChunkPos.x - Chunk.chunkSize; x <= newChunkPos.x + Chunk.chunkSize; x += Chunk.chunkSize) {
						for (int z = newChunkPos.z - Chunk.chunkSize; z <= newChunkPos.z + Chunk.chunkSize; z += Chunk.chunkSize) {
							buildList.Enqueue(new WorldPos(x, y * Chunk.chunkSize, z));
						}
					}
					updateList.Enqueue(new WorldPos(newChunkPos.x, y * Chunk.chunkSize, newChunkPos.z));
				};
				return;//only one column added to list at a time
			}
		}
	}

	void BuildChunk(WorldPos pos) {
		if (world.GetChunk(pos.x,pos.y,pos.z) == null)
			world.CreateChunk(world.LoadChunk(pos.x,pos.y,pos.z));
	}

	void LoadAndRenderChunks() {
		if (buildList.Count != 0) {
			int buildCount = buildList.Count;
			int buildIntensity = 8;

			float dist = Vector3.Distance(new Vector3(buildList.Peek().x, 0, buildList.Peek().z), new Vector3(transform.position.x, 0, transform.position.z));

			buildIntensity += 8 * Mathf.FloorToInt((256 - dist)/32);
			if(buildIntensity <= 0)
				buildIntensity = 8;

			for (int i = 0; i < buildCount && i < buildIntensity; i++) {
				BuildChunk(buildList.Dequeue());
			}
			return;//If chunks were built return early
		}

		if ( updateList.Count!=0) {
			WorldPos pos = updateList.Dequeue();
			Chunk chunk = world.GetChunk(pos.x, pos.y, pos.z);
			if (chunk != null) {
				chunk.container.Init();
				chunk.update = true;
			}
		}
	}

	bool DeleteChunks() {
		if(deleteTimer == 10) {
			deleteTimer = 0;
			var chunksToDelete = new List<WorldPos>();
			foreach(var chunk in world.chunks) {
				float dist = Vector3.Distance(new Vector3(chunk.Value.pos.x, 0, chunk.Value.pos.z), new Vector3(transform.position.x, 0, transform.position.z));
				if(dist > 256) {
					chunksToDelete.Add(chunk.Key);
				}
			}

			foreach(var chunk in chunksToDelete) {
				world.DestroyChunk(chunk.x, chunk.y, chunk.z);
			}
			return true;
		}
		deleteTimer++;

		return false;
	}

	void Update () {
		if(DeleteChunks())
			return;
		FindChunksToLoad();
		LoadAndRenderChunks();
	}
}
