using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[System.Serializable]
public class Boundary {
	
	public float xMin, xMax, zMin, zMax;
	
}

public class GameController : MonoBehaviour {

	public Boundary boundary;

	public GameObject hazard;
	public int hazardCount;
	public float spawnWaitSeconds;
	public float startWaitSeconds;
	public float waveWaitSeconds;

	public Text scoreText;
	public Text restartText;
	public Text gameOverText;
	public Text pausedText;
	private int score;
	private bool restart;
	private bool gameOver;

	private bool isPaused = false;
	
	private string restartMessage = "Press 'R' to Restart";
	
	public void AddScore(int scoreValue) {
		score += scoreValue;
		UpdateScore ();
	}
	
	public void GameOver() {
		gameOver = true;
	}
	
	void Start() {
		score = 0;
		gameOver = false;
		restart = false;
		gameOverText.text = "";
		restartText.text = "";
		pausedText.text = "";
		UpdateScore ();
		StartCoroutine (SpawnWaves ());
		UpdatePause (false);
		UpdateBoundary ();
#if UNITY_ANDROID
		restartMessage = "Touch to Restart";
#endif
	}

	void Update() {

		UpdateBoundary ();

		if (isPaused) {
			if (Input.GetButtonDown("Cancel")) {
				UpdatePause (false);
			}
		} else {
			if (Input.GetButtonDown("Cancel")) {
				UpdatePause (true);
			}
		}
		if (restart) {
			if (isRestartActionEnabled()) {
				UpdatePause (false);
				Application.LoadLevel (Application.loadedLevel);
			}
		}
	}

	private bool isRestartActionEnabled() {
		if (Input.GetKeyDown (KeyCode.R)) {
			return true;
		}
		if (Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Began) {
			return true;
		}
#if UNITY_ANDROID
		if (Input.GetButton("Submit")) {
			return true;
		}
#endif
		return false;
	}

	private void UpdatePause(bool newPause) {
		if (newPause == isPaused) {
			return;
		}
		isPaused = newPause;
		if (isPaused) {
			Object[] objects = FindObjectsOfType (typeof(GameObject));
			foreach (GameObject go in objects) {
				go.SendMessage ("OnPauseGame", SendMessageOptions.DontRequireReceiver);
			}
		} else {
			Object[] objects = FindObjectsOfType (typeof(GameObject));
			foreach (GameObject go in objects) {
				go.SendMessage ("OnResumeGame", SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	void OnPauseGame() {
		Time.timeScale = 0.0f;
		audio.Pause();
		pausedText.text = "Paused";
	}

	void OnResumeGame() {
		Time.timeScale = 1.0f;
		audio.Play();
		pausedText.text = "";
	}

	IEnumerator SpawnWaves() {

		yield return new WaitForSeconds (startWaitSeconds);

		while (true) {
			for (int i = 0; i < hazardCount; i++) {
				Boundary hazardBoundary = getHazardStartBoundary();
				float xMin = hazardBoundary.xMin;
				float xMax = hazardBoundary.xMax;
				float zEntry = hazardBoundary.zMax;

				Vector3 spawnPosition = new Vector3 (Random.Range (xMin, xMax), 0.0f, zEntry);
				Quaternion spawnRotation = Quaternion.identity;
				Instantiate (hazard, spawnPosition, spawnRotation);
				yield return new WaitForSeconds(spawnWaitSeconds);
			}
			yield return new WaitForSeconds(waveWaitSeconds);

			if (gameOver) {
				restartText.text = restartMessage;
				restart = true;
				gameOverText.text = "Game Over";
				break;
			}
		}
	}

	void UpdateScore() {
		scoreText.text = "Score: " + score;
	}

	void UpdateBoundary() {
		Camera cam = Camera.main;
		if (cam == null) {
			return;
		}
		boundary.zMin = -cam.orthographicSize + cam.transform.position.z;
		boundary.zMax = cam.orthographicSize + cam.transform.position.z;
		float horizontalSize = cam.aspect * cam.orthographicSize;
		boundary.xMin = -horizontalSize + cam.transform.position.x;;
		boundary.xMax = horizontalSize + cam.transform.position.x;
	}

	private Vector3 getTouchTargetPosition() {
		float x = Input.GetTouch (0).position.x;
		float y = Input.GetTouch (0).position.y;
		Debug.Log ("getTouchTargetPosition() x: " + x + " y: " + y);
		Vector3 screenVector = new Vector3 (x, y, 0.0f);
		Vector3 targetVector = Camera.current.ScreenToWorldPoint (screenVector);
		return targetVector;
	}
	
	private Boundary getHazardStartBoundary() {
		Boundary value = new Boundary ();
		value.xMin = boundary.xMin + 1.0f;
		value.xMax = boundary.xMax - 1.0f;
		value.zMin = boundary.zMax + 2.0f;
		value.zMax = boundary.zMax + 2.0f;
		return value;
	}
}
