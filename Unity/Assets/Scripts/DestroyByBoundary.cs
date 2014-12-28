using UnityEngine;
using System.Collections;

public class DestroyByBoundary : MonoBehaviour {

	private GameController gameController;
	private Transform boundaryTransform;
	
	void Start() {
		GameObject gameControllerObject = GameObject.FindWithTag ("GameController");
		if (gameControllerObject != null) {
			gameController = gameControllerObject.GetComponent<GameController>();
		}
		if (gameController == null) {
			Debug.Log ("Cannot find 'GameController' script");
		}

		GameObject gameBoundaryObject = GameObject.FindWithTag ("Boundary");
		if (gameBoundaryObject != null) {
			boundaryTransform = gameBoundaryObject.GetComponent<Transform>();
		}
		if (boundaryTransform == null) {
			Debug.Log ("Cannot find 'Boundary' transform");
		}
	}
	
	void Update() {
		Boundary boundary = getDestroyBoundary();
		float x = boundary.xMax - boundary.xMin;
		float y = 1.0f;
		float z = boundary.zMax - boundary.xMin;
		boundaryTransform.localScale.Set (x, y, z);
	}

	private Boundary getDestroyBoundary() {
		Boundary value = new Boundary ();
		value.xMin = gameController.boundary.xMin - 1.0f;
		value.xMax = gameController.boundary.xMax + 1.0f;
		value.zMin = gameController.boundary.zMin - 1.0f;
		value.zMax = gameController.boundary.zMax + 1.0f;
		return value;
	}

	void OnTriggerExit(Collider other) {
		Destroy(other.gameObject);
	}
	
}
