using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPaddleController : MonoBehaviour {
	private GameController master;

	public int paddleID; // Assigned externally, but safe to access from Start() and onward.
	private float angle, minAngle, maxAngle; // All stored as radian values
	
	void Start () {
		master = transform.parent.gameObject.GetComponent<GameController>() as GameController;
		
		float range = 2 * Mathf.PI / master.paddleCount;

		minAngle = paddleID * range;		
		angle = (paddleID + .5f) * range;
		maxAngle = (paddleID + 1) * range;
	}

	void Update () {
		float da = (master.hitAngle - angle);
			
		Move(da * master.AIDampeningFactor);
	}

	/**
		Moves the paddle by a specified delta angle, while still constraining the paddle within it's boundaries.
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
