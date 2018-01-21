using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPaddleController : MonoBehaviour {
	private GameController master;

	public int paddleID; // Assigned externally, but safe to access from Start() and onward.
	private float angle, minAngle, maxAngle; // All stored as radian values
	
	void Start () {
		master = transform.parent.gameObject.GetComponent<GameController>() as GameController;
		
		float range = 2 * Mathf.PI / master.GetPaddleCount();

		minAngle = paddleID * range;		
		angle = (paddleID + .5f) * range;
		maxAngle = (paddleID + 1) * range;
	}

	void Update () {		
		float ballAngle = master.GetBallAngle(); // Radians
		float da = (ballAngle - angle);
			
		Move(da / 50f);

		if(paddleID == 9) {
			Debug.Log("Ball Angle: " + (ballAngle * Mathf.Rad2Deg) + " | Paddle Angle: " + (angle * Mathf.Rad2Deg) + " | Delta Angle: " + (da * Mathf.Rad2Deg));
		}
	}

	/**
		Moves the paddle by a specified delta angle
	*/
	private void Move(float da) {
		float newAngle = angle + da;

		if(newAngle > maxAngle) newAngle = maxAngle;
		if(newAngle < minAngle) newAngle = minAngle;

		angle = newAngle;

		transform.position = new Vector3(Mathf.Cos(angle) * master.paddleDistance, Mathf.Sin(angle) * master.paddleDistance, 0);
		transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * angle + 90);
	}
}
