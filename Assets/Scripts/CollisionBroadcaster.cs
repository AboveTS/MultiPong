using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionBroadcaster : MonoBehaviour {
	public delegate void CollisionDelegate(Collision2D collision); // This unfortunately cannot be static
	public CollisionDelegate collisionDelegate;

	void OnCollisionEnter2D(Collision2D collision) {
		if(collisionDelegate != null) collisionDelegate(collision);
	}
}
