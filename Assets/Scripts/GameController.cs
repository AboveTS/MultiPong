using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
	public GameObject paddlePrefab;
	public GameObject ballPrefab;
	public int paddleDistance;
	public float initialBallVelocity;
	public float AIDampeningFactor = 1/50f;

	private int _paddleCount = 10;

	private float PaddleConvergenceAngle;

	private GameObject playerPaddle;
	private GameObject ball;

	/**
		Returns the angle between the +X axis and the ball's velocity.
	*/
	public float ballAngle {
		get {
			return Mathf.Deg2Rad * ((360 - Vector2.SignedAngle((Vector2) ball.GetComponent<Rigidbody2D>().velocity, Vector2.right)) % 360);
		}
	}

	/**
		Getter for the player count.
	*/
	public int paddleCount {
		get {
			return _paddleCount;
		}
	}

	void Start() {
		SpawnPaddles(_paddleCount);

		ball = Instantiate(ballPrefab, transform);
		CollisionBroadcaster cb = ball.AddComponent<CollisionBroadcaster>() as CollisionBroadcaster; // Subscribe to the OnCollision2D event of the ball to prevent excessive recomputation of the ball angle
		cb.collisionDelegate = OnBallCollision;
		ResetBall();
	}

	void Update() {
		
	}

	/**
		Instantiates a specified number 'n' of paddles. 'n-1' of these will be AI controlled, the remaining one will be assigned a player controller.
	*/
	private void SpawnPaddles(int paddleCount) {
		for (int i = 0; i < paddleCount; i++) {
			GameObject currentPaddle = Instantiate(paddlePrefab, transform);
			
			float theta = (i + 0.5f) * Mathf.PI * 2 / paddleCount;
			
			currentPaddle.transform.Translate(Mathf.Cos(theta) * paddleDistance, Mathf.Sin(theta) * paddleDistance, 0);
			currentPaddle.transform.Rotate(0, 0, Mathf.Rad2Deg * theta + 90);

			// Only the first paddle should have a player controller, the rest can be AI controlled
			
			if(i == 0) {
				currentPaddle.AddComponent<PlayerPaddleController>();
			} else {
				AIPaddleController ai = currentPaddle.AddComponent<AIPaddleController>();
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

	private void OnBallCollision(Collision2D collision) {
		// Compute partial solutions and combine (otherwise the math is crazy to read)
		float m = Mathf.Tan(ballAngle);
		float xb = ball.transform.position.x;
		float yb = ball.transform.position.y;

		float a = Mathf.Pow(m, 2) + 1;
		float b = 2 * m * (yb - m * xb);
		float c = Mathf.Pow(paddleDistance, 2) + m * xb * (2 * yb - m * xb) - Mathf.Pow(yb, 2);

		float x = (-b + Mathf.Sqrt( Mathf.Pow(b, 2) - 4 * a * c ) * ( Mathf.Sign(ball.GetComponent<Rigidbody2D>().velocity.x >= 0 ? 1 : -1 ) )) / (2 * a);

		Debug.Log(a + ", " + b + ", " + c);
	}
}
