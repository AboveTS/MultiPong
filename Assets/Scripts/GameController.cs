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

	[HideInInspector]
	public float hitAngle;

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

		CalculateHitAngle();
	}

	void Update() {
		
	}

	/**
		Instantiates a specified number 'n' of paddles. 'n-1' of these will be AI controlled, the remaining one will be assigned a player controller.
	*/
	private void SpawnPaddles(int paddleCount) {
		for (int i = 0; i < paddleCount; i++) {
			GameObject currentPaddle = Instantiate(paddlePrefab, transform);
			
			float theta = (i) * Mathf.PI * 2 / paddleCount;
			
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
		ball.transform.position = new Vector2(0, 0);
 
		float angle = Random.value * 2 * Mathf.PI;
		ball.GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Cos(angle) * initialBallVelocity, Mathf.Sin(angle) * initialBallVelocity);
	}

	private void DestroyPaddles() {
		foreach (GameObject paddle in GameObject.FindGameObjectsWithTag("Paddle")) {
			Destroy(paddle);
		}
	}

	private void OnBallCollision(Collision2D collision) {
		CalculateHitAngle();
	}

	/**
		Returns the angle between the +x axis and where the ball will collide with the arena.
		To prevent excessive recalculation, the return value is also stored into the variable "hitAngle".

		TODO: Vertical angle handling is not implemented!
	*/
	public float CalculateHitAngle() {
		// Sanitizes angle into a reasonable unsigned 0-2pi format
		Vector2 ballVelocity = ball.GetComponent<Rigidbody2D>().velocity;
		float ballAngle = MakeUnsignedAngle(Vector2.SignedAngle(Vector2.right, ballVelocity) * Mathf.Deg2Rad);

		// These equations can be derived by the intersection of the equation of a circle and a line representing the paddle and ball.
		float m = Mathf.Tan(ballAngle);
		float b = ball.transform.position.y - ball.transform.position.x * m;
		float fragmentA = -m * b;
		float fragmentB = Mathf.Sqrt( Mathf.Pow(paddleDistance, 2) * (m * m + 1) - b * b );
		float fragmentC = (m * m + 1);

		// The equations above are all fragments of the solution to a quadratic equation. Because of this, there will actually be two distinct
		// solutions - one that the ball is moving towards, and one it's moving away from. We calculate the first arbitrarily.
		float x = (fragmentA + fragmentB) / fragmentC;
		Vector2 intersection = new Vector2(x, m * x + b);

		// If adding the ball's velocity to a solution to the quadratic (what we think is the actual intersection) kicks the point outside the arena,
		// that means that it is actually on the correct side of the ball. (the ball is approaching it)
		if((intersection + ballVelocity).sqrMagnitude > Mathf.Pow(paddleDistance, 2)) {

		} else {
			// Our guess was incorrect, we actually need to use the other solution.
			//			   \/ changed from a plus to a minus
			x = (fragmentA - fragmentB) / fragmentC;
			intersection = new Vector2(x, m * x + b);
		}

		// Now that the correct intersection has been identified, we can calculate the angle to that point as a target for the AI.
		hitAngle = MakeUnsignedAngle(Mathf.Atan2(intersection.y, intersection.x));

		return hitAngle;
	}

	/**
		Takes a signed angle in radians (in the range [-2PI, 2PI]) and returns an unsigned, modulated value in the range [0, 2PI].
		These are equivalent, but sometimes math plays nicer with unsigned angles.
	*/
	private float MakeUnsignedAngle(float radians) {
		return (radians + Mathf.PI * 2) % (Mathf.PI * 2);
	}
}
