using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Chunk
{
	public Block[ , , ] blocks = new Block[chunkSize, chunkSize, chunkSize];
	public static int chunkSize = 16;
	public bool update = false;
	public World world;
	public WorldPos pos;

	public bool rendered;

	public MeshFilter filter;
	public MeshCollider coll;
	public ChunkContainer container;
	
	//returns the requested block at coords relative to chunk origin
	public Block GetBlock (int x, int y, int z)
	{
		if (CoordInRange (x) && CoordInRange (y) && CoordInRange (z)) {
			return blocks [x, y, z];
		}
		return world.GetBlock (pos.x + x, pos.y + y, pos.z + z);
	}

	public Face GetFace (int x, int y, int z, Block.Direction dir)
	{
		return GetBlock (x, y, z).GetFace (dir);
	}

	public static bool CoordInRange (int index)
	{
		if (index < 0 || index >= chunkSize)
			return false;
		return true;
	}

	public void SetBlock (int x, int y, int z, Block block)
	{
		if (CoordInRange (x) && CoordInRange (y) && CoordInRange (z)) {
			blocks [x, y, z] = block;
		} else {
			world.SetBlock (pos.x + x, pos.y + y, pos.z + z, block);
		}
	}

	//init blocks before the first render/build call
	public void InitBlocks ()
	{
		for(int x = 0; x < chunkSize; x++) {
			for(int y = 0; y < chunkSize; y++) {
				for(int z = 0; z < chunkSize; z++) {
					blocks[x, y, z].changed = false;
				}
			}
		}
	}

	//updates the chunk based on it's contents
	public void UpdateChunk ()
	{
		Profiler.BeginSample ("Update Chunk");
		rendered = true;

		MeshData meshData = new MeshData ();

		for(int a = 0; a < chunkSize; a++) {//a instead of x cause compiler is stupid
			for(int y = 0; y < chunkSize; y++) {
				for(int z = 0; z < chunkSize; z++) {
					blocks[a, y, z].Update(this, a, y, z);
				}
			}
		}

		//-------------------------------------GREEDY MESHING------------------------------------------------
		Face[] mask = new Face[chunkSize * chunkSize];
		int i, j, k, l, w, h, u, v, n;
		
		int[] x = new int []{0,0,0};
		int[] q = new int []{0,0,0};
		int[] du = new int[]{0,0,0}; 
		int[] dv = new int[]{0,0,0};
		Block.Direction side;
		Face voxelFace, voxelFace1;

		for (bool backFace = true, b = false; b != backFace; backFace = backFace && b, b = !b) { 
			for (int d = 0; d < 3; d++) {
				
				u = (d + 1) % 3; 
				v = (d + 2) % 3;
				
				x [0] = 0;
				x [1] = 0;
				x [2] = 0;
				
				q [0] = 0;
				q [1] = 0;
				q [2] = 0;
				q [d] = 1;

				if (d == 0) {
					side = backFace ? Block.Direction.west : Block.Direction.east;
				} else if (d == 1) {
					side = backFace ? Block.Direction.down : Block.Direction.up;
				} else {
					side = backFace ? Block.Direction.south : Block.Direction.north;
				}

				//move through dimension front to back
				for (x[d] = -1; x[d] < chunkSize;) {
					//generate the mask
					n = 0;
					
					for (x[v] = 0; x[v] < chunkSize; x[v]++) {
						for (x[u] = 0; x[u] < chunkSize; x[u]++) {
							
							/*
                             * Here we retrieve two voxel faces for comparison.
                             */
							voxelFace = (x [d] >= 0) ? GetFace (x [0], x [1], x [2], side) : null;
							voxelFace1 = (x [d] < chunkSize - 1) ? GetFace (x [0] + q [0], x [1] + q [1], x [2] + q [2], side) : null;
							
							/*
                             * Note that we're using the equals function in the voxel face class here, which lets the faces 
                             * be compared based on any number of attributes.
                             * 
                             * Also, we choose the face to add to the mask depending on whether we're moving through on a backface or not.
                             */
							mask [n++] = ((voxelFace != null && voxelFace1 != null && voxelFace.CanMeshWith (voxelFace1))) 
								? null 
									: backFace ? voxelFace1 : voxelFace;
						}
					}

					x [d]++;

					/*
                     * Now we generate the mesh for the mask
                     */
					n = 0;
					
					for (j = 0; j < chunkSize; j++) {
						for (i = 0; i < chunkSize;) {
							if (mask [n] != null) {
								
								/*
                                 * We compute the width
                                 */
								for (w = 1; i + w < chunkSize && mask[n + w] != null && mask[n + w].CanMeshWith(mask[n]); w++) {}
								
								/*
                                 * Then we compute height
                                 */
								bool done = false;
								
								for (h = 1; j + h < chunkSize; h++) {
									for (k = 0; k < w; k++) {
										if (mask [n + k + h * chunkSize] == null || !mask [n + k + h * chunkSize].CanMeshWith(mask [n])) {
											done = true;
											break;
										}
									}
									
									if (done)
										break;
								}
								/*
                                 * Here we check the "transparent" attribute in the VoxelFace class to ensure that we don't mesh 
                                 * any culled faces.
                                 */
								if (mask[n].isVisible) {
									/*
                                     * Add quad
                                     */
									x [u] = i;  
									x [v] = j;
									
									du [0] = 0;
									du [1] = 0;
									du [2] = 0;
									du [u] = w;
									
									dv [0] = 0;
									dv [1] = 0;
									dv [2] = 0;
									dv [v] = h;
									
									/*
                                     * And here we call the quad function in order to render a merged quad in the scene.
                                     * 
                                     * We pass mask[n] to the function, which is an instance of the VoxelFace class containing 
                                     * all the attributes of the face - which allows for variables to be passed to shaders - for 
                                     * example lighting values used to create ambient occlusion.
                                     */
									if(!backFace)
										AddQuad(mask[n].block, new Vector3[4] {new Vector3 (x [0], x [1], x [2]), new Vector3 (x [0] + du [0], x [1] + du [1], x [2] + du [2]), new Vector3 (x [0] + du [0] + dv [0], x [1] + du [1] + dv [1], x [2] + du [2] + dv [2]), new Vector3 (x [0] + dv [0], x [1] + dv [1], x [2] + dv [2])}, side, w, h, meshData);
									else
										AddQuad(mask[n].block, new Vector3[4] {new Vector3 (x [0], x [1], x [2]), new Vector3 (x [0] + dv [0], x [1] + dv [1], x [2] + dv [2]), new Vector3 (x [0] + du [0] + dv [0], x [1] + du [1] + dv [1], x [2] + du [2] + dv [2]), new Vector3 (x [0] + du [0], x [1] + du [1], x [2] + du [2])}, side, w, h, meshData);
								}
								
								/*
                                 * We zero out the mask
                                 */
								for (l = 0; l < h; ++l) {
									for (k = 0; k < w; ++k) {
										mask [n + k + l * chunkSize] = null;
									}
								}
								
								/*
                                 * And then finally increment the counters and continue
                                 */
								i += w; 
								n += w;
								
							} else {
								
								i++;
								n++;
							}
						}
					}
				}
			}
		}
			
		RenderMesh (meshData);
		Profiler.EndSample ();
	}

	protected MeshData AddQuad(Block b, Vector3[] t, Block.Tile tile, int w, int h, MeshData meshData) {
		if(t.Length != 4) {
			Debug.LogError("4 points are required for a quad... idiot");
		}
		for(int i = 0; i < 4; i++) {
			meshData.AddVertex(new Vector3(t[i].x, t[i].y, t[i].z));
		}
		meshData.AddQuadTriangles();

		List<Vector2> uv = new List<Vector2>();
//		int texHeight = Mathf.FloorToInt((1f/Block.tileSize) + 0.5f);
//		float sideCoord = Block.tileSize * ((float)(texHeight - tile.y));
//		uv.Add(new Vector2(0, sideCoord + (h * texHeight)));
//		uv.Add(new Vector2(w, sideCoord + (h * texHeight)));
//		uv.Add(new Vector2(0, sideCoord));
//		uv.Add(new Vector2(w, sideCoord));
//		meshData.uv.AddRange(uv);
		meshData.uv.AddRange(b.GetQuadUVs(tile));
		return meshData;
	}
	
	protected MeshData AddQuad(Block b, Vector3[] t, Block.Direction dir, int w, int h, MeshData meshData) {
		return AddQuad(b, t, b.GetTexturePosition(dir), w, h, meshData);
	}
		
	//sends the calculated mesh info to mesh and collision components
	void RenderMesh (MeshData meshData)
	{
		filter.mesh.Clear ();
		filter.mesh.vertices = meshData.vertices.ToArray ();
		filter.mesh.triangles = meshData.triangles.ToArray ();
			
		filter.mesh.uv = meshData.uv.ToArray ();
			
		if (meshData.useCustomNormals)
			filter.mesh.normals = meshData.normals.ToArray ();
		else
			filter.mesh.RecalculateNormals ();
			
		coll.sharedMesh = null;
		Mesh mesh = new Mesh ();
		mesh.vertices = meshData.colVertices.ToArray ();
		mesh.triangles = meshData.colTriangles.ToArray ();
		mesh.RecalculateNormals ();
			
		coll.sharedMesh = mesh;
	}
}
	