using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
	public GameObject paddlePrefab;
	public GameObject ballPrefab;
	public int paddleDistance;

	private GameObject playerPaddle;

	void Start () {
		Spawn(10);
	}

	void Spawn (int playerCount) {
		for(int i = 0; i < playerCount; i++) {
			GameObject currentPaddle = Instantiate(paddlePrefab, transform);
			
			float theta = i * Mathf.PI * 2 / playerCount;
			
			currentPaddle.transform.Translate(Mathf.Cos(theta) * paddleDistance , Mathf.Sin(theta) * paddleDistance, 0);
			currentPaddle.transform.Rotate(0, 0, Mathf.Rad2Deg * theta + 90);
		}
	}

	void DestroyAll() {

	}
	
	void Update () {
		
	}
}
