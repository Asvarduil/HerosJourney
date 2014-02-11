using UnityEngine;
using System.Linq;
using System.Collections.Generic;

// Programmer's Notes
// ------------------------------------------------
// This class provides the GUI implementation of
// the dialogue system.  It shows and advances
// text, gives dialogue rewards, and even plays
// supplementary sound effects or alters the
// background music as necessary, based on the
// dialogue data of the NPC that the user can
// interact with.

public class DialogueGUI : MonoBehaviour 
{
	#region Variables / Properties
	
	public bool DebugMode = false;
	public GUISkin skin;
	public bool GUIShowing = false;
	public bool DialogueAvailable = false;
	public float AdvanceDialogLockout = 2.0f;

	public AsvarduilImage Background;
	public AsvarduilLabel SpeakerName;
	public AsvarduilLabel SpeakerText;
	public AsvarduilButton NextButton;

	private float _nextAdvance;
	private string _speakerName;
	private DialogueThread _currentThread;
	private EntityText[] _textProviders;
	private Ambassador _ambassador;
	private Maestro _maestro;
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	public void Start()
	{
		_textProviders = (EntityText[]) FindObjectsOfType(typeof(EntityText));
		_ambassador = (Ambassador) FindObjectOfType(typeof(Ambassador));
		_maestro = (Maestro) FindObjectOfType(typeof(Maestro));
	}
	
	public void OnGUI()
	{
		if(! DialogueAvailable)
			return;
		
		GUI.skin = skin;
		
		Background.DrawMe();
		SpeakerName.DrawMe();
		SpeakerText.DrawMe();

		bool userWantsToAdvance = NextButton.IsClicked() 
			                      || Input.GetButtonUp("Interact");

		if(Time.time >= _nextAdvance
		   && userWantsToAdvance)
		{
			DialogueText text = _currentThread.AdvanceSpeakerText();
			if(text != default(DialogueText))
			{
				PresentLine(text);
			}
			
			if(_currentThread.TextExhausted)
			{
				if(DebugMode)
					Debug.Log("Dialog text is exhausted.");

				PlayerHasControl(true);
				HideElements();
			}
		}
	}
	
	public void FixedUpdate()
	{
		AcquireTextFromAvailableEntities();
		PresentAdvanceDialogButton();
		TweenElements();
	}
	
	#endregion Engine Hooks
	
	#region Methods
	
	private void PresentLine(DialogueText text)
	{
		UpdateAdvanceLockout();

		SpeakerName.Text = text.SpeakerName;
		SpeakerText.Text = text.SpeakerText;
		
		if(text.BGMOverride != null)
			_maestro.ChangeTunes(text.BGMOverride);
		
		if(text.OneShotClip != null)
			_maestro.PlaySoundEffect(text.OneShotClip, 1.0f);
		
		if(!string.IsNullOrEmpty(text.ConversationEvent))
		{
			if(DebugMode)
				Debug.Log("Sending message " + text.ConversationEvent + " to own scripts...");

			gameObject.SendMessage(text.ConversationEvent, SendMessageOptions.RequireReceiver);
		}
		
		if(!string.IsNullOrEmpty(text.ConversationGift))
			_ambassador.GainItem(text.ConversationGift);
		
		if(text.AltersProgression)
		{
			_ambassador.UpdateThread(
				text.QuestThreadName, 
				text.ResultingQuestThreadPhase, 
				text.ThreadQuestTitle, 
				text.ThreadQuestDetails
			);

			gameObject.SendMessage("RefreshQuestDetails", SendMessageOptions.DontRequireReceiver);
		}
		
		if(text.CausesSelfDestruct)
		{
			if(DebugMode)
				Debug.Log("Destroying current object, reverting flags, and re-acquiring list of text sources.");

			Destroy(_currentThread.CallingGameObject);
			DialogueAvailable = false;
			GUIShowing = false;

			_textProviders = (EntityText[]) FindObjectsOfType(typeof(EntityText));
		}
	}

	private void UpdateAdvanceLockout()
	{
		_nextAdvance = Time.time + AdvanceDialogLockout;
		NextButton.TargetTint.a = 0;
	}

	private void PresentAdvanceDialogButton()
	{
		if(!GUIShowing)
			return;

		if(Time.time <= _nextAdvance)
			return;

		NextButton.TargetTint.a = 1;
	}
	
	private void AcquireTextFromAvailableEntities()
	{
		if(_textProviders == null
		   || _textProviders.Length == 0)
			return;

		EntityText availableEntity = _textProviders.FirstOrDefault(t => t.CanTalk == true);
		if(DebugMode)
			Debug.Log("There is " + (availableEntity == default(EntityText) ? "no" : "an") + " NPC that can talk.");
		
		// Code Case: We no longer have an entity, but we still show there being dialogue.
		// User Case: Player has left a trigger with text still shown.
		// Desired Result: Stop showing text, reset line counter.
		if(availableEntity == default(EntityText)
		   && DialogueAvailable)
		{
			if(DebugMode)
				Debug.Log("User has left a text trigger, so hide text.");

			_currentThread.ResetIndex();
			DialogueAvailable = false;
			HideElements();
		}
		// Code Case: We have an entity, and we do not see any current dialogue.
		// User Case: Player has initiated a conversation sequence.
		// Desired Result: Start showing text, set line counter to first item, set speaker name.
		//                 Also, lock the player's player control script.
		else if(availableEntity != default(EntityText)
			    && (! DialogueAvailable))
		{
			if(DebugMode)
				Debug.Log("Player has initiated a conversation.");

			_currentThread = availableEntity.CurrentThread(_ambassador.SequenceCounters);
			_currentThread.ResetIndex();
			
			DialogueText currentText = _currentThread.GetCurrentText();
			if(currentText != null)
			{
				PresentLine(currentText);
				DialogueAvailable = true;
				
				PlayerHasControl(false);
				ShowElements();
			}
		}
		
		// Code Case: We have both an entity and show dialogue
		// User Case: An conversation is on-going
		// Desired Result: Do not mess with it!
		
		// Code Case: We have no entity and show no dialogue [Default State]
		// User Case: User is doing some non-conversation activity
		// Desired Result: Do nothing.
	}
	
	private void PlayerHasControl(bool canControl)
	{
		PlayerControl controls = (PlayerControl) FindObjectOfType(typeof(PlayerControl));
		if(controls == null)
		{
			if(DebugMode)
				Debug.LogWarning("Found no player control script in this scene!");
			
			return;
		}
		
		if(DebugMode)
			Debug.Log("Player controls will " + (canControl ? "not" : string.Empty) + " be locked.");
			
		if(canControl)
			controls.Resume();
		else
			controls.Halt();
	}
	
	private void HideElements()
	{
		GUIShowing = false;

		Background.TargetTint.a = 0;
		SpeakerName.TargetTint.a = 0;
		SpeakerText.TargetTint.a = 0;
		NextButton.TargetTint.a = 0;
	}
	
	private void ShowElements()
	{
		GUIShowing = true;

		Background.TargetTint.a = 1;
		SpeakerName.TargetTint.a = 1;
		SpeakerText.TargetTint.a = 1;
		NextButton.TargetTint.a = 1;
	}
	
	private void TweenElements()
	{
		Background.Tween();
		SpeakerName.Tween();
		SpeakerText.Tween();
		NextButton.Tween();
	}
	
	#endregion Methods
}
