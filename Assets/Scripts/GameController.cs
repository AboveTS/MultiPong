using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
	public GameObject paddlePrefab;
	public GameObject ballPrefab;
	public int paddleDistance;
	public float initialBallVelocity;

	private int paddleCount = 10;

	private GameObject playerPaddle;
	private GameObject ball;

	void Start() {
		SpawnPaddles (paddleCount);

		ball = Instantiate(ballPrefab, transform);
		ResetBall();
	}

	void SpawnPaddles (int playerCount) {
		for (int i = 0; i < playerCount; i++) {
			GameObject currentPaddle = Instantiate(paddlePrefab, transform);
			
			float theta = i * Mathf.PI * 2 / playerCount;
			
			currentPaddle.transform.Translate(Mathf.Cos(theta) * paddleDistance , Mathf.Sin(theta) * paddleDistance, 0);
			currentPaddle.transform.Rotate(0, 0, Mathf.Rad2Deg * theta + 90);

			// Only the first paddle should have a player controller, the rest can be AI controlled
			currentPaddle.AddComponent( i == 0 ? typeof(PlayerPaddleController) : typeof(AIPaddleController));
		}
	}

	void ResetBall() {
		transform.position = new Vector2(0, 0);

		float angle = Random.value * 2 * Mathf.PI;
		ball.GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Cos(angle) * initialBallVelocity, Mathf.Sin(angle) * initialBallVelocity);
	}

	void DestroyPaddles() {
		foreach (GameObject paddle in GameObject.FindGameObjectsWithTag("Paddle")) {
			Destroy(paddle);
		}
	}
	
	void Update() {

	}

	public float GetBallAngle() {
		return Vector2.SignedAngle((Vector2) ball.GetComponent<Rigidbody2D>().velocity, Vector2.right);
	}
}
