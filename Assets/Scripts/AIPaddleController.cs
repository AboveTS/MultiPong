using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPaddleController : MonoBehaviour {
	private GameController master;
	
	void Start () {
		master = transform.parent.gameObject.GetComponent<GameController>() as GameController;
		Debug.Log(master.GetBallAngle());
	}
	
	// Update is called once per frame
	void Update () {
		  
	}
}
