using UnityEngine;
using System.Collections.Generic;

public class wheeldraw : MonoBehaviour {

	public convexhull convex;
	public Rigidbody cartrigid;
	public MeshFilter[] meshFilters;
	public ScoreManager scoremanager;
	public float wheelScale;
	public float rotSpeed;
	public proceduralRoadGenerator generator;
	public Camera cam;
	private List<Vector3> vects;
	private bool isactive;
	private LineRenderer linerenderer;
	private Vector3 lastmousepos;
	private int wheelCount;
	private bool hasInit;

	// Use this for initialization
	void Start () {		
		linerenderer = GetComponent<LineRenderer> ();
		wheelCount = 0;
		hasInit = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (!hasInit) {
			scoremanager.LoadLeaderboard ();
			hasInit = true;
		}
		cartrigid.transform.Rotate(Vector3.up, rotSpeed * Time.deltaTime);
		//cam.transform.LookAt (cartrigid.transform.position);
		if (Input.GetMouseButton (0) && isactive == false) {
			isactive = true;
			vects = new List<Vector3> ();
			linerenderer.SetVertexCount (vects.Count);
			linerenderer.SetPositions (vects.ToArray ());
		} else if (Input.GetMouseButton (0) == false && isactive) {
			isactive = false;
			if (vects.Count < 3)
				return;
			vects.Add (vects [0]);
			Vector3[] convexHull = convexhull.QuickHull (vects);
			linerenderer.SetVertexCount (convexHull.Length);
			linerenderer.SetPositions (convexHull);
			Vector3[] bounds = getWheelBounds (convexHull);
			Vector3[] offsetedvects = new Vector3[convexHull.Length];
			for (int i = 0; i < convexHull.Length; i++) {
				offsetedvects[i] = convexHull[i];
				offsetedvects [i].x = Mathf.Abs (offsetedvects [i].x - bounds [0].x);
				offsetedvects [i].y = Mathf.Abs (offsetedvects [i].y - bounds [2].y);
				//offsetedvects [i].z = Mathf.Abs (offsetedvects [i].z);
				//offsetedvects [i].y -= Vector3.Distance (bounds [3], bounds [2]) * 0.5f;
				//offsetedvects [i].x -= Vector3.Distance (bounds [1], bounds [0]) * 0.5f;
			}
			WheelCollider collider = meshFilters [wheelCount].GetComponent<TireToWheel> ().wheelCollider;
			convex.BuildMesh (meshFilters [wheelCount++], offsetedvects, wheelScale, bounds);
			float radius = 0.5f * getWheelRadius(bounds) * wheelScale;
			collider.radius = radius;
			if (cartrigid.transform.position.y < radius) {
				Vector3 pos = cartrigid.transform.position;
				pos.y = radius * 2;
				cartrigid.transform.position = pos;
			}
			if (wheelCount >= 4) {
				gameObject.SetActive (false);
				for (int i = 0; i < 4; i++) {
					TireToWheel tiretowheel = meshFilters [i].GetComponent<TireToWheel> ();
					Vector3 avgpoint = Vector3.zero;
					for (int j = 0; j < meshFilters [i].mesh.vertices.Length; j++) {
						avgpoint += meshFilters [i].mesh.vertices [j];
					}
					avgpoint /= meshFilters [i].mesh.vertices.Length;
					tiretowheel.center = avgpoint;
					tiretowheel.wheelScale = wheelScale;
					collider = tiretowheel.wheelCollider;
					collider.enabled = true;
					//collider.center = new Vector3 (0, -Mathf.Abs(Vector3.Distance (bounds[3], bounds[2])) * wheelScale * 0.5f, Mathf.Abs(Vector3.Distance (bounds[1], bounds[0])) * wheelScale * 0.5f);
					/*if (i == 1 || i == 2) {
						tiretowheel.transform.localPosition = new Vector3 (-convex.offset * wheelScale, 0, 0);
					} else {
						tiretowheel.transform.localPosition = new Vector3 (convex.offset * wheelScale, 0, 0);
						//tiretowheel.transform.localPosition = new Vector3 (convex.offset, 0, 0);
					}*/
					tiretowheel.transform.localPosition = new Vector3 (-wheelScale, 0, 0);
				}
				cartrigid.useGravity = true;
				cartrigid.isKinematic = false;
				cartrigid.transform.localRotation = Quaternion.identity;
				cam.transform.parent = cartrigid.transform;
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
				gameObject.SetActive (false);
				generator.enabled = true;
			}				
		}

		if (isactive && (lastmousepos != Input.mousePosition)) {
			Vector3 mousepos = Input.mousePosition;
			lastmousepos = mousepos;
			mousepos.z = 3;
			vects.Add (cam.ScreenToWorldPoint(mousepos));
			linerenderer.SetVertexCount (vects.Count);
			linerenderer.SetPositions (vects.ToArray ());
		}
	}

	Vector3[] getWheelBounds(Vector3[] convexHull) {
		Vector3 A = convexHull[0];
		// Find the leftmost point in convexHull
		foreach (Vector3 v in convexHull) {
			if (v.x < A.x) {
				A = v;
			} else if (v.x == A.x && v.y < A.y) {
				A = v;
			}
		}
		Vector3 B = convexHull[0];
		// Find the rightmost point in convexHull
		foreach (Vector3 v in convexHull) {
			if (v.x > B.x) {
				B = v;
			} else if (v.x == B.x && v.y > B.y) {
				B = v;
			}
		}
		Vector3 C = convexHull[0];
		// Find the downmost point in convexHull
		foreach (Vector3 v in convexHull) {
			if (v.y < C.y) {
				C = v;
			} else if (v.y == C.y && v.x < C.x) {
				C = v;
			}
		}
		Vector3 D = convexHull[0];
		// Find the topmost point in convexHull
		foreach (Vector3 v in convexHull) {
			if (v.y > D.y) {
				D = v;
			} else if (v.y == D.y && v.x > D.x) {
				D = v;
			}
		}
		return new Vector3[4] { A, B, C, D };
	}

	float getWheelRadius(Vector3[] bounds) {
		return Mathf.Max (Vector3.Distance (bounds[1], bounds[0]), Vector3.Distance (bounds[3], bounds[2]));
	}
}
