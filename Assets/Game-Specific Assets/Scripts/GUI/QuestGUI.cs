using UnityEngine;
using System;
using System.Collections.Generic;

public class QuestGUI : MonoBehaviour 
{
	#region Variables / Properties

	public bool DebugMode = false;

	public List<QuestInfoWindow> QuestInfoWindows;
	private Ambassador _ambassador;

	#endregion Variables / Properties

	#region Engine Hooks

	public void Start()
	{
		_ambassador = Ambassador.Instance;

		RefreshQuestDetails();
	}

	#endregion Engine Hooks

	#region Methods

	public void OnGUI()
	{
		foreach(QuestInfoWindow current in QuestInfoWindows)
			current.DrawMe();
	}

	public void Update()
	{
		foreach(QuestInfoWindow current in QuestInfoWindows)
			current.Tween();
	}

	public void RefreshQuestDetails()
	{
		int i = 0;
		foreach(QuestInfoWindow current in QuestInfoWindows)
		{
			if(DebugMode)
				Debug.Log("Current quest being mapped: " + i + Environment.NewLine
				          + "The Ambassador has " + _ambassador.SequenceCounters.Count + " sequence counters available.");

			string questTitle = _ambassador.SequenceCounters[i].QuestTitle;
			string questDetails = _ambassador.SequenceCounters[i].QuestDetails;
			current.UpdateDetails(questTitle, questDetails);
			
			i++;
		}
	}

	public void SetVisibility(bool isVisible)
	{
		foreach(QuestInfoWindow current in QuestInfoWindows)
			if(isVisible)
				current.ShowElements();
			else
				current.HideElements();
	}

	#endregion Methods
}

[Serializable]
public class QuestInfoWindow : AsvarduilForm
{
	#region Variables / Properties

	public GUISkin Skin;
	public AsvarduilLabel QuestTitle;
	public AsvarduilLabel QuestDetails;

	#endregion Variables / Properties

	#region Constructor

	public QuestInfoWindow(AsvarduilImage bg, AsvarduilLabel WindowName) 
		: base(bg, WindowName)
	{
	}

	#endregion Constructor

	#region Overrides

	public void UpdateDetails(string questName, string questDetails)
	{
		QuestTitle.Text = questName;
		QuestDetails.Text = questDetails;
	}

	public override void DrawMe()
	{
		GUI.skin = Skin;

		Background.DrawMe();
		QuestTitle.DrawMe();
		QuestDetails.DrawMe();
	}

	public override void Tween ()
	{
		Background.Tween();
		QuestTitle.Tween();
		QuestDetails.Tween();
	}

	public void HideElements()
	{
		Background.TargetTint.a = 0;
		QuestTitle.TargetTint.a = 0;
		QuestDetails.TargetTint.a = 0;
	}

	public void ShowElements()
	{
		Background.TargetTint.a = 1;
		QuestTitle.TargetTint.a = 1;
		QuestDetails.TargetTint.a = 1;
	}

	#endregion Overrides
}
