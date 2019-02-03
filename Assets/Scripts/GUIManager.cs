using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GUIManager : MonoBehaviour {

	public GameObject splashScreen;
	public GameObject ingame;
	public GameObject gameOver;
	public GameObject playerCrosshair;
	public GameObject victory;

	private GameManager gameManager;

	void Start(){
		gameManager = GameManager.instance;
		ShowSplashScreen ();
	}

	public void ShowSplashScreen(){
		splashScreen.SetActive (true);
		ingame.SetActive (false);
		victory.SetActive (false);
		GameManager.instance.gameState = GameState.Paused;
		//gameOver.SetActive (false);
	}

	public void ShowIngameGUI(){
		splashScreen.SetActive (false);
		ingame.SetActive (true);
		gameOver.SetActive (false);
		playerCrosshair.SetActive (true);

		gameManager.gameState = GameState.Running;
	}

	public void ShowGameOver(){
		gameOver.SetActive (true);
		playerCrosshair.SetActive (false);

		gameManager.gameState = GameState.Paused;
	}

	public void ShowVictory(){
		ingame.SetActive (false);
		victory.SetActive (true);

		gameManager.gameState = GameState.Paused;
	}

	public void GotoMainMenu(){
		SceneManager.LoadScene ("03");
	}

}
