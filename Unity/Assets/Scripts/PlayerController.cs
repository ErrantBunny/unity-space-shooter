﻿using UnityEngine;
using System.Collections;

[System.Serializable]
public class Boundary {

	public float xMin, xMax, zMin, zMax;

}

public class PlayerController : MonoBehaviour {

	public float speed;
	public Boundary boundary;
	public float tilt;

	public GameObject shot;
	public Transform shotSpawn;
	public float fireDelay = 0.5f;
	private float nextFire = 0.0f;
	private bool isPaused;

	void Update() {
		if (isPaused) {
			return;
		}
		if (Input.GetButton("Fire1") && Time.time > nextFire) {
			nextFire = Time.time + fireDelay;
			Instantiate (shot, shotSpawn.position, shotSpawn.rotation);
			audio.Play ();
		}
	}

	void OnPauseGame() {
		isPaused = true;
	}

	void OnResumeGame() {
		isPaused = false;
	}

	void FixedUpdate() {
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");

		Vector3 movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);
		rigidbody.velocity = movement * speed;

		float xVel = Mathf.Clamp (rigidbody.position.x, boundary.xMin, boundary.xMax);
		float yVel = 0.0f;
		float zVel = Mathf.Clamp (rigidbody.position.z, boundary.zMin, boundary.zMax);
		rigidbody.position = new Vector3 (xVel, yVel, zVel);

		rigidbody.rotation = Quaternion.Euler (0.0f, 0.0f, -rigidbody.velocity.x * tilt);
	}
}
