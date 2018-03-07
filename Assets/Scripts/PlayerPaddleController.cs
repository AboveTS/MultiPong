using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPaddleController : MonoBehaviour {
	private GameController master;
	private float angle = 0;
	private float range;
	
	void Start () {
		master = transform.parent.gameObject.GetComponent<GameController>() as GameController;

		range = 2 * Mathf.PI / master.paddleCount;
	}
	
	// Update is called once per frame
	void Update () {
		UpdatePosition();
	}

	private void UpdatePosition() {
		Vector2 mouse = Input.mousePosition;
		Vector2 position =  Camera.main.ScreenToWorldPoint(new Vector3(mouse.x, mouse.y, 0));

		float angle = Mathf.Atan2(position.y, position.x);

		if(angle < 0) angle = 0;
		if(angle > range) angle = range;

		transform.position = new Vector3(Mathf.Cos(angle) * master.paddleDistance, Mathf.Sin(angle) * master.paddleDistance, 0);
		transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * angle + 90);
	}
}
