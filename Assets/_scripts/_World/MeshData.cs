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

	public void AddVertex(Vector3 vertex) {
		vertices.Add(vertex);
		if(useRenderDataForCollision) {
			colVertices.Add(vertex);
		}
	}

	//verticies must listed in a CLOCKWISE order
	//params are how many verts back to go (4 would be the vertex added 4 back)
	public void AddQuadTriangles(int t0, int t1, int t2, int t3) {
		triangles.Add(vertices.Count - (t0+1));
		triangles.Add(vertices.Count - (t1+1));
		triangles.Add(vertices.Count - (t2+1));
		
		triangles.Add(vertices.Count - (t0+1));
		triangles.Add(vertices.Count - (t2+1));
		triangles.Add(vertices.Count - (t3+1));
		
		if(useRenderDataForCollision) {
			colTriangles.Add(vertices.Count - (t0+1));
			colTriangles.Add(vertices.Count - (t1+1));
			colTriangles.Add(vertices.Count - (t2+1));
			
			colTriangles.Add(vertices.Count - (t0+1));
			colTriangles.Add(vertices.Count - (t2+1));
			colTriangles.Add(vertices.Count - (t3+1));
		}
	}

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
	
	public void AddTriangle(int t0, int t1, int t2) {
		triangles.Add(vertices.Count - (t0+1));
		triangles.Add(vertices.Count - (t1+1));
		triangles.Add(vertices.Count - (t2+1));
		if(useRenderDataForCollision) {
			colTriangles.Add(vertices.Count - (t0+1));
			colTriangles.Add(vertices.Count - (t1+1));
			colTriangles.Add(vertices.Count - (t2+1));
		}
	}

	//make tri out of last 3 verticies
	//verticies must added in a CLOCKWISE order
	public void AddTriangle() {
		triangles.Add(vertices.Count - 3);
		triangles.Add(vertices.Count - 2);
		triangles.Add(vertices.Count - 1);
		
		if(useRenderDataForCollision) {
			colTriangles.Add(vertices.Count - 3);
			colTriangles.Add(vertices.Count - 2);
			colTriangles.Add(vertices.Count - 1);
		}
	}

	//collision stuff
	public void AddColVertex(Vector3 vertex) {
		colVertices.Add(vertex);
	}
	
	public void AddColQuadTriangles(int t0, int t1, int t2, int t3) {
		colTriangles.Add(vertices.Count - (t0+1));
		colTriangles.Add(vertices.Count - (t1+1));
		colTriangles.Add(vertices.Count - (t2+1));
		
		colTriangles.Add(vertices.Count - (t0+1));
		colTriangles.Add(vertices.Count - (t2+1));
		colTriangles.Add(vertices.Count - (t3+1));
	}

	//make quad out of last 4 verticies
	//verticies must added in a CLOCKWISE order
	public void AddColQuadTriangles() {
		colTriangles.Add(vertices.Count - 4);
		colTriangles.Add(vertices.Count - 3);
		colTriangles.Add(vertices.Count - 2);
		
		colTriangles.Add(vertices.Count - 4);
		colTriangles.Add(vertices.Count - 2);
		colTriangles.Add(vertices.Count - 1);
		
	}
	
	public void AddColTriangle(int t0, int t1, int t2) {
		colTriangles.Add(vertices.Count - (t0+1));
		colTriangles.Add(vertices.Count - (t1+1));
		colTriangles.Add(vertices.Count - (t2+1));
	}

	//make tri out of last 3 verticies
	//verticies must added in a CLOCKWISE order
	public void AddColTriangle() {
		colTriangles.Add(vertices.Count - 3);
		colTriangles.Add(vertices.Count - 2);
		colTriangles.Add(vertices.Count - 1);
	}

}