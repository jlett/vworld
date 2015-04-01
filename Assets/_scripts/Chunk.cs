using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]

public class Chunk : MonoBehaviour {
	public Block[ , , ] blocks = new Block[chunkSize, chunkSize, chunkSize];
	public static int chunkSize = 16;
	public bool update = true;
	public World world;
	public WorldPos pos;

	MeshFilter filter;
	MeshCollider coll;

	void Start() {
		filter = gameObject.GetComponent<MeshFilter>();
		coll = gameObject.GetComponent<MeshCollider>();
	}

	void Update() {
		if(update) {
			update = false;
			UpdateChunk();
		}
	}

	//returns the requested block at coords relative to chunk origin
	public Block GetBlock(int x, int y, int z) {
		if(CoordInRange(x) && CoordInRange(y) && CoordInRange(z)) {
			return blocks[x, y, z];
		}
		return world.GetBlock(pos.x + x, pos.y + y, pos.z + z);
	}

	public static bool CoordInRange(int index) {
		if(index < 0 || index >= chunkSize)
			return false;
		return true;
	}

	public void SetBlock(int x, int y, int z, Block block) {
		if(CoordInRange(x) && CoordInRange(y) && CoordInRange(z)) {
			blocks[x, y, z] = block;
		} else {
			world.SetBlock(pos.x + x, pos.y + y, pos.z + z, block);
		}
	}

	//set changed flag to false on all blocks (called during initial world gen)
	public void SetBlocksUnmodified() {
		foreach(Block block in blocks) {
			block.changed = false;
		}
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

		filter.mesh.uv = meshData.uv.ToArray();
		filter.mesh.RecalculateNormals();

		coll.sharedMesh = null;
		Mesh mesh = new Mesh();
		mesh.vertices = meshData.colVertices.ToArray();
		mesh.triangles = meshData.colTriangles.ToArray();
		mesh.RecalculateNormals();
		coll.sharedMesh = mesh;
	}
}
