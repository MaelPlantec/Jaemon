using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

	public Transform target;

	public float smoothSpeed = 0.125f;
	public Vector3 offset;

	void Update()
	{
		Vector3 desiredPosition = target.position + offset;
		//Vector3 desiredPosition = Utils.PixelPos(target.position + offset, Camera.main);
		//Vector3 smoothedPosition = Vector3.Smooth(transform.position, desiredPosition, smoothSpeed);
		Vector3 smoothedPosition = new Vector3(
			Mathf.SmoothStep(transform.position.x, desiredPosition.x, smoothSpeed), 
			Mathf.SmoothStep(transform.position.y, desiredPosition.y, smoothSpeed),
			Mathf.SmoothStep(transform.position.z, desiredPosition.z, smoothSpeed));

		if (!Utils.Equals(transform.position, desiredPosition, 0.1f))
		{
			transform.position = smoothedPosition;
		}
	}

}
