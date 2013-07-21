using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*  Programmer's Notes:
    -----------------------------------------------------------------------------------------------------
	A player named Carmine got a score of 100. If the same name is added twice, we use the higher score.
	http://dreamlo.com/lb/(your super secret very long code)/add/Carmine/100

	A player named Carmine got a score of 1000 in 90 seconds.
	http://dreamlo.com/lb/(your super secret very long code)/add/Carmine/1000/90
	
	A player named Carmine got a score of 1000 in 90 seconds and is Awesome.
	http://dreamlo.com/lb/(your super secret very long code)/add/Carmine/1000/90/Awesome
 */

public class DreamloLeaderBoard : MonoBehaviour 
{
	#region Structures
	
	public struct Score {
		public string PlayerName;
		public int PlayerScore;
		public int PlayTime;
		public string DescriptionText;
	}
	
	#endregion Structures
	
	#region Variables / Properties
	
	public string WebServiceUrl = "http://dreamlo.com/lb/";
	public string PrivateCode = string.Empty;
	public string PublicCode = string.Empty;
	public string HighScores = string.Empty;
	
	#endregion Variables / Properties
	
	#region Methods
	
	public static DreamloLeaderBoard GetSceneDreamloLeaderboard()
	{
		return (DreamloLeaderBoard) FindObjectOfType(typeof(DreamloLeaderBoard));
	}
	
	public void AddScore(string playerName, int totalScore)
	{
		StartCoroutine(AddScoreWithPipe(playerName, totalScore));
	}
	
	public void AddScore(string playerName, int totalScore, int totalSeconds)
	{
		StartCoroutine(AddScoreWithPipe(playerName, totalScore, totalSeconds));
	}
	
	public void AddScore(string playerName, int totalScore, int totalSeconds, string shortText)
	{
		StartCoroutine(AddScoreWithPipe(playerName, totalScore, totalSeconds, shortText));
	}
	
	// This function saves a trip to the server. Adds the score and retrieves results in one trip.
	IEnumerator AddScoreWithPipe(string playerName, int totalScore)
	{
		playerName = Clean(playerName);
		
		WWW www = new WWW(WebServiceUrl + PrivateCode + "/add-pipe/" + WWW.EscapeURL(playerName) + "/" + totalScore.ToString());
		yield return www;
		HighScores = www.text;
	}
	
	IEnumerator AddScoreWithPipe(string playerName, int totalScore, int totalSeconds)
	{
		playerName = Clean(playerName);
		
		WWW www = new WWW(WebServiceUrl + PrivateCode + "/add-pipe/" + WWW.EscapeURL(playerName) + "/" + totalScore.ToString()+ "/" + totalSeconds.ToString());
		yield return www;
		HighScores = www.text;
	}
	
	IEnumerator AddScoreWithPipe(string playerName, int totalScore, int totalSeconds, string shortText)
	{
		playerName = Clean(playerName);
		shortText = Clean(shortText);
		
		WWW www = new WWW(WebServiceUrl + PrivateCode + "/add-pipe/" + WWW.EscapeURL(playerName) + "/" + totalScore.ToString() + "/" + totalSeconds.ToString()+ "/" + shortText);
		yield return www;
		HighScores = www.text;
	}
	
	IEnumerator GetScores()
	{
		HighScores = string.Empty;
		WWW www = new WWW(WebServiceUrl +  PublicCode  + "/pipe");
		yield return www;
		
		HighScores = www.text;
	}
	
	public void LoadScores()
	{
		StartCoroutine(GetScores());
	}
	
	public string[] ToStringArray()
	{
		if (string.IsNullOrEmpty(HighScores)) 
			return null;
		
		string[] rows = this.HighScores.Split(new char[] {'\n'}, System.StringSplitOptions.RemoveEmptyEntries);
		return rows;
	}
	
	public Score[] ToScoreArray()
	{
		string[] rows = ToStringArray();
		
		if (rows == null) 
			return null;
		
		if (rows.Length <= 0)
			return null;
		
		int rowcount = rows.Length;
		Score[] scoreList = new Score[rowcount];
		
		for (int i = 0; i < rowcount; i++)
		{
			string[] values = rows[i].Split(new char[] {'|'}, System.StringSplitOptions.RemoveEmptyEntries);
			
			Score current = new Score
			{
				PlayerName = values[0],
				PlayerScore = 0,
				PlayTime = 0,
				DescriptionText = string.Empty
			};

			if (values.Length > 1) 
				current.PlayerScore = CheckInt(values[1]);
			
			if (values.Length > 2) 
				current.PlayTime = CheckInt(values[2]);
			
			if (values.Length > 3) 
				current.DescriptionText = values[3];
			
			scoreList[i] = current;
		}
		
		return scoreList;
	}
	
	private string Clean(string s)
	{
		s = s.Replace("/", "");
		s = s.Replace("|", "");
		return s;
	}
	
	private int CheckInt(string s)
	{
		int x = 0;
		int.TryParse(s, out x);
		
		return x;
	}
	
	#endregion Methods
}
