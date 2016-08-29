using UnityEngine;
using System.Collections;

public class Score : MonoBehaviour {

	public Color[] colors;
	public string username;
	public float width;
	private LineRenderer linerenderer;
	private TextMesh textmesh;


	// Use this for initialization
	void Start () {
		linerenderer = GetComponent<LineRenderer> ();
		textmesh = GetComponent<TextMesh> ();
		linerenderer.SetPosition (0, transform.position);
		linerenderer.SetPosition (1, transform.position + new Vector3(width, 0, 0));
		Color color = colors[Random.Range(0, colors.Length-1)];
		linerenderer.material = new Material(Shader.Find("Particles/Additive"));
		linerenderer.SetColors (color, color);
		textmesh.text = username;
		BoxCollider collider = gameObject.AddComponent<BoxCollider> ();
		collider.isTrigger = true;
		collider.size = new Vector3 (width, 5, 5);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
