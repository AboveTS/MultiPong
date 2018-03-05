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
	public float ballAngle;
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

		CalculateCollisionAngle();
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
		CalculateCollisionAngle();
	}

	/** Returns the angle between the +x axis and the point where the ball will intersect with the arena boundary.
		A great place to optimize, as this should be possible with a tricky formula in O(1) time.

		The crappy workaround here uses an iterative depth-bounded binary search to get a decent approximation for the collision coordinate, and then finds the angle between it and RIGHT.
	*/
	public void CalculateCollisionAngle() {
		const int depth = 10;
		float diametricMultiplier = 0.5f;
		float velocityAngle = Vector2.SignedAngle(ball.GetComponent<Rigidbody2D>().velocity, Vector2.right) * Mathf.Deg2Rad;
		
		Vector2 velocityBasisVector = new Vector2(Mathf.Cos(velocityAngle), Mathf.Sin(velocityAngle));
		Vector2 coordinate = new Vector2(0, 0);

		for(int i = 2; i < depth; i++) {
			// (multiplier * diameter) * [cos angle, sin angle] + ball position
			coordinate = diametricMultiplier * 2 * paddleDistance * velocityBasisVector + (Vector2) ball.transform.position;

			float delta = 1 / Mathf.Pow(2, i);
			// If ball is inside arena, move the point further along
			if( Mathf.Sqrt(Mathf.Pow(coordinate.x, 2) + Mathf.Pow(coordinate.y, 2)) <= paddleDistance) {
				diametricMultiplier += delta;
			} else {
			// If ball is outside the arena, move the point inwards
				diametricMultiplier -= delta;
			}
		}

		// Coordinate now contains a rough approximation of the intersection

		ballAngle = -Mathf.Atan2(coordinate.y, coordinate.x);
		Debug.Log(ballAngle);
	}
}
