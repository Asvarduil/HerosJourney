using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

// Programmer's Notes:
// -------------------------------------------------
// Entity Text is merely a collection of dialogue
// text for a quasi-linear NPC.
//
// Text collections require a certain quest
// chain to be at a certain state.  If this
// condition is met, the player will be able
// to read the text for that quest state.
//
// Individual text entries can modify quest states,
// thus allowing side-quests to be implemented in
// a game!
//
// It's important to note, this is separated from
// the concern of presenting the text to the player.
// That is done in the DialogueGUI script, which
// observes eligible Entity Text instances.

public class EntityText : MonoBehaviour 
{
	#region Variables / Properties

	public bool DebugMode = false;
	public string AffectedTag = "Player";
	public bool CanTalk = false;
	public bool TriggeredOnEntry = false;
	public List<DialogueThread> NPCText;
	
	public GUISkin Skin;
	public FloatingButton TalkButton;
	
	private bool _showButton = false;
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	public void OnGUI()
	{
		if(! _showButton)
			return;
		
		GUI.skin = Skin;
		if(TalkButton.IsClicked()
		   || Input.GetButtonUp("Interact"))
		{
			if(DebugMode)
				Debug.Log("Entity flagged as being able to talk...");

			_showButton = false;
			CanTalk = true;
		}
	}
	
	public void FixedUpdate()
	{
		TalkButton.CalculatePosition(transform.position);
	}
	
	public void OnTriggerEnter(Collider who)
	{
		if(who.tag != AffectedTag)
			return;
		
		if(TriggeredOnEntry)
		{
			ActivateAttacks(who, false);

			CanTalk = true;
			_showButton = false;
			return;
		}
		
		_showButton = true;
	}
	
	public void OnTriggerExit(Collider who)
	{
		if(who.tag != AffectedTag)
			return;

		ActivateAttacks(who, true);
		
		CanTalk = false;
		_showButton = false;
	}
	
	#endregion Engine Hooks
	
	#region Methods
	
	public DialogueThread CurrentThread(List<SequenceCounter> phases)
	{
		// Find a thread that matches the first eligible phase.
		DialogueThread relevant = FindFirstRelevantPhase(phases);
		
		// Relevant thread still dosen't exist.  Just return an empty list.
		if(relevant == default(DialogueThread))
			return new DialogueThread();
		
		relevant.CallingGameObject = gameObject;
		return relevant;
	}
	
	private DialogueThread FindFirstRelevantPhase(List<SequenceCounter> phases)
	{
		DialogueThread relevant = default(DialogueThread);
		foreach(SequenceCounter phase in phases)
		{
			DialogueThread thread = NPCText.FirstOrDefault(t => t.EligibleForUse(phase));
			if(thread == default(DialogueThread))
			{
				if(DebugMode)
					Debug.Log("Phase " + phase + " will not be used...");

				continue;
			}

			if(DebugMode)
				Debug.Log("Found an eligible thread for dialogue phase " + phase);
			relevant = thread;
			break;
		}
		
		if(relevant == default(DialogueThread))
		{
			if(DebugMode)
				Debug.Log("Was unable to find a matching phase.  Using default dialogue instead...");
			relevant = NPCText.FirstOrDefault(x => x.IsDefaultText == true);
		}

		if(relevant == default(DialogueThread))
		{
			Debug.LogError("Was unable to find default dialogue.  Revise this immediately!");
		}
		
		relevant.CallingGameObject = gameObject;
		return relevant;
	}

	private void ActivateAttacks(Collider who, bool canAttack)
	{
		PlayerControl controls = who.gameObject.GetComponent<PlayerControl>();
		controls.canAttack = canAttack;
	}
	
	#endregion Methods
}

[Serializable]
public class DialogueThread
{
	#region Variables / Properties
	
	public string ThreadName;
	public bool IsDefaultText = false;
	public SequenceCounter RequiredPhase;
	public List<DialogueText> DialogueText;
	public GameObject CallingGameObject;
	
	private int _index = 0;
	
	public bool TextExhausted 
	{ 
		get 
		{ 
			if(DialogueText == null
			   || DialogueText.Count == 0)
				return true;
			
			return _index >= DialogueText.Count - 1; 
		} 
	}
	
	#endregion Variables / Properties
	
	#region Methods
	
	public void ResetIndex()
	{
		_index = 0;
	}
	
	public DialogueText AdvanceSpeakerText()
	{
		_index++;
		
		return (_index > DialogueText.Count)
			? default(DialogueText)
			: GetCurrentText();
	}
	
	public DialogueText GetCurrentText()
	{
		if(DialogueText == null)
			return new DialogueText();
		
		if(_index > DialogueText.Count - 1)
			return new DialogueText();
		
		return DialogueText[_index] ?? new DialogueText();
	}
	
	public DialogueText GetTextAtIndex(int index)
	{
		if(DialogueText == null)
			return new DialogueText();
		
		if(index > DialogueText.Count - 1)
			return new DialogueText();
		
		return DialogueText[index] ?? new DialogueText();
	}
	
	public bool EligibleForUse(SequenceCounter phase)
	{
		return phase.Name == RequiredPhase.Name
			   && phase.Phase == RequiredPhase.Phase;
	}
	
	#endregion Methods
}

[Serializable]
public class DialogueText
{
	public string SpeakerName;
	public string SpeakerText;
	public AudioClip BGMOverride;
	public AudioClip OneShotClip;
	public string ConversationGift;
	public string ConversationEvent;
	public bool AltersProgression = false;
	public string QuestThreadName;
	public int ResultingQuestThreadPhase;
	public string ThreadQuestTitle;
	public string ThreadQuestDetails;
	public bool CausesSelfDestruct = false;
}