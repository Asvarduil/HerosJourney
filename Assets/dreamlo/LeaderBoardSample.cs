using UnityEngine;
using System.Collections;

public class LeaderBoardSample : MonoBehaviour {
	float startTime = 10.0f;
	float timeLeft = 0.0f;
	
	int totalScore = 0;
	string playerName = "";
	
	enum gameState {
		waiting,
		running,
		enterscore,
		leaderboard
	};
	
	gameState gs;
	
	
	// Reference to the dreamloLeaderboard prefab in the scene
	
	DreamloLeaderBoard dl;
	
	void Start () 
	{
		// get the reference here...
		this.dl = DreamloLeaderBoard.GetSceneDreamloLeaderboard();
		
		this.timeLeft = startTime;
		this.gs = gameState.waiting;
	}
	
	void Update () 
	{
		if (this.gs == gameState.running)
		{
			timeLeft = Mathf.Clamp(timeLeft - Time.deltaTime, 0, this.startTime);
			if (timeLeft == 0)
			{
				this.gs = gameState.enterscore;
			}
		}
	}
	
	void OnGUI()
	{
		GUILayoutOption[] width200 = new GUILayoutOption[] {GUILayout.Width(200)};
		
		GUILayout.BeginArea(new Rect((Screen.width / 2) - 200, (Screen.height / 2) - 100, 400, 200));
		
		GUILayout.BeginVertical();
		GUILayout.Label("Time Left:" + this.timeLeft.ToString("0.000"));
		if (this.gs == gameState.waiting || this.gs == gameState.running)
		{
			if (GUILayout.Button("Click me as much as you can in " + this.startTime.ToString("0") + " seconds!"))
			{
				this.totalScore++;
				this.gs = gameState.running;
			}
			
			GUILayout.Label("Total Score: " + this.totalScore.ToString());
		}
		
		
		
		if (this.gs == gameState.enterscore)
		{
			GUILayout.Label("Total Score: " + this.totalScore.ToString());
			GUILayout.BeginHorizontal();
			GUILayout.Label("Your Name: ");
			this.playerName = GUILayout.TextField(this.playerName, width200);
			
			if (GUILayout.Button("Save Score"))
			{
				// add the score...
				dl.AddScore(this.playerName, totalScore);
				
				this.gs = gameState.leaderboard;
			}
			GUILayout.EndHorizontal();
		}
		
		if (this.gs == gameState.leaderboard)
		{
			GUILayout.Label("High Scores:");
			DreamloLeaderBoard.Score[] scoreList = dl.ToScoreArray();
			
			if (scoreList == null) 
			{
				GUILayout.Label("(loading...)");
			} 
			else 
			{
				foreach (DreamloLeaderBoard.Score currentScore in scoreList)
				{
					GUILayout.BeginHorizontal();
					GUILayout.Label(currentScore.PlayerName, width200);
					GUILayout.Label(currentScore.PlayerScore.ToString(), width200);
					GUILayout.EndHorizontal();
				}
			}
		}
		
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}
}
