using UnityEngine;
using System.Collections;

public class CartController : MonoBehaviour {

	public proceduralRoadGenerator generator;
	public WheelCollider wheelFR;
	public WheelCollider wheelFL;
	public WheelCollider wheelBR;
	public WheelCollider wheelBL;
	public Rigidbody spineRigid;
	public GameObject endmenu;
	public AudioClip[] falls;
	public AudioClip[] wins;
	//public Transform plane;
	public float turnRadius;
	public float antiroll;
	public float valueOfDeath;
	private bool alive;
	private AudioSource audiosource;

	// Use this for initialization
	void Start () {
		//Screen.lockCursor = true;
		audiosource = GetComponent<AudioSource>();
		alive = true;
	}
	
	// Update is called once per frame
	void Update () {		
		if (alive) {
			wheelFR.steerAngle = Input.GetAxis ("Mouse X") * turnRadius;
			wheelFL.steerAngle = Input.GetAxis ("Mouse X") * turnRadius;
			DoRollBar (wheelFR, wheelFL);
			DoRollBar (wheelBR, wheelBL);
			if (transform.position.y < valueOfDeath) {
				die ();
			}
		} else {
			Camera.main.transform.LookAt (this.transform.position);
		}
		//plane.position = this.transform.position + new Vector3 (0, 0, 81.8f);
	}

	void die() {
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		Camera.main.transform.parent = null;
		generator.enabled = false;
		alive = false;
		spineRigid.useGravity = true;
		spineRigid.constraints = RigidbodyConstraints.None;
		Destroy (spineRigid.GetComponent<HingeJoint> ());
		endmenu.SetActive (true);
		audiosource.clip = falls [Random.Range (0, falls.Length - 1)];
		audiosource.Play ();
	}

	void OnTriggerEnter(Collider collider) {
		audiosource.clip = wins [Random.Range (0, wins.Length - 1)];
		audiosource.Play ();
	}

	void DoRollBar(WheelCollider WheelL, WheelCollider WheelR) {
		WheelHit hit;
		float travelL = 1.0f;
		float travelR = 1.0f;

		bool groundedL = WheelL.GetGroundHit (out hit);
		if (groundedL)
			travelL = (-WheelL.transform.InverseTransformPoint (hit.point).y - WheelL.radius) / WheelL.suspensionDistance;

		bool groundedR = WheelR.GetGroundHit (out hit);
		if (groundedR)
			travelR = (-WheelR.transform.InverseTransformPoint (hit.point).y - WheelR.radius) / WheelR.suspensionDistance;

		float antiRollForce = (travelL - travelR) * antiroll;

		if (groundedL)
			GetComponent<Rigidbody> ().AddForceAtPosition (WheelL.transform.up * -antiRollForce, WheelL.transform.position);

		if (groundedR)
			GetComponent<Rigidbody> ().AddForceAtPosition (WheelR.transform.up * antiRollForce, WheelR.transform.position);
	}
}
