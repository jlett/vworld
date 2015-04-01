using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]

public class Chunk : MonoBehaviour {
	Block[ , , ] blocks;
	public static int chunkSize = 16;
	public bool update = true;

	MeshFilter filter;
	MeshCollider coll;

	void Start() {
		filter = gameObject.GetComponent<MeshFilter>();
		coll = gameObject.GetComponent<MeshCollider>();

		//example chunk
		blocks = new Block[chunkSize, chunkSize, chunkSize];

		for(int x = 0; x < chunkSize; x++) {
			for(int y = 0; y < chunkSize; y++) {
				for(int z = 0; z < chunkSize; z++) {
					blocks[x, y, z] = new BlockAir();
				}
			}
		}

		blocks[3, 5, 2] = new Block();
		UpdateChunk();
	}

	void Update() {

	}

	//returns the requested block at coords relative to chunk origin
	public Block GetBlock(int x, int y, int z) {
		return blocks[x, y, z];
	}

	//updates the chunk based on it's contents
	void UpdateChunk() {
		MeshData meshData = new MeshData();
		
		for(int x = 0; x < chunkSize; x++) {
			for(int y = 0; y < chunkSize; y++) {
				for(int z = 0; z < chunkSize; z++) {
					meshData = blocks[x, y, z].BlockData(this, x, y, z, meshData);
				}
			}
		}
		
		RenderMesh(meshData);
	}
	//sends the calculated mesh info to mesh and collision components
	void RenderMesh(MeshData meshData) {
		filter.mesh.Clear();
		filter.mesh.vertices = meshData.vertices.ToArray();
		filter.mesh.triangles = meshData.triangles.ToArray();
	}
}
