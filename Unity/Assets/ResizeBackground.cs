using UnityEngine;
using System.Collections;

public class ResizeBackground : MonoBehaviour {
	
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
		
		GameObject gameBoundaryObject = GameObject.FindWithTag ("Background");
		if (gameBoundaryObject != null) {
			boundaryTransform = gameBoundaryObject.GetComponent<Transform>();
		}
		if (boundaryTransform == null) {
			Debug.Log ("Cannot find 'Background' transform");
		}
	}
	
	void Update() {
		Boundary boundary = getBackgroundBoundary();
		float x = boundary.xMax - boundary.xMin;
		float y = boundary.zMax - boundary.zMin;
		float z = 1.0f;
		boundaryTransform.localScale = new Vector3(x, y, z);
	}
	
	private Boundary getBackgroundBoundary() {
		Boundary value = new Boundary ();
		float xMin = gameController.boundary.xMin;
		float xMax = gameController.boundary.xMax;

		float width = xMax - xMin;
		float scale = width / boundaryTransform.localScale.x;
		float height = scale * boundaryTransform.localScale.y;

		Camera cam = Camera.main;
		if (cam != null) {
			float cameraViewHeight = cam.orthographicSize * 2.0f;
			if (height < cameraViewHeight) {
				scale = cameraViewHeight / height;
				height *= scale;
				width *= scale;
			}
		} else {
			Debug.Log ("No camera");
		}

		value.zMin = -height / 2.0f;
		value.zMax = height / 2.0f;

		value.xMin = -width / 2.0f;
		value.xMax = width / 2.0f;
		return value;
	}
}
