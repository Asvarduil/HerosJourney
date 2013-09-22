using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

public class SaveFileAccess : MonoBehaviour 
{
	#region Variables / Properties
	
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
	
	public void SaveAmbassadorIntoFile()
	{
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
				string sceneLine = reader.ReadLine();
				string healthLine = reader.ReadLine();
				string damageLine = reader.ReadLine();				
				string itemLine = reader.ReadLine();
				string phaseLine = reader.ReadLine();
				
				result = ValidateSaveFileHeader(fileType)
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
	
	private string FormatSceneLine()
	{
		StringBuilder builder = new StringBuilder("Scene:");
		
		// Format of line:
		// Scene:[Name]|x>y>z|rx>ry>rz
		builder.Append(Application.loadedLevelName);
		builder.Append("|");
		
		// Detect the player on the spot.
		_player = GameObject.FindGameObjectWithTag("Player");
		
		builder.Append(_player.transform.position.x.ToString());
		builder.Append(">");
		builder.Append(_player.transform.position.y.ToString());
		builder.Append(">");
		builder.Append(_player.transform.position.z.ToString());
		builder.Append("|");
		
	    Vector3 playerEulerAngles = _player.transform.rotation.eulerAngles;
		builder.Append(playerEulerAngles.x.ToString());
		builder.Append(">");
		builder.Append(playerEulerAngles.y.ToString());
		builder.Append(">");
		builder.Append(playerEulerAngles.z.ToString());
		builder.Append(">");
		
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
		// Quests:A>n|B>n|C>n
		for(int i = 0; i < _ambassador.SequenceCounters.Count; i++)
		{
			SequenceCounter current = _ambassador.SequenceCounters[i];
			
			builder.Append(current.Name);
			builder.Append(">");
			builder.Append(current.Phase);
			
			if(i < _ambassador.SequenceCounters.Count - 1)
				builder.Append("|");
		}
		
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
			string[] phaseParts = current.Split('>');
			int phaseProgress = Convert.ToInt32(phaseParts[1]);
			
			_ambassador.UpdateThread(phaseParts[0], phaseProgress);
		}
		
		return true;
	}
	
	#endregion Interpretation Methods
	
	#region Helper Methods
	
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
