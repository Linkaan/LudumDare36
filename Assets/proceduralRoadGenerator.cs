using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class proceduralRoadGenerator : MonoBehaviour {

	public float width;
	public float length;
	public float blocks_ahead;
	public float blocks_behind;
	public int max_delta;
	public Transform target;
	public GameObject scorePrefab;
	public ScoreManager scoremanager;
	private MeshFilter meshfilter;
	private Mesh mesh;
	private MeshCollider meshcollider;
	private List<Vector3> verticies;
	private List<Vector2> uvs;
	private List<int> triangles;
	private float curx, curz;
	private bool hasInit;

	// Use this for initialization
	void Start () {
		meshcollider = GetComponent<MeshCollider> ();
		meshfilter = GetComponent<MeshFilter> ();
		mesh = meshfilter.mesh;
		verticies = new List<Vector3> ();
		uvs = new List<Vector2> ();
		triangles = new List<int> ();
		curx = 0;
		curz = 0;
		hasInit = false;
	}

	public int getScore() {
		return Mathf.CeilToInt(curz / length) - (int) blocks_ahead;
	}

	void UpdateBlock(float nx) {
		for (int i = 0; i < 4; i++) {
			verticies.RemoveAt (0);
			uvs.RemoveAt (0);
		}
		verticies.Add (new Vector3 (curx + 0, 0, curz + 0));
		verticies.Add (new Vector3 (curx + width, 0, curz + 0));
		verticies.Add (new Vector3 (curx + nx, 0, curz + length));
		verticies.Add (new Vector3 (curx + nx + width, 0, curz + length));
		uvs.Add (new Vector2 (curx + 0, curz + 0));
		uvs.Add (new Vector2 (curx + width, curz + 0));
		uvs.Add (new Vector2 (curx + nx, curz + length));
		uvs.Add (new Vector2 (curx + nx + width, curz + length));
		curz += length;
		curx += nx;
		mesh.vertices = verticies.ToArray ();
		mesh.uv = uvs.ToArray ();		
		mesh.RecalculateBounds ();
		mesh.RecalculateNormals ();
		meshcollider.sharedMesh = mesh;
		int value = Mathf.CeilToInt (curz / length);
		foreach (string username in scoremanager.GetUserIDs("score")) {
			if (value == scoremanager.GetScore (username, "score")) {				
				GameObject scoreobj = Instantiate (scorePrefab, new Vector3(curx, transform.position.y, curz), Quaternion.identity) as GameObject;
				Score score = scoreobj.GetComponent<Score> ();
				score.width = width;
				score.username = username;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {		
		if (!hasInit && scoremanager.ls == ScoreManager.leaderboardState.done) {
			hasInit = true;
			for (int i = 0; i < blocks_behind + blocks_ahead; i++) {
				float nx = Random.Range (-0.1f, 0.1f);
				verticies.Add (new Vector3 (curx + 0, 0, curz + 0));
				verticies.Add (new Vector3 (curx + width, 0, curz + 0));
				verticies.Add (new Vector3 (curx + nx, 0, curz + length));
				verticies.Add (new Vector3 (curx + nx + width, 0, curz + length));
				uvs.Add (new Vector2 (curx + 0, curz + 0));
				uvs.Add (new Vector2 (curx + width, curz + 0));
				uvs.Add (new Vector2 (curx + nx, curz + length));
				uvs.Add (new Vector2 (curx + nx + width, curz + length));
				curz += length;
				curx += nx;
				triangles.Add (i * 4 + 0);
				triangles.Add (i * 4 + 3);
				triangles.Add (i * 4 + 1);
				triangles.Add (i * 4 + 0);
				triangles.Add (i * 4 + 2);
				triangles.Add (i * 4 + 3);
			}
			mesh.vertices = verticies.ToArray ();
			mesh.triangles = triangles.ToArray ();
			mesh.uv = uvs.ToArray ();		
			mesh.RecalculateBounds ();
			mesh.RecalculateNormals ();
			meshcollider.sharedMesh = mesh;
		} else if (hasInit) {
			if ((target.position.z - curz) > -(blocks_ahead * length)) {
				for (int i = 0; i < blocks_behind / 2; i++) {
					UpdateBlock (Random.Range (-max_delta, max_delta));
				}
			}
		}
	}
}
