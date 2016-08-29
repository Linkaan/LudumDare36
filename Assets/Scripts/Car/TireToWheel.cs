using UnityEngine;
using System.Collections;

public class TireToWheel : MonoBehaviour {

	public WheelCollider wheelCollider;
	public Vector3 center;
	public float wheelScale;

	void Start() {
		center = Vector3.zero;
	}

	void Update() {
		/*
		float distanceTraveled = (wheelCollider.radius * Mathf.PI * wheelCollider.rpm * 60f / 1000f) * Time.deltaTime;
		float rotationInRadians = distanceTraveled / wheelCollider.radius;
		float rotationInDegrees = rotationInRadians * Mathf.Rad2Deg;
		transform.Rotate(0, rotationInDegrees, 0);
		*/
		if (center != Vector3.zero) {
			float distanceTraveled = (wheelCollider.radius * Mathf.PI * wheelCollider.rpm * 60f / 1000f) * Time.deltaTime;
			float rotationInRadians = distanceTraveled / wheelCollider.radius;
			float rotationInDegrees = rotationInRadians * Mathf.Rad2Deg;
			transform.RotateAround(transform.TransformPoint(center), transform.right, rotationInDegrees);
			//Vector3 p = new Vector3 (0, bounds [0].y, bounds [0].x);
			//Debug.DrawLine (transform.position, transform.TransformPoint(center), Color.red);
		}
	}

	void FixedUpdate () {
	//	transform.position = wheelCollider.su
		//UpdateWheelHeight(this.transform, wheelCollider);
	}


	void UpdateWheelHeight(Transform wheelTransform, WheelCollider collider) {
		
		Vector3 localPosition = wheelTransform.localPosition;
		
		WheelHit hit = new WheelHit();
		
		// see if we have contact with ground
		
		if (collider.GetGroundHit(out hit)) {

			float hitY = collider.transform.InverseTransformPoint(hit.point).y;

			localPosition.y = hitY + collider.radius;

			//wheelCollider.GetComponent<ParticleSystem>().enableEmission = true;

		} else {
			
			// no contact with ground, just extend wheel position with suspension distance
			
			localPosition = Vector3.Lerp (localPosition, -Vector3.up * collider.suspensionDistance, .05f);

		}
		
		// actually update the position
		
		wheelTransform.localPosition = localPosition;

		wheelTransform.localRotation = Quaternion.Euler(0, collider.steerAngle, 90);
		
	}


}
