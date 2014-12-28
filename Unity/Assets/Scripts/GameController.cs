using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	public GameObject hazard;
	public Vector3 spawnValues;
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

#if UNITY_ANDROID
		restartMessage = "Touch to Restart";
#endif
	}

	void Update() {
		if (Input.GetKeyDown (KeyCode.Escape)) {
			UpdatePause(!isPaused);
		}
		if (restart) {
			if (isRestartActionEnabled()) {
				UpdatePause (false);
				Application.LoadLevel (Application.loadedLevel);
			}
		}
	}

	private bool isRestartActionEnabled() {
		return (Input.GetKeyDown (KeyCode.R) ||
		       (Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Began));
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
				Vector3 spawnPosition = new Vector3 (Random.Range (-spawnValues.x, spawnValues.x), spawnValues.y, spawnValues.z);
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
}
