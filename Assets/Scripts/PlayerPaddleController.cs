using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPaddleController : MonoBehaviour {
	private GameController master;
	
	void Start () {
		master = transform.parent.gameObject.GetComponent<GameController>() as GameController;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
