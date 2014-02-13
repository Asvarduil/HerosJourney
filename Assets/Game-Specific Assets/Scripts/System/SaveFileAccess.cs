using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;

public class SaveFileAccess : MonoBehaviour 
{
	#region Variables / Properties

	public bool DebugMode = false;

	private GameObject _player;
	private Ambassador _ambassador;
	private TransitionManager _transition;
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	public void Start()
	{
		_ambassador = GetComponent<Ambassador>();
		_transition = GetComponent<TransitionManager>();
	}	
	
	#endregion Engine Hooks
	
	#region Access Methods

	public void SaveGameState()
	{
		switch(Application.platform)
		{
			case RuntimePlatform.WindowsEditor:
			case RuntimePlatform.OSXEditor:
			case RuntimePlatform.WindowsWebPlayer:
			case RuntimePlatform.OSXWebPlayer:
				SaveAmbassadorIntoPlayerConfig();
				break;

			case RuntimePlatform.WindowsPlayer:
			case RuntimePlatform.LinuxPlayer:
			case RuntimePlatform.OSXPlayer:
				SaveAmbassadorIntoFile();
				break;

			default:
				throw new Exception("(Save) Unexpected platform: " + Application.platform.ToString());
		}
	}

	public bool LoadGameState()
	{
		bool result = false;
		switch(Application.platform)
		{
			case RuntimePlatform.WindowsEditor:
			case RuntimePlatform.OSXEditor:
			case RuntimePlatform.WindowsWebPlayer:
			case RuntimePlatform.OSXWebPlayer:
				result = LoadAmbassadorFromPlayerConfig();
				break;

			case RuntimePlatform.WindowsPlayer:
			case RuntimePlatform.LinuxPlayer:
			case RuntimePlatform.OSXPlayer:
				result = LoadFileIntoAmbassador();
				break;

			default:
				throw new Exception("(Load) Unexpected platform: " + Application.platform.ToString());
		}

		return result;
	}

	public void SaveAmbassadorIntoPlayerConfig()
	{
		try
		{
			PlayerPrefs.SetString("Settings", FormatSettingsLine());
			PlayerPrefs.SetString("Scene", FormatSceneLine());
			PlayerPrefs.SetString("Health", FormatHealthLine());
			PlayerPrefs.SetString("Damage", FormatDamageLine());
			PlayerPrefs.SetString("Items", FormatItemLine());
			PlayerPrefs.SetString("Phase", FormatPhaseLine());

			PlayerPrefs.Save();
		}
		catch (PlayerPrefsException ppEx)
		{
			Debug.LogError("Cannot save > 1MB data to the Player Prefs!" + Environment.NewLine
			               + "Information:" + ppEx.Message);
		}

		if(DebugMode)
		{
			Debug.Log("PlayerConfig save complete.");
			DumpPlayerConfigToLog();
		}
	}

	public bool LoadAmbassadorFromPlayerConfig()
	{
		if(! PlayerPrefs.HasKey("Settings")
		   || !PlayerPrefs.HasKey("Scene")
		   || !PlayerPrefs.HasKey("Health")
		   || !PlayerPrefs.HasKey("Damage")
		   || !PlayerPrefs.HasKey("Items")
		   || !PlayerPrefs.HasKey("Phase"))
		{
			if(DebugMode)
				Debug.LogWarning("A data key does not exist!  This is probably a first-run scenario.");

			return false;
		}

		if(DebugMode)
		{
			Debug.Log("Loading from PlayerConfig...");
			DumpPlayerConfigToLog();
		}
		
		bool result = true;
		try
		{
			string settingsLine = PlayerPrefs.GetString("Settings");
			string sceneLine = PlayerPrefs.GetString("Scene");
			string healthLine = PlayerPrefs.GetString("Health");
			string damageLine = PlayerPrefs.GetString("Damage");
			string itemLine = PlayerPrefs.GetString("Items");
			string phaseLine = PlayerPrefs.GetString("Phase");

			result = SetupSettings(settingsLine)
				     && SetupSceneLoad(sceneLine)
					 && SetupHealth(healthLine)
					 && SetupDamage(damageLine)
					 && SetupObtainedItems(itemLine)
					 && SetupQuestPhases(phaseLine);
		}
		catch (Exception e)
		{
			Debug.LogWarning("Unhandled Exception Occurred!"
			                 + Environment.NewLine 
			                 + "Message:" + e.Message
			                 + Environment.NewLine 
			                 + "Stack Trace:" + e.StackTrace);
			result = false;
		}

		return result;
	}
	
	public void SaveAmbassadorIntoFile()
	{
		string settingsLine = FormatSettingsLine();
		string sceneLine = FormatSceneLine();
		string healthLine = FormatHealthLine();
		string damageLine = FormatDamageLine();
		string itemLine = FormatItemLine();
		string phaseLine = FormatPhaseLine();
		
		using(StreamWriter writer = new StreamWriter("hjcache.db", false))
		{
			try
			{
				writer.WriteLine("File:Hero's Journey Save File");
				writer.WriteLine(settingsLine);
				writer.WriteLine(sceneLine);
				writer.WriteLine(healthLine);
				writer.WriteLine(damageLine);
				writer.WriteLine(itemLine);		
				writer.WriteLine(phaseLine);
			}
			catch
			{
				throw;
			}
			finally
			{
				writer.Close();
			}
		}
	}
	
	public bool LoadFileIntoAmbassador()
	{
		bool result = true;
		using(StreamReader reader = new StreamReader("hjcache.db"))
		{
			try
			{
				string fileType = reader.ReadLine();
				string settingsLine = reader.ReadLine();
				string sceneLine = reader.ReadLine();
				string healthLine = reader.ReadLine();
				string damageLine = reader.ReadLine();				
				string itemLine = reader.ReadLine();
				string phaseLine = reader.ReadLine();
				
				result = ValidateSaveFileHeader(fileType)
						 && SetupSettings(settingsLine)
					     && SetupSceneLoad(sceneLine)
						 && SetupHealth(healthLine)
					     && SetupDamage(damageLine)
					     && SetupObtainedItems(itemLine)
					     && SetupQuestPhases(phaseLine);
			}
			catch (Exception e)
			{
				Debug.LogWarning("Unhandled Exception Occurred!"
					             + Environment.NewLine 
					             + "Message:" + e.Message
					             + Environment.NewLine 
					             + "Stack Trace:" + e.StackTrace);
				result = false;
			}
			finally
			{
				reader.Close();
			}
		}
		
		return result;
	}
	
	#endregion Access Methods
	
	#region Data Formatting Methods

	private string FormatSettingsLine()
	{
		StringBuilder builder = new StringBuilder("Settings:");

		builder.Append(Settings.graphicsLevel);
		builder.Append("|");
		builder.Append(Settings.soundEnabled);
		builder.Append("|");
		builder.Append(Settings.masterVolume.ToString("F2"));
		builder.Append("|");
		builder.Append(Settings.musVolume.ToString("F2"));
		builder.Append("|");
		builder.Append(Settings.sfxVolume.ToString("F2"));

		return builder.ToString();
	}
	
	private string FormatSceneLine()
	{
		StringBuilder builder = new StringBuilder("Scene:");
		
		// Format of line:
		// Scene:[Name]|x>y>z|rx>ry>rz
		builder.Append(Application.loadedLevelName);
		builder.Append("|");
		
		// Detect the player on the spot.
		_player = GameObject.FindGameObjectWithTag("Player");
		
		builder.Append(_player.transform.position.x.ToString("F2"));
		builder.Append(">");
		builder.Append(_player.transform.position.y.ToString("F2"));
		builder.Append(">");
		builder.Append(_player.transform.position.z.ToString("F2"));
		builder.Append("|");
		
	    Vector3 playerEulerAngles = _player.transform.rotation.eulerAngles;
		builder.Append(playerEulerAngles.x.ToString("F2"));
		builder.Append(">");
		builder.Append(playerEulerAngles.y.ToString("F2"));
		builder.Append(">");
		builder.Append(playerEulerAngles.z.ToString("F2"));
		
		return builder.ToString();
	}
	
	private string FormatHealthLine()
	{
		return String.Format("Health:{0}", _ambassador.MaxHP);
	}
	
	private string FormatDamageLine()
	{
		return String.Format("Damage:{0}", _ambassador.Damage);
	}
	
	private string FormatItemLine()
	{
		StringBuilder builder = new StringBuilder("Items:");
		
		// Format of line:
		// Items:A|B|C|E|F|G
		for(int i = 0; i < _ambassador.ItemList.Count; i++)
		{
			ObtainableItem current = _ambassador.ItemList[i];
			if(! current.Owned)
				continue;
			
			builder.Append(current.Name);
			if(i < _ambassador.ItemList.Count - 1)
				builder.Append("|");
		}
		
		return builder.ToString();
	}
	
	private string FormatPhaseLine()
	{
		StringBuilder builder = new StringBuilder("Quests:");
		
		// Format of line:
		// Quests:A>n,T>d|B>n,T>d|C>n,T>d
		for(int i = 0; i < _ambassador.SequenceCounters.Count; i++)
		{
			SequenceCounter current = _ambassador.SequenceCounters[i];
			if(DebugMode)
				Debug.Log("Serializing Sequence Counter #" + i + Environment.NewLine
				          + current.ToString());
			
			builder.Append(current.Name);
			builder.Append(">");
			builder.Append(current.Phase);
			builder.Append(",");
			builder.Append(current.QuestTitle);
			builder.Append(">");
			builder.Append(current.QuestDetails);
			
			if(i < _ambassador.SequenceCounters.Count - 1)
				builder.Append("|");
		}

		if(DebugMode)
			Debug.Log("Resulting phsae line:" + builder.ToString());
		
		return builder.ToString();
	}
	
	#endregion Data Formatting Methods
	
	#region Interpretation Methods
	
	private bool ValidateSaveFileHeader(string headerLine)
	{
		string[] parts = headerLine.Split(':');
		if(parts.Length != 2
		   || parts[0] != "File")
			return false;
		
		return parts[1] == "Hero's Journey Save File";
	}

	private bool SetupSettings(string settingsLine)
	{
		string[] mainParts = settingsLine.Split(':');
		if(mainParts.Length != 2
		   || mainParts[0] != "Settings")
			return false;

		string settingParts = mainParts[1];
		string[] settingItems = settingParts.Split('|');
		if(settingItems.Length != 5)
			return false;

		Settings.graphicsLevel = Convert.ToInt32(settingItems[0]);
		Settings.soundEnabled = Convert.ToBoolean(settingItems[1]);
		Settings.masterVolume = Convert.ToSingle(settingItems[2]);
		Settings.musVolume = Convert.ToSingle(settingItems[3]);
		Settings.sfxVolume = Convert.ToSingle(settingItems[4]);

		QualitySettings.SetQualityLevel((int) Settings.graphicsLevel);

		return true;
	}
	
	private bool SetupSceneLoad(string sceneLine)
	{
		string[] mainParts = sceneLine.Split(':');
		if(mainParts.Length != 2
		   || mainParts[0] != "Scene")
			return false;
		
		string[] locationParts = mainParts[1].Split('|');
		if(locationParts.Length != 3)
			return false;
		
		// Parse out the position Vector3...
		Vector3 position = ParseSequenceToVector3(locationParts[1], '>');
		Vector3 rotation = ParseSequenceToVector3(locationParts[2], '>');
		
		_transition.PrepareTransition(position, rotation, locationParts[0]);
		return true;
	}
	
	private bool SetupHealth(string healthLine)
	{
		string[] parts = healthLine.Split(':');
		if(parts.Length != 2
		   || parts[0] != "Health")
			return false;
		
		int hp = Convert.ToInt32(parts[1]);
		_ambassador.MaxHP = hp;
		
		return true;
	}
	
	private bool SetupDamage(string damageLine)
	{
		string[] parts = damageLine.Split(':');
		if(parts.Length != 2
		   || parts[0] != "Damage")
			return false;
		
		int damage = Convert.ToInt32(parts[1]);
		_ambassador.Damage = damage;
		
		return true;
	}
	
	private bool SetupObtainedItems(string itemLine)
	{
		string[] mainParts = itemLine.Split(':');
		if(mainParts.Length != 2
		   || mainParts[0] != "Items")
			return false;
		
		_ambassador.ItemList = new List<ObtainableItem>();
		
		string[] itemParts = mainParts[1].Split('|');
		foreach(string current in itemParts)
		{
			_ambassador.GainItem(current);
		}
		
		return true;
	}
	
	private bool SetupQuestPhases(string phaseLine)
	{
		string[] mainParts = phaseLine.Split(':');
		if(mainParts.Length != 2
		   || mainParts[0] != "Quests")
			return false;
		
		_ambassador.SequenceCounters = new List<SequenceCounter>();
		
		string[] itemParts = mainParts[1].Split('|');
		foreach(string current in itemParts)
		{
			string[] sequenceParts = current.Split(',');

			string[] phaseParts = sequenceParts[0].Split('>');
			int phaseProgress = Convert.ToInt32(phaseParts[1]);

			string[] detailParts = sequenceParts[1].Split('>');

			SequenceCounter newCounter = new SequenceCounter{
				Name = phaseParts[0],
				Phase = phaseProgress,
				QuestTitle = detailParts[0],
				QuestDetails = detailParts[1]
			};

			_ambassador.SequenceCounters.Add(newCounter);
		}
		
		return true;
	}
	
	#endregion Interpretation Methods
	
	#region Helper Methods

	private void DumpPlayerConfigToLog()
	{
		StringBuilder builder = new StringBuilder("Contents of the Player Config file:" + Environment.NewLine);

		string sceneLine = PlayerPrefs.GetString("Scene");
		string healthLine = PlayerPrefs.GetString("Health");
		string damageLine = PlayerPrefs.GetString("Damage");
		string itemLine = PlayerPrefs.GetString("Items");
		string phaseLine = PlayerPrefs.GetString("Phase");

		builder.Append("Scene line: " + sceneLine + Environment.NewLine);
		builder.Append("Health line: " + healthLine + Environment.NewLine);
		builder.Append("Damage line: " + damageLine + Environment.NewLine);
		builder.Append("Item line: " + itemLine + Environment.NewLine);
		builder.Append("Phase line: " + phaseLine + Environment.NewLine);

		Debug.Log(builder.ToString());
	}
	
	private Vector3 ParseSequenceToVector3(string sequence, char separator)
	{
		string[] parts = sequence.Split(separator);
		if(parts.Length != 3)
			throw new ArgumentException("There are not three parts to the token [" + sequence + "], given separator " + separator);
		
		Vector3 vector = new Vector3(0, 0, 0);
		vector.x = Convert.ToSingle(parts[0]);
		vector.y = Convert.ToSingle(parts[1]);
		vector.z = Convert.ToSingle(parts[2]);
		
		return vector;
	}
	
	#endregion Helper Methods
}
