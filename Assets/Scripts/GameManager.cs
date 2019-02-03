using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

public enum GameState{
	Paused,
	Running
}

public class GameManager : MonoBehaviour {
	public static GameManager instance;

	public GUIManager guiManager;
	public TargetManager targetManager;
	public GameObject player;
	public Text scoreText;
	public int countdownMinutes = 3;
	public Text countdownText;
	public int winningScore = 2000;

	[HideInInspector]
	public GameState gameState;

	private int score;

	void Awake(){
		//keep screen on
		Screen.sleepTimeout = SleepTimeout.NeverSleep;

		//set game manager instance to be accessible from everywhere
		if (GameManager.instance == null) {
			GameManager.instance = this;
		}
	}

	void Start(){
		Init ();
		//StartNewGame ();
	}

	public void StartNewGame(){
		//reset score
		ResetScore();

		//enable target manager
		targetManager.enabled = true;

		//start countdown
		StartCoroutine(Countdown());
	}

	public void EndGame(){
		//disable target manager
		targetManager.enabled = false;
		//stop coroutines
		StopAllCoroutines();
	}

	private void Init(){

		//Init targets
		targetManager.InitTargets();
	}

	public void ExitApplication(){
		#if UNITY_EDITOR
		EditorApplication.isPlaying = false;
		#else
		Application.Quit();
		#endif
	}

	public void AddPoints(int points){
		//add points to total score
		score += points;
		//check if player has reached the winning score
		if(score >= winningScore){
			StartCoroutine (WinGame ());
		}
		//update ui
		UpdateScoreText ();
	}

	private void UpdateScoreText(){
		//update score ui text
		scoreText.text = score.ToString();
	}

	private void ResetScore(){
		//reset score to zero
		score = 0;
		UpdateScoreText ();
	}

	private IEnumerator Countdown(){
		int i = countdownMinutes * 60;
		WaitForSeconds tick = new WaitForSeconds (1);

		int minutes;
		int seconds;

		//countdown timer
		while(i >= 0){
			minutes = i / 60;
			seconds = i - (minutes * 60);
			countdownText.text = String.Format ("{0:00}:{1:00}", minutes, seconds);
			yield return tick;
			i--;
		}

		//game over
		EndGame();
		guiManager.ShowGameOver();
	}

	private IEnumerator WinGame(){
		guiManager.ShowVictory ();
		yield return new WaitForSeconds (1);
		EndGame ();
	}
}
