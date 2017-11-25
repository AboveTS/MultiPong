using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour {
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void DisplayScreen(GameObject targetScreen) {
		foreach (GameObject screen in GameObject.FindGameObjectsWithTag("Screen")) {
			screen.SetActive(false);
		}

		targetScreen.SetActive(true);
	}
}
