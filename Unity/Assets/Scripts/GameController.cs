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
	}

	void Update() {
		if (Input.GetKeyDown (KeyCode.Escape)) {
			isPaused = !isPaused;
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
		if (restart) {
			if (Input.GetKeyDown(KeyCode.R)) {
				Application.LoadLevel (Application.loadedLevel);
			}
		}
	}

	void OnPauseGame() {
		Time.timeScale = 0.0f;
		audio.Pause();
		gameOverText.text = "Paused";
	}

	void OnResumeGame() {
		Time.timeScale = 1.0f;
		audio.Play();
		gameOverText.text = "";
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
				restartText.text = "Press 'R' to Restart";
				restart = true;
				break;
			}
		}
	}

	void UpdateScore() {
		scoreText.text = "Score: " + score;
	}
}
