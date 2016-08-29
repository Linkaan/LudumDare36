using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class convexhull : MonoBehaviour {

	public float offset = 10.0f;
	public int width = 10;
	public float spacing = 1f;
	private Mesh mesh;
	private List<Vector3> verticies;
	private List<int> triangles;
	private Triangulator triangulator;
	private bool hasdone;

	// Use this for initialization
	void Start () {
		triangulator = new Triangulator();
		verticies = new List<Vector3>();
		triangles = new List<int>();
	}

	void Update() {
		/**
		if (hullConvex.Length > 0 && hasdone == false) {
			hasdone = true;
			Debug.Log ("Building mesh");
			Vector2[] verts2d = new Vector2[hullConvex.Length];
			for (int i = 0; i < hullConvex.Length; i++) {
				verts2d [i].x = hullConvex [i].x;
				verts2d [i].y = hullConvex [i].y;
			}
			mesh = triangulator.CreateInfluencePolygon (verts2d);
			meshfilter.mesh = mesh;
			verticies.AddRange (mesh.vertices);
			triangles.AddRange (mesh.triangles);
			int normalLength = mesh.normals.Length;
			int submeshCount = mesh.subMeshCount;
			extrudeMesh (verts2d);
			//ReverseNormals (mesh, normalLength, submeshCount);
			//createMesh (hullConvex);
		}
		*/
	}

	public void BuildMesh(MeshFilter meshfilter, Vector3[] convexHull, float scale, Vector3[] bounds) {
		Vector2[] verts2d = new Vector2[convexHull.Length];
		for (int i = 0; i < convexHull.Length; i++) {
			verts2d [i].x = convexHull [i].x;
			verts2d [i].y = convexHull [i].y;
		}
		verticies.Clear ();
		triangles.Clear ();
		mesh = triangulator.CreateInfluencePolygon (verts2d);
		meshfilter.mesh = mesh;
		verticies.AddRange (mesh.vertices);
		triangles.AddRange (mesh.triangles);
		extrudeMesh (verts2d, meshfilter, bounds, scale);
		ReverseNormals (mesh); // workaround for normal problem, ha!
	}

	void ReverseNormals(Mesh mesh) {
		Vector3[] normals = mesh.normals;
		for (int i=0;i<normals.Length;i++)
			normals[i] = -normals[i];
		mesh.normals = normals;

		for (int m=0;m<mesh.subMeshCount;m++)
		{
			int[] triangles = mesh.GetTriangles(m);
			for (int i=0;i<triangles.Length;i+=3)
			{
				int temp = triangles[i + 0];
				triangles[i + 0] = triangles[i + 1];
				triangles[i + 1] = temp;
			}
			mesh.SetTriangles(triangles, m);
		}
	}

	void extrudeMesh(Vector2[] convexHull, MeshFilter meshfilter, Vector3[] bounds, float scale) {
		
		int verteciesCount = verticies.Count;
		for (int i = 0; i < verteciesCount; i++) {
			Vector3 v = verticies [i] + new Vector3 (0, offset, 0);
			verticies.Add (v);
		}
		List<Edge> edges = new List<Edge> ();
		for (int i = 0; i < convexHull.Length; i++) {
			if (i == convexHull.Length - 1) {
				edges.Add (new Edge (i, 0));
			} else {
				edges.Add(new Edge(i, i+1));
			}
		}
		/**
		for (int i = 0; i < triangles.Count; i += 3) {			
			edges.Add (new Edge (triangles [i + 0], triangles [i + 1]));
			edges.Add (new Edge (triangles [i + 1], triangles [i + 2]));
			edges.Add (new Edge (triangles [i + 2], triangles [i + 0]));
		}
*/
		/**
		for (int i = 0; i < edges.Count; i++) {
			for (int j = 0; j < edges.Count; j++) {
				if ((edges[i].p1 == edges[j].p2) && (edges[i].p2 == edges[j].p1)) {
					//Debug.Log ("removing (" + edges[i].p1 + ", " + edges[i].p2 + ") and (" + edges[j].p1 + ", " + edges[j].p2 + ")");
					Edge iedge = edges[i];
					Edge jedge = edges[j];
					edges.Remove(iedge);
					edges.Remove(jedge);
				}
			}
		}*/
		int count = triangles.Count;
		for (int i = 0; i < count; i += 3) {			
			triangles.Add(triangles [i + 2] + verteciesCount);
			triangles.Add(triangles [i + 1] + verteciesCount);
			triangles.Add(triangles [i + 0] + verteciesCount);
		}
		for (int i = 0; i < edges.Count; i++) {
			triangles.Add (edges [i].p1);
			triangles.Add (edges [i].p2);
			triangles.Add (edges [i].p2 + verteciesCount);
			triangles.Add (edges [i].p2 + verteciesCount);
			triangles.Add (edges [i].p1 + verteciesCount);
			triangles.Add (edges [i].p1);
		}

/*
		Edges.Add(new Edge(TriangleList[ii2].p1, TriangleList[ii2].p2));
		Edges.Add(new Edge(TriangleList[ii2].p2, TriangleList[ii2].p3));
		Edges.Add(new Edge(TriangleList[ii2].p3, TriangleList[ii2].p1));
*/
		/**
	
		int index = mesh.vertices.Length;
		for (; index < triangles.Count - 1; index++) {
			triangles.Add(index++); //0
			triangles.Add(index++); //1
			triangles.Add(index);   //2
			triangles.Add(index);   //2
			triangles.Add(index-1); //1
			triangles.Add(++index); //3
		}
		index = verticies.Count - 2;
		int index2 = mesh.vertices.Length;
		triangles.Add(index++);  //0
		triangles.Add(index);    //1
		triangles.Add(index2);   //2
		triangles.Add(index2++); //2
		triangles.Add(index);    //1
		triangles.Add(index2);   //3
		*/
		Vector3 center = Vector3.zero;
		Quaternion newRotation = new Quaternion();
		newRotation.eulerAngles = new Vector3(90,90,0);

		for (int i = 0; i < verticies.Count; i++) {
			verticies[i] = newRotation * (verticies[i] - center) + center;
		}
		mesh.vertices = verticies.ToArray ();
		mesh.triangles = triangles.ToArray();;
		mesh.RecalculateBounds ();
		Vector3 last_p = getPivotPoint (mesh);
		Debug.Log (last_p);
		Vector3 p = Vector3.zero;
		//Vector3 p = new Vector3(0, Vector3.Distance (bounds [3], bounds [2]) * 0.5f, Vector3.Distance (bounds [1], bounds [0]) * 0.5f);
		Vector3 diff = Vector3.Scale (mesh.bounds.extents, last_p - p);
		diff.y = 0;
		Transform transform = meshfilter.gameObject.transform;
		transform.localScale = new Vector3 (scale, scale, scale);
		Vector3 scaledDiff = Vector3.Scale (diff, transform.localScale);
		transform.position -= scaledDiff;

		for (int i = 0; i < verticies.Count; i++) {
			verticies [i] += diff;
		}
		//meshfilter.gameObject.transform.localScale = new Vector3 (scale, scale, scale);
		mesh.vertices = verticies.ToArray ();
		mesh.RecalculateBounds ();
		mesh.RecalculateNormals ();
		/*
		Vector3 newDiff = diff;
		newDiff.z = newDiff.x;
		newDiff.x = 0;
		transform.GetComponent<TireToWheel>().wheelCollider.center += newDiff;
		*/
		//mesh.Optimize ();


	}

	Vector3 getPivotPoint (Mesh mesh) {
		float p1, p2, p3;
		p1 = p2 = p3 = 0f;
		Bounds b = mesh.bounds;
		Vector3 offset = -1 * b.center;
		if (b.extents.x > 0) {
			p1 = offset.x / b.extents.x;
		}
		if (b.extents.y > 0) {
			p1 = offset.y / b.extents.y;
		}
		if (b.extents.z > 0) {
			p1 = offset.z / b.extents.z;
		}
		return new Vector3(p1, p2, p3);
	}

	/**
	void createMesh(Vector3[] verts) {
		List<Vector2> uvs = new List<Vector2> ();
		for (int i = 0; i < verts.Length; i++) {
			uvs.Add(new Vector2(verts[i].x, verts[i].y));
			verticies.Add(new Vector3(verts[i].x, 0, verts[i].y));
		}
		mesh = new Mesh ();
		mesh.vertices = verticies.ToArray();
		mesh.triangles = TriangulatePolygon(uvs.ToArray());
		mesh.uv = uvs.ToArray ();
		//mesh.RecalculateBounds ();
		mesh.RecalculateNormals ();
		//mesh.Optimize ();
		meshfilter.mesh = mesh;
	}

	int[] TriangulatePolygon(Vector2[] verts) {
		float xmin, xmax = verts [0].x;
		float ymin, ymax = verts [0].y;
		xmin = xmax;
		ymin = ymax;
		for (int i = 1; i < verts.Length; i++) {
			if (verts [i].x < xmin)
				xmin = verts [i].x;
			else if (verts [i].x > xmax)
				xmax = verts [i].x;
			if (verts [i].y < ymin)
				ymin = verts [i].y;
			else if (verts [i].y > ymax)
				ymax = verts [i].y;
		}
		float dx = xmax - xmin;
		float dy = ymax - ymin;
		float dmax = (dx > dy) ? dx : dy;
		float xmid = (xmax + xmin) * 0.5f;
		float ymid = (ymax + ymin) * 0.5f;
		Vector2[] expverts = new Vector2[3 + verts.Length];
		for (int i = 0; i < verts.Length; i++) {
			expverts [i] = verts [i];
		}
		expverts [verts.Length + 0] = new Vector2 ((xmid - 2 * dmax), (ymid - dmax));
		expverts [verts.Length + 1] = new Vector2 (xmid, (ymid + 2 * dmax));
		expverts [verts.Length + 2] = new Vector2 ((xmid + 2 * dmax), (ymid - dmax));
		List<Triangle> tris = new List<Triangle> ();
		tris.Add (new Triangle (verts.Length, verts.Length + 1, verts.Length + 2));
		for (int i = 0; i < verts.Length; i++) {
			List<Edge> edges = new List<Edge> ();
			for (int j = 0; j < tris.Count; j++) {
				if (InCircle (expverts [i], expverts [tris [j].a], expverts [tris [j].b], expverts [tris [j].c])) {
					edges.Add (new Edge (tris [j].a, tris [j].b));
					edges.Add (new Edge (tris [j].b, tris [j].c));
					edges.Add (new Edge (tris [j].c, tris [j].a));
					tris.RemoveAt (j);
					j--;
				}
			}
			if (i >= verts.Length)
				continue;
			for (int j = edges.Count - 2; j >= 0; j--) {
				for (int k = edges.Count - 3; k >= 0; k--) {
					if (edges [j].Equals (edges [k])) {
						edges.RemoveAt (j);
						edges.RemoveAt (k);
						k--;
						continue;
					}
				}
			}
			for (int j = 0; j < edges.Count; j++) {
				tris.Add (new Triangle (edges [j].a, edges [j].b, i));
			}
			edges.Clear ();
			edges = null;
		}
		for (int i = tris.Count - 1; i >= 0; i--) {
			if (tris [i].a >= verts.Length ||
			    tris [i].b >= verts.Length ||
			    tris [i].c >= verts.Length) {
				tris.RemoveAt (i);
			}
		}
		tris.TrimExcess ();
		int[] triangles = new int[3 * tris.Count];
		for (int i = 0; i < tris.Count; i++) {
			triangles [3 * i + 0] = tris [i].a;
			triangles [3 * i + 1] = tris [i].b;
			triangles [3 * i + 2] = tris [i].c;
		}
		return triangles;
	}

	bool InCircle(Vector2 point, Vector2 a, Vector2 b, Vector2 c) {
		if (Mathf.Abs (a.y - b.y) < float.Epsilon &&
		    Mathf.Abs (b.y - c.y) < float.Epsilon)
			return false;
		float m1, m2;
		float mx1, mx2;
		float my1, my2;
		float xc, yc;
		if (Mathf.Abs (b.y - a.y) < float.Epsilon) {
			m2 = -(c.x - b.x) / (c.y - b.y);
			mx2 = (b.x + c.x) * 0.5f;
			my2 = (b.y + c.y) * 0.5f;
			xc = (b.x + a.x) * 0.5f;
			yc = m2 * (xc - mx2) + my2;
		} else if (Mathf.Abs (c.y - b.y) < float.Epsilon) {
			m1 = -(b.x - a.x) / (b.y - a.y);
			mx1 = (a.x + b.x) * 0.5f;
			my1 = (a.y + b.y) * 0.5f;
			xc = (c.x + b.x) * 0.5f;
			yc = m1 * (xc - mx1) + my1;
		} else {
			m1 = -(b.x - a.x) / (b.y - a.y);
			m2 = -(c.x - b.x) / (c.y - b.y);
			mx1 = (a.x + b.x) * 0.5f;
			mx2 = (b.x + c.x) * 0.5f;
			my1 = (a.y + b.y) * 0.5f;
			my2 = (b.y + c.y) * 0.5f;
			xc = (m1 * mx1 - m2 * mx2 + my2 - my1) / (m1 - m2);
			yc = m1 * (xc - mx1) + my1;
		}
		float dx = b.x - xc;
		float dy = b.y - yc;
		float rsqr = dx * dx + dy * dy;
		dx = point.x - xc;
		dy = point.y - yc;
		double drsqr = dx * dx + dy * dy;
		return (drsqr <= rsqr);
	}*/

	public static Vector3[] QuickHull(List<Vector3> verts) {
		List<Vector3> Hull = new List<Vector3>();
		Vector3 A = verts[0];
		// Find the leftmost point in verts
		foreach (Vector3 v in verts) {
			if (v.x < A.x) {
				A = v;
			} else if (v.x == A.x && v.y < A.y) {
				A = v;
			}
		}
		Vector3 B = verts[0];
		// Find the rightmost point in verts
		foreach (Vector3 v in verts) {
			if (v.x > B.x) {
				B = v;
			} else if (v.x == B.x && v.y > B.y) {
				B = v;
			}
		}
		// Add leftmost and rightmost point to convex hull
		Hull.Add (A);
		Hull.Add (B);
		verts.Remove (A);
		verts.Remove (B);
		// Divide remaining points in two groups designated as S1 and S2
		List<Vector3> S1 = new List<Vector3>();
		List<Vector3> S2 = new List<Vector3>();
		foreach (Vector3 v in verts) {
			if (CalculateArea (A, v, B) > 0.0f) {
				S1.Add (v);
			} else if (CalculateArea (B, v, A) > 0.0f) {
				S2.Add (v);
			}
		}
		FindHull (S1, A, B, Hull);
		FindHull (S2, B, A, Hull);
		Hull.Add(Hull [0]);
		return Hull.ToArray ();
	}

	static void FindHull(List<Vector3> Sk, Vector3 P, Vector3 Q, List<Vector3> convexHull) {

		// Find points on convex hull from the set Sk of points
		// that are on the right side of the oriented line from P to Q
		if (Sk.Count == 0)
			return;
		Vector3 C = Sk[0];
		float largestArea = 0.0f;
		float area = 0.0f;
		// Find farthest point from segment PQ by determining the largest area by
		// forming the triangles (P, v, Q)
		foreach (Vector3 v in Sk) {
			area = CalculateArea (P, v, Q);
			if (area > largestArea) {
				largestArea = area;
				C = v; // farthest away
			}
		}
		Sk.Remove (C);
		// Add C to convexHull between P and Q
		convexHull.Insert(convexHull.IndexOf(Q), C);

		// Ignore points inside of triangle PCQ and divide remaining verts into
		// groups where S1 will be on the right side of PC and S2 are on the right
		// side of CQ
		List<Vector3> S1 = new List<Vector3>();
		List<Vector3> S2 = new List<Vector3>();
		foreach (Vector3 v in Sk) {
			if (!PointInTriangle (v, P, C, Q)) {
				if (CalculateArea (P, v, C) > 0.0f) {
					S1.Add (v);
				} else if (CalculateArea (C, v, Q) > 0.0f) {
					S2.Add (v);
				}
			}
		}
		FindHull (S1, P, C, convexHull);
		FindHull (S2, C, Q, convexHull);

	}

	static bool PointInTriangle(Vector3 point, Vector3 a, Vector3 b, Vector3 c) {
		bool b1, b2, b3;

		b1 = CalculateArea (point, a, b) < 0.0f;
		b2 = CalculateArea (point, b, c) < 0.0f;
		b3 = CalculateArea (point, c, a) < 0.0f;

		return ((b1 == b2) && (b2 == b3));
	}

	/*
	 * Signed area calculation
	 * See: http://geomalgorithms.com/a01-_area.html
	 */
	static float CalculateArea(Vector3 a, Vector3 b, Vector3 c) {
		return (b.x - a.x) * (c.y - a.y) - (c.x - a.x) * (b.y - a.y);
	}
}
/**
struct Triangle {
	public int a;
	public int b;
	public int c;

	public Triangle(int a, int b, int c) {
		this.a = a;
		this.b = b;
		this.c = c;
	}
}

class Edge {
	public int a;
	public int b;

	public Edge(int a, int b) {
		this.a = a;
		this.b = b;
	}

	public Edge() : this(0, 0) { }

	public bool Equals(Edge other) {
		return
			((this.a == other.b) && (this.b == other.a)) ||
			((this.a == other.a) && (this.b == other.b)); 
	}
}
*/