using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public class PressureSwitch : MonoBehaviour 
{
	#region Enumerations
	
	public enum ButtonType
	{
		Releasable,
		Toggle,
		OneShot
	}
	
	#endregion Enumerations
	
	#region Variables / Properties
	
	public bool DebugMode = false;
	public List<string> AllowedTags;
	
	public bool IsPressed = false;
	
	public ButtonType Type = ButtonType.Releasable;
	
	public Texture2D Unpressed;
	public Texture2D Pressed;
	public AudioClip ReleasedSfx;
	public AudioClip PressedSfx;
	
	private bool _isDisabled = false;
	private Maestro _maestro;
	private Renderer _renderer;
	
	#endregion Variables / Properties
	
	#region Engine Hooks
	
	public void Start()
	{
		_maestro = Maestro.DetectLastInstance();
		_renderer = GetComponentInChildren<Renderer>();
	}
	
	public void OnTriggerEnter(Collider who)
	{
		if(! AllowedTags.Contains(who.tag)
		   || _isDisabled)
			return;
		
		AudioClip sfx = null;
		Texture2D texture = null;
		string debugVerb;
		
		switch(Type)
		{
			case ButtonType.Toggle:
				debugVerb = "toggled";
				IsPressed = !IsPressed;
				texture = IsPressed ? Pressed : Unpressed;
				sfx = IsPressed ? PressedSfx : ReleasedSfx;
				break;
			
			case ButtonType.Releasable:
				debugVerb = "pressed";
				IsPressed = true;
				texture = Pressed;
				sfx = PressedSfx;
				break;
			
			default:
				debugVerb = "activated";
				_isDisabled = true;
				IsPressed = true;
				texture = Pressed;
				sfx = PressedSfx;
				break;
		}
		
		if(DebugMode)
			Debug.Log(String.Format("{0} with tag {1} has {2} switch {3}", who.name, who.tag, debugVerb, gameObject.name));
		
		if(texture != null)
			_renderer.material.SetTexture("_MainTex", texture);
		
		if (_maestro != null 
			&& sfx != null)
			_maestro.PlaySoundEffect(sfx);
	}
	
	public void OnTriggerExit(Collider who)
	{	
		// Coming off a switch does not alter a togglable switch.
		if(! AllowedTags.Contains(who.tag)
		   ||Type == ButtonType.Toggle
		   || Type == ButtonType.OneShot
		   || _isDisabled)
			return;
		
		if(DebugMode)
			Debug.Log(who.name + " with tag " + who.tag + " has released the switch!");
		
		IsPressed = false;
		
		if(Unpressed != null)
			_renderer.material.SetTexture("_MainTex", Unpressed);
		
		if (_maestro != null 
		    && ReleasedSfx != null)
			_maestro.PlaySoundEffect(ReleasedSfx);
	}
	
	#endregion Engine Hooks
}
