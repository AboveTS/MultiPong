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

	void Update() {

	}

	/**
		Instantiates a specified number 'n' of paddles. 'n-1' of these will be AI controlled, the remaining one will be assigned a player controller.
	*/
	private void SpawnPaddles(int playerCount) {
		for (int i = 0; i < playerCount; i++) {
			GameObject currentPaddle = Instantiate(paddlePrefab, transform);
			
			float theta = (i + 0.5f) * Mathf.PI * 2 / playerCount;
			
			currentPaddle.transform.Translate(Mathf.Cos(theta) * paddleDistance, Mathf.Sin(theta) * paddleDistance, 0);
			currentPaddle.transform.Rotate(0, 0, Mathf.Rad2Deg * theta + 90);

			// Only the first paddle should have a player controller, the rest can be AI controlled
			
			if(i == 0) {
				currentPaddle.AddComponent(typeof(PlayerPaddleController));
			} else {
				AIPaddleController ai = currentPaddle.AddComponent(typeof(AIPaddleController)) as AIPaddleController;
				ai.paddleID = i; // i is local so it must be directly provided to the AI (there are likely cleaner solutions but it's just a single variable so I'll let it slide)
			}
		}
	}

	/**
		Returns the ball to the arena's center and provides an 
	*/
	private void ResetBall() {
		transform.position = new Vector2(0, 0);

		float angle = Random.value * 2 * Mathf.PI;
		ball.GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Cos(angle) * initialBallVelocity, Mathf.Sin(angle) * initialBallVelocity);
	}

	private void DestroyPaddles() {
		foreach (GameObject paddle in GameObject.FindGameObjectsWithTag("Paddle")) {
			Destroy(paddle);
		}
	}
	
	/**
		Returns the angle between the +X axis and the ball's velocity.
	*/
	public float GetBallAngle() {
		return Vector2.SignedAngle((Vector2) ball.GetComponent<Rigidbody2D>().velocity, Vector2.right);
	}

	/**
		Getter method for the player count.
	*/
	public int GetPaddleCount() {
		return paddleCount;
	}
}
