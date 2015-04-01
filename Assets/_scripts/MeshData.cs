using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshData {
	public List<Vector3> vertices = new List<Vector3>();
	public List<int> triangles = new List<int>();//every 3 makes a triangle (each entry is an index of verticies)
	public List<Vector2> uv = new List<Vector2>();//every 2 entries makes a triangle (lower left, lower right)

	public List<Vector3> colVertices = new List<Vector3>();
	public List<int> colTriangles = new List<int>();
	public bool useRenderDataForCollision = false;

	public MeshData() {}

	//make quad out of last 4 verticies
	//verticies must added in a CLOCKWISE order
	public void AddQuadTriangles() {
		triangles.Add(vertices.Count - 4);
		triangles.Add(vertices.Count - 3);
		triangles.Add(vertices.Count - 2);

		triangles.Add(vertices.Count - 4);
		triangles.Add(vertices.Count - 2);
		triangles.Add(vertices.Count - 1);

		if(useRenderDataForCollision) {
			colTriangles.Add(vertices.Count - 4);
			colTriangles.Add(vertices.Count - 3);
			colTriangles.Add(vertices.Count - 2);
			
			colTriangles.Add(vertices.Count - 4);
			colTriangles.Add(vertices.Count - 2);
			colTriangles.Add(vertices.Count - 1);
		}
	}

	public void AddVertex(Vector3 vertex) {
		vertices.Add(vertex);
		if(useRenderDataForCollision) {
			colVertices.Add(vertex);
		}
	}

	public void AddTriangle(int tri) {
		triangles.Add(tri);
		if(useRenderDataForCollision) {
			colTriangles.Add(tri - (vertices.Count - colVertices.Count));
		}
	}

	//remove duplicate vertices, stiching the triangles together
	public void RemoveDoubles() {

	}
}
