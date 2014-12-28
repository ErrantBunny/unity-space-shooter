using UnityEngine;
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

	private GameController gameController;
	
	void Start() {
		GameObject gameControllerObject = GameObject.FindWithTag ("GameController");
		if (gameControllerObject != null) {
			gameController = gameControllerObject.GetComponent<GameController>();
		}
		if (gameController == null) {
			Debug.Log ("Cannot find 'GameController' script");
		}
		nextFire = 0.0f;
	}

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

#if UNITY_ANDROID
		if (isTouchAction() && Camera.current != null) {
			Vector3 target = getTouchTargetPosition();
			moveHorizontal = Mathf.Clamp (target.x - rigidbody.position.x, -1.0f, 1.0f);
			moveVertical = Mathf.Clamp (target.z - rigidbody.position.z, -1.0f, 1.0f);
		}
#endif

		Vector3 movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);
		rigidbody.velocity = movement * speed;

		float xVel = Mathf.Clamp (rigidbody.position.x, boundary.xMin, boundary.xMax);
		float yVel = 0.0f;
		float zVel = Mathf.Clamp (rigidbody.position.z, boundary.zMin, boundary.zMax);
		rigidbody.position = new Vector3 (xVel, yVel, zVel);

		rigidbody.rotation = Quaternion.Euler (0.0f, 0.0f, -rigidbody.velocity.x * tilt);
	}

	private bool isTouchAction() {
		return Input.touchCount > 0 &&
			(Input.GetTouch (0).phase == TouchPhase.Began ||
			 Input.GetTouch (0).phase == TouchPhase.Moved ||
			 Input.GetTouch (0).phase == TouchPhase.Stationary);
	}

	private Vector3 getTouchTargetPosition() {
		float x = Input.GetTouch (0).position.x;
		float y = Input.GetTouch (0).position.y;
		Debug.Log ("getTouchTargetPosition() x: " + x + " y: " + y);
		Vector3 screenVector = new Vector3 (x, y, 0.0f);
		Vector3 targetVector = Camera.current.ScreenToWorldPoint (screenVector);
		return targetVector;
	}
	
	private float getMoveVerticalTouch() {
		float diff = Input.GetTouch (0).position.y - rigidbody.position.y;
		return Mathf.Sign(diff);
	}
}
