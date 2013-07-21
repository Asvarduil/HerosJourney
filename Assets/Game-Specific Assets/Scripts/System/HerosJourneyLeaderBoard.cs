using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

/*
<style type="text/css">
.dreamloLBTable { border-collapse:collapse;text-align:center;width: 200px; }
.dreamloLBTable th { border-bottom: 1px solid #EEEEEE;font-weight:bold;margin:0;padding:4px; }
.dreamloLBTable td { border-bottom: 1px solid#EEEEEE;margin:0;padding:4px; }
</style>

<script src="http://dreamlo.com/lb/51a7e42e17f5131c7c4ff7c6/js" type="text/javascript"></script>

 */

/// <summary>
/// Leader board for Game Win
/// </summary>
public class HerosJourneyLeaderBoard : MonoBehaviour 
{
	#region Structures
	
	/// <summary>
	/// Structure for storing aspects of a player's score.
	/// </summary>
	private struct ScoreStructure
	{
		public string PlayerName; // Name of the player, as given on the Score Submit screen.
		public string PlayerMode; // Achievement the player was trying for.
		public int TimeTicks;     // Time in seconds to clear the game.
		public int Completion;    // 0 - 100, Heart Containers, Plot Advancement, Castle Items
	}
	
	#endregion Structures
	
	#region Variables / Properties
	
	public bool DebugMode = true;
	
	private Ambassador _ambassador;
	
	private DreamloLeaderBoard _leaderBoard;
	private List<ScoreStructure> _allScores = new List<ScoreStructure>();
	
	private static readonly string _specialUrl = "http://dreamlo.com/lb/nXKN2_NBe0G_mwg2nRs0gQ65nM0DWiAkSJSR12jv0AhQ";
	private static readonly string _privateCode = "nXKN2_NBe0G_mwg2nRs0gQ65nM0DWiAkSJSR12jv0AhQ";
	private static readonly string _publicCode = "51a7e42e17f5131c7c4ff7c6";
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	void Start() 
	{
		_ambassador = Ambassador.Instance;
		
		// This url must remain a secret, to prevent 'enhancements' to players' scores.
		_leaderBoard = DreamloLeaderBoard.GetSceneDreamloLeaderboard();
		_leaderBoard.WebServiceUrl = _specialUrl;
		_leaderBoard.PrivateCode = _privateCode;
		_leaderBoard.PublicCode = _publicCode;
	}
	
	void OnGUI()
	{
	}
	
	void Update() 
	{
	}
	
	#endregion Engine Hooks
	
	#region Methods
	
	public void AddScore(string playerName, int playTimeTicks, string mode = "Normal")
	{
		_leaderBoard.AddScore(playerName + ":" + mode, playTimeTicks);
	}
	
	public void AcquireLeaderBoard(string mode = "Normal")
	{
		_allScores = new List<ScoreStructure>();
		_leaderBoard.LoadScores();
		
		List<DreamloLeaderBoard.Score> allScores = _leaderBoard.ToScoreArray().ToList();
		foreach(var current in allScores)
		{
			string[] playerNameParts = current.PlayerName.Split(':');
			if(playerNameParts.Length != 2)
			{
				if(DebugMode)
					Debug.LogWarning("Expected player name parts to be divided by a :!" + Environment.NewLine
						             + "Retrieved string: " + current.PlayerName);
				
				continue;
			}
			
			ScoreStructure newScore = new ScoreStructure 
			{
				PlayerName = playerNameParts[0],
				PlayerMode = playerNameParts[1],
				TimeTicks = current.PlayTime,
				Completion = current.PlayerScore
			};
			
			_allScores.Add(newScore);
		}
	}
	
	#endregion Methods
}
