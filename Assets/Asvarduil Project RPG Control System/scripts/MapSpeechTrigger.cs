using UnityEngine;
using System;
using System.Collections.Generic;

public class MapSpeechTrigger : MonoBehaviour 
{
	#region Variables
	
	/// <summary>
	/// The skin that the GUI uses to draw.
	/// </summary>
	public GUISkin MapSkin;
	public GUISkin DialogueSkin;
	
	public AudioClip ButtonSound;
	
	/// <summary>
	/// Appears when a player is in the trigger.
	/// </summary>
	public FloatingButton SpeechNotice;
	
	/// <summary>
	/// The text background.
	/// </summary>
	public AsvarduilImage TextBackground;
	
	/// <summary>
	/// The (shown) speaker's name.
	/// </summary>
	public AsvarduilLabel Speaker;
	
	/// <summary>
	/// The displayed content.
	/// </summary>
	public AsvarduilLabel Content;
	
	/// <summary>
	/// A list of all the dialogue lines visible by this
	/// text trigger.
	/// </summary>
	public List<DialogueLine> DialogueLines = new List<DialogueLine>();
	
	private SpeechPhase _Phase = SpeechPhase.None;
	private int _DialogueIndex = 0;
	private int _DialogueStartIndex = 0;
	private Maestro _Maestro;
	
	#endregion Variables
	
	#region Enumerations
	
	/// <summary>
	/// Determines if the notice that the player
	/// can talk to the NPC is shown, that a conversation
	/// is happening, or that nothing is happening.
	/// </summary>
	private enum SpeechPhase
	{
		None,
		CanTalk,
		Conversation
	}
	
	#endregion Enumerations
	
	#region Engine Hooks
	
	public void Start()
	{
		_Maestro = Maestro.DetectLastInstance();
	}
	
	public void OnGUI()
	{	
		switch( _Phase )
		{
			case SpeechPhase.CanTalk:	
			    GUI.skin = MapSkin;
				if(SpeechNotice.IsClicked())
				{
					_Maestro.PlaySoundEffect(ButtonSound);
				
					_Phase = SpeechPhase.Conversation;
					_DialogueIndex = _DialogueStartIndex;
					TransitionTextbox();
				}
				break;
			
			case SpeechPhase.Conversation:
			    GUI.skin = DialogueSkin;
				TextBackground.DrawMe();
			    Speaker.DrawMe();
				Content.DrawMe();
			
				for(int i = 0; i < DialogueLines[_DialogueIndex].Buttons.Count; i++)
				{
					DialogueButton button = DialogueLines[_DialogueIndex].Buttons[i];
					if(button.Button.IsClicked())
					{
						_Maestro.PlaySoundEffect(ButtonSound);
					
						_DialogueIndex = button.DialogueIndex;
						_DialogueStartIndex = button.NewDialogueStartIndex;
						TransitionTextbox();
					}
				}
				break;
			
			default:
				break;
		}
	}
	
	public void FixedUpdate()
	{
		if(_Phase == SpeechPhase.CanTalk)
		{
			SpeechNotice.CalculatePosition(gameObject.transform.position);
		}
		
		// Tween textboxes if the phase is conversation.
		if(_Phase == SpeechPhase.Conversation)
		{
			TextBackground.Tween();
			Speaker.Tween();
			Content.Tween();
			
			for(int i = 0; i < DialogueLines[_DialogueIndex].Buttons.Count; i++)
			{
				DialogueButton button = DialogueLines[_DialogueIndex].Buttons[i];
				button.Button.Tween();
			}
		}
	}
	
	public void OnTriggerEnter(Collider who)
	{
		if(who.tag != "Player") { return; }
		
		_Phase = SpeechPhase.CanTalk;
	}
	
	public void OnTriggerExit(Collider who)
	{
		if(who.tag == "Player")
		{
			_Phase = SpeechPhase.None;
		}
	}
	
	#endregion Engine Hooks
	
	#region Methods
	
	public void TransitionTextbox()
	{	
		// Refade content and advance button.
		Content.Tint = new Color(0,0,0,0);
		
		if(_DialogueIndex >= DialogueLines.Count)
		{	
			// Hide the background, so that next time the user
			// speaks to an entity, the text background fades in.
			TextBackground.Tint = new Color(0,0,0,0);
			Speaker.Tint = new Color(0,0,0,0);
			
			_DialogueIndex = _DialogueStartIndex;
			_Phase = SpeechPhase.CanTalk;
			Content.Text = string.Empty;
		}
		else
		{
			_Phase = SpeechPhase.Conversation;
			Content.Text = DialogueLines[_DialogueIndex].ShownText;
			
			foreach(string thisMessage in DialogueLines[_DialogueIndex].Messages)
			{
				this.SendMessage(thisMessage, SendMessageOptions.DontRequireReceiver);
			}
		}
	}
	
	#endregion Methods
}

/// <summary>
/// Stores a line of dialogue, and
/// the buttons used to control it.
/// </summary>
[Serializable]
public class DialogueLine
{
	public string ShownText;
	public List<string> Messages;
	public List<DialogueButton> Buttons;
}

/// <summary>
/// Button, with line number to jump to on click.
/// </summary>
[Serializable]
public class DialogueButton
{
	public AsvarduilButton Button;
	public int DialogueIndex;
	public int NewDialogueStartIndex = 0;
}